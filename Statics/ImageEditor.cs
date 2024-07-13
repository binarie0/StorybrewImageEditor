
using StorybrewCommon.Scripting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SBImageLib
{
    /// <summary>
    /// Static class that every class inside of the namespace StorybrewImageEditor uses.<br/><br/>
    /// This is just so new variables don't need to be set every time.
    /// </summary>
    public static class ImageEditor
    {
        /// <summary>
        /// Mapset Path from which all reference images / generated images should stem from.
        /// </summary>
        internal static String MapsetPath = StoryboardObjectGenerator.Current.MapsetPath;
        private static ImageCodecInfo[] codecList = ImageCodecInfo.GetImageEncoders();
        internal static ImageCodecInfo JpegEncoder => codecList[1]; //used test generation to find these numbers
        internal static ImageCodecInfo PngEncoder => codecList[codecList.Length - 1]; //used test generation to find these numbers
        
        
        /// <summary>
        /// Gets the file extension of the path.
        /// </summary>
        /// <param name="path">Partial or full path.</param>
        /// <returns></returns>
        internal static string GetExtension(string path)
        {
            string[] spl = path.Split('.');
            if (spl.Length == 2)
            {
                return (spl[spl.Length - 1]);
            }
            return null;
        }
        
        //for reference
        internal static int jpg = "jpg".GetHashCode();
        /// <summary>
        /// Gets the codec of the image, which should be used in export
        /// </summary>
        /// <param name="extension">the file extension of the image</param>
        /// <returns></returns>
        internal static ImageCodecInfo GetCodec(string extension)
                    => (extension.GetHashCode() == jpg) ? JpegEncoder : PngEncoder;

        
        /// <summary>
        /// Controls an image's readonly attribute.
        /// </summary>
        /// <param name="fullpath">Full path of the image.</param>
        /// <param name="io">Set to true for readonly.</param>
        /// <exception cref="Exception"></exception>
        internal static void SetReadOnly(string fullpath, bool io)
        {
            if (fullpath == null) throw new Exception();

            try
            {
                FileInfo info = new FileInfo(fullpath);
                info.IsReadOnly = io;
            }
            catch
            {
                throw new Exception();
            }
        }
        
        
        /// <summary>
        /// Gets the full export path for an image.
        /// </summary>
        /// <param name="partialpath">Relative path compared to the mapset path.</param>
        /// <returns></returns>
        internal static string GetFullExportPath(string partialpath) => 
                Path.Combine(MapsetPath, partialpath).Replace('/', '\\');

        
        
        /// <summary>
        /// Creates a sub-folder at the specified path if it does not already exist.
        /// </summary>
        /// <param name="fullpath">Full Path of folder to be checked and generated.</param>
        /// <exception cref="Exception"></exception>
        internal static void CreateFolder(string partial)
        {
            string fullpath = Path.Combine(MapsetPath, partial).Replace('/', '\\');
            bool exists = System.IO.Directory.Exists(fullpath);
            if (!exists)
            {
                try
                {
                    DirectoryInfo d = System.IO.Directory.CreateDirectory(fullpath);
                }
                catch (Exception e)
                {
                    throw new Exception("Directory could not be created!", e);
                }
            }
        }
        
        
        /// <summary>
        /// Exports an image to the specified path and with the specified codec
        /// </summary>
        /// <param name="bmap">The bitmap to be converted into an image file.</param>
        /// <param name="exportPath">The full export path of the image.</param>
        /// <param name="codec">The codec to use for export.</param>
        /// <returns></returns>
        internal static void ExportImage(Bitmap bmap, 
                                    string exportPath, 
                                    ImageCodecInfo codec)
        {
            bmap.Save(exportPath, codec, null);
            bmap.Dispose();
        }

        internal static bool CheckBounds(Rectangle one, Rectangle two)
        {
            return ((one.IsEmpty && two.IsEmpty) ||
                (one.X == two.X && one.Y == two.Y && one.Width == two.Width && one.Height == two.Height));
        }


        /// <summary>
        /// Generates a Unique ID for the parameters inputted.
        /// </summary>
        /// <param name="BlurStrength"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="inversed"></param>
        /// <returns></returns>
        internal static string GenerateID(int BlurStrength, float r, float g, float b, bool inversed, Rectangle rect)
        {
            
            string blur = BlurStrength <= 0 ? "" : "-blur-" + BlurStrength.ToString();
            string rgb = (r < 0.01 && g < 0.01 && b < 0.01) ? "" : "-mono-r" + r.ToString() + "g" + g.ToString() + "b" + b.ToString();

            string inv = !inversed ? "" : "-inv-";
            
            
            
            string crop = (CheckBounds(rect, ImageHandler.FullImage)) ? "" : "x" + rect.X + "y" + rect.Y + "w" + rect.Width + "h" + rect.Height + "";
            return blur + rgb + inv + crop;
        }
        

        internal static string GetFileName(string path)
        {
            string[] a = path.Split('/');
            string[] b = a[a.Length - 1].Split('.');

            return b[0];
        }
            
    }
}
