using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    using Transitions;

    public class Parameter : ObservableObject
    {
        /*
        private ISet<Witness> witnesses;
        private IEnumerable<Witness> _witnesses;
        public IEnumerable<Witness> Witnesses
        {
            get { return _witnesses; }
            set
            {
                if (value == _witnesses)
                    return;

                _witnesses = value;
                RaisePropertyChanged("WitnessCount");
            }
        }

        public void FilterWitnesses(IEnumerable<Predicate<Witness>> filters, bool tighten)
        {
            if (tighten)
                Witnesses = Witnesses.Where(w => filters.All(f => f(w)));
            else
                Witnesses = witnesses.Where(w => filters.All(f => f(w)));
        }*/

        public int ID { get; private set; }

        public IEnumerable<int> Values { get; private set; }

        private string _mask;
        public string Mask
        {
            get { return _mask; }
            private set
            {
                if (_mask == value)
                    return;

                _mask = value;
                RaisePropertyChanged("Mask");
            }
        }

        //public int WitnessCount { get { return Witnesses.Count(); } }

        public int Cost { get; private set; }

        private double robustness;
        public double Robustness { get { return (robustness * 100); } }

        public IEnumerable<Transition> Transitions { get; private set; }

        public HashSet<Behaviour> Behaviours { get; private set; }
        public int PossibleBehaviours { get { return Behaviours.Count(); } }

        public Parameter(string line)
        {
            string[] data = line.Split(':');

            ID = int.Parse(data[0]);

            Values = data[1].Split(new string[] {",", "(", ")"}, StringSplitOptions.RemoveEmptyEntries).Select(v => int.Parse(v));
            Mask = "Unknown";

            Cost = int.Parse(data[2]);
            robustness = 0;
            double.TryParse(data[3], out robustness);

            Transitions = data[4].Split(new string[] {"),(","{", "}"}, StringSplitOptions.RemoveEmptyEntries).Select(t => new Transition(t));
            ComputePossibleBehaviours();
        }

        private void ComputePossibleBehaviours()
        {
            Behaviours = new HashSet<Behaviour>();
            Queue<Behaviour> behaviourStubs = new Queue<Behaviour>(Transitions.Where(t => (t.InitialState.BuchiAutomatonState == 0)).Select(t => new Behaviour(t)));

            while (behaviourStubs.Count > 0)
            {
                Behaviour behaviour = behaviourStubs.Dequeue();

                IEnumerable<Transition> possibleTransitions = Transitions
                    .Where(t => ((t.InitialState.Equals(behaviour.Sequence.Last())) && !behaviour.Transitions.Contains(t)));
                if (possibleTransitions.Count() == 0)
                {
                    Behaviours.Add(behaviour);
                    continue;
                }

                foreach (Transition t in possibleTransitions)
                    behaviourStubs.Enqueue(behaviour.Branch(t));
            }
        }

        /*
        public void AddWitness(Witness witness)
        {
            witnesses.Add(witness);
            Witnesses = witnesses;

            if ((ShortestWitness < 0) || (witness.Length < ShortestWitness))
                ShortestWitness = witness.Length;
        }*/

        public void CreateMask(RegulatoryContext regContext)
        {
            Mask = string.Join("; ",
                this.Values
                    .Zip(Enumerable.Range(0, Values.Count()),
                        (v, i) => string.Format("{0} = {1}", regContext.ContextMasks[i], v))
                    .ToArray());
        }

        public override string ToString()
        {
            return string.Format("[{0}]", string.Join(",", Values));
        }
    }
}
