using System;
using System.Collections.Generic;
using VoxelWars.Universe;
using VoxelWars.Vector;

namespace VoxelWars
{
	class Program
	{
		public static void Main(string[] args)
		{
			using (var game = new CoreGame())
			{
				game.Run(60);
			}
		}
	}
}