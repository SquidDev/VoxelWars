using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VoxelWars.Shaders;
using VoxelWars.Universe;
using VoxelWars.Universe.Render;
using VoxelWars.Vector;

namespace VoxelWars
{
	public class CoreGame : GameWindow
	{
		private readonly World world = new World();

		private Vector2 position = new Vector2(0, 16);
		private float zoom = 20;
		private byte current = 0;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			VSync = VSyncMode.On;
			GL.ClearColor(Color.CornflowerBlue);

			GL.PolygonOffset(1, 1);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			if (Keyboard[Key.ShiftLeft] || Keyboard[Key.ShiftRight])
			{
				current = (byte)MathHelper.Clamp(current + e.Delta, 0, Block.Blocks.Length - 1);
				Console.WriteLine(current);
			}
			else
			{
				zoom *= 1 + 0.1f * e.DeltaPrecise;
			}
		}

		protected void Place(int mouseX, int mouseY, Block block)
		{
			Matrix4 matrix = Matrix;
			matrix.Invert();

			Vector4 mouse = new Vector4(
				                mouseX / (float)Width * 2 - 1, 
				                1 - (mouseY / (float)Height * 2),
				                0, 
				                1.0f
			                );

			Vector4 transform = Vector4.Transform(mouse, matrix);
			int xPos = (int)Math.Round(transform.X);
			int yPos = (int)Math.Round(transform.Y);

			int zoom = (int)(1 / this.zoom * 10);
			int half = zoom * zoom / 4;
			if (zoom <= 0)
			{
				zoom = 1;
				half = 2;
			}


			for (int x = -zoom / 2; x <= zoom / 2; x++)
			{
				for (int y = -zoom / 2; y <= zoom / 2; y++)
				{
					if (x * x + y * y <= half) world.SetBlock(new Position(xPos + x, yPos + y), new BlockData(block));
				}
			}
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Mouse.LeftButton == ButtonState.Pressed)
			{
				Place(e.X, e.Y, Block.Air);
			}
			else if (e.Mouse.RightButton == ButtonState.Pressed)
			{
				Place(e.X, e.Y, Block.Blocks[current]);
			}
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
			if (Keyboard[Key.Escape])
			{
				Exit();
				return;
			}

			float speed = 1f;

			if (Keyboard[Key.ControlLeft]) speed *= 2;

			Vector2 forward = new Vector2(0, 1) * speed;
			Vector2 right = new Vector2(1, 0) * -speed;

			if (Keyboard[Key.W]) position += forward;
			if (Keyboard[Key.S]) position -= forward;
			if (Keyboard[Key.A]) position += right;
			if (Keyboard[Key.D]) position -= right;

			#if __MONO_CS__
			if (Mouse[MouseButton.Left])
			{
				Place(Mouse.GetCursorState().X, Mouse.GetCursorState().Y, Block.Air);
			}

			if (Mouse[MouseButton.Right])
			{
				Place(Mouse.GetCursorState().X, Mouse.GetCursorState().Y, Block.Air);
			}
			#endif

			Position pos = new Position((int)position.X, (int)position.Y) / Chunk.ChunkSize;
			world.Update(pos);
		}
		
		public Matrix4 Matrix
		{
			get
			{
				Matrix4 view = Matrix4.CreateTranslation(-position.X, -position.Y, 0) * Matrix4.CreateScale(zoom, zoom, zoom);
				Matrix4 projection = Matrix4.CreateOrthographicOffCenter(-Width / 2, Width / 2, -Height / 2, Height / 2, -1, 4);
				return view * projection;
			}
		}

		private float fps = 60;
		
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			float current = (float)(1 / e.Time);
//			if (Math.Abs(current - fps) > 10)
//			{
//				Console.WriteLine("High FPS change " + current + " => " + fps);
//			}
			
			fps = fps * 0.9f + current * 0.1f;
			
			Title = "Core Game: " + Math.Round(fps) + "fps";
			
			base.OnRenderFrame(e);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.PolygonOffsetFill);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			Position pos = new Position((int)position.X, (int)position.Y) / Chunk.ChunkSize;
			world.Render(Matrix, pos);

			SwapBuffers();
		}
	}
}

