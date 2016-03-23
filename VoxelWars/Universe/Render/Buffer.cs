using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace VoxelWars.Universe.Render
{
	public sealed class Buffer<T>
	{
		public readonly T[] Data;

		private int index = 0;

		public int Index { get { return index; } }

		public Buffer(int size)
		{
			Data = new T[size];
		}

		public void Add(T item)
		{
			Data[index++] = item;
		}

		public T this[int index]
		{
			get { return Data[index]; }
			set { Data[index] = value; }
		}
	}

	public sealed class DynamicBuffer<T> : ICollection<T>
	{
		public T[] Data;

		private int index = 0;

		public int Index { get { return index; } }

		public DynamicBuffer()
			: this(32)
		{
        	
		}
		public DynamicBuffer(int size)
		{
			Data = new T[size];
		}

		public void Add(T item)
		{
			if (index >= Data.Length)
			{
				T[] array = new T[index * 2];
				Array.Copy(Data, 0, array, 0, index);
				Data = array;
			}

			Data[index++] = item;
		}

		public void Clear()
		{
			index = 0;
		}

		public IEnumerable<T> Items()
		{
			for (int i = 0; i < index; i++)
			{
				yield return Data[i];
			}
		}

		public int Size { get { return Index * Marshal.SizeOf<T>(); } }

		public T this[int index]
		{
			get { return Data[index]; }
			set { Data[index] = value; }
		}
    	
		public int Count
		{
			get
			{
				// return index;
				throw new NotImplementedException();
			}
		}
    	
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
    	
		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}
    	
		public void CopyTo(T[] array, int arrayIndex)
		{
			Data.CopyTo(array, arrayIndex);
		}
    	
		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}
    	
		public IEnumerator<T> GetEnumerator()
		{
			return Items().GetEnumerator();
		}
    	
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Items().GetEnumerator();
		}
	}
}

