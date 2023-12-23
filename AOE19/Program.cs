using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE19
{
    class Program
    {
        public static Dictionary<string, List<Instruction>> Instructions = new Dictionary<string, List<Instruction>>();
        public static List<Dictionary<char, int>> Data = new List<Dictionary<char, int>>();


        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";

            var parts = File.ReadAllText(fileloc).Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);
            var instructions = parts[0].Split("\r\n");
            var data = parts[1].Split("\r\n");

            //preprocessing data
            foreach(var i in instructions)
            {
                var iparts = i.Split("{");
                var name = iparts[0];
                List<string> instrs = iparts[1].Split("}")[0].Split(",").ToList();
                List<Instruction> temp = new List<Instruction>();
                foreach(var instr in instrs)
                {
                    if (instr.Contains(":"))
                    {
                        var rparts = instr.Split(":");
                        var key = rparts[0][0];
                        var cmp = rparts[0][1];
                        var v = int.Parse(rparts[0].Substring(2));

                        temp.Add(new Instruction(key, cmp, v, rparts[1]));
                    }
                    else
                    {
                        temp.Add(new Instruction('\0', '\0', 0, instr));
                    }
                }
                Instructions.Add(name, temp);
            }

            foreach(var d in data)
            {
                Dictionary<char, int> item = new Dictionary<char, int>();
                var els = d.Substring(1, d.Length - 2).Split(",");
                foreach(var el in els)
                {
                    item.Add(el[0], int.Parse(el.Split("=")[1]));
                }

                Data.Add(item);
            }

            //part 1
            foreach(var item in Data)
            {
                if(Accept(item))
                {
                    result1 += item.Values.Sum();
                }
            }

            //part 2
            Dictionary<char, (int Low, int High)> Data2 = new Dictionary<char, (int Low, int High)>() { { 'x', (1, 4000) }, { 'm', (1, 4000) }, { 'a', (1, 4000) }, { 's', (1, 4000) } };
            result2 = Count(Data2);

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        public static long Count(Dictionary<char, (int Low, int High)> item, string name = "in")
        {
            if (name.Equals("R")) return 0;
            if (name.Equals("A"))
            {
                long r = 1;
                foreach(var pair in item)
                {
                    r *= pair.Value.High - pair.Value.Low + 1;
                }
                return r;
            }

            var rules = Instructions[name];

            long result = 0;

            foreach (var r in rules)
            {
                if (r.Key.Equals('\0')) result += Count(item, r.Target);
                else
                {
                    var ranges = item[r.Key];

                    (int Low, int High) TrueEq;
                    (int Low, int High) FalseEq;

                    if (r.Comparator.Equals('<'))
                    {
                        TrueEq = (ranges.Low, r.Value - 1);
                        FalseEq = (r.Value, ranges.High);
                    }
                    else
                    {
                        TrueEq = (r.Value + 1, ranges.High);
                        FalseEq = (ranges.Low, r.Value);
                    }

                    if (TrueEq.Low <= TrueEq.High)
                    {
                        var copy = new Dictionary<char, (int Low, int High)>(item);
                        copy[r.Key] = TrueEq;
                        result += Count(copy, r.Target);
                    }
                    if (FalseEq.Low <= FalseEq.High)
                    {
                        item = new Dictionary<char, (int Low, int High)>(item);
                        item[r.Key] = FalseEq;
                    }
                    else break;
                }
            }

            return result;
        }

        public static bool Accept(Dictionary<char, int> item, string name = "in")
        {
            if (name.Equals("R")) return false;
            if (name.Equals("A")) return true;

            var rules = Instructions[name];

            foreach(var r in rules)
            {
                if (r.Key.Equals('\0')) return Accept(item, r.Target);

                if (r.Comparator.Equals('<'))
                {
                    if (item[r.Key] < r.Value) return Accept(item, r.Target);
                }
                else
                {
                    if (item[r.Key] > r.Value) return Accept(item, r.Target);
                }
            }

            return false;
        }

        public class Instruction
        {
            public Instruction(char key, char comp, int value, string target)
            {
                Key = key;
                Comparator = comp;
                Value = value;
                Target = target;
            }

            public char Key { get; }
            public char Comparator { get; }
            public int Value { get; }
            public string Target { get; }
        }
    }
}
