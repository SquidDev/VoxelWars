using System;

namespace VoxelWars.Vector
{
	public struct Position : IEquatable<Position>
	{
		public int X;
		public int Y;

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Position(float x, float y)
		{
			X = (int)Math.Floor(x);
			Y = (int)Math.Floor(y);
		}

		public Position(double x, double y)
		{
			X = (int)Math.Floor(x);
			Y = (int)Math.Floor(y);
		}

		public int LengthSquared { get { return X * X + Y * Y; } }

		public override int GetHashCode()
		{
			return X * 104729 + Y;
		}

		public override bool Equals(object other)
		{
			return other is Position && Equals((Position)other);
		}

		public bool Equals(Position other)
		{
			return X == other.X && Y == other.Y;
		}

		public static Position operator/(Position a, int factor)
		{
			return new Position(a.X / (double)factor, a.Y / (double)factor);
		}

		public static Position operator*(Position a, int factor)
		{
			return new Position(a.X * factor, a.Y * factor);
		}

		public static Position operator%(Position a, int factor)
		{
			return new Position(a.X % factor, a.Y % factor);
		}

		public static Position operator+(Position a, Position b)
		{
			return new Position(a.X + b.X, a.Y - b.Y);
		}
        
		public static Position operator+(Position a, SByte2 b)
		{
			return new Position(a.X + b.X, a.Y + b.Y);
		}

		public static Position operator-(Position a, Position b)
		{
			return new Position(a.X - b.X, a.Y - b.Y);
		}
        
		public static bool operator==(Position a, Position b)
		{
			return a.Equals(b);
		}
        
		public static bool operator!=(Position a, Position b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}
	}
}

