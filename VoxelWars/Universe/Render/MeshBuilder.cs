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
	
	public struct Vertex
	{
		public readonly Vector2 Vector;
		public readonly float Metadata;
		
		public Vertex(float x, float y, float metadata)
		{
			Vector = new Vector2(x, y);
			Metadata = metadata;
		}
		
		public Vertex(Vector2 pos, float metadata)
		{
			Vector = pos;
			Metadata = metadata;
		}
		
		
		public override bool Equals(object obj)
		{
			return (obj is Vertex) && Equals((Vertex)obj);
		}
		
		public bool Equals(Vertex other)
		{
			return this.Vector == other.Vector && this.Metadata == other.Metadata;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				hashCode += 1000000007 * Vector.GetHashCode();
				hashCode += 1000000009 * Metadata.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(Vertex lhs, Vertex rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Vertex lhs, Vertex rhs)
		{
			return !(lhs == rhs);
		}

	}
	
	/// <summary>
	/// Used to build a set of meshes
	/// </summary>
	public sealed class MeshBuilder
	{
		public readonly IndexedCollection<Vertex, DynamicBuffer<Vertex>> Vertexes = new IndexedCollection<Vertex, DynamicBuffer<Vertex>>();
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
                	
					float sum = 0;
					int count = 0;
					if (threshold.Contains(SW))
					{
						sum += chunk.CurrentAccessor[x, y].Metadata;
						count++;
					}
					   
					if (threshold.Contains(SE))
					{
						sum += chunk.CurrentAccessor[x + 1, y].Metadata;
						count++;
					}
					      
					if (threshold.Contains(NE))
					{
						sum += chunk.CurrentAccessor[x + 1, y + 1].Metadata;
						count++;
					}
					         
					if (threshold.Contains(NW))
					{
						sum += chunk.CurrentAccessor[x, y + 1].Metadata;
						count++;
					}
					           
					float meta = count == 0 ? 0 : (sum / count);
					Position position = new Position(x, y);
                	
					switch (flag)
					{
						case 0:
							AddQuad(new Vector2(x, y), 1.0f, 1.0f, meta);
							break;
						case 1:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 centre = new Vector2(x + 1, y + 1);
                				
								AddTriangle(new Vector2(x, y + 1), west, centre, meta);
								AddTriangle(west, south, centre, meta);
								AddTriangle(south, new Vector2(x + 1, y), centre, meta);
								break;
							}
						case 2:
							{
								Vector2 east = Interpolate(position, Side.East);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 centre = new Vector2(x, y + 1);
                				
								AddTriangle(new Vector2(x + 1, y + 1), east, centre, meta);
								AddTriangle(east, south, centre, meta);
								AddTriangle(south, new Vector2(x, y), centre, meta);
								break;
							}
						case 3:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 east = Interpolate(position, Side.East);
								Vector2 top = new Vector2(x + 1, y + 1);
                				
								AddTriangle(west, top, new Vector2(x, y + 1), meta);
								AddTriangle(east, top, west, meta);
								break;
							}
						case 4:
							{
								Vector2 east = Interpolate(position, Side.East);
								Vector2 north = Interpolate(position, Side.North);
								Vector2 centre = new Vector2(x, y);
                				
								AddTriangle(new Vector2(x + 1, y), east, centre, meta);
								AddTriangle(east, north, centre, meta);
								AddTriangle(north, new Vector2(x, y + 1), centre, meta);
								break;
							}
						case 5:
							{
								AddTriangle(
									new Vector2(x + 1, y), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.South), meta
								);
								AddTriangle(
									new Vector2(x, y + 1), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.North), meta
								);
								break;
							}
						case 6:
							{
								Vector2 north = Interpolate(position, Side.North);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 left = new Vector2(x, y);
                				
								AddTriangle(north, left, new Vector2(x, y + 1), meta);
								AddTriangle(north, left, south, meta);
								break;
							}
						case 7:
							{
								AddTriangle(
									new Vector2(x, y + 1), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.North), meta
								);
								break;
							}
						case 8:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 north = Interpolate(position, Side.North);
								Vector2 centre = new Vector2(x + 1, y);
                				
								AddTriangle(new Vector2(x, y), west, centre, meta);
								AddTriangle(west, north, centre, meta);
								AddTriangle(north, new Vector2(x + 1, y + 1), centre, meta);
								break;
							}
						case 9:
							{
								Vector2 north = Interpolate(position, Side.North);
								Vector2 south = Interpolate(position, Side.South);
								Vector2 right = new Vector2(x + 1, y);
                				
								AddTriangle(north, right, new Vector2(x + 1, y + 1), meta);
								AddTriangle(north, right, south, meta);
								break;
							}
						case 10:
							{
								AddTriangle(
									new Vector2(x + 1, y + 1), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.North), meta
								);
								AddTriangle(
									new Vector2(x, y), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.South), meta
								);
								break;
							}
						case 11:
							{
								AddTriangle(
									new Vector2(x + 1, y + 1), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.North), meta
								);
								break;
							}
						case 12:
							{
								Vector2 west = Interpolate(position, Side.West);
								Vector2 east = Interpolate(position, Side.East);
								Vector2 bottom = new Vector2(x + 1, y);
                				
								AddTriangle(west, bottom, new Vector2(x, y), meta);
								AddTriangle(east, bottom, west, meta);
								break;
							}
						case 13:
							{
								AddTriangle(
									new Vector2(x + 1, y), 
									Interpolate(position, Side.East),
									Interpolate(position, Side.South), meta
								);
								break;
							}
						case 14:
							{
								AddTriangle(
									new Vector2(x, y), 
									Interpolate(position, Side.West),
									Interpolate(position, Side.South), meta
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
		
		private void AddQuad(Vector2 bottomLeft, float width, float height, float meta)
		{
			AddTriangle(
				bottomLeft,
				new Vector2(bottomLeft.X + width, bottomLeft.Y),
				new Vector2(bottomLeft.X + width, bottomLeft.Y + height),
				meta
			);
			
			AddTriangle(
				bottomLeft,
				new Vector2(bottomLeft.X, bottomLeft.Y + height),
				new Vector2(bottomLeft.X + width, bottomLeft.Y + height),
				meta
			);
		}
		
		private void AddTriangle(Vector2 a, Vector2 b, Vector2 c, float meta)
		{
			Indexes.Add((ushort)Vertexes.Add(new Vertex(a, meta)));
			Indexes.Add((ushort)Vertexes.Add(new Vertex(b, meta)));
			Indexes.Add((ushort)Vertexes.Add(new Vertex(c, meta)));
		}
	}
}
