using System;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Collections.Generic;

namespace VoxelWars.Shaders
{
	public class Shader : IDisposable
	{
		private readonly string fragmentPath;
		private readonly string vertexPath;
		private readonly Dictionary<string, ShaderAttribute> attributes;
		private readonly Dictionary<string, ShaderUniform> uniforms;
		private readonly int shaderId = 0;
		private int program;
        
		protected static Shader activeShader;

		public static Shader ActiveShader { get { return activeShader; } }

		public int Id { get { return program; } }

		public int ShaderId { get { return shaderId; } }

		public Shader(string name)
			: this("Resources/Shaders/" + name + ".v.glsl", "Resources/Shaders/" + name + ".f.glsl")
		{
		}

		public Shader(string vertexPath, string fragmentPath)
		{
			this.fragmentPath = fragmentPath;
			this.vertexPath = vertexPath;

			program = LoadProgram(vertexPath, fragmentPath);

			int size;
			GL.GetProgram(program, GetProgramParameterName.ActiveAttributes, out size);
			attributes = new Dictionary<string, ShaderAttribute>(size);

			GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out size);
			uniforms = new Dictionary<string, ShaderUniform>(size);
		}

		public ShaderAttribute GetAttribute(string name)
		{
			ShaderAttribute attr;
			if (!attributes.TryGetValue(name, out attr))
			{
				attr = new ShaderAttribute(program, name);
				attributes.Add(name, attr);
			}
                
			return attr;
		}

		public ShaderUniform GetUniform(string name)
		{
			ShaderUniform uniform;
			if (!uniforms.TryGetValue(name, out uniform))
			{
				uniform = new ShaderUniform(program, name);
				uniforms.Add(name, uniform);
			}

			return uniform;
		}

		public void Dispose()
		{
			if (activeShader == this)
			{
				Disable();
				activeShader = null;
			}

			GL.DeleteProgram(program);
			attributes.Clear();
			uniforms.Clear();
		}

		public virtual void Enable()
		{
			if (activeShader != this)
			{
				if (activeShader != null) activeShader.Disable();
				activeShader = this;

				GL.UseProgram(program);
				foreach (ShaderAttribute attribute in attributes.Values) attribute.Enable();
			}
		}

		public virtual void Disable()
		{
			activeShader = null;
			foreach (ShaderAttribute attribute in attributes.Values) attribute.Disable();
		}

		#region Loading

		public void Reload()
		{
			GL.DeleteProgram(program);

			program = LoadProgram(vertexPath, fragmentPath);
			foreach (ShaderAttribute attribute in attributes.Values) attribute.Reload(program);
			foreach (ShaderUniform uniform in uniforms.Values) uniform.Reload(program);
		}

		private static int LoadProgram(string vertex, string fragment)
		{
			int vert = LoadShader(vertex, ShaderType.VertexShader);
			int frag = LoadShader(fragment, ShaderType.FragmentShader);

			GL.CreateProgram();
			int program = GL.CreateProgram();
			GL.AttachShader(program, vert);
			GL.AttachShader(program, frag);
			GL.LinkProgram(program);

			int status;
			GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
			if (status == 0)
			{
				string message = "Cannot load program:\n" + GL.GetProgramInfoLog(status);
				GL.DeleteProgram(program);
				throw new Exception(message);
			}

			return program;
		}

		private static readonly String PreProcessor = String.Join(
			                                              "\n", 
			                                              new[]
			{
				"#version 120",
				"#define lowp",
				"#define mediump",
				"#define highp",
			}
		                                              ) + "\n";

		private static int LoadShader(string name, ShaderType type)
		{
			string contents;
			using (StreamReader reader = new StreamReader(name))
			{
				contents = reader.ReadToEnd();
			}

			int shader = GL.CreateShader(type);
			GL.ShaderSource(shader, PreProcessor + contents);

			GL.CompileShader(shader);

			int status;
			GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
			if (status == 0)
			{
				string message = "Cannot load shader: " + name + "\n" + GL.GetShaderInfoLog(shader);
				GL.DeleteShader(shader);
				throw new Exception(message);
			}

			return shader;
		}

		#endregion
	}
}

