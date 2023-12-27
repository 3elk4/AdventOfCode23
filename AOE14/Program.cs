using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE14
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\test1.txt";

            List<List<char>> Data = new List<List<char>>();
            foreach(var line in File.ReadAllLines(fileloc))
            {
                Data.Add(line.ToCharArray().ToList());
            }

            var firstGrid = new Grid(Data);

            long target = 1000000000;
            long t = 0;

            HashSet<string> Seen = new HashSet<string>() { firstGrid.Key };
            List<Grid> All = new List<Grid>() { firstGrid };

            var grid = firstGrid;

            //part1 and 2
            while (t < target)
            {
                t++;
                //Console.WriteLine(t);
                for (int j = 0; j < 4; ++j)
                {
                    Roll(grid.Data);
                    if (t == 1 && j == 0)
                    {
                        result1 = Result(grid.Data);
                    }
                    grid = new Grid(Rotate(grid.Data));
                }

                if (Seen.Contains(grid.Key)) break;

                Seen.Add(grid.Key);
                All.Add(grid);
            }

            var first = All.IndexOf(grid);
            var idx = (int)(((target - first) % (t - first)) + first - 1);
            grid = All[idx];

            result2 = Result(grid.Data);
            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        public class Grid : IEquatable<Grid>
        {
            public Grid(List<List<char>> data)
            {
                Data = data;
                Key = String.Join("", Data.Select(chars => new string(chars.ToArray())));
            }

            public List<List<char>> Data { get; }

            public string Key { get; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Grid);
            }

            public bool Equals(Grid other)
            {
                return other != null && Key.Equals(other.Key);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key);
            }

            public override string ToString()
            {
                return String.Join("\n", Data.Select(chars => new string(chars.ToArray())));
            }
        }

        static public long Result(List<List<char>> Data)
        {
            return Enumerable.Range(0, Data.Count)
                             .Select(r => Data[r].Where(ch => ch.Equals('O')).Count() * (Data.Count - r))
                             .Sum();
        }

        static public void Roll(List<List<char>> Data)
        {
            int rowsC = Data.Count, colsC = Data[0].Count;
            bool rolable = true;

            for (int c = 0; c < colsC; ++c)
            {
                for (int r = 0; r < rowsC; ++r)
                {
                    int j = r;
                    rolable = true;
                    while (rolable)
                    {
                        if (Data[j][c].Equals('O') && j > 0 && Data[j - 1][c].Equals('.'))
                        {
                            Data[j][c] = '.';
                            Data[j - 1][c] = 'O';
                        }
                        else rolable = false;
                        j--;
                    }
                }
            }
        }

        static public List<List<char>> Rotate(List<List<char>> Data)
        {
            int rowsC = Data.Count, colsC = Data[0].Count;
            List<List<char>> NewData = Enumerable.Range(0, colsC).Select(l => new List<char>(new char[rowsC])).ToList();

            for (int r = 0; r < rowsC; ++r)
            {
                for (int c = 0; c < colsC; ++c)
                {
                    NewData[c][rowsC - 1 - r] = Data[r][c];
                }
            }
            return NewData;
        }
    }
}
