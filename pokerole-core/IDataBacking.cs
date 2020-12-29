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
	public interface I
	public interface IDataBacking<T> where T : class, IDataBackedItem<T>
	{
		int ItemId { get; }
		T? GetBackedItem([NotNullWhen(true)]bool createIfNull);
		/// <summary>
		/// Try to get the given value, throwing a <see cref="ValueNotSetException"/> if the value was not set
		/// </summary>
		/// <typeparam name="V"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		V GetValue<V>([CallerMemberName] String prop = "");
		bool GetValue<V>([MaybeNullWhen(false)] out V value, [CallerMemberName] String prop = "");
		void SetValue<V>(V value, [CallerMemberName] String prop = "");

		IDataPointer<V> GetObject<V>(int id) where V : class, IDataBackedItem<V>;
		IDataPointer<V> GetObject<V>(Guid id) where V : class, IDataBackedItem<V>;
	}
	public interface IDataPointer<T> where T : class, IDataBackedItem<T>
	{
		int DbId { get; }
		Guid CrossBackingId { get; }
		bool IsLoaded { get; }
		void Load();
		/// <summary>
		/// Gets the value this pointer points to unless the value is not loaded, in which case it throws a
		/// <see cref="DataPointerNotLoadedException"/>
		/// </summary>
		T Value { get; }

	}
	public interface IDataBackedItem<T> where T : class, IDataBackedItem<T>
	{
		int ItemId { get; }
		IDataBacking<T> GetDataBacking();
	}

	[Serializable]
	public class DataPointerNotLoadedException : Exception
	{
		public DataPointerNotLoadedException() { }
		public DataPointerNotLoadedException(string message) : base(message) { }
		public DataPointerNotLoadedException(string message, Exception inner) : base(message, inner) { }
		protected DataPointerNotLoadedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
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
	public class DummyDataBacking<T> : IDataBacking<T> where T : class, IDataBackedItem<T>
	{
		private readonly Dictionary<String, object?> dataTable = new Dictionary<string, object?>();
		private T? backedItem;
		public int ItemId => GetValue<int>("ID");

		public T? GetBackedItem([NotNullWhen(true)] bool createIfNull)
		{
			if (!createIfNull || backedItem != null)
			{
				return backedItem;
			}
			backedItem = (T?)Activator.CreateInstance(typeof(T), this);
			return backedItem;
		}

		public IDataPointer<V> GetObject<V>(int id) where V : class, IDataBackedItem<V> => throw new NotImplementedException();
		public IDataPointer<V> GetObject<V>(Guid id) where V : class, IDataBackedItem<V> => throw new NotImplementedException();

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
