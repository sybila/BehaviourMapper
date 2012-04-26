using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class WitnessSet
    {
        private Dictionary<string, Witness> witnesses;

        public WitnessSet()
        {
            witnesses = new Dictionary<string, Witness>();
        }

        public Witness GetWitness(string witnessString)
        {
            if (!witnesses.ContainsKey(witnessString))
                witnesses.Add(witnessString, new Witness(witnessString.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries)));

            return witnesses[witnessString];
        }
    }
}
