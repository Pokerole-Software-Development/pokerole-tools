using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pokerole.Core
{
	public class ReadOnlySet<T> : ISet<T>
	{
		private readonly ISet<T> backer;

		public ReadOnlySet(ISet<T> backer)
		{
			this.backer = backer;
		}

		public int Count => backer.Count;

		public bool IsReadOnly => true;

		public bool Add(T item) => throw new NotSupportedException();
		public void Clear() => throw new NotSupportedException();
		public bool Contains(T item) => backer.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => backer.CopyTo(array, arrayIndex);
		public void ExceptWith(IEnumerable<T> other) => throw new NotSupportedException(); 
		public IEnumerator<T> GetEnumerator() => backer.GetEnumerator();
		public void IntersectWith(IEnumerable<T> other) => throw new NotSupportedException(); 
		public bool IsProperSubsetOf(IEnumerable<T> other) => backer.IsProperSubsetOf(other);
		public bool IsProperSupersetOf(IEnumerable<T> other) => backer.IsProperSupersetOf(other);
		public bool IsSubsetOf(IEnumerable<T> other) => backer.IsSubsetOf(other);
		public bool IsSupersetOf(IEnumerable<T> other) => backer.IsSupersetOf(other);
		public bool Overlaps(IEnumerable<T> other) => backer.Overlaps(other);
		public bool Remove(T item) => throw new NotSupportedException();
		public bool SetEquals(IEnumerable<T> other) => backer.SetEquals(other);
		public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException(); 
		public void UnionWith(IEnumerable<T> other) => throw new NotSupportedException(); 
		void ICollection<T>.Add(T item) => throw new NotSupportedException();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)backer).GetEnumerator();
	}
}
