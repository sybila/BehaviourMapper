using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Parameter_Filter
{
    class SpecieExpressionGraph
    {
        private class Line
        {
            public int Strength { get; set; }

            private int start;
            private int end;

            private int sourceValue;
            private int targetValue;

            public Line(int start, int end, int sourceValue, int targetValue)
            {
                this.start = start;
                this.end = end;

                this.sourceValue = sourceValue;
                this.targetValue = targetValue;
            }

            public XElement ToXml(XNamespace xmlns, int valueRange, int witnessCount)
            {
                return new XElement(xmlns + "line", new XAttribute("x1", start + 60), new XAttribute("x2", end + 60),
                    new XAttribute("y1", (560 - ((500 * sourceValue) / valueRange))), new XAttribute("y2", (560 - ((500 * targetValue) / valueRange))),
                    new XAttribute("style", string.Format("stroke:rgb(50,0,70);stroke-opacity:{0:0.000};stroke-width:2", ((double)Strength / witnessCount))));
            }

            public override bool Equals(object obj)
            {
                Line other = obj as Line;

                if (other == null)
                    return false;

                return ((this.start == other.start) && (this.end == other.end) && (this.sourceValue == other.sourceValue) &&
                    (this.targetValue == other.targetValue));
            }

            public override int GetHashCode()
            {
                int hash = 111;
                hash += 13 * start.GetHashCode();
                hash += 13 * end.GetHashCode();
                hash += 13 * sourceValue.GetHashCode();
                hash += 13 * targetValue.GetHashCode();
                return hash;
            }
        }

        private int specieIndex;
        private string specie;

        private TimeSerie timeSerie;

        private IList<int> xAxisPoints;

        private int specieMin;
        private int specieMax;

        private int witnessCount;
        private IList<Line> lines;

        public SpecieExpressionGraph(int specieIndex, RegulatoryContext regulatoryContext, TimeSerie timeSerie)
        {
            this.specieIndex = specieIndex;
            this.specie = regulatoryContext.Species.ElementAt(specieIndex);

            this.timeSerie = timeSerie;

            xAxisPoints = new List<int>();
            for (int i = 0; i < timeSerie.Measurements.Count(); i++)
                xAxisPoints.Add((1000 * i) / (timeSerie.Measurements.Count() - 1));

            specieMin = regulatoryContext.MinimalValues[specie];
            specieMax = regulatoryContext.MaximalValues[specie];

            witnessCount = 0;
            lines = new List<Line>();
        }

        public void MapWitness(Witness w)
        {
            int previousX = 0;
            IEnumerable<int> interMeasurementPath;

            for (int i = 1; i < timeSerie.Measurements.Count(); i++)
            {
                IEnumerable<State> postMeasurementPath = w.Sequence
                    .SkipWhile(s => !s.Equals(timeSerie.Measurements.ElementAt(i - 1)));

                interMeasurementPath = postMeasurementPath
                    .TakeWhile(s => !s.Equals(timeSerie.Measurements.ElementAt(i)))
                    .Concat(postMeasurementPath.SkipWhile(s => !s.Equals(timeSerie.Measurements.ElementAt(i))).Take(1))
                    .Select(s => s.ActivityLevels.ElementAt(specieIndex)).ToArray();

                for (int j = 1; j < interMeasurementPath.Count(); j++)
                {
                    int xCoordinate = ((((1000 / (timeSerie.Measurements.Count() - 1)) * j) / (interMeasurementPath.Count() - 1)) +
                        ((i - 1) * (1000 / (timeSerie.Measurements.Count() - 1))));

                    if (!xAxisPoints.Contains(xCoordinate))
                        xAxisPoints.Add(xCoordinate);

                    AddLine(new Line(previousX, xCoordinate, interMeasurementPath.ElementAt(j - 1), interMeasurementPath.ElementAt(j)));

                    previousX = xCoordinate;
                }
            }

            witnessCount++;
        }

        private void AddLine(Line line)
        {
            if (!lines.Contains(line))
                lines.Add(line);

            foreach (Line l in lines)
                if (l.Equals(line))
                    l.Strength++;
        }

        public void Export(string fileName, bool grid)
        {
            XDocument doc = new XDocument();

            XNamespace xmlns = "http://www.w3.org/2000/svg";
            XNamespace xlink = "http://www.w3.org/1999/xlink";
            XElement svg = new XElement(xmlns + "svg", new XAttribute("xmlns", "http://www.w3.org/2000/svg"),
                new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"),
                new XElement(xmlns + "text", new XAttribute("x", 100), new XAttribute("y", 40), new XAttribute("text-anchor", "start"),
                    new XAttribute("font-size", "26"), new XAttribute("fill", "black"), string.Format("Expression profile of {0}", specie)),
                new XElement(xmlns + "text", new XAttribute("x", 950), new XAttribute("y", 600), new XAttribute("text-anchor", "end"),
                    new XAttribute("font-size", "18"), new XAttribute("fill", "black"), "Measurements"),
                new XElement(xmlns + "text", new XAttribute("x", 20), new XAttribute("y", 300), new XAttribute("text-anchor", "middle"),
                    new XAttribute("style", "font-size:18;writing-mode:tb;glyph-orientation-vertical:0"), new XAttribute("fill", "black"),
                    string.Format("{0} activity level", specie)),
                new XElement(xmlns + "line", new XAttribute("x1", 60), new XAttribute("x2", 1060), new XAttribute("y1", 570), new XAttribute("y2", 570),
                    new XAttribute("style", "stroke:rgb(50,50,50);stroke-width:3")),
                new XElement(xmlns + "line", new XAttribute("x1", 60), new XAttribute("x2", 60), new XAttribute("y1", 60), new XAttribute("y2", 570),
                    new XAttribute("style", "stroke:rgb(50,50,50);stroke-width:3")),
                Enumerable.Range(1, timeSerie.Measurements.Count()).Zip(xAxisPoints.Take(timeSerie.Measurements.Count()),
                    (m, x) => new
                    {
                        Elements = new XElement[]
                        {
                            new XElement(xmlns + "text", new XAttribute("x", (x + 60)), new XAttribute("y", 585),
                                new XAttribute("text-anchor", "middle"), new XAttribute("fill", "black"), string.Format("{0}", m)),
                            new XElement(xmlns + "line", new XAttribute("x1", (x + 60)), new XAttribute("x2", (x + 60)), new XAttribute("y1", 60),
                                new XAttribute("y2", 570), new XAttribute("style", "stroke:rgb(50,50,50);stroke-opacity:0.5;stroke-width:0.25"))
                        }
                    }).SelectMany(elms =>
                    {
                        if (grid)
                            return elms.Elements;
                        else
                            return elms.Elements.Take(1);
                    }),
                Enumerable.Range(specieMin, (specieMax + 1 - specieMin)).Zip(Enumerable.Range(0, (specieMax + 1 - specieMin)),
                    (v, i) => new
                    {
                        Elements = new XElement[]
                        {
                            new XElement(xmlns + "text", new XAttribute("x", 58), new XAttribute("y", (560 - ((500 * i) / (specieMax - specieMin)))),
                                new XAttribute("text-anchor", "end"), new XAttribute("fill", "black"), string.Format("{0}", v)),
                            new XElement(xmlns + "line", new XAttribute("x1", 60), new XAttribute("x2", 1060),
                                new XAttribute("y1", (560 - ((500 * i) / (specieMax - specieMin)))),
                                new XAttribute("y2", (560 - ((500 * i) / (specieMax - specieMin)))),
                                new XAttribute("style", "stroke:rgb(50,50,50);stroke-opacity:0.5;stroke-width:0.25"))
                        }
                    }).SelectMany(elms =>
                    {
                        if (grid)
                            return elms.Elements;
                        else
                            return elms.Elements.Take(1);
                    }),
                lines.Select(l => l.ToXml(xmlns, (specieMax - specieMin), witnessCount)));

            doc.Add(svg);

            doc.Save(fileName);
        }
    }
}
