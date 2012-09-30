using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter.Transitions
{
    public class State
    {
        public IEnumerable<int> ActivityLevels { get; private set; }

        public int BuchiAutomatonState { get; private set; }

        public State(string activityLevels)
        {
            string[] data = activityLevels.Split(new string[] { ";", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);

            BuchiAutomatonState = int.Parse(data[1]);

            ActivityLevels = data[0].Split(',').Select(l => int.Parse(l));
        }

        public override string ToString()
        {
            return string.Format("({0};{1})", string.Join(",", ActivityLevels.ToArray()), BuchiAutomatonState);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is State))
                return false;

            State other = (State)obj;
            if ((BuchiAutomatonState != other.BuchiAutomatonState) || (ActivityLevels.Count() != other.ActivityLevels.Count()))
                return false;

            for (int i = 0; i < ActivityLevels.Count(); i++)
            {
                if ((ActivityLevels.ElementAt(i) < 0) || (other.ActivityLevels.ElementAt(i) < 0))
                    continue;

                if (ActivityLevels.ElementAt(i) != other.ActivityLevels.ElementAt(i))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 7;
            hash += (71 * ActivityLevels.GetHashCode());
            hash += (113 * BuchiAutomatonState);
            return hash;
        }
    }
}
