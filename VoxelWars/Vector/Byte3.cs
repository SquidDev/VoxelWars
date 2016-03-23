using System;
using System.Runtime.InteropServices;

namespace VoxelWars.Vector
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Byte3
	{
		public readonly byte X;
		public readonly byte Y;
		public readonly byte Z;

		public Byte3(byte x, byte y, byte z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Byte3(short x, short y, short z)
		{
			X = (byte)x;
			Y = (byte)y;
			Z = (byte)z;
		}

		public Byte3(int x, int y, int z)
		{
			X = (byte)x;
			Y = (byte)y;
			Z = (byte)z;
		}

		public static explicit operator Byte3(SByte3 item)
		{
			return new Byte3((byte)item.X, (byte)item.Y, (byte)item.Z);
		}

		public static Byte3 operator/(Byte3 a, int factor)
		{
			return new Byte3(a.X / factor, a.Y / factor, a.Z / factor);
		}

		public static Byte3 operator*(Byte3 a, int factor)
		{
			return new Byte3(a.X * factor, a.Y * factor, a.Z * factor);
		}

		public static Byte3 operator+(Byte3 a, Byte3 b)
		{
			return new Byte3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static Byte3 operator-(Byte3 a, Byte3 b)
		{
			return new Byte3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SByte3
	{
		public readonly sbyte X;
		public readonly sbyte Y;
		public readonly sbyte Z;

		public SByte3(sbyte x, sbyte y, sbyte z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public SByte3(short x, short y, short z)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
			Z = (sbyte)z;
		}

		public SByte3(int x, int y, int z)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
			Z = (sbyte)z;
		}
	}
}

