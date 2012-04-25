using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class Witness
    {
        public IEnumerable<State> Sequence { get; private set; }

        public int Length { get { return Sequence.Count(); } }

        public Witness(string[] states)
        {
            Sequence = states.Select(s => new State(s));
        }

        public override string ToString()
        {
            return string.Join("", Sequence.Select(s => s.ToString()));
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is Witness))
                return false;

            Witness other = (Witness)obj;
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
