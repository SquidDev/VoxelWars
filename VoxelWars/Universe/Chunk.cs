using VoxelWars.Vector;

namespace VoxelWars.Universe
{
	public struct BlockData
	{
		public static readonly BlockData Air = new BlockData(Block.Air);
		
		public readonly int Metadata;
		public readonly Block Block;
		
		public BlockData(Block block, int metadata = 0)
		{
			Block = block;
			Metadata = metadata;
		}
	}

	/// <summary>
	/// Description of Chunk.
	/// </summary>
	public partial class Chunk
	{
		public const byte ChunkSize = 16;

		private ChunkSetter currentAccessor;
		private ChunkSetter nextAccessor;

		internal bool changed = true;
        
		internal readonly Chunk[] neighbours = new Chunk[4];
		public readonly Position Position;

		public Chunk(Position position, Noise noise)
		{
			Position = position;

			nextAccessor = new ChunkSetter(this);
			Noise(noise);

			currentAccessor = new ChunkSetter(this);
			MoveNext();
		}

		public void Update()
		{
			foreach (Byte2 pos in currentAccessor.Updates)
			{
				currentAccessor[pos.X, pos.Y].Block.Update(this, pos);
			}
		}

		public void MoveNext()
		{
			ChunkSetter current = currentAccessor, next = nextAccessor;

			if (next.changed)
			{
				changed = true;
				currentAccessor = next;

				next.CopyTo(current);
				nextAccessor = current;
				
				next.changed = false;
				current.changed = false;
			}
		}

		public ChunkAccessor CurrentAccessor { get { return currentAccessor; } }
		public ChunkSetter NextAccessor { get { return nextAccessor; } }
	}
}
