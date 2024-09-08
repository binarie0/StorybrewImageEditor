using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBImageLib
{
    public static class OnePX
    {
        internal static bool generated = false;
        internal static string p = "sb/1px.png", blank = "";
        /// <summary>
        /// Generates a 1x1 image file to be used for vector scaling.
        /// </summary>
        public static void GenerateOnePX()
        {
            string path = Path.Combine(ImageEditor.MapsetPath, p).Replace('/', '\\');
            if (File.Exists(path)) { generated = true; return; }
            Bitmap bitmap = new Bitmap(1, 1);
            bitmap.SetPixel(0, 0, Color.White);
            try
            {
                bitmap.Save(path);
            }
            catch (Exception)
            {
                generated = false; return;
            }
            generated = true;
            return;
        }
        /// <summary>
        /// To be used if you do not know what the file path is<br/>
        /// Statically typed: "sb/1px.png"
        /// </summary>
        public static string FilePath => generated ? p : blank;
    }
}
