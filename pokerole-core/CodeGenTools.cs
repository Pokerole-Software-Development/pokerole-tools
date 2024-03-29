﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace Pokerole.Core
{
	internal static class CodeGenTools
	{
		
		/// <summary>
		/// This method is for the initial generation of the guids, not for population, and will likely only be invoked
		/// in Roslyn
		/// </summary>
		private static String GenerateBuiltInGuids<T>() where T : System.Enum
		{
			String typename = typeof(T).Name;
			StringBuilder codeBuilder = new StringBuilder();
			codeBuilder.AppendFormat("private static readonly IReadOnlyDictionary<{0}, Guid> baseGuids =\n",
				typename);
			codeBuilder.AppendFormat("new ReadOnlyDictionary<{0}, Guid>(new Dictionary<{0}, Guid>\n{{\n",
				typename);
			T[] types = (T[])Enum.GetValues(typeof(T));
			foreach (var type in types)
			{
				codeBuilder.AppendFormat("{{ {0}.{1}, Guid.Parse(\"{2}\") }},\n", typename, type, Guid.NewGuid());
			}
			codeBuilder.Append("}});");
			return codeBuilder.ToString();
		}
	}
}
