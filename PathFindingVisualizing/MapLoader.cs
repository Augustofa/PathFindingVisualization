using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingVisualizing {
    static internal class MapLoader {
        static string mapPath = "C:\\Users\\augus\\source\\repos\\Augustofa\\PathFindingVisualization\\PathFindingVisualizing\\map.txt";

        public static int[,] LoadMap() {
            string[] lines = File.ReadAllLines(mapPath);
            int rows = lines.Length;
            int cols = lines[0].Split(' ').Length;

            int[,] grid = new int[rows,cols];

            for(int r = 0; r < rows; r++) {
                string[] parts = lines[r].Split(' ');
                for(int c = 0; c < cols; c++) {
                    grid[r, c] = int.Parse(parts[c]);
                }
            }
            return grid;
        }
    }
}
