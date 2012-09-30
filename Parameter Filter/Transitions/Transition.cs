using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter.Transitions
{
    public class Transition
    {
        private State initialState;
        public State InitialState { get { return initialState; } }

        private State destinationState;
        public State DestinationState { get { return destinationState; } }

        public Transition(string source)
        {
            string[] states = source.Split('>');

            initialState = new State(states[0]);
            destinationState = new State(states[1]);
        }

        public override string ToString()
        {
            return string.Format("{0}>{1}", InitialState, DestinationState);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is Transition))
                return false;

            Transition other = (Transition)obj;
            return (this.InitialState.Equals(other.InitialState) && this.DestinationState.Equals(other.DestinationState));
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (107 * InitialState.GetHashCode());
            hash += (251 * DestinationState.GetHashCode());
            return hash;
        }
    }
}
