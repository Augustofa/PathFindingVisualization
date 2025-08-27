using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingVisualizing {
    static internal class MapLoader {
        static string mapPath = "E:\\Projetos Visual Studio\\PathFindingVisualizing\\PathFindingVisualizing\\map.txt";

        public static char[,] LoadMap() {
            string[] lines = File.ReadAllLines(mapPath);
            int rows = lines.Length;
            int cols = lines[0].Split(' ').Length;

            char[,] grid = new char[rows,cols];

            for(int r = 0; r < rows; r++) {
                string[] parts = lines[r].Split(' ');
                for(int c = 0; c < cols; c++) {
                    grid[r, c] = parts[c][0];
                }
            }
            return grid;
        }
    }
}
