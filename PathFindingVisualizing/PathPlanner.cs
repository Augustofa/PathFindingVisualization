using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingVisualizing {
	internal class PathPlanner {
		int[,] grid;
		Position start, goal;

		public PathPlanner(int[,] grid, Tuple<Position, Position> startAndGoalPos) {
			this.grid = grid;
			start = startAndGoalPos.Item1;
			goal = startAndGoalPos.Item2;
		}

		public void CalcBestPath() {

		}

		private class State : IComparable<State> {
			public Position pos;
			public int g, h;
			public int f => g + h;
			public State parent;

			public State(int row, int col) {
				pos = new Position(row, col);
			}

			public int CompareTo(State other) {
				return f.CompareTo(other.f);
			}
		}
	}
}
