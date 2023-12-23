using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE18
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";

            List<string[]> data = File.ReadAllLines(fileloc).Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)).ToList();

            Dictionary<string, Point> dirs = new Dictionary<string, Point>() { { "U", (0, -1) }, { "D", (0, 1) }, { "L", (-1, 0) }, { "R", (1, 0) } };
            List<Point> points = new List<Point>() { (0, 0)};
            var start = points.Last();

            //part1
            foreach (var d in data)
            {
                var dir = dirs[d[0]];
                var n = int.Parse(d[1]);

                for(int i = 0; i < n; ++i)
                {
                    var item = start + dir;
                    points.Add(item);
                    start = item;
                }
            }

            result1 = CountArea(points);
            Console.WriteLine(result1);

            //part2
            points = new List<Point>() { (0, 0) };
            start = points.Last();

            foreach (var d in data)
            {
                var color = d[2].Substring(2, 6);
                var dir = dirs["RDLU"[int.Parse(color[color.Length - 1].ToString())].ToString()];
                var n = int.Parse(color.Substring(0, 5), System.Globalization.NumberStyles.HexNumber);

                for (int i = 0; i < n; ++i)
                {
                    var item = start + dir;
                    points.Add(item);
                    start = item;
                }
            }

            result2 = CountArea(points);
            Console.WriteLine(result2);
        }

        public class Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public (int X, int Y) Pos()
            {
                return (X, Y);
            }

            public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

            public static implicit operator (int, int)(Point p) => (p.X, p.Y);
            public static implicit operator Point((int, int) t) => new Point(t.Item1, t.Item2);

        }

        static public long CountArea(List<Point> points)
        {
            ///https://en.wikipedia.org/wiki/Shoelace_formula
            ///https://en.wikipedia.org/wiki/Pick%27s_theorem
            
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
            result += (A - result / 2 + 1);

            return result;
        }
    }
}
