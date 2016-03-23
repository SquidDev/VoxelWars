using System;
using OpenTK.Graphics.OpenGL;

namespace VoxelWars.Shaders
{
	public class ShaderAttribute
	{
		private int attributeId;
		private readonly string name;

		public ShaderAttribute(int program, string name)
		{
			attributeId = GetAttribute(program, name);
			this.name = name;
		}

		public int Id { get { return attributeId; } }

		public void Reload(int program)
		{
			attributeId = GetAttribute(program, name);
		}

		public void Enable()
		{
			GL.EnableVertexAttribArray(attributeId);
		}

		public void Disable()
		{
			GL.DisableVertexAttribArray(attributeId);
		}

		public void Bind(int size, VertexAttribPointerType type, bool normalised, int stride, IntPtr pointer)
		{
			GL.VertexAttribPointer(attributeId, size, type, normalised, stride, pointer);
		}

		private static int GetAttribute(int program, string name)
		{
			int attrib = GL.GetAttribLocation(program, name);
			if (attrib == -1) throw new Exception("Cannot load " + name);
			return attrib;
		}
	}

	public class ShaderUniform
	{
		private int uniformId;
		private readonly string name;

		public int Id { get { return uniformId; } }

		public ShaderUniform(int program, string name)
		{
			uniformId = GetUniform(program, name);
			this.name = name;
		}

		public void Reload(int program)
		{
			uniformId = GetUniform(program, name);
		}

		private static int GetUniform(int program, string name)
		{
			int attrib = GL.GetUniformLocation(program, name);
			if (attrib == -1) throw new Exception("Cannot load " + name);
			return attrib;
		}
	}
}

