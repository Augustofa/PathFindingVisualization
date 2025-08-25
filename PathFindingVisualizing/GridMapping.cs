using System;
using System.Collections.Generic;
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

	internal class GridMapping {
		public Dictionary<int, Brush> terrainColors = new Dictionary<int, Brush> {
			{ 200, Brushes.DarkGray },
			{ 1, Brushes.LightGray },
			{ 5, Brushes.Gray }
		};

		public int[,] grid;
		public Border[,] gridUI;

		public GridMapping(Border[,] gridUI) {
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {

				}
			}
		}
	}
}
