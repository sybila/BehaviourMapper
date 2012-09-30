using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter.Transitions
{
    public static class BehaviourExtensions
    {
        public static Behaviour Branch(this Behaviour behaviour, Transition transition)
        {
            if (!transition.InitialState.Equals(behaviour.Sequence.Last()))
                return null;

            return new Behaviour(behaviour, transition);
        }
    }
}
