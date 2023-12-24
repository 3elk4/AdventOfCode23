using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AOE17
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";
            int[][] grid = File.ReadAllLines(fileloc).Select(line => line.ToCharArray().Select(ch => int.Parse(ch.ToString())).ToArray()).ToArray();

            //part 1
            result1 = MinimizeHeatLoss(grid, (0, 0, 0, 0, 0, 0));
            result2 = MinimizeHeatLoss2(grid, (0, 0, 0, 0, 0, 0));

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        //should be able to use class instead of tuples but now is simple and special comparison for class is needed (heap comparison)

        static private List<(int X, int Y)> dirs = new List<(int X, int Y)>() { (0, 1), (1, 0), (0, -1), (-1, 0) };

        static public long MinimizeHeatLoss(int[][] grid, (int HL, int X, int Y, int DirX, int DirY, int Steps) startValue)
        {
            HashSet<(int X, int Y, int DirX, int DirY, int Steps)> seen = new HashSet<(int, int, int, int, int)>();

            MinHeap<(int HL, int X, int Y, int DirX, int DirY, int Steps)> minHeap = new MinHeap<(int HL, int X, int Y, int DirX, int DirY, int Steps)>();
            minHeap.Insert(startValue);

            while (minHeap.Count > 0)
            {
                var item = minHeap.Extract();
                var s = (item.X, item.Y, item.DirX, item.DirY, item.Steps);

                if (item.Y == grid.Length - 1 && item.X == grid[0].Length - 1) return item.HL;
                if (seen.Contains(s)) continue;

                seen.Add(s);

                if (item.Steps < 3 && (item.DirX, item.DirY) != (0, 0))
                {
                    (int X, int Y) newPos = (item.X + item.DirX, item.Y + item.DirY);

                    if (newPos.X >= 0 && newPos.X < grid[0].Length && newPos.Y >= 0 && newPos.Y < grid.Length)
                    {
                        minHeap.Insert((item.HL + grid[newPos.Y][newPos.X], newPos.X, newPos.Y, item.DirX, item.DirY, item.Steps + 1));
                    }
                }

                foreach (var dir in dirs)
                {
                    if (dir != (item.DirX, item.DirY) && dir != (-item.DirX, -item.DirY))
                    {
                        (int X, int Y) newPos = (item.X + dir.X, item.Y + dir.Y);

                        if (newPos.X >= 0 && newPos.X < grid[0].Length && newPos.Y >= 0 && newPos.Y < grid.Length)
                        {
                            minHeap.Insert((item.HL + grid[newPos.Y][newPos.X], newPos.X, newPos.Y, dir.X, dir.Y, 1));
                        }
                    }
                }
            }

            return 0;
        }

        static public long MinimizeHeatLoss2(int[][] grid, (int HL, int X, int Y, int DirX, int DirY, int Steps) startValue)
        {
            HashSet<(int X, int Y, int DirX, int DirY, int Steps)> seen = new HashSet<(int, int, int, int, int)>();

            MinHeap<(int HL, int X, int Y, int DirX, int DirY, int Steps)> minHeap = new MinHeap<(int HL, int X, int Y, int DirX, int DirY, int Steps)>();
            minHeap.Insert(startValue);

            while (minHeap.Count > 0)
            {
                var item = minHeap.Extract();
                var s = (item.X, item.Y, item.DirX, item.DirY, item.Steps);

                if (item.Y == grid.Length - 1 && item.X == grid[0].Length - 1 && item.Steps >= 4) return item.HL;
                if (seen.Contains(s)) continue;

                seen.Add(s);

                if (item.Steps < 10 && (item.DirX, item.DirY) != (0, 0))
                {
                    (int nx, int ny) newPos = (item.X + item.DirX, item.Y + item.DirY);

                    if (newPos.nx >= 0 && newPos.nx < grid[0].Length && newPos.ny >= 0 && newPos.ny < grid.Length)
                    {
                        minHeap.Insert((item.HL + grid[newPos.ny][newPos.nx], newPos.nx, newPos.ny, item.DirX, item.DirY, item.Steps + 1));
                    }
                }

                if (item.Steps >= 4 || (item.DirX, item.DirY) == (0, 0))
                {
                    foreach (var dir in dirs)
                    {
                        if (dir != (item.DirX, item.DirY) && dir != (-item.DirX, -item.DirY))
                        {
                            (int nx, int ny) newPos = (item.X + dir.Item1, item.Y + dir.Item2);

                            if (newPos.nx >= 0 && newPos.nx < grid[0].Length && newPos.ny >= 0 && newPos.ny < grid.Length)
                            {
                                minHeap.Insert((item.HL + grid[newPos.ny][newPos.nx], newPos.nx, newPos.ny, dir.Item1, dir.Item2, 1));
                            }
                        }
                    }
                }

            }

            return 0;
        }

    }
}
