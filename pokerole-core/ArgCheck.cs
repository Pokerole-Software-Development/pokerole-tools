/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Diagnostics.CodeAnalysis;

namespace Pokerole.Core
{
	internal class ArgCheck
	{
		internal static void NotNull<T>([NotNull]T item, string paramName)
		{
			if (paramName is null)
			{
				throw new ArgumentNullException(nameof(paramName));
			}
			if (item is null)
			{
				throw new ArgumentNullException(paramName);
			}
		}
	}
}