using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter.Transitions
{
    public class Behaviour
    {
        public List<State> Sequence { get; private set; }
        public List<Transition> Transitions { get; private set; }

        public Behaviour(Transition beginning)
        {
            Sequence = new List<State>() { beginning.InitialState, beginning.DestinationState };
            Transitions = new List<Transition>() { beginning };
        }

        public Behaviour(Behaviour beginning, Transition continuation)
        {
            Sequence = beginning.Sequence;
            Transitions = beginning.Transitions;

            Sequence.Add(continuation.DestinationState);
            Transitions.Add(continuation);
        }
    }
}
