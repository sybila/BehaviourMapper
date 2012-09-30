using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Parameter_Filter
{
    public static class ParameterParser
    {
        public static IEnumerable<Parameter> Parse(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Parameter param = new Parameter(line);

                    yield return param;
                }
            }
        }
    }
}
