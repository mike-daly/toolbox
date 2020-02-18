using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DBSpecGen
{
    public class PieChart
    {
        #region public methods
        public Bitmap Draw(Color bgColor, int width, int height, float radius, LabelAndValue[] labelAndValue, decimal total, string title, string htmPagePrefix, string htmContainingDirectory, out string imageMapGuts)
        {         
            imageMapGuts = "";

            // if there are no wedges, return a 1x1 empty bitmap.
            if (!(total > 0))
            {
                return new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            }

            // Create a new image and erase the background
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            SolidBrush brush = new SolidBrush(bgColor);
            graphics.FillRectangle(brush, 0, 0, width, height);

            // Create brushes for coloring the pie chart
            SolidBrush[] brushes = new SolidBrush[20];
            brushes[0] = new SolidBrush(Color.Red);
            brushes[1] = new SolidBrush(Color.Orange);
            brushes[2] = new SolidBrush(Color.Yellow);
            brushes[3] = new SolidBrush(Color.LightGreen);
            brushes[4] = new SolidBrush(Color.Green);
            brushes[5] = new SolidBrush(Color.Blue);
            brushes[6] = new SolidBrush(Color.Indigo);
            brushes[7] = new SolidBrush(Color.Violet);
            brushes[8] = new SolidBrush(Color.Maroon);
            brushes[9] = new SolidBrush(Color.Magenta);
            brushes[10] = new SolidBrush(Color.BurlyWood);
            brushes[11] = new SolidBrush(Color.Brown);
            brushes[12] = new SolidBrush(Color.Olive);
            brushes[13] = new SolidBrush(Color.OliveDrab);
            brushes[14] = new SolidBrush(Color.DarkGreen);
            brushes[15] = new SolidBrush(Color.Lavender);
            brushes[16] = new SolidBrush(Color.LavenderBlush);
            brushes[17] = new SolidBrush(Color.Aquamarine);
            brushes[18] = new SolidBrush(Color.Azure);
            brushes[19] = new SolidBrush(Color.AliceBlue);

            int minPercentage = 1;           
            decimal subtotal = 0;
            for (int i = 0; i < labelAndValue.Length && labelAndValue[i] != null; ++i)
            {
                int percentage = (int)(100 * labelAndValue[i].Size/total);
                if (percentage < minPercentage)
                {
                    continue;
                }
                subtotal += (decimal)(labelAndValue[i].Size);
            }

            Pen blackPen = new Pen(Color.Black, 1f);

            // Draw the pie chart
            float start = 0.0f;
            float end = 0.0f;
            decimal current = 0.0m;
            double angleInRadians = 0;
            double lastSweepInRadians = 0;
            for (int i = 0; i < labelAndValue.Length && labelAndValue[i] != null; i++)
            {
                int percentage = (int)(100 * labelAndValue[i].Size/total);
                if (percentage < minPercentage)
                {
                    continue;
                }

                current += labelAndValue[i].Size;
                start = end;
                end = (float) (current / total) * 360.0f;
                angleInRadians += lastSweepInRadians / 2 + (Math.PI / 180) * (end - start) / 2;
                lastSweepInRadians = (Math.PI / 180) * (end - start);
                string drawString = labelAndValue[i].Name + " (" + percentage + "%)";
                imageMapGuts += DrawWedgeAndLabel(graphics, 
                    brushes[i % brushes.Length], 
                    blackPen, 
                    width, 
                    height, 
                    radius, 
                    angleInRadians, 
                    drawString, 
                    start, 
                    end,
                    htmContainingDirectory + "\\" + htmPagePrefix + labelAndValue[i].Name.Replace(" ","").Replace(".","") + ".htm");
            }

            if ((total - subtotal)/total > (decimal)0.01)
            {
                current += total - subtotal;
                start = end;
                end = (float) (current / total) * 360.0f; 
                angleInRadians += lastSweepInRadians / 2 + (Math.PI / 180) * (end - start) / 2;
                DrawWedgeAndLabel(graphics, new SolidBrush(Color.White), blackPen, width, height, radius, angleInRadians, "Other", start, end, "");
            }

            // put the title on.
            if (title.Length > 0)
            {
                Font drawFont = new Font("Verdana", 11, FontStyle.Bold);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                float x = width/2 - graphics.MeasureString(title, drawFont).Width/2;
                float y = 5;
                graphics.DrawString(title, drawFont, drawBrush, x, y);
            }

            return bitmap;
        }
        #endregion

        #region private methods
        private string DrawWedgeAndLabel(Graphics graphics, Brush brush, Pen pen, float width, float height, float radius, double angleInRadians, string label, float start, float end, string url)
        {
            double extraRadius = 0;
            double power = 2;
            
            double slope = 6;
            if (angleInRadians < Math.PI/2)
            {
                extraRadius = Math.Pow(slope * angleInRadians, power);
            }
            else if (angleInRadians < 3 * Math.PI/2)
            {
                extraRadius = Math.Pow(Math.Abs( -slope * (angleInRadians - Math.PI)), power);
            }
            else
            {
                extraRadius = Math.Pow(-slope * (angleInRadians - 2 * Math.PI), power);
            }
            

            graphics.FillPie(brush, width/2 - radius, height/2 - radius, 2 * radius, 2 * radius, start, end - start);
            graphics.DrawPie(pen,   width/2 - radius, height/2 - radius, 2 * radius, 2 * radius, start, end - start);
           
            float lineStartX = (float)(width/2  + (1.1 * radius) * Math.Cos(angleInRadians));
            float lineStartY = (float)(height/2 + (1.1 * radius) * Math.Sin(angleInRadians));
            float lineEndX   = (float)(width/2  + (1.4 * radius + extraRadius) * Math.Cos(angleInRadians));
            float lineEndY   = (float)(height/2 + (1.4 * radius + extraRadius) * Math.Sin(angleInRadians));

            string drawString = label;
            Font drawFont = new Font("Verdana", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            float x = angleInRadians > Math.PI/2 && angleInRadians < 3*Math.PI/2 ? lineEndX - graphics.MeasureString(drawString, drawFont).Width : lineEndX;
            float y = angleInRadians < Math.PI ? lineEndY : lineEndY - 10;
            graphics.DrawString(drawString, drawFont, drawBrush, x, y);
            graphics.DrawLine(pen, lineStartX, lineStartY, lineEndX, lineEndY);

            string imageMapGuts = "";

            if (System.IO.File.Exists(url))
            {
                string polycoord = (int)(width/2) + "," + (int)(height/2);
                double numPointsOnArc = Math.Abs(end - start);
                for (int i = 0; i <= numPointsOnArc; ++i)
                {
                    polycoord += "," + (int)(width/2  + (radius * Math.Cos((start + (i/numPointsOnArc)*(end - start)) * 2 * Math.PI/360)));
                    polycoord += "," + (int)(height/2 + (radius * Math.Sin((start + (i/numPointsOnArc)*(end - start)) * 2 * Math.PI/360)));
                }
                polycoord += "," + (int)(width/2) + "," + (int)(height/2);

                string labelcoord = (int)x + "," + (int)y + "," + (int)(x + graphics.MeasureString(drawString, drawFont).Width) + "," + (int)(y + graphics.MeasureString(drawString, drawFont).Height);
            
                imageMapGuts  = "<area title='" + label + "' shape='rect' coords='" + labelcoord + "' href='" + url + "'/>";
                imageMapGuts += "<area title='" + label + "' shape='poly' coords='" + polycoord + "' href='" + url + "'/>";
            }

            return imageMapGuts;
        }
        #endregion
    }
}
