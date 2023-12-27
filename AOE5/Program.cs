using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE5
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            // data preprocessing
            string fileloc = @"data\input.txt";
            var text = File.ReadAllText(fileloc);
            var parts = text.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

            List<long> Seeds = parts[0]
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .OfType<string>()
                .Select(x => Int64.Parse(x))
                .ToList();

            List <List<MapElement>> MapElements = new List<List<MapElement>>();

            for (int i = 1; i < parts.Length; ++i)
            {
                var temp = new List<MapElement>();

                var lines = parts[i].Split("\r\n");
                for(int j = 1; j < lines.Length; ++j)
                {
                    var values = lines[j].Split(" ");
                    temp.Add(new MapElement(Int64.Parse(values[0]), Int64.Parse(values[1]), Int64.Parse(values[2])));
                }

                MapElements.Add(temp);
            }

            //part1
            List<long> locations = new List<long>();

            foreach (var seed in Seeds)
            {
                long seedToLoc = seed;
                foreach(var mapList in MapElements)
                {
                    seedToLoc = DoMapping(mapList, seedToLoc);
                }
                locations.Add(seedToLoc);
                
            }
            result1 = locations.Min();

            Console.WriteLine(result1);

            //part2
            /// It takes too long for now, it is needed to use ranges properly
            List<SeedRange> SeedRanges = new List<SeedRange>();
            for (int i = 0; i < Seeds.Count(); i += 2)
            {
                SeedRanges.Add(new SeedRange(Seeds[i], Seeds[i] + Seeds[i + 1]));
            }
            foreach (var mapList in MapElements)
            {
                SeedRanges = DoRangeMapping(mapList, SeedRanges);
            } 
            result2 = SeedRanges.Select(sr => sr.Start).Min();

            Console.WriteLine(result2);
        }

        static public long DoMapping(List<MapElement> list, long value)
        {
            foreach(var me in list)
            {
                if (value >= me.SourceStart && value < me.SourceStart + me.Offset) return value + me.DestinationStart - me.SourceStart;
            }

            return value;
        }

        static public List<SeedRange> DoRangeMapping(List<MapElement> list, List<SeedRange> values)
        {
            List<SeedRange> result = new List<SeedRange>();
            int l = values.Count();

            for(int i = 0; i < l; ++i)
            {
                SeedRange sr = values[i];
                foreach(var me in list)
                {
                    var meOffset = me.DestinationRange.Start - me.SourceRange.Start;
                    if (sr.End <= me.SourceRange.Start || me.SourceRange.End <= sr.Start) continue;

                    var inRange = new SeedRange(Math.Max(sr.Start, me.SourceRange.Start), Math.Min(sr.End, me.SourceRange.End));
                    var leftRange = new SeedRange(sr.Start, inRange.Start);
                    var rightRange = new SeedRange(inRange.End, sr.End);

                    if (leftRange.Start < leftRange.End) values.Add(leftRange);
                    else if (rightRange.Start < rightRange.End) {
                        values.Add(rightRange);
                        result.Add(new SeedRange(inRange.Start + meOffset, inRange.End + meOffset));
                        l = values.Count();
                        break;
                    }
                    else
                    {
                        result.Add(sr);
                    }
                }
            }

            return result;
        }

        public class MapElement
        {
            public MapElement(long dest, long src, long offset)
            {
                DestinationStart = dest;
                SourceStart = src;
                Offset = offset;

                SourceRange = new SeedRange(src, src + offset);
                DestinationRange = new SeedRange(dest, dest + offset);
            }

            public long DestinationStart { get; }
            public long SourceStart { get; }
            public long Offset { get; }

            public SeedRange SourceRange { get; }
            public SeedRange DestinationRange { get; }
        };

        public class SeedRange
        {
            public SeedRange(long start, long end)
            {
                Start = start;
                End = end;
            }

            public long Start { get; }
            public long End { get; }
        }
    }
}
