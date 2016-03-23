using System;
using System.Runtime.InteropServices;

namespace VoxelWars.Vector
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Byte2
	{
		public readonly byte X;
		public readonly byte Y;

		public Byte2(byte x, byte y)
		{
			X = x;
			Y = y;
		}

		public Byte2(short x, short y)
		{
			X = (byte)x;
			Y = (byte)y;
		}

		public Byte2(int x, int y)
		{
			X = (byte)x;
			Y = (byte)y;
		}

		public static explicit operator Byte2(SByte2 item)
		{
			return new Byte2((byte)item.X, (byte)item.Y);
		}

		public static Byte2 operator/(Byte2 a, int factor)
		{
			return new Byte2(a.X / factor, a.Y / factor);
		}

		public static Byte2 operator*(Byte2 a, int factor)
		{
			return new Byte2(a.X * factor, a.Y * factor);
		}

		public static Byte2 operator+(Byte2 a, Byte2 b)
		{
			return new Byte2(a.X + b.X, a.Y + b.Y);
		}

		public static Byte2 operator-(Byte2 a, Byte2 b)
		{
			return new Byte2(a.X - b.X, a.Y - b.Y);
		}
        
		public override string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}
		
		public override bool Equals(object obj)
		{
			return (obj is Byte2) && Equals((Byte2)obj);
		}
		
		public bool Equals(Byte2 other)
		{
			return this.X == other.X && this.Y == other.Y;
		}
		
		public override int GetHashCode()
		{
			return X << 8 | Y;
		}
		
		public static bool operator ==(Byte2 lhs, Byte2 rhs)
		{
			return lhs.X == rhs.X && lhs.Y == rhs.Y;
		}
		
		public static bool operator !=(Byte2 lhs, Byte2 rhs)
		{
			return lhs.X != rhs.X || lhs.Y != rhs.Y;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SByte2
	{
		public readonly sbyte X;
		public readonly sbyte Y;

		public SByte2(sbyte x, sbyte y)
		{
			X = x;
			Y = y;
		}

		public SByte2(short x, short y)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
		}

		public SByte2(int x, int y)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
		}
        
		public override string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}

	}
}

