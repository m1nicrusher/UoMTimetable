using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;



namespace ProjectTimetable
{
    class TimetableRenderer
    {
        private readonly Bitmap _bitmap;
        private readonly int _width, _height;
        private readonly Graphics _graphics;

        private readonly int _topMargin, _leftMargin;
        private readonly int _startTime, _endTime;
        // Preset 5 means Mon-Fri (will change to any in the future)
        private readonly int _days = 5;
        // The height of 15 mins
        private readonly int _verticalInterval;
        private readonly int _horizontalInterval;

        private readonly OneWeekEvents _owe;

        /// <summary>
        /// Create the calendar image
        /// </summary>
        /// <param name="width">Width of image</param>
        /// <param name="height">Height of image</param>
        /// <param name="topMargin">Height of top bar (date bar)</param>
        /// <param name="leftMargin">Width of left bar (time bar)</param>
        public TimetableRenderer(int width, int height, int topMargin, int leftMargin, OneWeekEvents owe)
        {
            this._width = width;
            this._height = height;
            this._leftMargin = leftMargin;
            this._topMargin = topMargin;
            this._startTime = owe.EarliestHour;
            this._endTime = owe.LatestHour;
            this._verticalInterval = (_height - _topMargin) / ((_endTime - _startTime + 1) * 4);
            this._horizontalInterval = (_width - _leftMargin) / (_days);
            this._owe = owe;
            this._bitmap = new Bitmap(_width, _height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb); ;
            this._graphics = Graphics.FromImage(_bitmap);
            this._graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            this._graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        private void Save(string fileName)
        {
            _bitmap.Save(fileName);
        }

        /// <summary>
        /// Set the image background with solid color
        /// </summary>
        /// <param name="color">Background color</param>
        private void RenderBackground(Color color)
        {
            using (SolidBrush sb = new SolidBrush(color))
            {
                this._graphics.FillRectangle(sb, 0, 0, _width, _height);
            }
        }

        private void RenderLines()
        {

        }

        private void RenderVerticalLines(Color color, int topMargin)
        {
            using (Pen p = new Pen(color))
            {
                for (int i = _leftMargin; i < _width; i += _horizontalInterval)
                {
                    _graphics.DrawLine(p, i, topMargin, i, _height);
                }
            }
        }

        private void RenderHorizontalLines(Color color1, Color color2)
        {
            using (Pen p1 = new Pen(color1))
            using (Pen p2 = new Pen(color2))
            {
                // Draw Hour Lines
                for (int i = _topMargin; i < _height; i += _verticalInterval * 4)
                {
                    _graphics.DrawLine(p1, 0, i, _width, i);
                }

                //Draw Half-hour Lines
                for (int i = _topMargin + _verticalInterval * 2; i < _height; i += _verticalInterval * 4)
                {
                    _graphics.DrawLine(p2, 0, i, _width, i);
                }
            }
        }

        // This function checks the room size and your text and appropriate font
        //  for your text to fit in room
        // PreferedFont is the Font that you wish to apply
        // Room is your space in which your text should be in.
        // LongString is the string which it's bounds is more than room bounds.
        private Font FindFont(
           System.Drawing.Graphics g,
           string longString,
           Size Room,
           Font PreferedFont
        )
        {
            // you should perform some scale functions!!!
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio)
                  ? ScaleRatio = HeightScaleRatio
                  : ScaleRatio = WidthScaleRatio;

            float ScaleFontSize = PreferedFont.Size * ScaleRatio;

            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }

        private void RenderText(Color color, string fontFamily, int fontSize, Rectangle area, StringFormat format, string text)
        {
            Font font = null;
            try
            {
                if (fontSize == 0)
                    font = FindFont(_graphics, text, area.Size, new Font(fontFamily, 10000));
                else
                    font = new Font(fontFamily, fontSize);
                using (SolidBrush sb = new SolidBrush(color))
                {
                    _graphics.DrawString(text, font, sb, area, format);
                }
            }
            finally { if (font != null) font.Dispose(); }
        }

        private void RenderHours(Color color, string fontFamily, int fontSize)
        {
            for (int i = _startTime; i <= _endTime; i++)
            {
                int t = i - _startTime;
                Rectangle rect = new Rectangle(0, _topMargin + 1 + t * _verticalInterval * 4, _leftMargin, _verticalInterval * 2 - 1);
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = sf.LineAlignment = StringAlignment.Center;
                    RenderText(color, fontFamily, fontSize, rect, sf, $"{i.ToString("d2")}:00");
                }

                // Test real region
                // _graphics.FillRectangle(new SolidBrush(Color.Red), rect);
            }
        }

