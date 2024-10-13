namespace AOE23
{
    public static class IntExtention
    {
        public static bool InRange(this int value, int min, int max) => value >= min && value <= max;
    }

    class Program
    {
        private static ISet<(int r, int c)> PossibleDirs = new HashSet<(int, int)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };

        private static Dictionary<char, ISet<(int r, int c)>> dirsSlides = new Dictionary<char, ISet<(int r, int c)>>()
        {
            { '^', new HashSet<(int, int)>() { (-1, 0) } },
            { 'v', new HashSet<(int, int)>() { (1, 0) } },
            { '<', new HashSet<(int, int)>() { (0, -1) } },
            { '>', new HashSet<(int, int)>() { (0, 1) } },
            { '.', new HashSet<(int, int)>() { (-1, 0), (1, 0), (0, -1), (0, 1) } }
        };

        private static Dictionary<(int, int), Dictionary<(int, int), int>> Graph = null;
        private static HashSet<(int r, int c)> Seen = new HashSet<(int r, int c)>();

        private static (int, int) Start;
        private static (int, int) End;
        private static int Rows;
        private static int Cols;

        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\data.txt";

            var grid = PreprocessInput(fileloc);

            Start = (0, Array.IndexOf(grid[0], '.'));
            End = (grid.Length - 1, Array.IndexOf(grid[^1], '.'));

            Rows = grid.Length;
            Cols = grid[0].Length;

            var crossings = GetCrossings(grid);

            //part1
            Graph = PrepareGraphIncludingSlides(grid, crossings);
            Seen.Clear();
            result1 = (long)LongestHike(Start);
            Console.WriteLine(result1);

            //part2
            Graph = PrepareGraphExcludingSlides(grid, crossings);
            Seen.Clear();
            result2 = (long)LongestHike(Start);
            Console.WriteLine(result2);
        }

        private static char[][] PreprocessInput(string fileloc) => File.ReadAllLines(fileloc).Select(line => line.ToCharArray()).ToArray();

        private static List<(int, int)> GetCrossings(char[][] grid)
        {
            List<(int r, int c)> points = new() { Start, End };

            for (int r = 0; r < Rows; ++r)
            {
                for (int c = 0; c < Cols; ++c)
                {
                    if (grid[r][c] == '#') continue;

                    int nnum = 0;
                    foreach (var dir in PossibleDirs)
                    {
                        (int r, int c) neighbour = (r + dir.r, c + dir.c);
                        if (neighbour.r.InRange(0, Rows - 1) && neighbour.c.InRange(0, Cols - 1) && grid[neighbour.r][neighbour.c] != '#') nnum++;
                    }

                    if (nnum >= 3) points.Add((r, c));
                }
            }

            return points;
        }

        private static Dictionary<(int, int), Dictionary<(int, int), int>> PrepareGraphIncludingSlides(char[][] grid, List<(int, int)> crossings)
        {
            Dictionary<(int, int), Dictionary<(int, int), int>> graph = crossings.ToDictionary(point => point, point => new Dictionary<(int, int), int>());

            foreach (var point in crossings)
            {
                var stack = new Stack<(int n, (int r, int c) pt)>();
                stack.Push((0, point));
                var seen = new HashSet<(int r, int c)>() { point };

                while (stack.Count > 0)
                {
                    var item = stack.Pop();

                    if (item.n != 0 && crossings.Contains(item.pt))
                    {
                        graph[point][item.pt] = item.n;
                        continue;
                    }

                    foreach (var dir in dirsSlides[grid[item.pt.r][item.pt.c]])
                    {
                        (int r, int c) nn = (item.pt.r + dir.r, item.pt.c + dir.c);

                        if (nn.r.InRange(0, Rows - 1) && nn.c.InRange(0, Cols - 1) && grid[nn.r][nn.c] != '#' && !seen.Contains(nn))
                        {
                            stack.Push((item.n + 1, nn));
                            seen.Add(nn);
                        }
                    }
                }
            }

            return graph;
        }

        private static Dictionary<(int, int), Dictionary<(int, int), int>> PrepareGraphExcludingSlides(char[][] grid, List<(int, int)> crossings)
        {
            Dictionary<(int, int), Dictionary<(int, int), int>> graph = crossings.ToDictionary(point => point, point => new Dictionary<(int, int), int>());

            foreach (var point in crossings)
            {
                var stack = new Stack<(int n, (int r, int c) pt)>();
                stack.Push((0, point));
                var seen = new HashSet<(int r, int c)>() { point };

                while (stack.Count > 0)
                {
                    var item = stack.Pop();

                    if (item.n != 0 && crossings.Contains(item.pt))
                    {
                        graph[point][item.pt] = item.n;
                        continue;
                    }

                    foreach (var dir in PossibleDirs)
                    {
                        (int r, int c) nn = (item.pt.r + dir.r, item.pt.c + dir.c);

                        if (nn.r.InRange(0, Rows - 1) && nn.c.InRange(0, Cols - 1) && grid[nn.r][nn.c] != '#' && !seen.Contains(nn))
                        {
                            stack.Push((item.n + 1, nn));
                            seen.Add(nn);
                        }
                    }
                }
            }

            return graph;
        }

        private static float LongestHike((int r, int c) point)
        {
            if (point == End) return 0;

            float m = float.NegativeInfinity;

            Seen.Add(point);

            foreach (var item in Graph[point])
            {
                if (!Seen.Contains(item.Key))
                {
                    m = Math.Max(m, LongestHike(item.Key) + Graph[point][item.Key]);
                }
            }

            Seen.Remove(point);

            return m;
        }
    }
}

