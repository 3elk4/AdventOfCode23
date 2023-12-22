using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOE8
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 1;
            long result2 = 1;

            string fileloc = @"data\input.txt";
            var lines = File.ReadAllLines(fileloc);

            Dictionary<string, Crossways> mappings = new Dictionary<string, Crossways>();
            char[] path = lines[0].ToCharArray();

            for (int i = 2; i < lines.Length; ++i)
            {
                var matches = Regex.Matches(lines[i], @"[0-9A-Z]+");
                mappings.Add(matches[0].Value, new Crossways(matches[1].Value, matches[2].Value));
            }

            var curr = "AAA";
            var keepLooping = true;

            //part1
            while (keepLooping)
            {
                foreach (var c in path)
                {
                    curr = c.Equals('L') ? mappings[curr].Left : mappings[curr].Right;

                    if (curr.Equals("ZZZ"))
                    {
                        keepLooping = false;
                        break;
                    }
                    result1++;
                }
            }

            Console.WriteLine(result1);

            //part2
            List<long> steps = new List<long>();
            var endingsA = mappings.Where(el => el.Key.EndsWith("A"));

            foreach (var element in endingsA)
            {
                var curr2 = element.Key;
                int count = 1;
                keepLooping = true;

                while (keepLooping)
                {
                    foreach (var c in path)
                    {
                        if (curr2.EndsWith("Z"))
                        {
                            steps.Add(count - 1);
                            keepLooping = false;
                            break;
                        }

                        curr2 = c.Equals('L') ? mappings[curr2].Left : mappings[curr2].Right;
                        count++;
                    }
                }
            }

            // Least common multiple because all cycles must end at the same time in different number of steps (multiple)
            result2 = LCMArray(steps);
            Console.WriteLine(result2);
        }

        public static long LCMArray(List<long> values)
        {
            long lcm_of_array_elements = 1;
            int divisor = 2;

            while (true)
            {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < values.Count(); i++)
                {
                    // lcm_of_array_elements (n1, n2, ... 0) = 0.
                    // For negative number we convert into
                    // positive and calculate lcm_of_array_elements.
                    if (values[i] == 0)
                    {
                        return 0;
                    }
                    else if (values[i] < 0)
                    {
                        values[i] = values[i] * (-1);
                    }
                    if (values[i] == 1)
                    {
                        counter++;
                    }

                    // Divide element_array by devisor if complete
                    // division i.e. without remainder then replace
                    // number with quotient; used for find next factor
                    if (values[i] % divisor == 0)
                    {
                        divisible = true;
                        values[i] = values[i] / divisor;
                    }
                }

                // If divisor able to completely divide any number
                // from array multiply with lcm_of_array_elements
                // and store into lcm_of_array_elements and continue
                // to same divisor for next factor finding.
                // else increment divisor
                if (divisible)
                {
                    lcm_of_array_elements = lcm_of_array_elements * divisor;
                }
                else
                {
                    divisor++;
                }

                // Check if all element_array is 1 indicate 
                // we found all factors and terminate while loop.
                if (counter == values.Count)
                {
                    return lcm_of_array_elements;
                }
            }
        }

        class Crossways
        {
            public Crossways(string left, string right)
            {
                Left = left;
                Right = right;
            }

            public string Left { get; }
            public string Right { get; }
        }
    }
}
