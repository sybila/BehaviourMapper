using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Parameter_Filter
{
    public class RegulatoryContext
    {
        public string[] Species { get; private set; }
        public string[] ContextMasks { get; private set; }

        public Dictionary<string, int> MinimalValues { get; private set; }
        public Dictionary<string, int> MaximalValues { get; private set; }

        public RegulatoryContext(int numOfRegulations, int numOfSpecies)
        {
            Species = new string[numOfSpecies];

            for (int i = 0; i < numOfSpecies; i++)
                Species[i] = string.Format("s{0}", i);

            ContextMasks = new string[numOfRegulations];
            MinimalValues = new Dictionary<string, int>();
            MaximalValues = new Dictionary<string, int>();

            for (int i = 0; i < numOfRegulations; i++)
            {
                ContextMasks[i] = string.Format("v{0}", i);
                MinimalValues.Add(ContextMasks[i], 0);
                MaximalValues.Add(ContextMasks[i], 1);
            }
        }

        private IEnumerable<int[]> ComputeContexts(IEnumerable<int> regulators)
        {
            return Enumerable.Range(0, (int)Math.Pow(2, regulators.Count()))
                .Select(num =>
                {
                    List<int> context = new List<int>();
                    int n = num;

                    for (int i = 0; i < regulators.Count(); i++)
                    {
                        if ((n % 2) == 1)
                            context.Add(regulators.ElementAt(i));

                        n /= 2;
                    }

                    return context.ToArray();
                });
        }

        public RegulatoryContext(string modelFile)
        {
            XDocument doc = XDocument.Load(modelFile);
            IEnumerable<XElement> species = doc.Element("MODEL").Element("STRUCTURE").Elements("SPECIE");

            Species = species.Zip(Enumerable.Range(0, species.Count()),
                (elem, num) => ((elem.Attribute("name") != null) ? elem.Attribute("name").Value : string.Format("{0}", num))).ToArray();

            ContextMasks = Species
                .Zip(species
                    .Select(s => s.Elements("REGUL").Select(r => int.Parse(r.Attribute("source").Value)))
                    .Select(r => ComputeContexts(r)),
                    (name, cons) => new { Name = name, Cons = cons })
                .SelectMany(ncon => ncon.Cons.Select(con => string.Format("{0}{{{1}}}",
                        ncon.Name, string.Join(",", con.Select(i => Species[i]).ToArray())))).ToArray();

            MinimalValues = Species.ToDictionary(s => s, _ => 0);
            MaximalValues = Species
                .Zip(species, (name, elem) => new { Specie = name, Max = ((elem.Attribute("max") != null) ? int.Parse(elem.Attribute("max").Value) : 1) })
                .ToDictionary(s => s.Specie, s => s.Max);
        }
    }
}
