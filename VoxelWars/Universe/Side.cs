using System;
using VoxelWars.Vector;

namespace VoxelWars.Universe
{
	public enum Side
	{
		/// <summary>
		/// +Y
		/// </summary>
		North = 0,

		/// <summary>
		/// -Y
		/// </summary>
		South = 1,

		/// <summary>
		/// +X
		/// </summary>
		East = 2,

		/// <summary>
		/// -X
		/// </summary>
		West = 3,
	}

	public static class SideExtensions
	{
		public static Side Opposite(this Side side)
		{
			return (Side)(1 ^ (int)side);
		}

		public static readonly SByte2[] Vectors = new SByte2[]
		{
			new SByte2(0, 1),
			new SByte2(0, -1),
			new SByte2(1, 0),
			new SByte2(-1, 0),
		};
        
		public static readonly SByte2[] Offsets = new SByte2[]
		{
			new SByte2(0, 0),
			new SByte2(0, -1),
			new SByte2(0, 0),
			new SByte2(-1, 0),
		};
        
		public static readonly SByte2[] Axes = new SByte2[]
		{
			new SByte2(0, 1),
			new SByte2(0, 1),
			new SByte2(1, 0),
			new SByte2(1, 0),
		};

		public static readonly Side[] Sides = new Side[]
		{
			Side.North,
			Side.South,
			Side.East,
			Side.West,
		};

		public static SByte2 Vector(this Side side)
		{
			return Vectors[(int)side];
		}
        
		public static SByte2 Offset(this Side side)
		{
			return Offsets[(int)side];
		}
        
		public static SByte2 Axis(this Side side)
		{
			return Axes[(int)side];
		}
	}
}


