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
		public int row;
		public int col;

		public Position() { }

		public Position(int row, int col) {
			this.row = row;
			this.col = col;
		}
	}

	internal class GridMapper {
		public Dictionary<char, Brush> terrainColors = new Dictionary<char, Brush> {
			{ 'M', Brushes.SaddleBrown },
			{ 'P', Brushes.LightGray },
			{ 'R', Brushes.Gray },
			{ 'S', Brushes.DarkGreen },
			{ 'G', Brushes.DarkRed },
			//{ -1, Brushes.DarkBlue }
		};

		public Dictionary<char, int> terrainWeights = new Dictionary<char, int> {
			{ 'M', 200 },
			{ 'P', 1 },
			{ 'R', 5 },
			{ 'S', 0 },
			{ 'G', 0 },
		};

		public GridMapper(Border[,] gridUI, char[,] grid) {
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {
					if(!terrainColors.ContainsKey(grid[i, j])) continue;
					
					gridUI[i, j].Background = terrainColors[grid[i, j]];
				}
			}
		}

		public Tuple<Position, Position> GetStartAndGoalPosition(char[,] grid) {
			Position start = new Position();
			Position goal = new Position();
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {
					if(grid[i,j] == 'S') {
						start.row = i;
						start.col = j;
					} else if(grid[i,j] == 'G') {
						goal.row = i;
						goal.col = j;
					}
				}
			}
			return new Tuple<Position, Position>(start, goal);
		}

		public int[,] ConvertToWeightMap(char[,] grid, double[] battleTimes) {
			int[,] weightGrid = new int[42,42];

			int currentBattle = 0;
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {
					if(terrainWeights.TryGetValue(grid[i,j], out int weight)) {
						weightGrid[i, j] = weight;
					} else if(grid[i,j] == 'B') {
						weightGrid[i, j] = Convert.ToInt32(battleTimes[currentBattle]);
						currentBattle++;
					}
				}
			}

			return weightGrid;
		}
	}
}
