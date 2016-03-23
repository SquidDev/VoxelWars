using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VoxelWars.Shaders;
using VoxelWars.Universe.Render;
using VoxelWars.Utils;

namespace VoxelWars.Universe
{
	public partial class Chunk
	{
		private readonly VBO indicies = new VBO();
		private readonly VBO verticies = new VBO();
		private bool hasData = false;
		private readonly List<Slice> slices = new List<Slice>();
    	
		public void Refresh()
		{
			changed = false;
			MeshBuilder builder = new MeshBuilder(this);
            
			slices.Clear();
			slices.Add(builder.BuildThreshold(Block.Dirt, Blocks.Dirt));
			slices.Add(builder.BuildThreshold(Block.Grass, Blocks.Grass));
			slices.Add(builder.BuildThreshold(Block.Sand, Blocks.Sand));
			slices.Add(builder.BuildThreshold(Block.Water, Blocks.Water));
            
			hasData = builder.Bind(verticies, indicies);
		}

		public void Render(Matrix4 mvp, float timer)
		{
			if (changed) Refresh();

			if (hasData)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, verticies.Id);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicies.Id);
            	
				foreach (Slice slice in slices)
				{
					IBlockShader shader = slice.Shader;
					shader.Enable(mvp, new Vector2(Position.X * ChunkSize, Position.Y * ChunkSize), timer);
					shader.Draw(slice.Start, slice.Size);
					shader.Disable();
				}
			}
		}
	}
}

