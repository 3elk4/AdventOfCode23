using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE21
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";
            char[][] grid = File.ReadAllLines(fileloc).Select(line => line.ToCharArray()).ToArray();

            var startPos = CoordinatesOf<char>(grid, 'S');

            //part1
            result1 = GardenPlotsCount(grid, new GardenPlot(startPos, 64));

            Console.WriteLine(result1);
        }

        public class GardenPlot
        {
            public int X { get; }
            public int Y { get; }
            public int Max { get; }
            public GardenPlot(int x, int y, int max)
            {
                X = x;
                Y = y;
                Max = max;
            }

            public GardenPlot((int x, int y) pos, int max)
            {
                X = pos.x;
                Y = pos.y;
                Max = max;
            }

            public (int X, int Y) Pos()
            {
                return (X, Y);
            }

        }

        public static (int x, int y) CoordinatesOf<T>( T[][] matrix, T value)
        {
            for (int y = 0; y < matrix.Length; ++y)
            {
                for (int x = 0; x < matrix[0].Length; ++x)
                {
                    if (matrix[y][x].Equals(value))
                        return (x, y);
                }
            }

            return (-1, -1);
        }

        public static long GardenPlotsCount(char[][] grid, GardenPlot gp)
        {
            LinkedList<GardenPlot> toSee = new LinkedList<GardenPlot>();
            toSee.AddLast(gp);

            HashSet<(int x, int y)> seen = new HashSet<(int x, int y)>() { gp.Pos() };
            HashSet<(int x, int y)> result = new HashSet<(int x, int y)>();
            List<(int x, int y)> dirs = new List<(int x, int y)>() { (1, 0), (-1, 0), (0, 1), (0, -1) };

            while (toSee.Count > 0)
            {
                var item = toSee.First();
                toSee.RemoveFirst();
                var currentPos = item.Pos();

                if (item.Max % 2 == 0) result.Add(currentPos);
                if (item.Max == 0) continue;

                foreach (var dir in dirs)
                {
                    (int x, int y) newPos = (currentPos.X + dir.x, currentPos.Y + dir.y);
                    if (newPos.x < 0 || newPos.x >= grid[0].Length ||
                        newPos.y < 0 || newPos.y >= grid.Length ||
                        grid[newPos.y][newPos.x].Equals('#') ||
                        seen.Contains(newPos)) continue;

                    seen.Add(newPos);
                    toSee.AddLast(new GardenPlot(newPos.x, newPos.y, item.Max - 1));
                }
            }

            return result.Count;
        }
    }
}
