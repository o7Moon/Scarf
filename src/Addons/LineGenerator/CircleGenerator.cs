using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace linerider.Game.LineGenerator
{
    public class CircleGenerator : Generator
    {
        public double radius; //Radius of the circle
        public Vector2d position; //Centre of the circle
        public int lineCount; //Number of lines used to generate the circle
        public bool invert;
        public bool reverse;
        public LineType _lineType;
        public LineType lineType
        {
            set { _lineType = value; ReGenerate_Preview(); }
            get { return _lineType; }
        }

        public CircleGenerator(string _name, double _radius, Vector2d _position, int _lineCount, bool _invert)
        {
            name = _name;
            lines = new List<GameLine>();
            radius = _radius;
            position = _position;
            lineCount = _lineCount;
            invert = _invert;
            _lineType = LineType.Blue;
        }

        public override void Generate_Internal(TrackWriter trk)
        {
            var points = new List<Vector2d>();
            for (double frac = 0.0; frac < 1.0; frac += 1.0 / (double)lineCount)
            {
                double ang = frac * 2.0 * 3.1415926535897932384626433832795028841971; //There must be a better way of writing pi
                points.Add(position + radius * new Vector2d(Math.Cos(ang), Math.Sin(ang)));
            }
            if (invert != reverse) // XOR
            {
                for (int i = 1; i < points.Count; i++)
                {
                    lines.Add(CreateLine(trk, points[i], points[i - 1], lineType, reverse));
                }
                lines.Add(CreateLine(trk, points[0], points[points.Count - 1], lineType, reverse));
            }
            else
            {
                for (int i = 1; i < points.Count; i++)
                {
                    lines.Add(CreateLine(trk, points[i - 1], points[i], lineType, reverse));
                }
                lines.Add(CreateLine(trk, points[points.Count - 1], points[0], lineType, reverse));
            }
        }
        public override void Generate_Preview_Internal(TrackWriter trk)
        {
            Generate_Internal(trk);
        }
    }
}
