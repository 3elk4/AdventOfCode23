namespace AOE20
{
    public record Signal(string Sender, string Receiver, bool Value);

    public abstract class Gate
    {
        public string Name { get; init; }
        public IEnumerable<string> Inputs { get; init; }
        public IEnumerable<string> Outputs { get; init; }

        public Gate(string name, IEnumerable<string> inputs, IEnumerable<string> outputs)
        {
            this.Name = name;
            this.Inputs = inputs;
            this.Outputs = outputs;
        }

        public abstract IEnumerable<Signal> Handle(Signal signal);

        public abstract void Reset();
    }

    public class FlipFlop : Gate
    {
        private bool currentState;
        public FlipFlop(string name, IEnumerable<string> inputs, IEnumerable<string> outputs) : base(name, inputs, outputs)
        {
            currentState = false;
        }

        public override IEnumerable<Signal> Handle(Signal signal)
        {
            if (!signal.Value)
            {
                currentState = !currentState;
                return Outputs.Select(o => new Signal(Name, o, currentState));
            }

            return [];
        }

        public override void Reset()
        {
            currentState = false;
        }
    }

    public class Nand : Gate
    {
        private Dictionary<string, bool> currentState;
        public Nand(string name, IEnumerable<string> inputs, IEnumerable<string> outputs) : base(name, inputs, outputs)
        {
            currentState = inputs.ToDictionary(input => input, _ => false);
        }

        public override IEnumerable<Signal> Handle(Signal signal)
        {
            currentState[signal.Sender] = signal.Value;
            var value = !currentState.Values.All(b => b);

            return Outputs.Select(o => new Signal(Name, o, value));
        }

        public override void Reset()
        {
            currentState = Inputs.ToDictionary(input => input, _ => false);
        }
    }

    public class Repeater : Gate
    {
        public Repeater(string name, IEnumerable<string> inputs, IEnumerable<string> outputs) : base(name, inputs, outputs) { }

        public override IEnumerable<Signal> Handle(Signal signal)
        {
            return Outputs.Select(o => new Signal(Name, o, signal.Value));
        }

        public override void Reset() { }
    }
}
