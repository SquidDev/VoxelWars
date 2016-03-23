using System;
using OpenTK;
using VoxelWars.Vector;

namespace VoxelWars.Universe
{
	public partial class World
	{
		private static readonly Position[] Offsets;

		private const int GenerateCap = 10;

		private static Position[] GenerateOffset(int rX, int rY)
		{
			Position[] positions = new Position[(rX * 2 + 1) * (rY * 2 + 1)];
			int index = 0;
			for (int x = -rX; x <= rX; x++)
			{
				for (int y = -rY; y <= rY; y++)
				{
					positions[index++] = new Position(x, y);
				}
			}

			Array.Sort(positions, (a, b) => a.LengthSquared.CompareTo(b.LengthSquared));
			return positions;
		}

		static World()
		{
			Offsets = GenerateOffset(7, 7);
		}

		public void Update(Position position)
		{
			int generated = 0;
			foreach (Position offset in Offsets)
			{
				Position chunkPos = position + offset;
				if (!chunks.ContainsKey(chunkPos))
				{
					Chunk temp = this[chunkPos];
					if (++generated >= GenerateCap) break;
				}
			}
		}

		private float timer = 0;

		public void Render(Matrix4 view, Position position)
		{
			// Yep. It is horrible
			foreach (Chunk chunk in chunks.Values) chunk.Update();
			foreach (Chunk chunk in chunks.Values) chunk.MoveNext();

			timer += 0.001f;
			foreach (Position offsetPos in Offsets)
			{
				Chunk chunk;
				if (!chunks.TryGetValue(position + offsetPos, out chunk)) continue;

				Matrix4 model = Matrix4.CreateTranslation(
					                chunk.Position.X * Chunk.ChunkSize,
					                chunk.Position.Y * Chunk.ChunkSize,
					                0
				                );
				Matrix4 mvp = model * view;
                
				chunk.Render(mvp, timer);
			}
		}
	}
}

