using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PathFindingVisualizing {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		GridMapper gridMapper;
		BattlePlanner battlePlanner;
		PathPlanner pathPlanner;
		char[,] grid;
		List<Node> bestPath;

		public MainWindow() {
			InitializeComponent();

			Border[,] gridUI = new Border[42,42]; 
			for(int i = 0; i < 42; i++) {
				for(int j = 0; j < 42; j++) {
					var rect = new Border {
						Width = 29,
						Height = 29,
						Background = Brushes.LightGray,
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(1),
						Tag = new Tuple<int, int>(i, j)
					};

					gridUI[i, j] = rect;
					rect.MouseLeftButtonDown += StartPaintingSquares;
					rect.MouseEnter += PaintSquareHover;
					MapView.Children.Add(rect);
				}
			}
			MouseUp += StopPaintingSquares;

			grid = MapLoader.LoadMap();
			gridMapper = new GridMapper(gridUI, grid);
		}

		private void StartClick(object sender, RoutedEventArgs e) {
			battlePlanner = new BattlePlanner();
			double battleTime = battlePlanner.PlanBattles();
			battleTimeTxt.Text = battleTime.ToString("0");

			var stopwatch = Stopwatch.StartNew();
			double[] battleTimes = battlePlanner.GetBattleTimes();
			long totalTime = stopwatch.ElapsedMilliseconds;
			int[,] weightGrid = gridMapper.ConvertToWeightMap(grid, battleTimes);
			Tuple<Position, Position> startAndEndPos = gridMapper.GetStartAndGoalPosition(grid);

			pathPlanner = new PathPlanner(weightGrid, gridMapper.battles, startAndEndPos);
			stopwatch = Stopwatch.StartNew();
			bestPath = pathPlanner.FindBestPath();
			totalTime += stopwatch.ElapsedMilliseconds;
			int bestTime = pathPlanner.GetPathTime();
			bestTimeTxt.Text = bestTime.ToString();

			Double calcTime = totalTime / 1000.0;
			calculationTime.Text = calcTime.ToString("0.000") + "s";
			searchPanel.Visibility = Visibility.Visible;
			resultsPanel.Visibility = Visibility.Visible;
			ResetPath();
		}

		private void ViewPathClick(object sender, RoutedEventArgs e) {
			ResetPath();
			gridMapper.DrawPath(bestPath);
			currentTimeTxt.Text = bestTimeTxt.Text;
		}

		private void NextStepClick(object sender, RoutedEventArgs e) {
			int currentTime = gridMapper.DrawStep(bestPath, Brushes.Blue);
			currentTimeTxt.Text = currentTime.ToString();
		}

		private bool pauseSteps = false;
        private void PlayStepsClick(object sender, RoutedEventArgs e) {
			ResetPath();

			ToggleButtons(false);
			PlayStepsAsync();
			playStepsBtn.Visibility = Visibility.Collapsed;
			pauseStepsBtn.Visibility = Visibility.Visible;
        }

		private async Task PlayStepsAsync() {
            for(int i = 0; i < bestPath.Count; i++) {
				if(pauseSteps) break;
                NextStepClick(null, null);
				await Task.Delay(20);
            }
			pauseSteps = false;
            playStepsBtn.Visibility = Visibility.Visible;
            pauseStepsBtn.Visibility = Visibility.Collapsed;
            ToggleButtons(true);
        }

		private void PauseStepsClick(object sender, RoutedEventArgs e) {
			pauseSteps = true;
            playStepsBtn.Visibility = Visibility.Visible;
            pauseStepsBtn.Visibility = Visibility.Collapsed;
            return;
        }

		private void PlaySearchClick(object sender, RoutedEventArgs e) {
			ResetPath();

			ToggleButtons(false);
			pathPlanner.RemoveRepeatsFromSearch();
			playSearchBtn.Visibility = Visibility.Collapsed;
			pauseSearchBtn.Visibility = Visibility.Visible;
			PlaySearchAsync(pathPlanner.searchPath);
		}

		private async Task PlaySearchAsync(List<Node> searchPath) {
			playStepsBtn.IsEnabled = false;
			for(int i = 0; i < searchPath.Count; i++) {
				if(pauseSteps) break;
				gridMapper.DrawStep(searchPath, Brushes.Red);
				await Task.Delay(1);
			}
			pauseSteps = false;
			playSearchBtn.Visibility = Visibility.Visible;
			pauseSearchBtn.Visibility = Visibility.Collapsed;
			playStepsBtn.IsEnabled = true;
			ToggleButtons(true);
		}

		private void PauseSearchClick(object sender, RoutedEventArgs e) {
			pauseSteps = true;
			playSearchBtn.Visibility = Visibility.Visible;
			pauseSearchBtn.Visibility = Visibility.Collapsed;
			return;
		}

		private void ToggleButtons(bool state) {
			playSearchBtn.IsEnabled = state;
			customMapBtn.IsEnabled = state;
            startBtn.IsEnabled = state;
            nextStepBtn.IsEnabled = state;
            viewPathBtn.IsEnabled = state;
        }

		private void ResetPath() {
			gridMapper.ResetPath();
			currentTimeTxt.Text = "0";
		}

		private void CustomMapClick(object sender, RoutedEventArgs e) {
			ResetPath();
			infoStack.Visibility = Visibility.Hidden;
			customMapStack.Visibility = Visibility.Visible;
		}

		private void ConfirmCustomMapClick(object sender, RoutedEventArgs e) {
			infoStack.Visibility = Visibility.Visible;
			customMapStack.Visibility = Visibility.Hidden;
		}

		private char selectedTerrain = 'R';
		private bool paintingTerrain = false;
		private void StartPaintingSquares(object sender, RoutedEventArgs e) {
			paintingTerrain = true;
			PaintSquareHover(sender, e);
		}

		private void PaintSquareHover(object sender, RoutedEventArgs e) {
			if(!paintingTerrain) return;
			var border = sender as Border;
			if(border?.Tag is Tuple<int,int> pos) {
				int row = pos.Item1;
				int col = pos.Item2;

				if(gridMapper.IsSpecialTerrain(grid[row, col])) {
					paintingTerrain = false;
					MessageBox.Show("Posição especial, não foi possível trocar");
					return;
				};
				grid[row, col] = selectedTerrain;
				gridMapper.DrawPosition(row, col, selectedTerrain);
			}
		}

		private void StopPaintingSquares(object sender, RoutedEventArgs e) {
			paintingTerrain = false;
		}

		private void PaletteColorClick(object sender, RoutedEventArgs e) {
			if(sender is Button btn && btn.Tag is string terrain) {
				selectedTerrain = terrain[0];
			}
		}
	}
}
