using System;
using System.Collections.Generic;

namespace VoxelWars.Utils
{
	/// <summary>
	/// Description of CountingCollection.
	/// </summary>
	public class IndexedCollection<T, TColl>
		where TColl : ICollection<T>, new()
	{
		private readonly TColl collection = new TColl();
		private int counter = 0;
		private readonly Dictionary<T, int> elements = new Dictionary<T, int>();
		
		public TColl Collection { get { return collection; } }
		
		public int Add(T element)
		{
			int index;
			if (elements.TryGetValue(element, out index))
			{
				return index;
			}
			
			index = counter++;
			collection.Add(element);
			elements.Add(element, index);
			return index;
		}
	}
}
