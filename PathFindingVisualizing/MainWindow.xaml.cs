using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
						BorderThickness = new Thickness(1)
					};

					gridUI[i, j] = rect;
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
            startBtn.IsEnabled = state;
            nextStepBtn.IsEnabled = state;
            playStepsBtn.IsEnabled = state;
            viewPathBtn.IsEnabled = state;
        }

		private void ResetPath() {
			gridMapper.ResetPath();
			currentTimeTxt.Text = "0";
		}
    }
}
