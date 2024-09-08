using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBImageLib
{
    internal class GrayscaleEffect
    {
        internal float r, g, b;
        internal float[][] colorMatrix;
        internal Bitmap Bitmap;
        internal int _width, _height;
        internal GrayscaleEffect(Bitmap bitmap, float r, float g, float b)
        {
            this.Bitmap = bitmap;
            _width = bitmap.Width;
            _height = bitmap.Height;
            this.r = r; this.g = g; this.b = b;

            colorMatrix = new float[][]
                {
                    new float[] {r,r,r,0,0},        //idek whats going on here
                    new float[] {g,g,g,0,0},        //but apparently this works
                    new float[] {b,b,b,0,0},
                    new float[] {0,0,0,1,0},
                    new float[] {0,0,0,0,1}         //dummy element
                };
        }
        internal void Dispose()
        {
            Bitmap.Dispose();
        }
        internal Bitmap Process()
        {
            Bitmap newbitmap = new Bitmap(_width, _height);

            ColorMatrix matrix;
            matrix = new ColorMatrix(colorMatrix);

            ImageAttributes attributes = new ImageAttributes(); 

            attributes.SetColorMatrix(matrix);
            Graphics g = Graphics.FromImage(newbitmap);
            
                g.DrawImage(this.Bitmap,
                         new Rectangle(0, 0, _width, _height), //size
                         0, 0, _width, _height, //size
                         GraphicsUnit.Pixel, attributes); //attributes of graphics

                return newbitmap;
            

            

            
        }
    }
}
