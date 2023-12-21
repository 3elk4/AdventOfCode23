using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOE3
{
    class Program
    {
        static void Main(string[] args)
        {
            int result1 = 0;
            int result2 = 0;

            string fileloc = @"data\input.txt";
            var lines = File.ReadAllLines(fileloc);

            //part1
            List<PossiblePartNumber> PossiblePartNumbers = new List<PossiblePartNumber>();
            List<Symbol> Symbols = new List<Symbol>();

            for(int i = 0; i < lines.Length; ++i)
            {
                var matchesNumbers = Regex.Matches(lines[i], @"\d+");
                var matchesSymbols = Regex.Matches(lines[i], @"[^\d\.]");

                foreach (Match m in matchesNumbers)
                {
                    PossiblePartNumbers.Add(new PossiblePartNumber(Int32.Parse(m.Value), m.Index, m.Index + m.Length - 1, i));
                }
                foreach (Match m in matchesSymbols)
                {
                    Symbols.Add(new Symbol(m.Value, m.Index, i));
                }
            }

            result1 = SumOfPartNumbers(PossiblePartNumbers, Symbols);

            //part2
            PossiblePartNumbers.Clear();
            Symbols.Clear();

            for (int i = 0; i < lines.Length; ++i)
            {
                var matchesNumbers = Regex.Matches(lines[i], @"\d+");
                var matchesSymbols = Regex.Matches(lines[i], @"\*");

                foreach (Match m in matchesNumbers)
                {
                    PossiblePartNumbers.Add(new PossiblePartNumber(Int32.Parse(m.Value), m.Index, m.Index + m.Length - 1, i));
                }
                foreach (Match m in matchesSymbols)
                {
                    Symbols.Add(new Symbol(m.Value, m.Index, i));
                }
            }

            result2 = SumOfMultipliesOfPartNumbers(PossiblePartNumbers, Symbols);

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        static public int SumOfPartNumbers(List<PossiblePartNumber> possiblePartNumbers, List<Symbol> symbols)
        {
            int result = 0;
            foreach(var symbol in symbols)
            {
                var partNumbers = possiblePartNumbers.Where(ppn => IsPartNumber(ppn, symbol));
                foreach (var partNumber in partNumbers)
                {
                    result += partNumber.Value;
                }

                possiblePartNumbers = possiblePartNumbers.Except(partNumbers).ToList();
            }

            return result;
        }

        static public int SumOfMultipliesOfPartNumbers(List<PossiblePartNumber> possiblePartNumbers, List<Symbol> symbols)
        {
            int result = 0;
            foreach (var symbol in symbols)
            {
                var partNumbers = possiblePartNumbers.Where(ppn => IsPartNumber(ppn, symbol));
                if(partNumbers.Count() == 2)
                {
                    result += partNumbers.First().Value * partNumbers.Last().Value;
                }
            }

            return result;
        }

        static private bool IsPartNumber(PossiblePartNumber pn, Symbol symbol)
        {
            List<(int, int)> possibleCoords = new List<(int, int)>()
            {
                (symbol.Start.X, symbol.Start.Y - 1),
                (symbol.Start.X, symbol.Start.Y + 1),
                (symbol.Start.X - 1, symbol.Start.Y),
                (symbol.Start.X + 1, symbol.Start.Y),
                (symbol.Start.X - 1, symbol.Start.Y - 1),
                (symbol.Start.X - 1, symbol.Start.Y + 1),
                (symbol.Start.X + 1, symbol.Start.Y - 1),
                (symbol.Start.X + 1, symbol.Start.Y + 1)
            };

            if (possibleCoords.Contains(pn.Start) || possibleCoords.Contains(pn.End)) return true;

            return false;
        }

        public class PossiblePartNumber
        {
            public PossiblePartNumber(int value, int startX, int endX, int startEndY)
            {
                Value = value;
                Start = (startX, startEndY);
                End = (endX, startEndY);
            }

            public int Value { get; }

            public (int X, int Y) Start { get; }
            public (int X, int Y) End { get; }
        }

        public class Symbol
        {
            public Symbol(string value, int startX, int startY)
            {
                Value = value;
                Start = (startX, startY);
            }

            public string Value { get; }
            public(int X, int Y) Start { get; }
    }
    }
}
