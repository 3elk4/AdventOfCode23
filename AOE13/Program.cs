using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE13
{
    class Program
    {
        static void Main(string[] args)
        {

            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";

            var text = File.ReadAllText(fileloc);
            var parts = text.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

            //part 1 and 2
            foreach(var part in parts)
            {
                var rows = part.Split("\r\n");
                var cols = PrepareColsFromRows(rows);

                var mirrorRows = FindMirror(rows, 0);
                var mirrorCols = FindMirror(cols, 0);
                result1 += 100 * mirrorRows;
                result1 += mirrorCols;

                var mirrorRows2 = FindMirror(rows, 1);
                var mirrorCols2 = FindMirror(cols, 1);
                result2 += 100 * mirrorRows2;
                result2 += mirrorCols2;
            }
            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        static public string[] PrepareColsFromRows(string[] rows)
        {
           return Enumerable.Range(0, rows[0].Length)
                .Select(c => String.Concat(Enumerable.Range(0, rows.Length).Select(r => rows[r][c]).ToArray()))
                .ToArray();
        }

        static public int FindMirror(string[] input, int countResult)
        {
            for (int i = 0; i < input.Length - 1; ++i)
            {
                int l = i;
                int h = (i + 1);

                int count = 0;

                while (l >= 0 && h < input.Length)
                {
                    count += HammingDistance(input[l], input[h]);
                    l--;
                    h++;
                }

                if (count == countResult) return i + 1;
            }

            return 0;
        }

        static private int HammingDistance(string s1, string s2)
        {
            if (s1.Length != s2.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            return s1.ToCharArray().Zip(s2.ToCharArray(), (c1, c2) => new { c1, c2 }).Count(m => m.c1 != m.c2);
        }
    }
}
