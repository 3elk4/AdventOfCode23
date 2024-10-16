using System.Text.RegularExpressions;

namespace AOE20
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileloc = @"data\data.txt";

            var gates = PreprocessInput(fileloc);

            //part1
            Console.WriteLine(SendSignal(gates, 1000));

            //part2
            Console.WriteLine(TurnOnMachine(gates));
        }

        static Dictionary<string, Gate> PreprocessInput(string fileloc)
        {
            var descriptions = File.ReadAllLines(fileloc).Append("rx ->")
                .Select(line =>
                {
                    var words = Regex.Matches(line, "\\w+").Select(m => m.Value).ToArray();

                    return (Name: words.First(),
                            Type: line[0],
                            Outputs: words[1..]);
                });

            var Inputs = (string name) => descriptions.Where(d => d.Outputs.Contains(name)).Select(d => d.Name);

            return descriptions
                .ToDictionary(
                    x => x.Name,
                    x => x.Type switch
                    {
                        '&' => (Gate)new Nand(x.Name, Inputs(x.Name), x.Outputs),
                        '%' => (Gate)new FlipFlop(x.Name, Inputs(x.Name), x.Outputs),
                        _ => (Gate)new Repeater(x.Name, Inputs(x.Name), x.Outputs)
                    }
                );
        }

        static long SendSignal(Dictionary<string, Gate> gates, int times = 1)
        {
            var result = Enumerable.Range(0, times).SelectMany(_ => PushButton(gates)).Select(s => s.Value).ToArray();

            return result.Count(v => v) * result.Count(v => !v);
        }

        static IEnumerable<Signal> PushButton(Dictionary<string, Gate> gates)
        {
            var q = new Queue<Signal>();
            q.Enqueue(new Signal("button", "broadcaster", false));

            while (q.TryDequeue(out var signal))
            {
                yield return signal;

                var handler = gates[signal.Receiver];
                foreach (var signalT in handler.Handle(signal))
                {
                    q.Enqueue(signalT);
                }
            }
        }

        static long TurnOnMachine(Dictionary<string, Gate> gates)
        {
            var nand = gates["rx"].Inputs.Single();
            var branches = gates[nand].Inputs;

            return branches.Aggregate(1L, (m, branch) =>
            {
                ResetAll(gates);
                int i = 0;
                for (i = 1; ; i++)
                {
                    var signals = PushButton(gates).ToArray();
                    if (signals.Any(s => s.Sender == branch && s.Value))
                    {
                        return m * i;
                    }
                }
            });
        }

        static void ResetAll(Dictionary<string, Gate> gates)
        {
            foreach (var gate in gates)
                gate.Value.Reset();
        }
    }
}
