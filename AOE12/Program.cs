using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE12
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\test1.txt";

            var lines = File.ReadAllLines(fileloc);

            //part1
            foreach(var line in lines)
            {
                var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                result1 += PossibilitiesCount(parts[0], parts[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList());
            }
            Console.WriteLine(result1);

            //part2
            /// Need to be cached, probably key class or tuple
            foreach (var line in lines)
            {
                var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                for(int i = 0; i < 4; ++i)
                {
                    parts[0] += "?" + parts[0];
                    parts[1] += "," + parts[1];
                }
                
                result2 += PossibilitiesCount2(parts[0], parts[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList());
            }
            Console.WriteLine(result2);

        }

        static public IEnumerable<string> PossibleWays(string input)
        {
            if (input.Contains('?'))
            {
                var index = input.IndexOf('?');
                foreach (var x in PossibleWays(input.Substring(0, index) + "." + input.Substring(index + 1)))
                {
                    yield return x;
                }
                foreach (var x in PossibleWays(input.Substring(0, index) + "#" + input.Substring(index + 1)))
                {
                    yield return x;
                }
            }
            else
            {
                yield return input;
            }
        }
        static private List<int> PossibleCombinations(string input)
        {
            List<int> result = new List<int>();
            var combs =  input.Split(".", StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < combs.Length; ++i)
            {
                result.Add(combs[i].Length);
            }

            return result; 
        }

        static public long PossibilitiesCount(string input, List<int> combinations)
        {
            long result = 0;

            foreach(var possibility in PossibleWays(input))
            {
                var temp = PossibleCombinations(possibility);
                if (temp.SequenceEqual(combinations)) result++;
            }

            return result;
        }

        static public IEnumerable<string> PossibleWays2(string input, List<int> combinations)
        {
            if (input.Contains('?'))
            {
                var index = input.IndexOf('?');
                foreach (var x in PossibleWays(input.Substring(0, index) + "." + input.Substring(index + 1)))
                {
                    if(PossibleCombinations(x).SequenceEqual(combinations))
                        yield return x;
                }
                foreach (var x in PossibleWays(input.Substring(0, index) + "#" + input.Substring(index + 1)))
                {
                    if (PossibleCombinations(x).SequenceEqual(combinations))
                        yield return x;
                }
            }
            else
            {
                if (PossibleCombinations(input).SequenceEqual(combinations))
                    yield return input;
            }
        }

        static public long PossibilitiesCount2(string input, List<int> combinations)
        {
            int result = 0;

            foreach (var possibility in PossibleWays2(input, combinations))
            {
                result++;
            }

            return result;
        }
    }
}
