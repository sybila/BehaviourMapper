using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class Witness : ObservableObject
    {
        private ISet<Parameter> parameters;
        public IEnumerable<Parameter> Parameters { get { return parameters; } }

        public int ParameterCount { get { return Parameters.Count(); } }

        public IEnumerable<State> Sequence { get; private set; }

        public int Length { get { return Sequence.Count(); } }

        public string Mask { get { return this.ToString(); } }

        public Witness(string[] states)
        {
            parameters = new HashSet<Parameter>();

            Sequence = states.Select(s => new State(s));
        }

        public void AddParameter(Parameter parameter)
        {
            parameters.Add(parameter);

            RaisePropertyChanged("ParameterCount");
        }

        public override string ToString()
        {
            return string.Join("", Sequence.Select(s => s.ToString()));
        }

        public override bool Equals(object obj)
        {
            Witness other = obj as Witness;
            if (other == null)
                return false;

            return this.Sequence.SequenceEqual(other.Sequence);
        }

        public override int GetHashCode()
        {
            int hash = 11;
            hash += (137 + Sequence.GetHashCode());
            return hash;
        }
    }
}
