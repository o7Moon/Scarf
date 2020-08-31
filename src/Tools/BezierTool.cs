//  Author:
//       Noah Ablaseau <nablaseau@hotmail.com>
//
//  Copyright (c) 2017 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using linerider.Rendering;
using OpenTK;
using System;
using Color = System.Drawing.Color;
using OpenTK.Input;
using linerider.Game;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace linerider.Tools
{
    public class BezierTool : Tool
    {
        public override MouseCursor Cursor
        {
            get { return game.Cursors["line"]; }
        }

        public override bool ShowSwatch
        {
            get
            {
                return true;
            }
        }
        public bool Snapped = false;
        private const float MINIMUM_LINE = 0.01f;
        private bool _addflip;
        private List<Vector2d> points = new List<Vector2d> { };
        private Vector2d _end;
        private Vector2d _start;
        private int resolution = 100; // TODO: Make customizable ingame

        public BezierTool()
            : base()
        {
        }

        public override void OnChangingTool()
        {
            Stop();
        }
        public override void OnMouseDown(Vector2d pos)
        {
            Active = true;
            var gamepos = ScreenToGameCoords(pos);
            if (EnableSnap)
            {
                using (var trk = game.Track.CreateTrackReader())
                {
                    var snap = TrySnapPoint(trk, gamepos, out bool success);
                    if (success)
                    {
                        _start = snap;
                        Snapped = true;
                    }
                    else
                    {
                        _start = gamepos;
                        Snapped = false;
                    }
                }
            }
            else
            {
                _start = gamepos;
                Snapped = false;
            }


            _addflip = UI.InputUtils.Check(UI.Hotkey.LineToolFlipLine);
            _end = _start;
            game.Invalidate();
            base.OnMouseDown(pos);
        }

        public override void OnMouseRightDown(Vector2d pos)
        {
            Active = false;
            _addflip = UI.InputUtils.Check(UI.Hotkey.LineToolFlipLine);
            using (var trk = game.Track.CreateTrackWriter())
            {

                List <Vector2> newPoints = GameRenderer.GenerateBezierCurve(points.ToArray(), resolution).ToList();
                newPoints.Add((Vector2) _end);
                game.Track.UndoManager.BeginAction();
                for (int i = 1; i < newPoints.Count; i++)
                {
                    Vector2d _start = (Vector2d)newPoints[i - 1];
                    Vector2d _end = (Vector2d)newPoints[i];
                    if ((_end - _start).Length >= MINIMUM_LINE)
                    {
                        var added = CreateLine(trk, _start, _end, _addflip, Snapped, EnableSnap);
                        if (added is StandardLine)
                        {
                            game.Track.NotifyTrackChanged();
                        }
                    }
                }
                game.Track.UndoManager.EndAction();
            }
            points.Clear();
            game.Invalidate();
            base.OnMouseRightDown(pos);
        }

        public override void OnMouseMoved(Vector2d pos)
        {
            if (Active)
            {
                _end = ScreenToGameCoords(pos);
                if (game.ShouldXySnap())
                {
                    _end = Utility.SnapToDegrees(_start, _end);
                }
                else if (EnableSnap)
                {
                    using (var trk = game.Track.CreateTrackReader())
                    {
                        var snap = TrySnapPoint(trk, _end, out bool snapped);
                        if (snapped && snap != _start)
                        {
                            _end = snap;
                        }
                    }
                }
                game.Invalidate();
            }
            base.OnMouseMoved(pos);
        }

        public override void OnMouseUp(Vector2d pos)
        {
            game.Invalidate();
            if (Active)
            {
                points.Add(_start);
                var diff = _end - _start;
                var x = diff.X;
                var y = diff.Y;
                if (game.ShouldXySnap())
                {
                    _end = Utility.SnapToDegrees(_start, _end);
                }
                else if (EnableSnap)
                {
                    using (var trk = game.Track.CreateTrackWriter())
                    {
                        var snap = TrySnapPoint(trk, _end, out bool snapped);
                        if (snapped && snap != _start)
                        {
                            _end = snap;
                        }
                    }
                }
            }
            Snapped = false;
            base.OnMouseUp(pos);
        }
        public override void Render()
        {
            base.Render();
            if (Active)
            {
                var diff = _end - _start;
                var x = diff.X;
                var y = diff.Y;
                Color c = Color.FromArgb(200, 150, 150, 150);

                List<Vector2> newPoints = new List<Vector2> { };
                for (int i = 0; i < points.Count; i++)
                {
                    newPoints.Add((Vector2)points[i]);
                }
                newPoints.Add((Vector2)_end);

                switch (Swatch.Selected)
                {
                    case LineType.Blue:
                        renderPoints(points, Settings.Lines.StandardLine);
                        GameRenderer.DrawBezierCurve(newPoints.ToArray(), Settings.Lines.StandardLine, resolution);
                        break;

                    case LineType.Red:
                        renderPoints(points, Settings.Lines.AccelerationLine);
                        GameRenderer.DrawBezierCurve(newPoints.ToArray(), Settings.Lines.AccelerationLine, resolution);
                        break;

                    case LineType.Scenery:
                        renderPoints(points, Settings.Lines.SceneryLine);
                        GameRenderer.DrawBezierCurve(newPoints.ToArray(), Settings.Lines.SceneryLine, resolution);
                        break;
                }
            }

        }
        private void renderPoints(List<Vector2d> points, Color color)
        {
            for (int i = 0; i < points.Count; i++)
            {
                GameRenderer.DrawCircle(points[i], 5, color);
            }
        }
        public override void Cancel()
        {
            Stop();
        }
        public override void Stop()
        {
            Active = false;
            points.Clear();
        }
    }
}