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
	internal class GridMapper {
		char[,] originalGrid;
		public Border[,] gridUI;
		public int[,] battles;

		public Dictionary<char, Brush> terrainColors = new Dictionary<char, Brush> {
			{ 'M', Brushes.SaddleBrown },
			{ 'P', Brushes.LightGray },
			{ 'R', Brushes.Gray },
			{ 'B', Brushes.DarkGoldenrod },
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
			this.gridUI = gridUI;
			this.originalGrid = grid;
			DrawGrid(grid);
		}

		public void DrawGrid(char[,] grid) {
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
			battles = new int[42, 42];

			int currentBattle = 0;
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {
					if(terrainWeights.TryGetValue(grid[i,j], out int weight)) {
						weightGrid[i, j] = weight;
						battles[i, j] = -1;
					} else if(grid[i,j] == 'B') {
						weightGrid[i, j] = Convert.ToInt32(battleTimes[currentBattle]);
						battles[i, j] = currentBattle;
						currentBattle++;
					}
				}
			}

			return weightGrid;
		}

		public void DrawPath(List<Node> path) {
			foreach(Node node in path) {
				gridUI[node.row, node.col].Background = Brushes.Blue;
			}
		}

		private int currentStep = 0;
		public int DrawStep(List<Node> path) {
			Node node = path[currentStep++];
			gridUI[node.row, node.col].Background = Brushes.Blue;
			return node.g;
		}

		public void ResetPath() {
			currentStep = 0;
			DrawGrid(originalGrid);
		}
	}

	public class Position {
		public int row;
		public int col;

		public Position() { }

		public Position(int row, int col) {
			this.row = row;
			this.col = col;
		}

		public override bool Equals(object obj) {
			Position other = obj as Position;
			return (this.row == other.row) && (this.col == other.col);
		}

		public override int GetHashCode() {
			return (row, col).GetHashCode();
		}
	}
}
