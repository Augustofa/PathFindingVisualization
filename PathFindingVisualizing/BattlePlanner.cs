using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingVisualizing {
	internal class BattlePlanner {
		public int[] battles = { 50, 55, 60, 70, 75, 80, 85, 90, 95, 100, 110, 120 };
		public double[] powers = { 1.5, 1.4, 1.3, 1.2, 1.1 };

		Dictionary<int, int> path;

		public double PlanBattles() {
			int[] lives = Enumerable.Repeat(5, 5).ToArray();
			Dictionary<int, double> memory = new Dictionary<int, double>();
			path = new Dictionary<int, int>();

			double bestTime = PlanNextBattle(0, lives, memory, path);
			PrintSolution();

			return bestTime;
		}

		private double PlanNextBattle(int battle, int[] currentLives, Dictionary<int, double> memory, Dictionary<int, int> path) {
			if(battle == 12) {
				return 0;
			}

			var state = EncodeState(battle, currentLives);
			if(memory.TryGetValue(state, out var cached)) {
				return cached;
			}

			double best = double.MaxValue;
			int bestMask = -1;
			for(int mask = 1; mask < (1 << 5); mask++) {
				double battlePower = 0;
				int[] auxLives = (int[])currentLives.Clone();
				bool valid = true;

				for(int k = 0; k < 5; k++) {
					if((mask & (1 << k)) != 0) {
						if(auxLives[k] == 0) {
							valid = false;
							break;
						}
						auxLives[k]--;
						battlePower += powers[k];
					}
				}

				if(!valid) continue;

				double battleTime = battles[battle] / battlePower;
				double totalTime = battleTime + PlanNextBattle(battle + 1, auxLives, memory, path);

				if(totalTime < best) {
					best = totalTime;
					bestMask = mask;
				}
			}

			memory[state] = best;
			path[state] = bestMask;
			return best;
		}

		// encodes the battle index + the remaining lives as a state for memoization
		private int EncodeState(int battle, int[] remainingLives) {
			int code = battle;
			for(int i = 0; i < remainingLives.Length; i++) {
				code = code * 6 + remainingLives[i];
			}
			return code;
		}

		private double CalcBattleTime(int battle, int mask) {
			double powerSum = 0;
			for(int k = 0; k < powers.Length; k++) {
				if((mask & (1 << k)) != 0) {
					powerSum += powers[k];
				}
			}
			return battles[battle] / powerSum;
		}

		private List<List<int>> PrintSolution() {
			List<List<int>> solution = new List<List<int>>();
			int[] remainingLives = Enumerable.Repeat(5, 5).ToArray();
			int battle = 0;

			while(battle < 12) {
				int state = EncodeState(battle, remainingLives);
				int mask = path[state];
				List<int> team = new List<int>();
				for(int k = 0; k < powers.Length; k++) {
					if((mask & (1 << k)) != 0) {
						team.Add(k);
						remainingLives[k]--;
					}
				}
				solution.Add(team);
				Debug.Print(battles[battle] + " - " + string.Join(",", team) + " = " + CalcBattleTime(battle, mask).ToString());
				battle++;
			}

			return solution;
		}

		public double[] GetBattleTimes() {
			double[] battleTimes = new double[12];
			int[] remainingLives = Enumerable.Repeat(5, 5).ToArray();
			int battle = 0;

			while(battle < 12) {
				int state = EncodeState(battle, remainingLives);
				int mask = path[state];
				List<int> team = new List<int>();
				for(int k = 0; k < powers.Length; k++) {
					if((mask & (1 << k)) != 0) {
						team.Add(k);
						remainingLives[k]--;
					}
				}
				battleTimes[battle] = CalcBattleTime(battle, mask);
				battle++;
			}
			Debug.Print(string.Join(",", battleTimes));

			return battleTimes;
		}
	}
}
