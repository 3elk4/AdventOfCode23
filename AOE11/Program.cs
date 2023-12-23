using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AOE11
{
    class Program
    {
        static private List<List<char>> grid = new List<List<char>>();
        static private List<int> indexesEmptyRows = new List<int>();
        static private List<int> indexesEmptyColumns = new List<int>();
        static private List<Galaxy> galaxies = new List<Galaxy>();

        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";

            var lines = File.ReadAllLines(fileloc);

            //retreive data
            for (int i = 0; i < lines.Length; ++i)
            {
                var row = new List<char>();
                for(int j = 0; j < lines[i].Length; ++j)
                {
                    if (lines[i][j].Equals('#')) galaxies.Add(new Galaxy(j, i));
                    row.Add(lines[i][j]);
                }
                grid.Add(row);
            }

            //expand rows
            int rows = grid.Count, cols = grid[0].Count;

            for (int i = 0; i < rows; ++i){
                if (grid[i].All(x => x.Equals('.'))) indexesEmptyRows.Add(i);
            }

            //expand columns
            var empty = true;
            for (int i = 0; i < cols; ++i)
            {
                empty = true;
                for (int j = 0; j < rows; ++j)
                {
                    if (grid[j][i].Equals('#')) empty = false;
                }

                if (empty)
                {
                    indexesEmptyColumns.Add(i);
                }
            }

            //part 1
            int scale = 1;
            result1 = Result(scale);

            //part 2
            scale = (int)Math.Pow(10, 6) - 1;
            result2 = Result(scale);

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        static public long Result(int scale)
        {
            long result = 0;
            for (int i = 0; i < galaxies.Count; ++i)
            {
                var galaxy = galaxies[i];
                for (int j = i + 1; j < galaxies.Count; ++j)
                {
                    var nextGalaxy = galaxies[j];

                    result += Math.Abs(galaxy.X - nextGalaxy.X) + Math.Abs(galaxy.Y - nextGalaxy.Y);

                    int minRow = Math.Min(galaxy.Y, nextGalaxy.Y), maxRow = Math.Max(galaxy.Y, nextGalaxy.Y);
                    int minCol = Math.Min(galaxy.X, nextGalaxy.X), maxCol = Math.Max(galaxy.X, nextGalaxy.X); ;
                    foreach (var rowIndex in indexesEmptyRows)
                    {
                        if (minRow <= rowIndex && rowIndex <= maxRow) result += scale;
                    }

                    foreach (var colIndex in indexesEmptyColumns)
                    {
                        if (colIndex >= minCol && colIndex <= maxCol) result += scale;
                    }
                }
            }
            return result;
        }

        public class Galaxy
        {
            public Galaxy(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

    }
}
