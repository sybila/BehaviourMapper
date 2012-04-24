using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Parameter_Filter
{
    public class RegulatoryContext
    {
        public string[] ContextMasks { get; private set; }

        public Dictionary<string, int> MinimalValues { get; private set; }
        public Dictionary<string, int> MaximalValues { get; private set; }

        public RegulatoryContext(int numOfRegulations)
        {
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

        public RegulatoryContext(string modelFile)
        {
            XDocument doc = XDocument.Load(modelFile);
            IEnumerable<XElement> species = doc.Element("MODEL").Element("STRUCTURE").Elements("SPECIE");

            var maskedSpecies = species
                .SelectMany(e => e.Element("REGULATIONS").Elements("REGUL").Select(r => new { Specie = e, Regul = r.Attribute("mask").Value }))
                .Select(reg => new
                {
                    Specie = reg.Specie,
                    Mask = string.Format("{0}{{{1}}}",
                        reg.Specie.Attribute("name").Value,
                        string.Join(",", Enumerable.Range(0, reg.Regul.Length)
                            .Where(i => reg.Regul[i] == '1')
                            .Select(i => species.ElementAt(int.Parse(reg.Specie.Element("INTERACTIONS").Elements("INTER")
                                .ElementAt(i).Attribute("source").Value)).Attribute("name").Value).ToArray()))
                });

            ContextMasks = maskedSpecies.Select(s => s.Mask).ToArray();
            MinimalValues = maskedSpecies.ToDictionary(s => s.Mask, s => 0);
            MaximalValues = maskedSpecies.ToDictionary(s => s.Mask, s => int.Parse(s.Specie.Attribute("max").Value));
        }
    }
}
