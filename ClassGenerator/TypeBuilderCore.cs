﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.CodeDom.Compiler;

namespace Pokerole
{
	public enum SourceKind
	{
		CSharp,
		TypeScript,
		Json
	}
	public static class Generator
	{
		public static Action<CompilerError> logError = error => throw new InvalidOperationException(
			$"Error logger was not set. Could not log {error}");
		public static SourceKind sourceKind;
		public static (XmlSchema primarySchema, Dictionary<String, ClassDef> classes) CompileSchema()
		{
			Assembly thisAssembly = Assembly.GetExecutingAssembly();
			var baseDir = Path.GetDirectoryName(thisAssembly.Location) ??
				throw new InvalidOperationException("Assembly parent directory missing?!?!?");
			XmlSchemaSet schemaSet = new XmlSchemaSet();
			String structuresPath = Path.Combine(baseDir, "Structures.xsd");
			//var schemaStream = thisAssembly.GetManifestResourceStream("Pokerole.Structures.xsd") ??
			//	throw new InvalidOperationException("Missing Structures.xsd (");
			XmlSchema primarySchema = XmlSchema.Read(new FileStream(structuresPath, FileMode.Open), null);
			schemaSet.Add(primarySchema);
			String[] additionalSchemaFiles = {
				"ExternalTypes.xsd",
				"MissingTypes.xsd"
			};
			foreach (var filename in additionalSchemaFiles)
			{
				schemaSet.Add(XmlSchema.Read(new FileStream(Path.Combine(baseDir, filename), FileMode.Open), null));
			}
			schemaSet.Compile();
			if (!schemaSet.IsCompiled || !primarySchema.IsCompiled)
			{
				throw new InvalidOperationException("Schema failed to compile");
			}
			var schemaItems = primarySchema.Items;
			Dictionary<String, ClassDef> classes = new Dictionary<String, ClassDef>();
			foreach (XmlSchemaObject item in schemaItems)
			{
				XmlSchemaComplexType? typeDef = item as XmlSchemaComplexType;
				if (typeDef == null)
				{
					XmlSchemaElement? element = item as XmlSchemaElement;
					if (element != null && element.Name == "PokeroleData")
					{
						//skip that one
						continue;
					}
					String message = "Handling of type not implemnted: " + item;
					logError?.Invoke(new CompilerError(structuresPath, item.LineNumber, item.LinePosition,
						"Unknown type", message));
					continue;
				}
				XmlSchemaType baseType = typeDef.BaseXmlSchemaType;
				bool isDataItem = baseType.Name == "BaseDataItem" || baseType.Name == "MutableBaseDataItem";
				bool isMutable = baseType.Name == "MutableBaseDataItem";
				ClassDef def = new ClassDef(isDataItem, IsReferenceType(schemaSet, typeDef.QualifiedName),
					typeDef.Name, typeDef, isMutable);
				classes[def.name] = def;
			}
			//parse things further since we now know all classes in the template file
			foreach (var def in classes.Values)
			{
				XmlSchemaComplexType typeDef = def.definition;
				XmlSchemaSequence? sequence = def.definition.ContentTypeParticle as XmlSchemaSequence;
				def.fields = new List<FieldDef>((sequence == null ? 0 : sequence.Items.Count) + typeDef.Attributes.Count);
				foreach (XmlSchemaAttribute attr in typeDef.Attributes)
				{
					FieldDef field = new FieldDef(attr.Name, new FieldType(NormalizeType(attr.SchemaTypeName.Name),
						IsReferenceType(schemaSet, attr.SchemaTypeName)));
					field.isAttribute = true;
					if (attr.Annotation != null)
					{
						XmlSchemaDocumentation? documentation = attr.Annotation.Items.OfType<XmlSchemaDocumentation>().FirstOrDefault();
						if (documentation != null)
						{
							XmlNode node = documentation.Markup.FirstOrDefault();
							if (node != null)
							{
								field.documentation = node.InnerText;
							}
						}
					}
					def.fields.Add(field);
				}
				if (sequence == null)
				{
					continue;
				}
				foreach (XmlSchemaElement item in sequence.Items)
				{
					if (item.Name == "DataId")
					{
						//skip that one. It will be present in the base class
						continue;
					}
					FieldType? genericType = null;
					FieldType? listType = null;
					String? proxyType = null;
					if (item.UnhandledAttributes != null)
					{
						foreach (XmlAttribute unhandled in item.UnhandledAttributes)
						{
							switch (unhandled.LocalName)
							{
								//case "keyType":
								//keyType = GrabTypeFromAttribute(item, unhandled, structuresPath, schemaSet, classes);
								//break;
								//case "valueType":
								//valueType = GrabTypeFromAttribute(item, unhandled, structuresPath, schemaSet, classes);
								//break;
								case "listItemType":
									listType = GrabTypeFromAttribute(item, unhandled, structuresPath, schemaSet, classes);
									break;
								case "genericType":
									genericType = GrabTypeFromAttribute(item, unhandled, structuresPath, schemaSet, classes);
									break;
								case "proxyType":
									proxyType = unhandled.InnerText;
									break;
								default:
									logError?.Invoke(new CompilerError(structuresPath, item.LineNumber, item.LinePosition,
									"Unknown attribute", String.Format("Attribute '{0}' is unknown", unhandled.LocalName)));
									break;
							}
						}
					}
					FieldDef field = new FieldDef(item.Name, FieldType.ResolveType(genericType, listType,
						item.SchemaTypeName, schemaSet, classes));
					field.name = item.Name;
					if (item.Annotation != null)
					{
						XmlSchemaDocumentation? documentation = item.Annotation.Items.OfType<XmlSchemaDocumentation>().FirstOrDefault();
						if (documentation != null)
						{
							XmlNode node = documentation.Markup.FirstOrDefault();
							if (node != null)
							{
								field.documentation = node.InnerText;
							}
						}
					}
					field.type = FieldType.ResolveType(genericType, listType, item.SchemaTypeName,
						schemaSet, classes);
					field.type.isNullable = item.IsNillable;
					field.proxyType = proxyType;
					def.fields.Add(field);
				}
			}
			return (primarySchema, classes);
			//return new Data
			//{
			//	primarySchema = primarySchema,
			//	classes = classes
			//};
		}


