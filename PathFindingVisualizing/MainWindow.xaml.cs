using System;
using System.Collections.Generic;
using System.Diagnostics;
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
					rect.MouseLeftButtonDown += PaintSquareClick;
					MapView.Children.Add(rect);
				}
			}

			grid = MapLoader.LoadMap();
			gridMapper = new GridMapper(gridUI, grid);
		}

		private void StartClick(object sender, RoutedEventArgs e) {
			battlePlanner = new BattlePlanner();
			double battleTime = battlePlanner.PlanBattles();
			battleTimeTxt.Text = battleTime.ToString("0");

			double[] battleTimes = battlePlanner.GetBattleTimes();
			int[,] weightGrid = gridMapper.ConvertToWeightMap(grid, battleTimes);
			Tuple<Position, Position> startAndEndPos = gridMapper.GetStartAndGoalPosition(grid);

			pathPlanner = new PathPlanner(weightGrid, gridMapper.battles, startAndEndPos);
			bestPath = pathPlanner.FindBestPath();
			int bestTime = pathPlanner.GetPathTime();
			bestTimeTxt.Text = bestTime.ToString();

			resultsPanel.Visibility = Visibility.Visible;
			ResetPath();
		}

		private void ViewPathClick(object sender, RoutedEventArgs e) {
			ResetPath();
			gridMapper.DrawPath(bestPath);
			currentTimeTxt.Text = bestTimeTxt.Text;
		}

		private void NextStepClick(object sender, RoutedEventArgs e) {
			int currentTime = gridMapper.DrawStep(bestPath);
			currentTimeTxt.Text = currentTime.ToString();
		}

        private void PlayStepsClick(object sender, RoutedEventArgs e) {
			ResetPath();

			ToggleButtons(false);
			PlayStepsAsync();
        }

		private async Task PlayStepsAsync() {
            for(int i = 0; i < bestPath.Count; i++) {
                NextStepClick(null, null);
				await Task.Delay(100);
            }
			ToggleButtons(true);
        }

		private void ToggleButtons(bool state) {
			customMapBtn.IsEnabled = state;
            startBtn.IsEnabled = state;
            nextStepBtn.IsEnabled = state;
            playStepsBtn.IsEnabled = state;
            viewPathBtn.IsEnabled = state;
        }

		private void ResetPath() {
			gridMapper.ResetPath();
			currentTimeTxt.Text = "0";
		}

		private void CustomMapClick(object sender, RoutedEventArgs e) {
			infoStack.Visibility = Visibility.Hidden;
			customMapStack.Visibility = Visibility.Visible;
		}

		private void ConfirmCustomMapClick(object sender, RoutedEventArgs e) {
			infoStack.Visibility = Visibility.Visible;
			customMapStack.Visibility = Visibility.Hidden;
		}

		private char selectedTerrain = 'R';
		private void PaintSquareClick(object sender, RoutedEventArgs e) {
			var border = sender as Border;
			if(border?.Tag is Tuple<int,int> pos) {
				int row = pos.Item1;
				int col = pos.Item2;

				grid[row, col] = selectedTerrain;
				gridMapper.DrawPosition(row, col, selectedTerrain);
			}
		}

		private void PaletteColorClick(object sender, RoutedEventArgs e) {
			if(sender is Button btn && btn.Tag is string terrain) {
				selectedTerrain = terrain[0];
			}
		}
	}
}
