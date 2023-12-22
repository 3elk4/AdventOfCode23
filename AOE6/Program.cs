using System;
using System.Collections.Generic;
using System.IO;

namespace AOE6
{
    class Program
    {
        static void Main(string[] args)
        {

            long result1 = 1;
            long result2 = 0;

            string fileloc = @"data\input.txt";

            var lines = File.ReadAllLines(fileloc);
            var times = lines[0].Split(": ", StringSplitOptions.RemoveEmptyEntries)[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var distances = lines[1].Split(": ", StringSplitOptions.RemoveEmptyEntries)[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            //part1
            List<Score> scores = new List<Score>();
            for (int i = 0; i < times.Length; ++i)
            {
                scores.Add(new Score(Int32.Parse(times[i]), Int32.Parse(distances[i])));
            }

            foreach (var score in scores)
            {
                result1 *= score.CountWaysToWin();
            }

            //part2
            Score score2 = new Score(Int64.Parse(String.Join("", times)), Int64.Parse(String.Join("", distances)));
            result2 = score2.CountWaysToWin();

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        public class Score
        {
            public Score(long time, long distance)
            {
                Time = time;
                Distance = distance;
            }

            public long Time { get; set; }
            public long Distance { get; set; }

            public long CountWaysToWin()
            {
                int result = 0;
                for (int speed = 1; speed < this.Time; ++speed)
                {
                    var distance = speed * (this.Time - speed);
                    if (distance > this.Distance) result++;
                }

                return result;
            }

            //Another solution for this task (especially when big data) is using ranges defined by quadratic function and its roots
        }
    }
}
