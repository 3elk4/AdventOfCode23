namespace AOE24
{
    public static class DecimalExtention
    {
        public static bool InRange(this decimal value, long min, long max) => value >= min && value <= max;
    }

    public static class IntersectionExtention
    {
        private static long MinRange = 200000000000000;
        private static long MaxRange = 400000000000000;

        public static bool RangeIntersection(this Hailstone h1, Hailstone h2)
        {
            var result = h1.Intersection(h2);

            if (!result.HasValue) return false;

            return result.Value.X.InRange(MinRange, MaxRange) && result.Value.Y.InRange(MinRange, MaxRange) &&
                    h1.FuturePoint(result.Value.X, result.Value.Y) && h2.FuturePoint(result.Value.X, result.Value.Y);
        }

        private static bool FuturePoint(this Hailstone h, decimal x, decimal y)
        {
            return (x - h.Position.X) * h.Velocity.X >= 0 && (y - h.Position.Y) * h.Velocity.Y >= 0;
        }
    }

    public record Vec3(decimal X, decimal Y, decimal Z);

    public class Hailstone
    {
        public Vec3 Position;
        public Vec3 Velocity;

        public decimal A => Velocity.Y;
        public decimal B => -Velocity.X;
        public decimal C => Velocity.Y * Position.X - Velocity.X * Position.Y;

        public Hailstone(Vec3 position, Vec3 velocity)
        {
            this.Position = position;
            this.Velocity = velocity;
        }

        public override string ToString()
        {
            return $"Position: ({Position.X},{Position.Y},{Position.Z}), Velocity: ({Velocity.X},{Velocity.Y},{Velocity.Z})";
        }

        public (decimal X, decimal Y)? Intersection(Hailstone other)
        {
            if (this.A * other.B == this.B * other.A)
                return null;

            decimal x = (this.C * other.B - other.C * this.B) / (this.A * other.B - other.A * this.B);
            decimal y = (other.C * this.A - this.C * other.A) / (this.A * other.B - other.A * this.B);

            return (x, y);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;

            string fileloc = @"data\data.txt";

            var hailstones = PreprocessInput(fileloc);

            //part1
            result1 = IntersectionXYCount(hailstones);
            Console.WriteLine(result1);

            //part2
            Console.WriteLine(StoneResultSearch(hailstones.ToList()));
        }

        static IEnumerable<Hailstone> PreprocessInput(string fileloc)
        {
            return File.ReadAllLines(fileloc)
                .Select(line =>
                {
                    decimal[] positions = line
                                     .Replace("@", ",")
                                     .Split(",")
                                     .Select(x => decimal.Parse(x))
                                     .ToArray();

                    return new Hailstone(new Vec3(positions[0], positions[1], positions[2]),
                                    new Vec3(positions[3], positions[4], positions[5]));
                });
        }

        static int IntersectionXYCount(IEnumerable<Hailstone> hailstones) =>
            Enumerable.Range(0, hailstones.Count())
                .Select(h1Idx =>
                    Enumerable.Range(h1Idx + 1, hailstones.Count() - h1Idx - 1)
                    .Count(h2Idx => h1Idx != h2Idx && hailstones.ElementAt(h1Idx).RangeIntersection(hailstones.ElementAt(h2Idx)))
                )
                .Sum();

        static decimal StoneResultSearch(List<Hailstone> hailstones)
        {
            var translateXY = (Hailstone h, (decimal, decimal) vel) => new Hailstone(h.Position, new Vec3(h.Velocity.X - vel.Item1, h.Velocity.Y - vel.Item2, h.Velocity.Z));
            var translateXZ = (Hailstone h, (decimal, decimal) vel) => new Hailstone(new Vec3(h.Position.X, h.Position.Z, h.Position.Y), new Vec3(h.Velocity.X - vel.Item1, h.Velocity.Z - vel.Item2, h.Velocity.Y));

            var resultXY = Solve2D(hailstones, translateXY);
            var resultXZ = Solve2D(hailstones, translateXZ);

            return resultXY.p1 + resultXY.p2 + resultXZ.p2;
        }

        static bool Hits(Hailstone h, (decimal X, decimal Y)? intRes)
        {
            var d = (intRes.Value.X - h.Position.X) * h.Velocity.Y - (intRes.Value.Y - h.Position.Y) * h.Velocity.X;
            return Math.Abs(d) < (decimal)0.0001;
        }

        private static (decimal p1, decimal p2) Solve2D(List<Hailstone> hailstones, Func<Hailstone, (decimal, decimal), Hailstone> translate)
        {
            int s = 300;

            for (var v1 = -s; v1 < s; v1++)
            {
                for (var v2 = -s; v2 < s; v2++)
                {
                    (decimal, decimal) vel = (v1, v2);

                    var result = translate(hailstones[0], vel).Intersection(translate(hailstones[1], vel));

                    if (!result.HasValue) continue;

                    if (hailstones.All(h => Hits(translate(h, vel), result)))
                    {
                        return (result.Value.X, result.Value.Y);
                    }
                }
            }
            throw new ArgumentException();
        }
    }
}