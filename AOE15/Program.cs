using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE15
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";
            var records = File.ReadAllText(fileloc).Split(",", StringSplitOptions.RemoveEmptyEntries);

            //part 1
            result1 = records.Select(str => str.HASH()).Sum();

            //part 2
            var boxes = Enumerable.Range(0, 256).Select(box => new List<string>()).ToList();
            var lensPower = new Dictionary<string, int>();


            foreach(var record in records)
            {
                if (record.Contains('-'))
                {
                    var label = record.Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
                    var idx = label.HASH();
                    if (boxes[idx].Contains(label)) boxes[idx].Remove(label); 
                }
                else
                {
                    var parts = record.Split("=", StringSplitOptions.RemoveEmptyEntries);
                    var len = int.Parse(parts[1]);
                    var label = parts[0];
                    var idx = label.HASH();

                    if (!boxes[idx].Contains(label)) boxes[idx].Add(label);

                    lensPower[label] = len;
                }
            }

            for(int b = 0; b < boxes.Count; ++b)
            {
                for(int l = 0; l < boxes[b].Count; ++l)
                {
                    result2 += (b + 1) * (l + 1) * lensPower[boxes[b][l]];
                }
            }

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }
    }

    static class HASHStringExtention
    {
        static public int HASH(this string input)
        {
            int result = 0;
            foreach (var ch in input.ToCharArray())
            {
                result += ch;
                result *= 17;
                result %= 256;
            }
            return result;
        }
    }
}
