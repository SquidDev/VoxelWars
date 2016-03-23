using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using VoxelWars.Loaders;
using VoxelWars.Universe.Render;

namespace VoxelWars.Shaders
{
	public class Blocks
	{
		public static readonly BlockShader Dirt = 
			new BlockShader(
				new ShaderData()
				{
					Color = new Vector3(139 / 255.0f, 69 / 255.0f, 19 / 255.0f),
					Fragment = "block", Vertex = "block",
				}
			);
		public static readonly BlockShader Grass = 
			new BlockShader(
				new ShaderData()
				{
					Color = new Vector3(0.0f, 1.0f, 0.0f),
					Fragment = "block", Vertex = "block",
				}
			);
		public static readonly BlockShader Sand = 
			new BlockShader(
				new ShaderData()
				{
					Color = new Vector3(246 / 255.0f, 1.0f, 0.0f),
					Fragment = "block", Vertex = "block",
				}
			);
		public static readonly BlockShader Water = 
			new BlockShader(
				new ShaderData()
				{
					Color = new Vector3(0.0f, 0.0f, 1.0f),
					Fragment = "block", Vertex = "water",
					// Timer = true, Offset = true,
					Metadata = true,
				}
			);
	}

	public interface IBlockShader
	{
		void Enable(Matrix4 mvp, Vector2 offset, double timer);

		void Disable();

		void Draw(int start, int length);
	}

	public class BlockShader : Shader, IBlockShader
	{
		public readonly ShaderAttribute Coord;

		public readonly ShaderUniform MVP;
		public readonly ShaderUniform Timer;
		public readonly ShaderUniform Color;
		public readonly ShaderUniform Offset;
		public readonly ShaderAttribute Metadata;

		public readonly IReadOnlyList<EnableCap> Enabled;
		public readonly IReadOnlyList<EnableCap> Disabled;
        
		private readonly Vector3 color;

		public BlockShader(ShaderData data)
			: base("Resources/Shaders/" + data.Vertex + ".v.glsl", "Resources/Shaders/" + data.Fragment + ".f.glsl")
		{
			try
			{
				GL.UseProgram(Id);

				Coord = GetAttribute("coord");
				MVP = GetUniform("mvp");
				if (data.Timer) Timer = GetUniform("timer");
				if (data.Offset) Offset = GetUniform("offset");
				if (data.Metadata) Metadata = GetAttribute("metadata");
                
				Color = GetUniform("color");
				color = data.Color;

//                TextureUnit unit = TextureUnit.Texture0;
//                foreach (KeyValuePair<string, string> texture in data.Textures)
//                {
//                    ShaderUniform uniform = GetUniform(texture.Key);
//                    TextureLoader.Bind(this, uniform, TextureLoader.Load(texture.Value), unit);
//
//                    unit++;
//                }

				Enabled = data.Enabled;
				Disabled = data.Disabled;
			}
			catch (Exception)
			{
				Dispose();
				throw;
			}
		}

		public void Enable(Matrix4 mpv, Vector2 offset, double timer)
		{
			Enable();
			GL.UniformMatrix4(MVP.Id, false, ref mpv);
			if (Timer != null) GL.Uniform1(Timer.Id, (float)timer);
			if (Offset != null) GL.Uniform2(Offset.Id, offset);
			GL.Uniform3(Color.Id, color);
		}

		public override void Enable()
		{
			base.Enable();
			foreach (EnableCap enable in Enabled) GL.Enable(enable);
			foreach (EnableCap disable in Disabled) GL.Disable(disable);
		}

		public override void Disable()
		{
			base.Disable();
			foreach (EnableCap enable in Enabled) GL.Disable(enable);
			foreach (EnableCap disable in Disabled) GL.Enable(disable);
		}

		public void Draw(int start, int length)
		{
			GL.VertexAttribPointer(Coord.Id, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0);
			if (Metadata != null) GL.VertexAttribPointer(Metadata.Id, 1, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), Marshal.SizeOf<Vector2>());
			GL.DrawElements(PrimitiveType.Triangles, length, DrawElementsType.UnsignedShort, start);
		}
	}
}
