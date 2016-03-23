using System;
using System.Runtime.InteropServices;

namespace VoxelWars.Vector
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Byte4
	{
		public readonly byte X;
		public readonly byte Y;
		public readonly byte Z;
		public readonly byte W;

		public Byte4(byte x, byte y, byte z, byte w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Byte4(short x, short y, short z, short w)
		{
			X = (byte)x;
			Y = (byte)y;
			Z = (byte)z;
			W = (byte)w;
		}

		public Byte4(int x, int y, int z, int w)
		{
			X = (byte)x;
			Y = (byte)y;
			Z = (byte)z;
			W = (byte)w;
		}

		public override string ToString()
		{
			return "(" + X + ", " + Y + ", " + Z + ", " + W + ")";
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SByte4
	{
		public readonly sbyte X;
		public readonly sbyte Y;
		public readonly sbyte Z;
		public readonly sbyte W;

		public SByte4(sbyte x, sbyte y, sbyte z, sbyte w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public SByte4(short x, short y, short z, short w)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
			Z = (sbyte)z;
			W = (sbyte)w;
		}

		public SByte4(int x, int y, int z, int w)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
			Z = (sbyte)z;
			W = (sbyte)w;
		}
	}
}

