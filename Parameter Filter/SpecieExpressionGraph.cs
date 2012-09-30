using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Parameter_Filter
{
    using Transitions;

    class SpecieExpressionGraph
    {
        public static readonly short GRID = 1;
        public static readonly short HIGHLIGHT_MEASUREMENTS = 2;

        private static readonly int WIDTH = 1000;
        private static readonly int HEIGHT = 500;

        private static readonly int STROKE_WIDTH = 2;

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
                if (targetValue == sourceValue)
                {
                    return new XElement(xmlns + "line", new XAttribute("x1", (start + 60)), new XAttribute("x2", (end + 60)),
                       new XAttribute("y1", (HEIGHT + 60 - ((HEIGHT * sourceValue) / valueRange))),
                       new XAttribute("y2", (HEIGHT + 60 - ((HEIGHT * targetValue) / valueRange))),
                       new XAttribute("style", string.Format("stroke:rgb(50,0,70);stroke-opacity:{0:0.000};stroke-width:{1}", ((double)Strength / witnessCount), STROKE_WIDTH)));
                }

                return new XElement(xmlns + "path", new XAttribute("d", string.Format("M{0},{1} C{2} L{3},{4} C{5}",
                        (start + 60), (HEIGHT + 60 - ((HEIGHT * sourceValue) / valueRange)),
                        GetArc((start + 60), (HEIGHT + 60 - ((HEIGHT * sourceValue) / valueRange)), (start + 60 + ((end - start) / 3)),
                            ((HEIGHT + 60 - ((HEIGHT * sourceValue) / valueRange)) + (Math.Sign(sourceValue - targetValue) * (100 / (valueRange)))), true),
                            (end + 60 - ((end - start) / 3)),
                            ((HEIGHT + 60 - ((HEIGHT * targetValue) / valueRange)) + (Math.Sign(targetValue - sourceValue) * (100 / (valueRange)))),
                        GetArc((end + 60 - ((end - start) / 3)),
                            ((HEIGHT + 60 - ((HEIGHT * targetValue) / valueRange)) + (Math.Sign(targetValue - sourceValue) * (100 / (valueRange)))),
                            (end + 60), (HEIGHT + 60 - ((HEIGHT * targetValue) / valueRange)), false))),
                    new XAttribute("style", string.Format("stroke:rgb(50,0,70);stroke-opacity:{0:0.000};stroke-width:{1};fill:none",
                        ((double)Strength / witnessCount), STROKE_WIDTH)));
            }

            private string GetArc(int sourceX, int sourceY, int targetX, int targetY, bool convex)
            {
                if (convex)
                    return string.Format("{0},{1} {2},{3} {4},{5}", (sourceX + ((targetX - sourceX) / 2)), sourceY,
                        (sourceX + (((targetX - sourceX) * 3) / 4)), (sourceY + ((targetY - sourceY) / 2)), targetX, targetY);
                else
                    return string.Format("{0},{1} {2},{3} {4},{5}", (sourceX + ((targetX - sourceX) / 4)), (sourceY + ((targetY - sourceY) / 2)),
                        (sourceX + ((targetX - sourceX) / 2)), targetY, targetX, targetY);
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

        private int behaviourCount;
        private IList<Line> lines;

        public SpecieExpressionGraph(int specieIndex, RegulatoryContext regulatoryContext, TimeSerie timeSerie)
        {
            this.specieIndex = specieIndex;
            this.specie = regulatoryContext.Species.ElementAt(specieIndex);

            this.timeSerie = timeSerie;

            xAxisPoints = new List<int>();
            for (int i = 0; i < timeSerie.Measurements.Count(); i++)
                xAxisPoints.Add((WIDTH * i) / (timeSerie.Measurements.Count() - 1));

            specieMin = regulatoryContext.MinimalValues[specie];
            specieMax = regulatoryContext.MaximalValues[specie];

            behaviourCount = 0;
            lines = new List<Line>();
        }

        public void MapBehaviour(Behaviour behaviour)
        {
            int previousX = 0;
            IEnumerable<int> interMeasurementPath;

            for (int i = 1; i < timeSerie.Measurements.Count(); i++)
            {
                IEnumerable<State> postMeasurementPath = behaviour.Sequence
                    .SkipWhile(s => !s.Equals(timeSerie.Measurements.ElementAt(i - 1)));

                interMeasurementPath = postMeasurementPath
                    .TakeWhile(s => !s.Equals(timeSerie.Measurements.ElementAt(i)))
                    .Concat(postMeasurementPath.SkipWhile(s => !s.Equals(timeSerie.Measurements.ElementAt(i))).Take(1))
                    .Select(s => s.ActivityLevels.ElementAt(specieIndex)).ToArray();

                for (int j = 1; j < interMeasurementPath.Count(); j++)
                {
                    int xCoordinate = ((((WIDTH / (timeSerie.Measurements.Count() - 1)) * j) / (interMeasurementPath.Count() - 1)) +
                        ((i - 1) * (WIDTH / (timeSerie.Measurements.Count() - 1))));

                    if (!xAxisPoints.Contains(xCoordinate))
                        xAxisPoints.Add(xCoordinate);

                    AddLine(new Line(previousX, xCoordinate, interMeasurementPath.ElementAt(j - 1), interMeasurementPath.ElementAt(j)));

                    previousX = xCoordinate;
                }
            }

            behaviourCount++;
        }

        private void AddLine(Line line)
        {
            if (!lines.Contains(line))
                lines.Add(line);

            foreach (Line l in lines)
                if (l.Equals(line))
                    l.Strength++;
        }

        public void Export(string fileName, short modifiers)
        {
            XDocument doc = new XDocument();

            XNamespace xmlns = "http://www.w3.org/2000/svg";
            XNamespace xlink = "http://www.w3.org/1999/xlink";
            XElement svg = new XElement(xmlns + "svg", new XAttribute("xmlns", "http://www.w3.org/2000/svg"),
                new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"),
                new XElement(xmlns + "text", new XAttribute("x", 100), new XAttribute("y", 40), new XAttribute("text-anchor", "start"),
                    new XAttribute("font-size", "26"), new XAttribute("fill", "black"), string.Format("Expression profile of {0}", specie)),
                /*new XElement(xmlns + "text", new XAttribute("x", 950), new XAttribute("y", 610), new XAttribute("text-anchor", "end"),
                    new XAttribute("font-size", "18"), new XAttribute("fill", "black"), "Measurements"),
                new XElement(xmlns + "text", new XAttribute("x", 20), new XAttribute("y", 300), new XAttribute("text-anchor", "middle"),
                    new XAttribute("style", "font-size:18;writing-mode:tb;glyph-orientation-vertical:0"), new XAttribute("fill", "black"),
                    string.Format("{0} activity level", specie)),*/
                Enumerable.Range(1, timeSerie.Measurements.Count()).Zip(xAxisPoints.Take(timeSerie.Measurements.Count()),
                    (m, x) => new
                    {
                        Elements = new XElement[]
                        {
                            new XElement(xmlns + "rect",
                                new XAttribute("width", ((20 + (10 * timeSerie.Measurements.First().ActivityLevels.Count())) / ((m == 1) ? 2 : 1))),
                                new XAttribute("height", (HEIGHT + 30)),
                                new XAttribute("x", ((m == 1) ? 60 : (x + 60 - ((20 + (10 * timeSerie.Measurements.First().ActivityLevels.Count())) / 2)))),
                                new XAttribute("y", 60),
                                new XAttribute("style", string.Format("stroke:rgb(50,50,50);stroke-width:0.25;fill:rgb(135,200,210);opacity:0.4"))),
                            new XElement(xmlns + "text", new XAttribute("x", (x + 60)), new XAttribute("y", (HEIGHT + 85)),
                                new XAttribute("text-anchor", "middle"), new XAttribute("fill", "black"),
                                (((modifiers & 2) > 0) ? timeSerie.Measurements.ElementAt(m - 1).ToString() : m.ToString())),
                            new XElement(xmlns + "line", new XAttribute("x1", (x + 60)), new XAttribute("x2", (x + 60)), new XAttribute("y1", 60),
                                new XAttribute("y2", (HEIGHT + 70)), new XAttribute("style", "stroke:rgb(50,50,50);stroke-opacity:0.5;stroke-width:0.25"))
                        }
                    }).SelectMany(elms =>
                    {
                        IEnumerable<XElement> elems = elms.Elements;
                        if ((modifiers & 1) == 0)
                            elems = elems.Take(2);
                        if ((modifiers & 2) == 0)
                            elems = elems.Skip(1);

                        return elems;
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
                        if ((modifiers & 1) > 0)
                            return elms.Elements;
                        else
                            return elms.Elements.Take(1);
                    }),
                new XElement(xmlns + "line", new XAttribute("x1", 60), new XAttribute("x2", (WIDTH + 60)), new XAttribute("y1", (HEIGHT + 70)), new XAttribute("y2", (HEIGHT + 70)),
                    new XAttribute("style", "stroke:rgb(50,50,50);stroke-width:3")),
                new XElement(xmlns + "line", new XAttribute("x1", 60), new XAttribute("x2", 60), new XAttribute("y1", 60), new XAttribute("y2", (HEIGHT + 70)),
                    new XAttribute("style", "stroke:rgb(50,50,50);stroke-width:3")),
                lines.Select(l => l.ToXml(xmlns, (specieMax - specieMin), behaviourCount)));

            doc.Add(svg);

            doc.Save(fileName);
        }
    }
}
