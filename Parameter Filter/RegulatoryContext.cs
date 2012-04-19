using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public class RegulatoryContext
    {
        public static RegulatoryContext regulatoryContext;

        public string[] ContextMasks { get; private set; }

        public Dictionary<string, int> MinimalValues { get; private set; }
        public Dictionary<string, int> MaximalValues { get; private set; }

        private RegulatoryContext()
        {
            MinimalValues = new Dictionary<string, int>();
            MaximalValues = new Dictionary<string, int>();
        }

        public static void Init(int numOfRegulations)
        {
            regulatoryContext = new RegulatoryContext();

            regulatoryContext.ContextMasks = new string[numOfRegulations];

            for (int i = 0; i < numOfRegulations; i++)
            {
                regulatoryContext.ContextMasks[i] = string.Format("v{0}", i);
                regulatoryContext.MinimalValues.Add(regulatoryContext.ContextMasks[i], 0);
                regulatoryContext.MaximalValues.Add(regulatoryContext.ContextMasks[i], 1);
            }
        }
    }
}
