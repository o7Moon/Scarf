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

using linerider.Utils;
using OpenTK.Graphics;
using System;
using System.Drawing;

namespace linerider.Game
{
    public class GameTrigger
    {
        public const int TriggerTypes = 3;
        public int Start;
        public int End;
        public TriggerType TriggerType;
        //Zoom
        public float ZoomTarget = 4;
        //BG
        public int backgroundRed;
        public int backgroundGreen;
        public int backgroundBlue;
        //Line Color
        public int lineRed;
        public int lineGreen;
        public int lineBlue;

        public bool CompareTo(GameTrigger other)
        {
            if (other == null)
                return false;
            return TriggerType == other.TriggerType &&
            Start == other.Start &&
            End == other.End &&
            ZoomTarget == other.ZoomTarget;
        }
        public bool ActivateZoom(int hitdelta, ref float currentzoom)
        {
            bool handled = false;
            if (TriggerType == TriggerType.Zoom)
            {
                int zoomframes = End - Start;
                if (currentzoom != ZoomTarget)
                {
                    if (hitdelta >= 0 && hitdelta < zoomframes)
                    {
                        var diff = ZoomTarget - currentzoom;
                        currentzoom = currentzoom + (diff / (zoomframes - hitdelta));
                        handled = true;
                    }
                    else
                    {
                        currentzoom = ZoomTarget;
                    }
                }
            }
            return handled;
        }
        public bool ActivateBG(int hitdelta, int currentFrame, ref Color4 staticCurrentColor, ref Color4 currentColorChanging, ref Color4 frameInfoColor)
        {
            bool handled = false;
            if (TriggerType == TriggerType.BGChange)
            {
                float fadeframes = End - Start;
                float frame = (currentFrame - Start);

                if (!staticCurrentColor.Equals(new Color4((float)this.backgroundRed, (float)this.backgroundGreen, (float)this.backgroundBlue, 255)))
                {
                    if (frame < fadeframes)
                    {
                        float diffR = this.backgroundRed - staticCurrentColor.R * 255;
                        float diffG = this.backgroundGreen - staticCurrentColor.G * 255;
                        float diffB = this.backgroundBlue - staticCurrentColor.B * 255;
                        float newR = ((staticCurrentColor.R * 255) + (diffR * (frame / fadeframes))) / 255;
                        float newG = ((staticCurrentColor.G * 255) + (diffG * (frame / fadeframes))) / 255;
                        float newB = ((staticCurrentColor.B * 255) + (diffB * (frame / fadeframes))) / 255;

                        if (newR > 1) { newR = newR / 255; }
                        if (newG > 1) { newG = newG / 255; }
                        if (newB > 1) { newB = newB / 255; }

                        frameInfoColor = new Color4((float)newR, (float)newG, (float)newB, 255);
                        currentColorChanging = new Color4((float)newR, (float)newG, (float)newB, 255);

                        //Console.WriteLine("R: " + newR);
                        //Console.WriteLine("G: " + newG);
                        //Console.WriteLine("B: " + newB);

                        handled = true;
                    }
                    else
                    {
                        staticCurrentColor = new Color4((float)this.backgroundRed / 255, (float)this.backgroundGreen / 255, (float)this.backgroundBlue / 255, 255);
                        frameInfoColor = new Color4((float)this.backgroundRed / 255, (float)this.backgroundGreen / 255, (float)this.backgroundBlue / 255, 255);
                        currentColorChanging = new Color4((float)this.backgroundRed / 255, (float)this.backgroundGreen / 255, (float)this.backgroundBlue / 255, 255);
                    }
                }
            }
             return handled;
        }

        public bool ActivateLine(int hitdelta, ref Color staticCurrentColor, ref Color notStaticCurrentColor, int currentFrame, ref Color frameLineColor)
        {
            bool handled = false;
            if (TriggerType == TriggerType.LineColor)
            {
                float fadeframes = End - Start;
                float frame = (currentFrame - Start);

                if (!staticCurrentColor.Equals(Color.FromArgb(255, this.lineRed, this.lineGreen, this.lineBlue)))
                {
                    if (frame < fadeframes)
                    {
                        float diffR = this.lineRed - staticCurrentColor.R;
                        float diffG = this.lineGreen - staticCurrentColor.G;
                        float diffB = this.lineBlue - staticCurrentColor.B;
                        float newR = (staticCurrentColor.R + (diffR * (frame / fadeframes)));
                        float newG = (staticCurrentColor.G + (diffG * (frame / fadeframes)));
                        float newB = (staticCurrentColor.B + (diffB * (frame / fadeframes)));

                        frameLineColor = Color.FromArgb(255, (int)newR, (int)newG, (int)newB);
                        notStaticCurrentColor = Color.FromArgb(255, (int)newR, (int)newG, (int)newB);

                        //Console.WriteLine(newR);
                        //Console.WriteLine(newG);
                        //Console.WriteLine(newB);

                        handled = true;
                    }
                    else
                    {
                        staticCurrentColor = Color.FromArgb(255, this.lineRed, this.lineGreen, this.lineBlue);
                        frameLineColor = Color.FromArgb(255, this.lineRed, this.lineGreen, this.lineBlue);
                        notStaticCurrentColor = Color.FromArgb(255, this.lineRed, this.lineGreen, this.lineBlue);
                    }
                }
            }
            return handled;
        }

        public GameTrigger Clone()
        {
            return new GameTrigger()
            {
                Start = Start,
                End = End,
                TriggerType = TriggerType,
                ZoomTarget = ZoomTarget
            };
        }
    }
}