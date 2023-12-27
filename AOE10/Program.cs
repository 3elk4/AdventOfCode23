using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE10
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";
            List<Pipe> data = new List<Pipe>();

            var lines = File.ReadAllLines(fileloc);
            int maxX = 0, maxY = lines.Length;

            for (int i = 0; i < lines.Length; ++i) 
            {
                var chars = lines[i].ToCharArray();
                if (maxX == 0) maxX = chars.Length;
                for(int j = 0; j < chars.Length; ++j)
                {
                    data.Add(new Pipe(chars[j], j, i));
                }
            }

            var curr = data.Find(pipe => pipe.Sign.Equals('S'));
            List<Pipe> path = new List<Pipe>() { curr };
            LinkedList<Pipe> PipesToCheck = new LinkedList<Pipe>();
            PipesToCheck.AddLast(curr);

            var upFrom = "S|JL";
            var upTo = "|7F";
            var downFrom = "S|7F";
            var downTo = "|JL";
            var leftFrom = "S-J7";
            var leftTo = "-LF";
            var rightFrom = "S-LF";
            var rightTo = "-J7";

            //part 1 and 2
            while (PipesToCheck.Any())
            {
                curr = PipesToCheck.First();
                PipesToCheck.RemoveFirst();

                var up = data.Find(pipe => pipe.X == curr.X && pipe.Y == curr.Y - 1);
                var down = data.Find(pipe => pipe.X == curr.X && pipe.Y == curr.Y + 1);
                var left = data.Find(pipe => pipe.X == curr.X - 1 && pipe.Y == curr.Y);
                var right = data.Find(pipe => pipe.X == curr.X + 1 && pipe.Y == curr.Y);

                //up
                if (up != null)
                {
                    if (upFrom.Contains(curr.Sign) && upTo.Contains(up.Sign) && !path.Contains(up))
                    {
                        path.Add(up);
                        PipesToCheck.AddLast(up);
                    }
                }
                //down
                if (down != null)
                {
                    if (downFrom.Contains(curr.Sign) && downTo.Contains(down.Sign) && !path.Contains(down))
                    {
                        path.Add(down);
                        PipesToCheck.AddLast(down);
                    }
                }
                //left
                if (left != null)
                {
                    if (leftFrom.Contains(curr.Sign) && leftTo.Contains(left.Sign) && !path.Contains(left))
                    {
                        path.Add(left);
                        PipesToCheck.AddLast(left);
                    }
                }
                //right
                if (right != null)
                {
                    if (rightFrom.Contains(curr.Sign) && rightTo.Contains(right.Sign) && !path.Contains(right))
                    {
                        path.Add(right);
                        PipesToCheck.AddLast(right);
                    }
                }

            }

            result1 = (long)Math.Floor((double)path.Count / 2);

            Console.WriteLine(result1);

            //part2
            result2 = CountArea(path
                .OrderByDescending(pipe => pipe.Y)
                .ThenBy(pipe => pipe.X)
                .ToList());
            Console.WriteLine(result2);
        }

        /// <summary>
        /// Counts inner area of the polygon
        /// </summary>
        /// <param name="points"> Points should be order in a proper way, from A to B, From B to C etc.</param>
        /// <returns></returns>
        static public long CountArea(List<Pipe> points)
        {
            long result = points.Count - 1;
            long A = 0;

            for (int i = 0; i < points.Count; ++i)
            {
                long y = points[i].Y;
                long x1 = i - 1 < 0 ? points[points.Count - 1].X : points[i - 1].X;
                long x2 = points[(i + 1) % points.Count].X;

                A += y * (x1 - x2);
            }

            A = Math.Abs(A) / 2;

            return (A - result / 2 + 1);
        }

        public class Pipe : IEquatable<Pipe>
        {
            public Pipe(char sign, int x, int y)
            {
                Sign = sign;
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
            public char Sign { get; }

            public bool Equals(Pipe other)
            {
                return other != null &&
                       X == other.X &&
                       Y == other.Y &&
                       Sign == other.Sign;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Pipe);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Sign);
            }
        }
    }
}
