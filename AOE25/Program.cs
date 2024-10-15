namespace AOE25
{
    public static class DictionaryExtention
    {
        public static void Add<T, U>(this Dictionary<T, List<U>> dict, T key, U value)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new();

            dict[key].Add(value);
        }

        public static void Rebind(this Dictionary<string, List<string>> graph, string oldNode, string newNode)
        {
            foreach (var n in graph[oldNode])
            {
                while (graph[n].Remove(oldNode))
                {
                    graph[n].Add(newNode);
                }
            }
        }
    }

    class Program
    {
        private static Random r = new Random(127);

        static void Main(string[] args)
        {
            string fileloc = @"data\data.txt";

            var (cutSize, c1, c2) = FindCut(fileloc);

            while (cutSize != 3)
            {
                (cutSize, c1, c2) = FindCut(fileloc);
            }

            Console.WriteLine(c1 * c2);
        }

        private static Dictionary<string, List<string>> PreprocessInput(string fileloc)
        {
            var graph = new Dictionary<string, List<string>>();

            File.ReadAllLines(fileloc).ToList()
                .ForEach(line =>
                {
                    var elements = line.Replace(":", "").Split(" ");

                    for (int i = 1; i < elements.Length; ++i)
                    {
                        graph.Add(elements[0], elements[i]);
                        graph.Add(elements[i], elements[0]);
                    }
                });

            return graph;
        }

        private static (int size, int c1, int c2) FindCut(string fileloc)
        {
            var graph = PreprocessInput(fileloc);
            var nodeSize = graph.Keys.ToDictionary(k => k, _ => 1);

            while (graph.Count > 2)
            {
                var u = graph.Keys.ElementAt(r.Next(graph.Count));
                var v = graph[u][r.Next(graph[u].Count)];

                var merged = $"merge-{u}-{v}";

                graph[merged] = graph[u].Where(x => x != v).Concat(graph[v].Where(x => x != u)).ToList();

                graph.Rebind(u, merged);
                graph.Rebind(v, merged);

                nodeSize[merged] = nodeSize[u] + nodeSize[v];

                graph.Remove(u);
                graph.Remove(v);
            }

            var nodeA = graph.Keys.First();
            var nodeB = graph.Keys.Last();
            return (graph[nodeA].Count(), nodeSize[nodeA], nodeSize[nodeB]);
        }
    }
}