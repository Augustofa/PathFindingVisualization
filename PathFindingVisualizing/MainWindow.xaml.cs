using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			double bestTime = battlePlanner.PlanBattles();
			battleTimeTxt.Text = bestTime.ToString("0.00");

			double[] battleTimes = battlePlanner.GetBattleTimes();
			int[,] weightGrid = gridMapper.ConvertToWeightMap(grid, battleTimes);
			Tuple<Position, Position> startAndEndPos = gridMapper.GetStartAndGoalPosition(grid);

			pathPlanner = new PathPlanner(weightGrid, startAndEndPos);
		}
	}
}
