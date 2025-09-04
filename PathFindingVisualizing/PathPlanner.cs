using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingVisualizing {
	internal class PathPlanner {
		public int[,] grid;
		public int[,] waypoints;
		Node start, goal;

		public List<Node> bestPath;
		public List<Node> searchPath = new List<Node>();

		public PathPlanner(int[,] grid, int[,] waypoints, Tuple<Position, Position> startAndGoalPos) {
			this.grid = grid;
			this.waypoints = waypoints;
			goal = new Node(startAndGoalPos.Item2.row, startAndGoalPos.Item2.col) {
				waypointMask = (1 << 12) - 1
			};
			start = new Node(startAndGoalPos.Item1.row, startAndGoalPos.Item1.col) { 
				g = 0,
				h = Heuristic(startAndGoalPos.Item1.row, startAndGoalPos.Item1.col, 0)
			};

		}

		public List<Node> FindBestPath() {
			int maskCount = 1 << 12;
			int[,,] gScore = new int[42, 42, maskCount];

			for(int r = 0; r < 42; r++)
				for(int c = 0; c < 42; c++)
					for(int m = 0; m < maskCount; m++)
						gScore[r, c, m] = int.MaxValue;

			gScore[start.row, start.col, start.waypointMask] = 0;

			var nextNodes = new SortedSet<Node>();
			searchPath = new List<Node>();
			var firstVisited = new HashSet<(int, int)>();

			nextNodes.Add(start);

			while(nextNodes.Count > 0) {
				var current = nextNodes.Min;
				nextNodes.Remove(current);

				var posKey = (current.row, current.col);
				if(!firstVisited.Contains(posKey)) {
					firstVisited.Add(posKey);
					searchPath.Add(current);
				}

				if(current.row == goal.row && current.col == goal.col &&
					current.waypointMask == goal.waypointMask) {
					bestPath = RetracePath(current);
					return bestPath;
				}

				foreach(var (dr, dc) in Directions) {
					int nr = current.row + dr;
					int nc = current.col + dc;

					if(nr < 0 || nc < 0 || nr >= 42 || nc >= 42) continue;

					int newMask = current.waypointMask;
					if(waypoints[nr, nc] != -1) {
						newMask |= (1 << waypoints[nr, nc]);
					}

					int tentativeG = current.g + grid[nr, nc];

					if(tentativeG < gScore[nr, nc, newMask]) {
						gScore[nr, nc, newMask] = tentativeG;

						var neighbor = new Node(nr, nc) {
							g = tentativeG,
							h = Heuristic(nr, nc, newMask),
							waypointMask = newMask,
							parent = current
						};

						nextNodes.Add(neighbor);
					}
				}
			}

			return null;
		}

		private IEnumerable<(int row, int col)> GetNeighbors(Node current) {
			foreach(var (drow, dcol) in Directions) {
				int row = current.row + drow;
				int col = current.col + dcol;
				if(row >= 0 && col >= 0 && row < 42 && col < 42) {  
					yield return (row, col);
				}
			}
		}

		private List<Node> RetracePath(Node current) {
			var path = new List<Node>();
			while(current != null) {
				path.Add(current);
				current = current.parent;
			}
			path.Reverse();

			return path;
		}

		private void PrintGrid() {
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {
					Debug.Write(grid[i, j]);
				}
				Debug.Write("\n");
			}
		}

		public int GetPathTime() {
			if(bestPath == null) return int.MaxValue;
			return bestPath[bestPath.Count - 1].g;
		}

		private int Heuristic(int row, int col, int mask) {
			return WaypointHeuristic(row, col, mask);
		}

		private int WaypointHeuristic(int row, int col, int waypointMask) {
			int bestDist = int.MaxValue;

			for(int i = 0; i < 12; i++) {
				if((waypointMask & (1 << i)) != 0) continue;

				bestDist = Math.Min(bestDist, Math.Abs(waypoints[i,0] - row) + Math.Abs(waypoints[i,1] - col) + Math.Abs(goal.row - waypoints[i, 0]) + Math.Abs(goal.row - waypoints[i, 1]));
			}

			if(bestDist == int.MaxValue) {
				return GoalHeuristic(row, col);
			}
			
			return bestDist;
		}

        private int GoalHeuristic(int row, int col) {
            return Math.Abs(goal.row - row) + Math.Abs(goal.col - col);
        }

		HashSet<Tuple<int, int>> searchedPos = new HashSet<Tuple<int, int>>();
		public void RemoveRepeatsFromSearch() {
			if(searchedPos.Count > 0) return;
			for(int i = 0; i < searchPath.Count; i++) {
				var pos = new Tuple<int,int>(searchPath[i].row, searchPath[i].col);
				if(searchedPos.Contains(pos)) { 
					searchPath.RemoveAt(i);
					i--;
				}
				searchedPos.Add(pos);
			}
		}

		public List<Node> FindBestPathAlternative() {
			var nextNodes = new SortedSet<Node>();
			var visitedNodes = new HashSet<Node>();

			nextNodes.Add(start);

			while(nextNodes.Count > 0) {
				var current = nextNodes.First();

				nextNodes.Remove(current);

				if(!visitedNodes.Add(current)) {
					continue;
				}
				searchPath.Add(current);

				if(current.Equals(goal)) {
					bestPath = RetracePath(current);
					return bestPath;
				}

				foreach(var (row, col) in GetNeighbors(current)) {
					int newMask = current.waypointMask;

					// Checks if position is a necessary waypoint
					if(waypoints[row, col] != -1) {
						newMask |= (1 << waypoints[row, col]);
					}

					var neighbor = new Node(row, col) {
						g = current.g + grid[row, col],
						h = Heuristic(row, col, newMask),
						waypointMask = newMask,
						parent = current
					};

					nextNodes.Add(neighbor);
				}
			}

			return null;
		}


		private static readonly (int dr, int dc)[] Directions = new (int, int)[] {
			(0, 1), 
			(0, -1),
			(1, 0), 
			(-1, 0) 
		};

	}
	public class Node : IComparable<Node> {
		public int row, col;
		public int g, h;
		public int f => g + h;
		public int waypointMask;
		public Node parent;

		public Node(int row, int col, int battlesMask = 0) {
			this.row = row;
			this.col = col;
			this.waypointMask = battlesMask;
		}

		public int CompareTo(Node other) {
			int cmp = f.CompareTo(other.f);
			if(cmp == 0) {
				cmp = (row * 42 + col).CompareTo(other.row * 42 + other.col);
			}
			return cmp;
		}

		public override int GetHashCode() {
			return (row, col, waypointMask).GetHashCode();
		}

		public override bool Equals(object obj) {
			var other = obj as Node;
			return (row == other.row) && (col == other.col) && (waypointMask == other.waypointMask);
		}
	}
}