		static bool IsReferenceType(XmlSchemaSet schemaSet, XmlQualifiedName name)
		{
			switch (name.Name)
			{
				case "string":
				case "byte[]":
				case "base64Binary":
					return true;
			}
			XmlSchemaObject item = schemaSet.GlobalTypes[name];
			XmlSchemaComplexType? type = item as XmlSchemaComplexType;
			if (type != null)
			{
				switch (type.Name)
				{
					case "ItemReference":
					case "DataId":
					case "Height":
					case "Weight":
						return false;
				}
				//if (type.Name == "ItemReference" || type.Name == "DataId"){
				//return false;
				//}
				return true;
			}
			return false;
		}
		static FieldType? GrabTypeFromAttribute(XmlSchemaElement parent, XmlAttribute unhandled, String structuresPath,
			XmlSchemaSet schemaSet, Dictionary<String, ClassDef> classes)
		{
			String verified;
			try
			{ verified = XmlConvert.VerifyName(unhandled.Value); }
			catch (XmlException)
			{
				logError?.Invoke(new CompilerError(structuresPath, parent.LineNumber, parent.LinePosition,
					"Unknown type", String.Format("Given type is not fully qualified: '{0}'",
					unhandled.Value)));
				return null;
			}
			XmlQualifiedName name;
			String[] parts = verified.Split(':');
			if (parts.Length > 1)
			{
				name = new XmlQualifiedName(parts[1], parts[0]);
			}
			else
			{
				name = new XmlQualifiedName(verified);
			}
			ClassDef? def;
			if (!classes.TryGetValue(name.Name, out def))
			{
				def = null;
			}
			return new FieldType(NormalizeType(name.Name), IsReferenceType(schemaSet, name), def);
		}
		public static String LowercaseInitial(String input)
		{
			if (String.IsNullOrEmpty(input))
			{
				return input;
			}
			String result = char.ToLowerInvariant(input[0]) + input.Substring(1);
			if (result == "throw")
			{
				return "@throw";
			}
			return result;
		}
		static String NormalizeType(String input)
		{
			switch (input)
			{
				case "boolean":
					return "bool";
				case "base64Binary":
					return "byte[]";
				case "color":
					return "Color";
			}
			return input;
		}
		public class ClassDef
		{
			public bool isDataItem;
			public bool isReferenceType;
			public String name;
			public XmlSchemaComplexType definition;
			public List<FieldDef> fields = new List<FieldDef> ();
			public bool isMutable;
			public ClassDef(bool isDataItem, bool isReferenceType, String name, XmlSchemaComplexType definition,
				bool isMutable)
			{
				this.isDataItem = isDataItem;
				this.isReferenceType = isReferenceType;
				this.name = name;
				this.definition = definition;
				this.isMutable = isMutable;
			}
		}
		public class FieldDef
		{
			public bool isAttribute;
			public String? documentation;
			public string name;
			public bool referById = false;
			//public bool isDict;
			//public bool isList;
			public FieldType type;
			public String? proxyType;
			//public String keyType, valueType;
			//public String listType;
			//public String genericType;
			//public bool nullable;
			//public bool isReferenceType;
			public FieldDef(String name, FieldType type)
			{
				this.name = name;
				this.type = type;
			}
			public String NonNullReference()
			{
				return type.IsReferenceType ? $"{name}!" : $"{name} ?? default";
			}
		}
		public class FieldType
		{
			public String plainType;
			public bool isNullable = false;
			public bool IsReferenceType { get; }
			public ClassDef? ClassType { get; }
			private FieldType? genericType;
			public FieldType? ListType { get; }
			public FieldType(String plainType, bool isReferenceType)
			{
				this.plainType = plainType;
				IsReferenceType = isReferenceType;
			}
			public FieldType(String plainType, bool isReferenceType, ClassDef? classDef)
			{
				this.plainType = plainType;
				IsReferenceType = isReferenceType;
				ClassType = classDef;
			}
			//FieldType(String plainType, FieldType genericType
			public static FieldType ResolveType(FieldType? genericType, FieldType? listType,
				XmlQualifiedName typeName, XmlSchemaSet schemaSet, Dictionary<String, ClassDef> classes)
			{
				return new FieldType(genericType, listType, typeName, schemaSet, classes);
			}
			FieldType(FieldType? genericType, FieldType? listType,
				XmlQualifiedName typeName, XmlSchemaSet schemaSet, Dictionary<String, ClassDef> classes)
			{
				ClassDef? tempDef;
				IsReferenceType = IsReferenceType(schemaSet, typeName) || listType != null;
				plainType = NormalizeType(typeName.Name);
				if (classes.TryGetValue(plainType, out tempDef))
				{
					ClassType = tempDef;
				}
				this.genericType = genericType;
				this.ListType = listType;
			}
			public bool NeedsXmlBuilder()
			{
				if (ClassType != null || plainType == "ItemReference")
				{
					return true;
				}
				if (ListType != null)
				{
					return ListType.NeedsXmlBuilder();
				}
				return false;
			}
			public static String GetBasicTypeDeclaration(String type, FieldType? genericType, bool nullable)
			{
				String? generic = genericType != null ? genericType.plainType : null;
				return GetBasicTypeDeclaration(type, generic, nullable);
			}
			public static String GetBasicTypeDeclaration(FieldType type, FieldType? genericType, bool nullable)
			{
				return GetBasicTypeDeclaration(type.plainType, genericType, nullable);
			}
			public static String GetBasicTypeDeclaration(String type, String? genericType, bool nullable)
			{
				if (!String.IsNullOrEmpty(genericType))
				{
					String val = String.Format("{0}<{1}>", type, genericType);
					return nullable ? val + "?" : val;
				}
				return nullable ? type + "?" : type;
			}
			public String GetTypeDeclaration(bool readOnly)
			{
				if (ListType == null)
				{
					//something "simple"
					return GetBasicTypeDeclaration(plainType, genericType, isNullable);
				}
				//is a list
				String itemType = GetBasicTypeDeclaration(ListType, genericType, false);
				return String.Format(readOnly ? "IReadOnlyList<{0}>" : "List<{0}>", itemType);
			}
			public String GetBuilderTypeDeclaration()
			{
				if (ListType == null)
				{
					return $"{GetBasicTypeDeclaration(plainType, genericType, false)}.Builder";
				}
				return String.Format("{0}.Builder[]", GetBasicTypeDeclaration(ListType, genericType, false));
			}
			public String GetListType()
			{
				if (ListType == null)
				{
					throw new InvalidOperationException("Type is not a list");
				}
				return GetBasicTypeDeclaration(ListType, genericType, false);
			}
			public String GetTypeDeclaration(bool readOnly, bool forceNullable)
			{
				if (isNullable || !forceNullable || ListType != null)
				{
					return GetTypeDeclaration(readOnly);
				}
				return $"{GetTypeDeclaration(readOnly)}?";
			}
			public String GetAssignment(String incomingValue, bool readOnly)
			{
				if (ListType == null)
				{
					return incomingValue;
				}
				String itemType = GetBasicTypeDeclaration(ListType, genericType, false);
				if (!readOnly)
				{
					return $"new List<{itemType}>({incomingValue})";
				}
				return $"new List<{itemType}>({incomingValue}).AsReadOnly()";
				//if (!readOnly)
				//{
				//return $"new Dictionary<{keyType}, {valueType}>({incomingValue})";
				//}
				//return $"new ReadOnlyDictionary<{keyType}, {valueType}>(new Dictionary<{keyType}, {valueType}>({incomingValue}))";
			}
		}

	}
}