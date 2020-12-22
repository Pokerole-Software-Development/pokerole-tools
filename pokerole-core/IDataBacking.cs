/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pokerole.Core
{
	public interface IDataBacking<T> where T : IDataBackedItem<T>
	{
		/// <summary>
		/// Try to get the given value, throwing a <see cref="ValueNotSetException"/> if the value was not set
		/// </summary>
		/// <typeparam name="V"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		V GetValue<V>([CallerMemberName] String prop = "");
		bool GetValue<V>([MaybeNullWhen(false)] out V value, [CallerMemberName] String prop = "");
		void SetValue<V>(V value, [CallerMemberName] String prop = "");
	}
	public interface IDataBackedItem<T> where T : IDataBackedItem<T>
	{
		IDataBacking<T> GetDataBacking();
	}

	[Serializable]
	public class ValueNotSetException : Exception
	{
		public ValueNotSetException() { }
		public ValueNotSetException(string message) : base(message) { }
		public ValueNotSetException(string message, Exception inner) : base(message, inner) { }
		protected ValueNotSetException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
	public class DummyDataBacking<T> : IDataBacking<T> where T : IDataBackedItem<T>
	{
		private readonly Dictionary<String, object?> dataTable = new Dictionary<string, object?>();

		public V GetValue<V>([CallerMemberName] string prop = "")
		{
			if (!GetValue(out V value, prop))
			{
				throw new ValueNotSetException("prop");
			}
			return value;
		}
		public bool GetValue<V>([MaybeNullWhen(false)] out V value, [CallerMemberName] string prop = "")
		{

			if (!dataTable.TryGetValue(prop, out Object? val)){
				value = default;
				return false;
			}
			else
			{
				//if the caller says it can be null, then it can be null. Let the caller handle that
				value = (V)val!;

				return true;
			}
		}

		public void SetValue<V>(V value, [CallerMemberName] string prop = "")
		{
			dataTable[prop] = value;
		}
	}
}
