using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Linq;

namespace VoxelWars.Universe.Render
{
	public sealed class VBOHandler : IDisposable
	{
		public static readonly VBOHandler Instance = new VBOHandler();

		public Queue<int> vbos = new Queue<int>();

		public VBOHandler()
		{
		}

		public int Aquire()
		{
			return vbos.Count > 0 ? vbos.Dequeue() : GL.GenBuffer();
		}

		public void Release(int id)
		{
			vbos.Enqueue(id);
		}

		public void Dispose()
		{
			foreach (int vbo in vbos) GL.DeleteBuffer(vbo);
		}
	}

	public sealed class VBO
	{
		private int id;
		private bool active;
		
		public int Id
		{ 
			get
			{ 
				if (!active) throw new InvalidOperationException("Cannot get Id from disabled VBO");
				return id;
			} 
		}

		public void Aquire()
		{
			if (!active)
			{
				active = true;
				id = VBOHandler.Instance.Aquire();
			}
		}

		public void Release()
		{
			if (active)
			{
				active = false;
				VBOHandler.Instance.Release(id);
			}
		}
	}
}

