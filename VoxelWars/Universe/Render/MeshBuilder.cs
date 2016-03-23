#define GL
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VoxelWars.Shaders;
using VoxelWars.Utils;
using VoxelWars.Vector;

namespace VoxelWars.Universe.Render
{
	public struct Slice
	{
		public readonly int Start;
		public readonly int Size;
		public readonly IBlockShader Shader;
    	
		public Slice(int start, int size, IBlockShader shader)
		{
			Start = start;
			Size = size;
			Shader = shader;
		}
	}
	
	/// <summary>
	/// Used to build a set of meshes
	/// </summary>
	public sealed class MeshBuilder
	{
		public readonly IndexedCollection<Vector2, DynamicBuffer<Vector2>> Vertexes = new IndexedCollection<Vector2, DynamicBuffer<Vector2>>();
		public readonly DynamicBuffer<ushort> Indexes = new DynamicBuffer<ushort>();
		
		private readonly Chunk chunk;
		
		public MeshBuilder(Chunk chunk)
		{
			this.chunk = chunk;
		}
		
		public Slice BuildThreshold(Block threshold, IBlockShader shader)
		{
			int start = Indexes.Size, count = Indexes.Index;
			BuildTriangles(threshold);
			return new Slice(start, Indexes.Index - count, shader);
		}
		
		public bool Bind(VBO verticies, VBO indicies)
		{
			if (Vertexes.Collection.Size == 0)
			{
				verticies.Release();
				indicies.Release();
				
				return false;
			}
			else
			{
				verticies.Aquire();
				indicies.Aquire();
				
				GL.BindBuffer(BufferTarget.ArrayBuffer, verticies.Id);
				GL.BufferData(BufferTarget.ArrayBuffer, Vertexes.Collection.Size, Vertexes.Collection.Data, BufferUsageHint.StaticDraw);

				GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicies.Id);
				GL.BufferData(BufferTarget.ElementArrayBuffer, Indexes.Size, Indexes.Data, BufferUsageHint.StaticDraw);
				
				return true;
			}
		}
		
		private void BuildTriangles(Block threshold)
		{
			for (int x = -1; x < Chunk.ChunkSize; x++)
			{
				for (int y = -1; y < Chunk.ChunkSize; y++)
				{
					Block SW = chunk.CurrentAccessor[x, y].Block;
					Block SE = chunk.CurrentAccessor[x + 1, y].Block;
					Block NE = chunk.CurrentAccessor[x + 1, y + 1].Block;
					Block NW = chunk.CurrentAccessor[x, y + 1].Block;

					if (x == -1 || y == -1) continue;
                	
					int flag = (
					               (threshold.Contains(SW) ? 0 : 1) +
					               (threshold.Contains(SE) ? 0 : 2) +
					               (threshold.Contains(NE) ? 0 : 4) +
					               (threshold.Contains(NW) ? 0 : 8)
					           );
                	
					Position position = new Position(x, y);
                	
					switch (flag)
					{
						case 0:
							AddQuad(new Vector2(x, y), 1.0f, 1.0f);
							break;
						case 1:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 centre = new Vector2(x + 1, y + 1);
                				
								AddTriangle(new Vector2(x, y + 1), west, centre);
								AddTriangle(west, south, centre);
								AddTriangle(south, new Vector2(x + 1, y), centre);
								break;
							}
						case 2:
							{
								Vector2 east = Interpolate(position, Side.East);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 centre = new Vector2(x, y + 1);
                				
								AddTriangle(new Vector2(x + 1, y + 1), east, centre);
								AddTriangle(east, south, centre);
								AddTriangle(south, new Vector2(x, y), centre);
								break;
							}
						case 3:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 east = Interpolate(position, Side.East);
								Vector2 top = new Vector2(x + 1, y + 1);
                				
								AddTriangle(west, top, new Vector2(x, y + 1));
								AddTriangle(east, top, west);
								break;
							}
						case 4:
							{
								Vector2 east = Interpolate(position, Side.East);
								Vector2 north = Interpolate(position, Side.North);
								Vector2 centre = new Vector2(x, y);
                				
								AddTriangle(new Vector2(x + 1, y), east, centre);
								AddTriangle(east, north, centre);
								AddTriangle(north, new Vector2(x, y + 1), centre);
								break;
							}
						case 5:
							{
								AddTriangle(
									new Vector2(x + 1, y), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.South)
								);
								AddTriangle(
									new Vector2(x, y + 1), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.North)
								);
								break;
							}
						case 6:
							{
								Vector2 north = Interpolate(position, Side.North);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 left = new Vector2(x, y);
                				
								AddTriangle(north, left, new Vector2(x, y + 1));
								AddTriangle(north, left, south);
								break;
							}
						case 7:
							{
								AddTriangle(
									new Vector2(x, y + 1), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.North)
								);
								break;
							}
						case 8:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 north = Interpolate(position, Side.North);
								Vector2 centre = new Vector2(x + 1, y);
                				
								AddTriangle(new Vector2(x, y), west, centre);
								AddTriangle(west, north, centre);
								AddTriangle(north, new Vector2(x + 1, y + 1), centre);
								break;
							}
						case 9:
							{
								Vector2 north = Interpolate(position, Side.North);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 right = new Vector2(x + 1, y);
                				
								AddTriangle(north, right, new Vector2(x + 1, y + 1));
								AddTriangle(north, right, south);
								break;
							}
						case 10:
							{
								AddTriangle(
									new Vector2(x + 1, y + 1), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.North)
								);
								AddTriangle(
									new Vector2(x, y), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.South)
								);
								break;
							}
						case 11:
							{
								AddTriangle(
									new Vector2(x + 1, y + 1), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.North)
								);
								break;
							}
						case 12:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 east = Interpolate(position, Side.East);
								Vector2 bottom = new Vector2(x + 1, y);
                				
								AddTriangle(west, bottom, new Vector2(x, y));
								AddTriangle(east, bottom, west);
								break;
							}
						case 13:
							{
								AddTriangle(
									new Vector2(x + 1, y), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.South)
								);
								break;
							}
						case 14:
							{
								AddTriangle(
									new Vector2(x, y), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.South)
								);
								break;
							}
                			
                			
					}
				}
			}	
		}
		
		private Vector2 Interpolate(Position position, Side side)
		{
			SByte2 axis = side.Axis();
			SByte2 oDir = side.Opposite().Offset();
			Vector2 vec = Vector2.One * 0.5f;
			
			return new Vector2(position.X + -oDir.X + vec.X * axis.Y, position.Y + -oDir.Y + vec.Y * axis.X);
		}
		
		private void AddQuad(Vector2 bottomLeft, float width, float height)
		{
			AddTriangle(
				bottomLeft,
				new Vector2(bottomLeft.X + width, bottomLeft.Y),
				new Vector2(bottomLeft.X + width, bottomLeft.Y + height)
			);
			
			AddTriangle(
				bottomLeft,
				new Vector2(bottomLeft.X, bottomLeft.Y + height),
				new Vector2(bottomLeft.X + width, bottomLeft.Y + height)
			);
		}
		
		private void AddTriangle(Vector2 a, Vector2 b, Vector2 c)
		{
			Indexes.Add((ushort)Vertexes.Add(a));
			Indexes.Add((ushort)Vertexes.Add(b));
			Indexes.Add((ushort)Vertexes.Add(c));
		}
	}
}
