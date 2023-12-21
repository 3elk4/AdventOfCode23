using System;
using System.Collections.Generic;
using System.IO;

namespace AOE2
{
    class Program
    {
        private static Dictionary<string, int> MaxCubes = new Dictionary<string, int>(){
            { "red", 12 },
            { "green", 13 },
            { "blue", 14 }
        };

        static void Main(string[] args)
        {
            int result1 = 0;
            int result2 = 0;

            string fileloc = @"data\input.txt";

            //part1
            foreach (var line in File.ReadLines(fileloc))
            {
                var parts = line.Split(": ");
                if (GameIsPossible(parts[1])) result1 += Int32.Parse(parts[0].Split(" ")[1]);
            }

            //part2
            foreach (var line in File.ReadLines(fileloc))
            {
                var parts = line.Split(": ");
                result2 += GameMinMultiply(parts[1]);
            }

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        static bool GameIsPossible(string game)
        {
            var subsets = game.Split("; ");
            foreach(var subset in subsets)
            {
                var cubes = subset.Split(", ");
                foreach(var cube in cubes)
                {
                    var result = cube.Split(" ");
                    if (Int32.Parse(result[0]) > MaxCubes[result[1]]) return false;
                }
            }
            return true;
        }

        static int GameMinMultiply(string game)
        {
            int maxRed = 0, maxBlue = 0, maxGreen =0 ;
            var subsets = game.Split("; ");

            foreach (var subset in subsets)
            {
                var cubes = subset.Split(", ");
                foreach (var cube in cubes)
                {
                    var result = cube.Split(" ");
                    var value = Int32.Parse(result[0]);

                    if (result[1].Equals("red") && value > maxRed) maxRed = value;
                    if (result[1].Equals("blue") && value > maxBlue) maxBlue = value;
                    if (result[1].Equals("green") && value > maxGreen) maxGreen = value;
                }
            }
            return maxRed * maxGreen * maxBlue;
        }
    }
}
