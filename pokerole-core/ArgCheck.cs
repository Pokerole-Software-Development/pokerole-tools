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