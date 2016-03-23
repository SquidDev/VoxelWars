using System;
using System.Collections.Generic;
using System.Xml.Linq;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace VoxelWars.Loaders
{
	public class ShaderData
	{
		public string Fragment;
		public string Vertex;

		// public readonly IReadOnlyDictionary<string, string> Textures;
		public bool Timer = false;
		public bool Offset = false;
		public bool Metadata = false;
		public Vector3 Color;

		public IReadOnlyList<EnableCap> Enabled = new List<EnableCap>();
		public IReadOnlyList<EnableCap> Disabled = new List<EnableCap>();
	}
}
