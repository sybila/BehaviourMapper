using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Parameter_Filter
{
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
            IEnumerable<XElement> states = doc.Element("MODEL").Element("AUTOMATON").Elements("STATE");

            Measurements = states
                .SelectMany(e => e.Element("TRANSITIONS").Elements("TRANS")
                    .Select(t => t.Attribute("label").Value)
                    .Where(l => ((l != "tt") && (l != "ff"))))
                .Select(s =>
                {
                    Dictionary<string, string> activityLevels = s.Split('&').ToDictionary(al => al.Split('=')[0], al => al.Split('=')[1]);

                    return new State(string.Join(",", regCon.Species.Select(sp => activityLevels[sp] ?? "-1")));
                })
                .ToArray();
        }

        public override string ToString()
        {
            return string.Join("", Measurements.Select(m => m.ToString()).ToArray());
        }
    }
}
