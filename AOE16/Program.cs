using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE16
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";

            char[][] grid = File.ReadAllLines(fileloc).Select(line => line.ToCharArray()).ToArray();
            
            GridItem gi = (0, 0, 1, 0);

            //part1
            result1 = BeamLight(grid, gi);

            //part2
            int maxR = grid.Length;
            int maxC = grid[0].Length;

            for (int r = 0; r < maxR; ++r)
            {
                result2 = Math.Max(result2, BeamLight(grid, (0, r, 1, 0)));
                result2 = Math.Max(result2, BeamLight(grid, (maxC-1, r, -1, 0)));
            }

            for(int c = 0; c < maxC; ++c)
            {
                result2 = Math.Max(result2, BeamLight(grid, (c, 0, 0, 1)));
                result2 = Math.Max(result2, BeamLight(grid, (c, maxR-1, 0, -1)));
            }

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        public class GridItem : IEquatable<GridItem>
        {
            public GridItem(int x, int y, int dirX, int dirY)
            {
                X = x;
                Y = y;
                DirX = dirX;
                DirY = dirY;
            }

            public GridItem((int X, int Y) pos, ( int DirX, int DirY) dir)
            {
                X = pos.X;
                Y = pos.Y;
                DirX = dir.DirX;
                DirY = dir.DirY;
            }

            public int X { get; }
            public int Y { get; }
            public int DirX { get; }
            public int DirY { get; }

            public static GridItem operator +(GridItem a, GridItem b) => new GridItem(a.X + b.X, a.Y + b.Y, a.DirX + b.DirX, a.DirY + b.DirY);

            public static implicit operator (int, int, int, int)(GridItem p) => (p.X, p.Y, p.DirX, p.DirY);
            public static implicit operator GridItem((int, int, int, int) t) => new GridItem(t.Item1, t.Item2, t.Item3, t.Item4);

            public (int X, int Y) NextXY()
            {
                return (X + DirX, Y + DirY);
            }

            public (int X, int Y) Pos()
            {
                return (X, Y);
            }

            public (int DirX, int DirY) Dir()
            {
                return (DirX, DirY);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as GridItem);
            }

            public bool Equals(GridItem other)
            {
                return other != null && X == other.X && Y == other.Y && DirX == other.DirX && DirY == other.DirY;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }
        }

        static public int BeamLight(char[][] grid, GridItem gridItem)
        {
            HashSet<GridItem> result = new HashSet<GridItem>() { gridItem };
            LinkedList<GridItem> toSee = new LinkedList<GridItem>();
            toSee.AddLast(gridItem);

            while (toSee.Count > 0)
            {
                var curr = toSee.First();
                var nextPosition = curr.NextXY();
                var dir = curr.Dir();
                var pos = curr.Pos();
                toSee.RemoveFirst();

                if (nextPosition.X < 0 || nextPosition.X >= grid.Length || nextPosition.Y < 0 || nextPosition.Y >= grid[0].Length) continue;

                char ch = grid[nextPosition.Y][nextPosition.X];
                var newGridItem = new GridItem(nextPosition, dir);

                if (ch.Equals('.') || (ch.Equals('-') && dir.DirX != 0) || ch.Equals('|') && dir.DirY != 0)
                {
                    if(!result.Contains(newGridItem)) toSee.AddLast(newGridItem);
                }
                else if (ch.Equals('/'))
                {
                    dir = (dir.DirY * (-1), dir.DirX * (-1));
                    newGridItem = new GridItem(nextPosition, dir);
                    if (!result.Contains(newGridItem)) toSee.AddLast(newGridItem);
                }
                else if (ch.Equals('\\'))
                {
                    dir = (dir.DirY, dir.DirX);
                    newGridItem = new GridItem(nextPosition, dir);
                    if (!result.Contains(newGridItem)) toSee.AddLast(newGridItem);
                }
                else
                {
                    if (ch.Equals('|'))
                    {
                        dir = (0, 1);
                        newGridItem = new GridItem(nextPosition, dir);
                        if (!result.Contains(newGridItem)) toSee.AddLast(newGridItem);
                        dir = (0, -1);
                        newGridItem = new GridItem(nextPosition, dir);
                        if (!result.Contains(newGridItem)) toSee.AddLast(newGridItem);
                    }
                    else
                    {
                        dir = (1, 0);
                        newGridItem = new GridItem(nextPosition, dir);
                        if (!result.Contains(newGridItem)) toSee.AddLast(newGridItem);
                        dir = (-1, 0);
                        newGridItem = new GridItem(nextPosition, dir);
                        if (!result.Contains(newGridItem)) toSee.AddLast(newGridItem);
                    }
                }

                result.Add(newGridItem);
            }

            return result.Select(gi => (gi.X, gi.Y)).Distinct().Count();
        }
    }
}
