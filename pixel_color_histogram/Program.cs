// See https://aka.ms/new-console-template for more information

//using System.Windows.Media.Imaging;
using System.Collections.Immutable;
using System.Drawing;

if (args.Length > 0)
{
    Console.WriteLine("args:  {0}", args[0]);
}
else
{
    Console.WriteLine("no args");
    return;
}



Bitmap bImage;
try
{
    bImage = new Bitmap(args[0]);
}
catch (Exception ex)
{
    Console.WriteLine("failed to create bitmap");
    Console.WriteLine("Exception:  {0}", ex);
    return;
}


Dictionary<Color, Int32> colorCount = new Dictionary<Color, Int32>();
Console.WriteLine("bitmap Dimensions:  {0},{1}", bImage.Width, bImage.Height);

//Bitmap usefulBitmap = bImage.Clone(new Rectangle(0, 0, bImage.Width, bImage.Height), System.Drawing.Imaging.PixelFormat.Format32bppRgb);


for (int x = 0; x < bImage.Width; x++)
{
    for (int y = 0; y < bImage.Height; y++)
    {
        Color color = bImage.GetPixel(x, y);

        Color binnedColor = Color.FromArgb(color.R > 100 ? 255 : 0, color.G > 100 ? 255 : 0, color.B > 100 ? 255 : 0);

        if (!colorCount.ContainsKey(binnedColor))
        {
            colorCount.Add(binnedColor, 1);
        }
        else
        {
            colorCount[binnedColor]++;
        }
    }
}


Console.WriteLine("color bins:  {0}", colorCount.Count);
foreach (Color color in colorCount.Keys)
{
    Console.WriteLine("  color:  {0}    count:  {1}", color, colorCount[color]);
}

