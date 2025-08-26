using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace PathFindingVisualizing {
	public class Position {
		public int terrain;
	}

	internal class GridMapper {
		public Dictionary<int, Brush> terrainColors = new Dictionary<int, Brush> {
			{ 200, Brushes.SaddleBrown },
			{ 1, Brushes.LightGray },
			{ 5, Brushes.Gray },
			{ 0, Brushes.DarkGreen },
			{ -2, Brushes.DarkRed },
			{ -1, Brushes.DarkBlue }
		};
		public int[,] grid = MapLoader.LoadMap();

		public GridMapper(Border[,] gridUI) {
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {
					if(!terrainColors.ContainsKey(grid[i, j])) continue;
					
					gridUI[i, j].Background = terrainColors[grid[i, j]];
				}
			}
		}
	}
}
