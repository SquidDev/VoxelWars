using System;
using System.Diagnostics;
using OpenTK;
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
	/// </summary>
	public abstract class Block
	{
		public static readonly Block Air = new BasicBlock(BlockType.Air, 0);
		public static readonly Block Dirt = new BasicBlock(BlockType.Solid, 1);
		public static readonly Block Grass = new BasicBlock(BlockType.Solid, 2);
		public static readonly Block Sand = new FallingSand(BlockType.Solid, 3);
		public static readonly Block Water = new WaterBlock(20, 0);
		
		public static readonly Block[] Blocks = new Block[] { Dirt, Grass, Sand, Water };

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
		
		public virtual int DefaultMetadata { get { return 0; } }

		/// <summary>
		/// If this block requires an update tick
		/// </summary>
		public virtual bool RequiresUpdate { get { return false; } }

		public virtual void Update(Chunk chunk, Byte2 position)
		{
			Debug.Assert(RequiresUpdate, "Cannot update non-updateable tile");
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

	/// <summary>
	/// http://w-shadow.com/blog/2009/09/01/simple-fluid-simulation/
	/// </summary>
	public class WaterBlock : BasicBlock
	{
		/// <summary>
		/// Default mass of a block of water
		/// </summary>
		public readonly int MaxMass = 1000;
		
		/// <summary>
		/// Maximum amount to compress by
		/// </summary>
		public readonly int MaxCompress;
		
		/// <summary>
		/// Minimum mass before it is considered dry
		/// </summary>
		public readonly int MinMass = 0;
		
		/// <summary>
		/// Minimum flow per tick
		/// </summary>
		public readonly int MinFlow = 10;
		
		/// <summary>
		/// Maximum flow per tick
		/// </summary>
		public readonly int MaxFlow = 1000;
		
		public WaterBlock(int waterDelta, int density)
			: base(BlockType.Fluid, density)
		{
			MaxCompress = waterDelta;
		}
		
		public override bool RequiresUpdate { get { return true; } }
		
		public override int DefaultMetadata { get { return MaxMass; } }
		
		public override void Update(Chunk chunk, Byte2 position)
		{
			int remaining = Handle(chunk, position);
			if (remaining <= MinMass)
			{
				chunk.NextAccessor[position] = BlockData.Air;
			}
			else
			{
				chunk.NextAccessor[position] = new BlockData(this, remaining);
			}
		}
		
		public int Handle(Chunk chunk, Byte2 position)
		{
			ChunkAccessor current = chunk.CurrentAccessor;
			ChunkSetter next = chunk.NextAccessor;
			
			int x = position.X, y = position.Y;
			
			int remaining = current[position.X, position.Y].Metadata;
			int original = remaining;
			if (remaining <= MinMass) return remaining;
			
			//The block below this one
			BlockData data = current[x, y - 1];
			if (data.Block.Type != BlockType.Solid)
			{
				int flow = GetStableMass(remaining + data.Metadata) - data.Metadata;
				if (flow > MinFlow) flow /= 2; //leads to smoother flow
				flow = MathHelper.Clamp(flow, 0, Math.Min(MaxFlow, remaining));

				next[x, y - 1] = new BlockData(this, next[x, y - 1].Metadata + flow);
				remaining -= flow;
			}
			
			if (remaining <= MinMass) return remaining;
			
			// To the left
			data = current[x - 1, y];
			if (data.Block.Type != BlockType.Solid)
			{
				//Equalize the amount of water in this block and it's neighbour
				int flow = (original - data.Metadata) / 4;
				if (flow > MinFlow) flow /= 2;
				flow = MathHelper.Clamp(flow, 0, remaining);

				next[x - 1, y] = new BlockData(this, next[x - 1, y].Metadata - flow);
				remaining -= flow;
			}
			
			// To the right
			data = current[x + 1, y];
			if (data.Block.Type != BlockType.Solid)
			{
				//Equalize the amount of water in this block and it's neighbour
				int flow = (original - data.Metadata) / 4;
				if (flow > MinFlow) flow /= 2;
				flow = MathHelper.Clamp(flow, 0, remaining);

				next[x + 1, y] = new BlockData(this, next[x + 1, y].Metadata - flow);
				remaining -= flow;
			}
			
			// Above
			data = current[x, y + 1];
			if (data.Block.Type != BlockType.Solid)
			{
				int flow = remaining - GetStableMass(remaining + data.Metadata);
				if (flow > MinFlow) flow /= 2; //leads to smoother flow
				flow = MathHelper.Clamp(flow, 0, Math.Min(MaxFlow, remaining));

				next[x, y + 1] = new BlockData(this, next[x, y + 1].Metadata + flow);
				remaining -= flow;
			}
			
			if (remaining <= MinMass) return remaining;
			
			return remaining;
		}
		
		private int GetStableMass(int totalMass)
		{
			if (totalMass <= MaxMass)
			{
				return MaxMass;
			}
			else if (totalMass < 2 * MaxMass + MaxCompress)
			{
				return (MaxMass * MaxMass + totalMass * MaxCompress) / (MaxMass + MaxCompress);
			}
			else
			{
				return (totalMass + MaxCompress) / 2;
			}
		}
	}
}
