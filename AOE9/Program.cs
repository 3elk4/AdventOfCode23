using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE9
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";
            List<List<List<long>>> data = new List<List<List<long>>>();

            foreach(var line in File.ReadLines(fileloc))
            {
                var parsedValues = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse);
                data.Add(DistinctHistory(parsedValues));
            }

            //part 1 and 2
            foreach(var dataHist in data)
            {
                result1 += ExtrapolateHistory(dataHist);
                result2 += ExtrapolateHistoryBackwards(dataHist);
            }

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        static public long ExtrapolateHistory(List<List<long>> dataHist)
        {
            long last = 0;

            for(int j = dataHist.Count()-2; j >= 0; --j)
            {
                last = last + dataHist[j].Last();
            }

            return last;
        }

        static public long ExtrapolateHistoryBackwards(List<List<long>> dataHist)
        {
            long first = 0;

            for (int j = dataHist.Count() - 2; j >= 0; --j)
            {
                first = dataHist[j].First() - first;
            }

            return first;
        }

        static public List<List<long>> DistinctHistory(IEnumerable<long> dataHist)
        {
            List<long> current = dataHist.ToList();
            List<List<long>> result = new List<List<long>>() { current };

            while (!current.All(x => x == 0))
            {
                List<long> nextResult = new List<long>();
                for (int i = 0; i < current.Count() - 1; ++i)
                {
                    nextResult.Add(current[i + 1] - current[i]);
                }

                current = nextResult;
                result.Add(nextResult);
            }

            return result;
        }
    }
}
