using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AOE22
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\data.txt";
            var bricks = PreprocessInput(fileloc);

            //part1
            //result1 = BricksToRemoveCount(bricks);

            //Console.WriteLine(result1);

            //part2
            result2 = SumOfFallenBricks(bricks);

            Console.WriteLine(result2);
        }

        static IEnumerable<Brick> PreprocessInput(string fileloc)
        {
            return File.ReadAllLines(fileloc)
                .Select(line =>
                {
                    int[] positions = line
                                     .Replace("~", ",")
                                     .Split(",")
                                     .Select(x => int.Parse(x))
                                     .ToArray();

                    return new Brick(new Vector3(positions[0], positions[1], positions[2]),
                                    new Vector3(positions[3], positions[4], positions[5]));
                })
                .OrderBy(brick => brick.start.Z);
        }

        public class Brick
        {
            public Vector3 start;
            public Vector3 end;

            public Brick(Vector3 start, Vector3 end)
            {
                this.start = start;
                this.end = end;
            }

            public override string ToString()
            {
                return $"START: ({start.X},{start.Y},{start.Z}), END: ({end.X},{end.Y},{end.Z})";
            }
        }

        public static int BricksToRemoveCount(IEnumerable<Brick> bricks)
        {
            bricks = SettleBricks(bricks.ToList());

            (Dictionary<int, HashSet<int>> supported, Dictionary<int, HashSet<int>> supports) brickSupporters = GetBrickSupporters(bricks);

            return brickSupporters.supports
                .Where(support => support.Value.All(value => brickSupporters.supported[value].Count() >= 2))
                .Count();
        }

        public static int SumOfFallenBricks(IEnumerable<Brick> bricks)
        {
            bricks = SettleBricks(bricks.ToList());

            (Dictionary<int, HashSet<int>> supported, Dictionary<int, HashSet<int>> supports) brickSupporters = GetBrickSupporters(bricks);

            int total = 0;

            for (int i = 0; i < bricks.Count(); ++i)
            {
                var toCheck = brickSupporters.supports[i].Where(sup => brickSupporters.supported[sup].Count() == 1);
                var queue = new Queue<int>(toCheck);
                var falling = new HashSet<int>(queue);
                falling.Add(i);

                while (queue.Count != 0)
                {
                    var brickToCheck = queue.Dequeue();
                    foreach (var j in brickSupporters.supports[brickToCheck].Except(falling))
                    {
                        if (brickSupporters.supported[j].IsSubsetOf(falling))
                        {
                            queue.Enqueue(j);
                            falling.Add(j);
                        }
                    }
                }

                total += falling.Count - 1;
            }

            return total;
        }

        private static IEnumerable<Brick> SettleBricks(IList<Brick> bricks)
        {
            int maxZ = 1;
            for (int i = 0; i < bricks.Count(); ++i)
            {
                maxZ = 1;
                for (int j = 0; j < i; ++j)
                {
                    if (Overlaps(bricks[i], bricks[j]))
                    {
                        maxZ = Math.Max(maxZ, (int)bricks[j].end.Z + 1);
                    }
                }

                bricks[i].end.Z -= bricks[i].start.Z - maxZ;
                bricks[i].start.Z = maxZ;
            }

            return bricks.OrderBy(brick => brick.start.Z);
        }

        private static (Dictionary<int, HashSet<int>> supported, Dictionary<int, HashSet<int>> supports) GetBrickSupporters(IEnumerable<Brick> bricks)
        {
            var Range = Enumerable.Range(0, bricks.Count());
            Dictionary<int, HashSet<int>> supported = Range.ToDictionary(i => i, i => new HashSet<int>());
            Dictionary<int, HashSet<int>> supports = Range.ToDictionary(i => i, i => new HashSet<int>());

            for (int i = 0; i < bricks.Count(); ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    var upper = bricks.ElementAt(i);
                    var lower = bricks.ElementAt(j);

                    if (Overlaps(lower, upper) && upper.start.Z == lower.end.Z + 1)
                    {
                        //key is supported by value
                        supported[i].Add(j);
                        //key supports value
                        supports[j].Add(i);
                    }
                }
            }

            return (supported, supports);
        }

        private static bool Overlaps(Brick brick1, Brick brick2)
        {
            return Math.Max(brick1.start.X, brick2.start.X) <= Math.Min(brick1.end.X, brick2.end.X) &&
                Math.Max(brick1.start.Y, brick2.start.Y) <= Math.Min(brick1.end.Y, brick2.end.Y);
        }
    }
}
