using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using static Pokerole.Generator;

namespace Pokerole
{
	internal class JsonSchemaGenerator
	{
		const String baseUri = "https://pokeroleproject.com/json-schemas";
		public static void Main()
		{
			JSchemaGenerator generator = new JSchemaGenerator();
			sourceKind = SourceKind.Json;
			var (primarySchema, classes) = CompileSchema();
			JSchema root = new JSchema()
			{
				SchemaVersion = new Uri("https://json-schema.org/draft/2020-12/schema"),
				Id = new Uri(baseUri)
			};
			//UUID= String + format(uuid)
			//List= Array + OneOf (generic type = "items" : { "type" : foo }}

			foreach (var pair in classes)
			{
				String name = pair.Key;
				var classDef = pair.Value;
				JSchema def = new JSchema()
				{
					Id = new Uri($"{baseUri}/{name}")
				};
				if (!classDef.isReferenceType)
				{
					def.
				}
				if (classDef.isReferenceType)
				{

				}
				JSchema root = new JSchema()
				{
					Anchor = pair.Key
				};
				root.Prop
			}
			JSchema schema = new JSchema();
			//schema.
		}
		static JSchemaType ToJsonType(FieldType type)
		{
			if (type.ListType != null)
			{
				return JSchemaType.Array;
			}
			switch (type.plainType.ToLowerInvariant())
			{
				case "string":
					return JSchemaType.String;
				case "boolean":
					return JSchemaType.Boolean;
				case "int":
				case "integer":
					return JSchemaType.Integer;
				default:
					return JSchemaType.Object;
			}
		}
	}
}
