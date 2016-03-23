using System;

namespace VoxelWars.Universe
{
	public partial class Chunk
	{
		public const float Resolution = 0.01f;

		private void Noise(Noise noise)
		{
			for (byte x = 0; x < ChunkSize; x++)
			{
				for (byte y = 0; y < ChunkSize; y++)
				{
					float value = noise.Generate2D((x + Position.X * Chunk.ChunkSize) * Resolution, (y + Position.Y * Chunk.ChunkSize) * Resolution);
					int val = (int)(value * 255);
					if (val > 150)
					{
						NextAccessor[x, y] = new BlockData(Block.Sand);
					}
					else if (val > 50)
					{
						NextAccessor[x, y] = new BlockData(Block.Grass);
					}
					else if (val > 0)
					{
						NextAccessor[x, y] = new BlockData(Block.Dirt);
					}
					else
					{
						NextAccessor[x, y] = new BlockData(Block.Air);
					}
				}
			}
		}
	}
}

