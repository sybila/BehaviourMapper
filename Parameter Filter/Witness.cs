using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class Witness
    {
        private IEnumerable<State> witness;

        public int Length { get { return witness.Count(); } }

        public Witness(string[] states)
        {
            witness = states.Select(s => new State(s));
        }

        public override string ToString()
        {
            return string.Join("", witness.Select(s => s.ToString()));
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is Witness))
                return false;

            Witness other = (Witness)obj;
            return this.witness.SequenceEqual(other.witness);
        }

        public override int GetHashCode()
        {
            int hash = 11;
            hash += (137 + witness.GetHashCode());
            return hash;
        }
    }
}
