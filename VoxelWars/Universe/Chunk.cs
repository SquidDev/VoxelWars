using System;
using VoxelWars.Vector;

namespace VoxelWars.Universe
{
	public struct BlockData
	{
		public static readonly BlockData Air = new BlockData(Block.Air);
		
		public readonly float Metadata;
		public readonly Block Block;
		
		public BlockData(Block block, float metadata)
		{
			Block = block.Create(metadata);
			Metadata = metadata;
		}
		
		public BlockData(Block block)
		{
			Block = block;
			Metadata = block.DefaultMetadata;
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
				CurrentAccessor[pos.X, pos.Y].Block.Update(this, pos);
			}
			
			for(byte y = 0; y < ChunkSize; y++)
			{
				// WriteRow(y, CurrentAccessor);
				// WriteRow(y, NextAccessor);
			}
		}
		
		private static void WriteRow(byte y, ChunkAccessor accessor)
		{
			bool written = false;
			double sum = 0;
			for(byte x = 0; x < ChunkSize; x++)
			{
				if(accessor[x, y].Block is WaterBlock)
				{
					sum += accessor[x, y].Metadata;
					Console.Write(accessor[x, y].Metadata + " ");
					written = true;
				}
			}
			if (written) Console.WriteLine("=> " + sum);
		}

		public void MoveNext()
		{
			ChunkSetter current = currentAccessor, next = nextAccessor;

			if (next.changed)
			{
				changed = true;
				currentAccessor = next;
				next.next = false;

				next.CopyTo(current);
				nextAccessor = current;
				current.next = true;
				
				next.changed = false;
				current.changed = false;
			}
		}

		public ChunkAccessor CurrentAccessor { get { return currentAccessor; } }
		public ChunkSetter NextAccessor { get { return nextAccessor; } }
		
		public bool TryGetPosition(int x, int y, out Chunk chunk, out Byte2 blockPos)
		{
			chunk = null;
			blockPos = new Byte2();

			if (x < 0)
			{
				Chunk n = neighbours[(byte)Side.West];
				return n != null && n.TryGetPosition(x + Chunk.ChunkSize, y, out chunk, out blockPos);
			}

			if (y < 0)
			{
				Chunk n = neighbours[(byte)Side.South];
				return n != null && n.TryGetPosition(x, y + Chunk.ChunkSize, out chunk, out blockPos);
			}

			if (x >= Chunk.ChunkSize)
			{
				Chunk n = neighbours[(byte)Side.East];
				return n != null && n.TryGetPosition(x - Chunk.ChunkSize, y, out chunk, out blockPos);
			}

			if (y >= Chunk.ChunkSize)
			{
				Chunk n = neighbours[(byte)Side.North];
				return n != null && n.TryGetPosition(x, y - Chunk.ChunkSize, out chunk, out blockPos);
			}

			chunk = this;
			blockPos = new Byte2(x, y);
			return true;
		}
	}
}
