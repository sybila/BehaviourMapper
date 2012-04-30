using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class Parameter : ObservableObject
    {
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
        }

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

        public int WitnessCount { get { return Witnesses.Count(); } }

        private int _shortestWitness;
        public int ShortestWitness
        {
            get { return _shortestWitness; }
            set
            {
                if (value == _shortestWitness)
                    return;

                _shortestWitness = value;
                RaisePropertyChanged("ShortestWitness");
            }
        }

        private double _robustness;
        public double Robustness
        {
            get { return (_robustness * 100); }
            set
            {
                if (value == _robustness)
                    return;

                _robustness = value;
                RaisePropertyChanged("Robustness");
            }
        }

        public Parameter(string values)
        {
            ShortestWitness = (-1);
            witnesses = new HashSet<Witness>();

            this.Values = values.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(v => int.Parse(v));
            Mask = "Unknown";

            Robustness = 0.0;
        }

        public void AddWitness(Witness witness)
        {
            witnesses.Add(witness);
            Witnesses = witnesses;

            if ((ShortestWitness < 0) || (witness.Length < ShortestWitness))
                ShortestWitness = witness.Length;
        }

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
