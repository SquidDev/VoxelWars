using System;
using VoxelWars.Vector;

namespace VoxelWars.Universe
{
	public enum BlockType
	{
		Air,
		Solid,
		Fluid,
	}

	public static class BlockExtensions
	{
		public static bool Contains(this Block threshold, Block left)
		{
			if (left == threshold) return true;
			if (left.Type != threshold.Type) return false;
			
			switch (left.Type)
			{
				case BlockType.Air:
					return true;
				case BlockType.Fluid:
					return left.Density == threshold.Density;
				case BlockType.Solid:
					return left.Density >= threshold.Density;
				default:
					throw new ArgumentException("Unknown type " + left.Type);
			}
		}
	}

	/// <summary>
	/// An abstract block
	/// http://w-shadow.com/blog/2009/09/01/simple-fluid-simulation/
	/// </summary>
	public abstract class Block
	{
		public static readonly Block Air = new BasicBlock(BlockType.Air, 0);
		public static readonly Block Dirt = new BasicBlock(BlockType.Solid, 1);
		public static readonly Block Grass = new BasicBlock(BlockType.Solid, 2);
		public static readonly Block Sand = new FallingSand(BlockType.Solid, 3);
		public static readonly Block Water = new WaterBlock(2, 2);
		
		public static readonly Block[] Blocks = new Block[] { Dirt, Grass, Sand };

		/// <summary>
		/// Get the density of this block. 
		/// Blocks can merge with blocks with a density greater than this. 
		/// </summary>
		public abstract int Density { get; }

		/// <summary>
		/// Get the type of this block.
		/// Blocks can only merge with blocks of the same type.
		/// </summary>
		public abstract BlockType Type { get; }

		/// <summary>
		/// If this block requires an update tick
		/// </summary>
		public virtual bool RequiresUpdate { get { return false; } }

		public virtual void Update(Chunk chunk, Byte2 position)
		{
		}
	}

	public class BasicBlock : Block
	{
		private readonly BlockType type;
		private readonly int density;

		public override BlockType Type { get { return type; } }

		public override int Density { get { return density; } }

		public BasicBlock(BlockType type, int density)
		{
			this.type = type;
			this.density = density;
		}
	}

	public class FallingSand : BasicBlock
	{
		public FallingSand(BlockType type, int density)
			: base(type, density)
		{
		}

		public override bool RequiresUpdate { get { return true; } }

		public override void Update(Chunk chunk, Byte2 position)
		{
			if (chunk.CurrentAccessor[position.X, position.Y - 1].Block.Type == BlockType.Air)
			{
				chunk.NextAccessor[position.X, position.Y - 1] = chunk.CurrentAccessor[position.X, position.Y];
				chunk.NextAccessor[position.X, position.Y] = BlockData.Air;
			}
		}
	}
	
	public class WaterBlock : BasicBlock
	{
		public readonly int WaterDelta;
		
		public WaterBlock(int waterDelta, int density)
			: base(BlockType.Fluid, density)
		{
			WaterDelta = waterDelta;
		}
	}
}
