using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class WitnessSet
    {
        private Dictionary<string, Witness> witnesses;

        public IEnumerable<Witness> Witnesses { get { return witnesses.Values; } }

        public WitnessSet()
        {
            witnesses = new Dictionary<string, Witness>();
        }

        public Witness GetWitness(string witnessString, Parameter parameter)
        {
            if (!witnesses.ContainsKey(witnessString))
            {
                Witness witness = new Witness(witnessString.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries));
                witness.AddParameter(parameter);
                witnesses.Add(witnessString, witness);
            }
            else
                witnesses[witnessString].AddParameter(parameter);

            return witnesses[witnessString];
        }
    }
}
