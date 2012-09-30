using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Parameter_Filter
{
    using Transitions;

    public class TimeSerie
    {
        public IEnumerable<State> Measurements { get; private set; }

        public TimeSerie(string fileName, int numberOfSpecies)
        {
            Measurements = ParseTimeSerieFile(fileName).Select(s => new State(s));
        }

        private IEnumerable<string> ParseTimeSerieFile(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    yield return line;
            }
        }

        public TimeSerie(string fileName, RegulatoryContext regCon)
        {
            XDocument doc = XDocument.Load(fileName);
            IEnumerable<XElement> states = doc.Element("MODEL").Element("SERIES").Elements("EXPR");

            Measurements = states
                .Select(s => 
                {
                    Dictionary<string, string> activityLevels = s.Attribute("values").Value
                        .Split(new string[] {"&", "(", ")"}, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(al => al.Split('=')[0], al => al.Split('=')[1]);

                    return string.Join(",", regCon.Species.Select(sp => (activityLevels.ContainsKey(sp) ? activityLevels[sp] : "-1")));
                })
                .Zip(Enumerable.Range(0, states.Count()), (state, buchiState) => new State(string.Format("({0};{1})", state, buchiState)))
                .ToArray();
        }

        public State GetNext(State measurement)
        {
            for (int i = 0; i < (Measurements.Count() - 1); i++)
            {
                if (measurement.Equals(Measurements.ElementAt(i)))
                    return (Measurements.ElementAt(i + 1));
            }

            return null;
        }

        public override string ToString()
        {
            return string.Join("", Measurements.Select(m => m.ToString()).ToArray());
        }
    }
}
