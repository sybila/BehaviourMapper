using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public static class ParameterParser
    {
        public static IEnumerable<Parameter> Parse(string filePath, WitnessSet witnessSet)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                Parameter param = null;

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.First() == '[')
                    {
                        if (param != null)
                            yield return param;

                        param = new Parameter(line.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                    else
                        param.AddWitness(witnessSet.GetWitness(line));
                }

                yield return param;
            }
        }
    }
}
