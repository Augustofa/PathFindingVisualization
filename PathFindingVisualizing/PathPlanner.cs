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

		public PathPlanner(int[,] grid, int[,] waypoints, Tuple<Position, Position> startAndGoalPos) {
			this.grid = grid;
			this.waypoints = waypoints;
			goal = new Node(startAndGoalPos.Item2.row, startAndGoalPos.Item2.col) {
				waypointMask = (1 << 12) - 1
			};
			start = new Node(startAndGoalPos.Item1.row, startAndGoalPos.Item1.col) { 
				g = 0,
				h = Heuristic(startAndGoalPos.Item1.row, startAndGoalPos.Item1.col)
			};

		}

		public List<Node> FindBestPath() {
			var nextNodes = new SortedSet<Node>();
			var visitedNodes = new HashSet<Node>();

			nextNodes.Add(start);

			while(nextNodes.Count > 0) {
				var current = nextNodes.First();

				nextNodes.Remove(current);

				if(current.Equals(goal)) {
					bestPath = RetracePath(current);
					return bestPath;
				}
				if(!visitedNodes.Add(current)) {
					continue;
				}

				var currentNeighbors = GetNeighbors(current);
				foreach(var neighbor in currentNeighbors) {
					int newMask = current.waypointMask;

					// Checks if position is a necessary waypoint
					if(waypoints[neighbor.row, neighbor.col] != -1) {
						newMask |= (1 << waypoints[neighbor.row, neighbor.col]);
					}

					neighbor.g = current.g + grid[neighbor.row, neighbor.col];
					neighbor.h = Heuristic(neighbor.row, neighbor.col);
					neighbor.waypointMask = newMask;
					neighbor.parent = current;

					nextNodes.Add(neighbor);
				}
			}

			return null;
		}

		private List<Node> GetNeighbors(Node current) {
			var neighbors = new List<Node>();
			foreach(var (drow, dcol) in Directions) {
				int row = current.row + drow;
				int col = current.col + dcol;
				if(row >= 0 && col >= 0 && row < 42 && col < 42) {  
					neighbors.Add(new Node(row, col));
				}
			}
			return neighbors;
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
			//foreach(var node in bestPath) {
			//	Console.WriteLine(node.row + " " + node.col);
			//}
			return bestPath[bestPath.Count - 1].g;
		}

		private int Heuristic(int row, int col) {
			return GoalHeuristic(row, col);
		}

		private int WaypointHeuristic(int row, int col) {
			int bestDist = int.MaxValue;
			for(int i = 0; i < 12; i++) {
				bestDist = Math.Min(bestDist, Math.Abs(waypoints[i,0] - row) + Math.Abs(waypoints[i,1] - col));
			}
			return bestDist;
		}

        private int GoalHeuristic(int row, int col) {
            return Math.Abs(goal.row - row) + Math.Abs(goal.col - col);
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
