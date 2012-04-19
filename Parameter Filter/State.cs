using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    class State
    {
        private IEnumerable<int> activityLevels;

        public State(string activityLevels)
        {
            this.activityLevels = activityLevels.Split(',').Select(l => int.Parse(l));
        }

        public override string ToString()
        {
            return string.Format("({0})", string.Join(",", activityLevels.ToArray()));
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is State))
                return false;

            State other = (State)obj;
            return this.activityLevels.Equals(other.activityLevels);
        }

        public override int GetHashCode()
        {
            int hash = 7;
            hash += (71 * activityLevels.GetHashCode());
            return hash;
        }
    }
}