        private void RenderMonth(Color color, int month, string fontFamily, int fontSize)
        {
            // Note: I'm not sure what's the quickist (most efficient) way to do this but I assume a string[] woulf be nice on both speed and size.
            string[] months = { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            Rectangle rect = new Rectangle(0, 0, _leftMargin, _topMargin);
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = sf.LineAlignment = StringAlignment.Center;
                RenderText(color, fontFamily, fontSize, rect, sf, months[month]);
            }
        }

        private void RenderDays(Color color, DateTime startDate, string fontFamily, int fontSize)
        {
            for (int i = 0; i < _days; i++)
            {
                Rectangle rect = new Rectangle(_leftMargin + 1 + i * _horizontalInterval, 0, _horizontalInterval - 1, _topMargin);
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Far;
                    RenderText(color, fontFamily, fontSize, rect, sf, startDate.Day.ToString("d2"));
                    startDate = startDate.AddDays(1);
                }
            }
        }

        private void RenderEvent(Color color, Rectangle rect)
        {
            using (SolidBrush sb = new SolidBrush(color))
            {
                _graphics.FillRectangle(sb, rect);
            }
        }

        private void RenderEvent(Color color, Rectangle rect, Color titleColor, string fontFamily, int fontSize, string title, string subtitle = null)
        {
            RenderEvent(color, rect);
            Rectangle mainRect, subRect;
            if (subtitle != null)
            {
                mainRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
                subRect = new Rectangle(rect.X, rect.Y + rect.Height / 2 + 1, rect.Width, rect.Height / 2);
                // _graphics.FillRectangle(new SolidBrush(Color.FromArgb(125, Color.Red)), mainRect);
                // _graphics.FillRectangle(new SolidBrush(Color.FromArgb(125, Color.Blue)), subRect);
                RenderText(
                titleColor,
                fontFamily,
                fontSize,
                subRect,
                new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                },
                subtitle
            );
            }
            else
            {
                mainRect = rect;
            }
            RenderText(
                titleColor,
                fontFamily,
                fontSize,
                mainRect,
                new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                },
                title
            );
        }

        private Dictionary<DateTime, Rectangle> blocks = new Dictionary<DateTime, Rectangle>();
        public void Render(string fileName)
        {
            RenderBackground(Color.Black);
            RenderHorizontalLines(Color.Gray, Color.FromArgb(66, 66, 66));
            RenderVerticalLines(Color.Gray, 150);
            RenderHours(Color.White, "Ubuntu", 14);
            RenderMonth(Color.White, _owe.FirstDayOfWeek.Month, "Ubuntu", 24);

            var startDate = _owe.FirstDayOfWeek;
            System.Console.WriteLine(startDate.ToString());
            RenderDays(Color.White, startDate, "Ubuntu", 30);

            // Calculate coordinate of each block
            if (_owe.Events.Count != 0)
            {
                var dayTime = startDate.Date;
                for (int day = 0; day <= _days; day++)
                {
                    dayTime = startDate.AddDays(day);
                    for (int quarter = 0; quarter < 4 * (_endTime - _startTime + 1) + 1; quarter++)
                    {
                        Rectangle rect = new Rectangle(_leftMargin + 1 + day * _horizontalInterval, _topMargin + 1 + quarter * _verticalInterval, _horizontalInterval - 1, _verticalInterval - 1);
                        System.Console.WriteLine(dayTime.ToString());
                        blocks.Add(dayTime.AddHours(_startTime).AddMinutes(quarter * 15), rect);
                    }
                }


                Random r = new Random();

                foreach (var item in _owe.Events)
                {
                    int x, y, width, height;
                    var rect1 = blocks[item.StartTime];
                    var rect2 = blocks[item.EndTime];
                    x = rect1.X;
                    y = rect1.Y;
                    System.Console.WriteLine(rect1.X.ToString() + rect1.Y.ToString());
                    width = rect1.Width;
                    height = rect2.Y - rect1.Y - 1;
                    RenderEvent(
                        Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256)),
                        new Rectangle(x, y, width, height),
                        Color.White,
                        "Ubuntu",
                        12,
                        item.Subject + " | " + item.Type,
                        $"{item.StartTime.ToShortTimeString()}-{item.EndTime.ToShortTimeString()}"
                    );
                }
            }
            Save(fileName);
        }
    }
}