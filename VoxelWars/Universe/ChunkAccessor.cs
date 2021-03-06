using System;
using System.Collections.Generic;
using System.Diagnostics;

using VoxelWars.Vector;

namespace VoxelWars.Universe
{
	public class ChunkAccessor
	{
		protected readonly BlockData[,] Blocks = new BlockData[Chunk.ChunkSize, Chunk.ChunkSize];
		internal bool next = false;
		internal readonly HashSet<Byte2> Updates = new HashSet<Byte2>();
		protected readonly Chunk Chunk;

		public ChunkAccessor(Chunk chunk)
		{
			this.Chunk = chunk;
		}

		internal void CopyTo(ChunkAccessor destination)
		{
			Array.Copy(Blocks, destination.Blocks, Chunk.ChunkSize * Chunk.ChunkSize);
			destination.Updates.Clear();
			destination.Updates.UnionWith(Updates);
		}

		public BlockData this[byte x, byte y]
		{
			get { return Blocks[x, y]; }

		}

		public BlockData this[Byte2 position]
		{
			get { return this[position.X, position.Y]; }
		}

		public BlockData this[int x, int y]
		{
			get
			{
				Chunk chunk;
				Byte2 position;
				return Chunk.TryGetPosition(x, y, out chunk, out position) 
					? (next ? chunk.NextAccessor : chunk.CurrentAccessor)[position] 
					: BlockData.Air;
			}
		}

		public bool TryGetBlock(int x, int y, out BlockData block, out Chunk chunk, out Byte2 position)
		{
			if (Chunk.TryGetPosition(x, y, out chunk, out position))
			{
				block = (next ? chunk.NextAccessor : chunk.CurrentAccessor)[position.X, position.Y];
				return true;
			}
			else
			{
				block = BlockData.Air;
				return false;
			}
		}
	}

	public class ChunkSetter : ChunkAccessor
	{
		internal bool changed;

		public ChunkSetter(Chunk chunk)
			: base(chunk)
		{
		}

		public new BlockData this[byte x, byte y]
		{
			get { return base[x, y]; }
			set
			{
				Block current = Blocks[x, y].Block;
				Blocks[x, y] = value;
				if (current == null ? value.Block.RequiresUpdate : (current.RequiresUpdate != value.Block.RequiresUpdate))
				{
					if (value.Block.RequiresUpdate)
					{
						Updates.Add(new Byte2(x, y));
					}
					else
					{
						Debug.Assert(Updates.Remove(new Byte2(x, y)));
					}
				}
				changed = true;

				// Update neighbours
				if (x == 0)
				{
					Chunk n = Chunk.neighbours[(byte)Side.West];
					if (n != null) n.NextAccessor.changed = true;
				}
				else if (x == Chunk.ChunkSize - 1)
				{
					Chunk n = Chunk.neighbours[(byte)Side.East];
					if (n != null) n.NextAccessor.changed = true;
				}

				if (y == 0)
				{
					Chunk n = Chunk.neighbours[(byte)Side.South];
					if (n != null) n.NextAccessor.changed = true;
				}
				else if (y == Chunk.ChunkSize - 1)
				{
					Chunk n = Chunk.neighbours[(byte)Side.North];
					if (n != null) n.NextAccessor.changed = true;
				}                
			}
		}

		public new BlockData this[Byte2 position]
		{
			get { return base[position]; }
			set { this[position.X, position.Y] = value; }
		}

		public new BlockData this[int x, int y]
		{
			get { return base[x, y]; }
			set
			{
				Chunk chunk;
				Byte2 position;
				if (Chunk.TryGetPosition(x, y, out chunk, out position))
				{
					chunk.NextAccessor[position] = value;
				}
				else
				{
					// TODO: Load?
					// Console.WriteLine("Setting in unloaded chunk");
				}
			}
		}
	}
}

