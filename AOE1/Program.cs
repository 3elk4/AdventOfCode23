using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOE1
{
    class Program
    {
        private static Dictionary<string, int> WordDigit = new Dictionary<string, int>()
        {
            { "zero", 0 }, { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }, { "five", 5 }, { "six", 6 }, { "seven", 7 }, { "eight", 8 }, { "nine", 9 },
            { "0", 0 }, { "1", 1 }, { "2", 2 }, { "3", 3 }, { "4", 4 }, { "5", 5 }, { "6", 6 }, { "7", 7 }, { "8", 8 }, { "9", 9 },
        };

        static void Main(string[] args)
        {
            int result1 = 0;
            int result2 = 0;

            string fileloc = @"data\input.txt";

            //part1
            foreach (var line in File.ReadLines(fileloc))
            {
                result1 += GetNumberFromLine(line);
            }

            //part2
            foreach (var line in File.ReadLines(fileloc))
            {
                result2 += GetNumberFromLineFromWords(line);
            }

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        private static int GetNumberFromLine(string line)
        {
            string result = new String(line.ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

            return Int32.Parse(result[0].ToString()) * 10 + Int32.Parse(result[result.Length-1].ToString());
        }

        private static int GetNumberFromLineFromWords(string line)
        {
            IEnumerable<WordDigitClass> wordsDigits = GetWordsDigits(line);

            WordDigitClass first = wordsDigits.OrderBy(wd => wd.StartIndex).First();
            WordDigitClass last = wordsDigits.OrderByDescending(wd => wd.EndIndex).First();

            return first.Value * 10 + last.Value;
        }

        private static IEnumerable<WordDigitClass> GetWordsDigits(string line)
        {
            foreach (var keypair in WordDigit)
            {
                var matches = Regex.Matches(line, keypair.Key);
                foreach(Match m in matches)
                {
                    yield return new WordDigitClass(m.Index, m.Index + m.Length -1, keypair.Value);
                }
            }
        }

        class WordDigitClass
        {
            public WordDigitClass(int startIndex, int endIndex, int value)
            {
                StartIndex = startIndex;
                EndIndex = endIndex;
                Value = value;
            }

            public int StartIndex { get; }
            public int EndIndex { get; }
            public int Value { get; }
        }
    }
}
