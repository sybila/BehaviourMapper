using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class State
    {
        public IEnumerable<int> ActivityLevels { get; private set; }

        public State(string activityLevels)
        {
            ActivityLevels = activityLevels.Split(',').Select(l => int.Parse(l));
        }

        public override string ToString()
        {
            return string.Format("({0})", string.Join(",", ActivityLevels.ToArray()));
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is State))
                return false;

            State other = (State)obj;
            return this.ActivityLevels.Equals(other.ActivityLevels);
        }

        public override int GetHashCode()
        {
            int hash = 7;
            hash += (71 * ActivityLevels.GetHashCode());
            return hash;
        }
    }
}
