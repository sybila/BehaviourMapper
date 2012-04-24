using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class ParameterStatistic : ObservableObject
    {
        public class ValueStatistic
        {
            public int Context { get; private set; }
            public double Percentage { get; private set; }

            public ValueStatistic(int context, double percentage)
            {
                Context = context;
                Percentage = percentage;
            }
        }

        public class Context
        {
            public string Mask { get; private set; }
            public Dictionary<int, int> ValueCounts;

            public IEnumerable<ValueStatistic> Values
            {
                get
                {
                    return ValueCounts.Keys.Select(v => new ValueStatistic(v, (ValueCounts[v] * 100.0 / ValueCounts.Values.Sum())));
                }
            }

            public Context(string mask)
            {
                Mask = mask;
                ValueCounts = new Dictionary<int, int>();
            }

            public void AddValue(int value)
            {
                if (ValueCounts.ContainsKey(value))
                    ValueCounts[value]++;
                else
                    ValueCounts.Add(value, 1);
            }
        }

        private ParameterSet set;

        public IEnumerable<Context> Contexts
        {
            get
            {
                for (int i = 0; i < set.Parameters.First().Values.Count(); i++)
                {
                    Context context = new Context(set.RegulatoryContext.ContextMasks[i]);

                    foreach (int value in set.Parameters.Select(p => p.Values.ElementAt(i)))
                        context.AddValue(value);

                    yield return context;
                }
            }
        }

        public ParameterStatistic(ParameterSet set)
        {
            this.set = set;
        }

        public void Refresh()
        {
            RaisePropertyChanged("Contexts");
        }
    }
}
