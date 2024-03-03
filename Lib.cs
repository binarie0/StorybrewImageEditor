using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;

namespace StorybrewImageEditor
{
    /// <summary>
    /// Edits an inputted sprite and outputs to a new path. <br/>  
    /// <br/>
    /// EditedSprite should be called like so: <br/>
    /// EditedSprite sprite = EditedSprite.NewSprite(OldPath, NewPath); <br/>
    /// <br/>
    /// All Commands can be added like so: <br/>
    /// sprite.Blur(10); *10 pixels* <br/>
    /// sprite.BlackAndWhite(); *default black and white, can be edited for custom ranges* <br/>
    /// </summary>
    public class EditedOsbSprite
    {
        #region Paths
        /// <summary>
        /// New Sprite's path. Will be relative to mapset folder.
        /// </summary>
        public string Path;
        public double Width, Height;
        /// <summary>
        /// Old Sprite Path. 
        /// </summary>
        private string OldPath;
        private string OldPath_FileName;
        private string OldPath_FileExtension;

        private string FullyQualifiedOldPath;
        private string FullyQualifiedNewPath;

        private static ImageCodecInfo[] codecList = ImageCodecInfo.GetImageEncoders();
        #endregion
        #region Bitmap
        private Bitmap Bitmap;
        #endregion
        #region Sprite Commands

        private BlurCommand BlurCommand = null;
        private int BlurCommand_Hash;
        private BlackAndWhiteCommand BlackAndWhiteCommand = null;
        private int BlackAndWhiteCommand_Hash;

        #endregion

        private string DEBUG_FullPath() => FullyQualifiedNewPath;
        public static EditedOsbSprite NewSprite(string OldPath)
            =>
            //checks if file exists. if it does, make new instance
            File.Exists((ImageEditor.MapsetPath + "\\" + OldPath).Replace("/", "\\")) ?
                new EditedOsbSprite(OldPath) : null;
        // private StoryboardObjectGenerator Generator; //specifically for debug
        private EditedOsbSprite(string OldPath)
        {
            this.OldPath = OldPath;

            InitializePathSubstrings();

            this.FullyQualifiedOldPath = ImageEditor.MapsetPath + "/" + OldPath;
            //this.FullyQualifiedNewPath = MapsetFolder + "/" + NewPath;
            this.Bitmap = new Bitmap(Image.FromFile(FullyQualifiedOldPath));

            this.Width = this.Bitmap.Width; this.Height = this.Bitmap.Height;
        }
        private void InitializePathSubstrings()
        {
            string[] strs = OldPath.Split('/');
            string[] strs2 = strs[strs.Length - 1].Split('.');


            OldPath_FileName = strs2[0];
            OldPath_FileExtension = "." + strs2[1];

        }
        /// <summary>
        /// Blurs the image.
        /// </summary>
        /// <param name="strength">Strength *in pixels* of the blur effect.</param>
        public void Blur(int strength)
        {
            //set mins
            if (strength < 0) return;
            //set blur command
            this.BlurCommand = new BlurCommand(strength);
            this.BlurCommand_Hash = strength;
        }
        /// <summary>
        /// Turns the image black and white.
        /// </summary>
        /// <param name="r">Strength (between 0 and 1) of the reds.</param>
        /// <param name="g">Strength (between 0 and 1) of the greens.</param>
        /// <param name="b">Strength (between 0 and 1) of the blues.</param>
        public void Grayscale(float r = 0.5f, float g = 0.5f, float b = 0.5f)
        {
            //set mins and maxes
            if (r < 0) r = 0; if (r > 1) r = 1;
            if (g < 0) g = 0; if (g > 1) g = 1;
            if (b < 0) b = 0; if (b > 1) b = 1;
            //set command
            this.BlackAndWhiteCommand = new BlackAndWhiteCommand(r, g, b);
            //17000 and 288 are just random numbers it doesn't really matter
            this.BlackAndWhiteCommand_Hash = (int)(r*g*17000 + b*b*288);
            //create unique id
        }

        public static ImageCodecInfo JpegEncoder => codecList[1]; //used test generation to find these numbers
        public static ImageCodecInfo PngEncoder => codecList[codecList.Length - 1]; //used test generation to find these numbers
        private string GeneratePath()
        {
            //precondition -- if both null then it's still the original image
            if (BlurCommand == null && BlackAndWhiteCommand == null) { Path = OldPath; return null; }


            //convert everything to a string

            string aPath = "sb/" + OldPath_FileName + "_" +
                            (BlurCommand == null ?
                                "[]" :
                                "[" + BlurCommand.GetStrength().ToString() + "]") +
                            (BlackAndWhiteCommand == null ?
                                "[]" :
                                "[" + BlackAndWhiteCommand.GetColorString() + "]");
                            

            return aPath;
        }

        private bool ExistenceOfGeneration()
        {
            //set fully qualified path name
            FullyQualifiedNewPath = ImageEditor.MapsetPath + "/" + Path;
            //Generator.Log(FullyQualifiedNewPath);
            return (File.Exists(FullyQualifiedNewPath));

        }
        public void Export(ImageCodecInfo codec = null)
        {

            //WORKS
            #region Generate Paths
            //generates new path based on blur and black and white command
            string newPath = GeneratePath();


            #endregion
            //if null, then image is old path (set in GeneratePath)
            if (newPath == null) { return; }
            //if a file already exists with that exact code, then it's already been generated.



            //WORKS
            #region Default Encoding Exports

            if (codec == null)
            {
                codec = PngEncoder;
            }
            #endregion
            #region Check For Existence
            //now that null check is done, no matter what the image should be generated at that path
            Path = newPath + (codec == PngEncoder ? ".png" : ".jpg");
            //if FQPN exists with that exact code, then image has already been generated

            //checks existence 
            //DEBUG :
            //checker still needs documentation
            if (ExistenceOfGeneration()) return;
            #endregion
            //set new bitmap
            Bitmap newbitmap = new Bitmap(this.Bitmap.Width, this.Bitmap.Height),
                transfer;

            //graphics object can be used twice 
            Graphics graphics = null;
            bool flag = false;
            //possible variables
            GaussianBlur gblur;
            ColorMatrix matrix;
            float[][] colors;
            ImageAttributes attributes;
            float r, g, b, a = 1, w = 1;
            if (BlackAndWhiteCommand != null)
            {
                flag = true;
                //just so i don't need to reference it constantly
                r = BlackAndWhiteCommand.GetRed();
                g = BlackAndWhiteCommand.GetGreen();
                b = BlackAndWhiteCommand.GetBlue();

                //get graphics object
                graphics = Graphics.FromImage(newbitmap);

                //create and instantiate color matrix
                colors = new float[][]
                {
                    new float[] {r,r,r,0,0},
                    new float[] {g,g,g,0,0},
                    new float[] {b,b,b,0,0},
                    new float[] {0,0,0,a,0},
                    new float[] {0,0,0,0,w}
                };

                //initialize matrix
                matrix = new ColorMatrix(colors);

                //initialize attributes
                attributes = new ImageAttributes();

                //set color matrix
                attributes.SetColorMatrix(matrix);

                //draw original image on !new image! except with grayscale color matrix
                graphics.DrawImage(this.Bitmap,
                         new Rectangle(0, 0, this.Bitmap.Width, this.Bitmap.Height), //size
                         0, 0, this.Bitmap.Width, this.Bitmap.Height, //size
                         GraphicsUnit.Pixel, attributes); //attributes of graphics

            }
            if (BlurCommand != null)
            {
                //saves memory since we dont want 2 bitmaps if we dont need it
                //couldve used marshal.copy but it just makes more sense this way
                transfer = flag ? newbitmap : null;
                //set gaussianblur instance
                gblur = new GaussianBlur(flag ? transfer : this.Bitmap);
                //set new bitmap equal to the output
                newbitmap = gblur.Process(BlurCommand.GetStrength());

                //honestly can't believe it's that easy

            }



            /* DEBUG : Invalid Characters In Path Name
            
            string prefix = "Position: ", midfix = " Character: ", postfix = " Short Value: ";
            char[] backtochar = FullyQualifiedNewPath.ToCharArray();

            //set directory character to lowercase?
            backtochar[0] = ToLower(backtochar[0]);

            char[] invalid_filename = System.IO.Path.GetInvalidFileNameChars();
            for (int i = 0; i < backtochar.Length; i++)
            {
                for (int t = 0; t < invalid_filename.Length; t++)
                {
                    if (backtochar[i] == invalid_filename[t])
                    {
                        Generator.Log(prefix + i.ToString() + midfix + backtochar[i] + postfix + (short)(backtochar[i]));
                        if (backtochar[i] == '/') backtochar[i] = '\\';
                        
                    }
                }
            }

            string test2 = backtochar.ToString();
            Generator.Log(test2);
            //this doesn't return anything
            string test = System.IO.Path.Combine(MapsetFolder, Path).Replace('/', '\\');
            Generator.Log("System.IO.Path.Combine + Replace returns " + test);
            //DEBUG:
            //see if fully qualified new path has invalid characters
            Generator.Log(@FullyQualifiedNewPath.Replace('/', '\\'));

            */
            newbitmap.Save(System.IO.Path.Combine(ImageEditor.MapsetPath, Path).Replace('/', '\\')
                            , codec, null);


            //graphics.Dispose(); apparently it's already disposed??
            return;

        }


    }
    internal class BlurCommand
    {
        private int strength;
        internal BlurCommand(int strength)
        {
            this.strength = strength;
        }
        internal int GetStrength() => strength;
    }
    internal class BlackAndWhiteCommand
    {
        private float Red;
        private float Blue;
        private float Green;
        internal BlackAndWhiteCommand(float red, float blue, float green)
        {
            Red=red;
            Blue=blue;
            Green=green;
        }
        internal string GetColorString()
        {
            return (Red.ToString() + "~" + Green.ToString() + "~" + Blue.ToString());
        }
        internal float GetRed() => Red;
        internal float GetGreen() => Green;
        internal float GetBlue() => Blue;
    }
    /// <summary>
    /// Provides support for Gaussian Blur, written by mdymel and Gisburne2000 on github.<br/>
    /// link here: https://github.com/mdymel/superfastblur <br/>
    /// DISCLAIMER: this is a direct copy-paste.
    /// </summary>
    internal class GaussianBlur
    {
        private readonly int[] _alpha;
        private readonly int[] _red;
        private readonly int[] _green;
        private readonly int[] _blue;

        private readonly int _width;
        private readonly int _height;

        private readonly ParallelOptions _pOptions = new ParallelOptions { MaxDegreeOfParallelism = 16 };

        internal GaussianBlur(Bitmap image)
        {
            var rct = new Rectangle(0, 0, image.Width, image.Height);
            var source = new int[rct.Width * rct.Height];
            var bits = image.LockBits(rct, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(bits.Scan0, source, 0, source.Length);
            image.UnlockBits(bits);

            _width = image.Width;
            _height = image.Height;

            _alpha = new int[_width * _height];
            _red = new int[_width * _height];
            _green = new int[_width * _height];
            _blue = new int[_width * _height];

            Parallel.For(0, source.Length, _pOptions, i =>
            {
                _alpha[i] = (int)((source[i] & 0xff000000) >> 24);
                _red[i] = (source[i] & 0xff0000) >> 16;
                _green[i] = (source[i] & 0x00ff00) >> 8;
                _blue[i] = (source[i] & 0x0000ff);
            });
        }

        internal Bitmap Process(int radial)
        {
            var newAlpha = new int[_width * _height];
            var newRed = new int[_width * _height];
            var newGreen = new int[_width * _height];
            var newBlue = new int[_width * _height];
            var dest = new int[_width * _height];

            Parallel.Invoke(
                () => gaussBlur_4(_alpha, newAlpha, radial),
                () => gaussBlur_4(_red, newRed, radial),
                () => gaussBlur_4(_green, newGreen, radial),
                () => gaussBlur_4(_blue, newBlue, radial));

            Parallel.For(0, dest.Length, _pOptions, i =>
            {
                if (newAlpha[i] > 255) newAlpha[i] = 255;
                if (newRed[i] > 255) newRed[i] = 255;
                if (newGreen[i] > 255) newGreen[i] = 255;
                if (newBlue[i] > 255) newBlue[i] = 255;

                if (newAlpha[i] < 0) newAlpha[i] = 0;
                if (newRed[i] < 0) newRed[i] = 0;
                if (newGreen[i] < 0) newGreen[i] = 0;
                if (newBlue[i] < 0) newBlue[i] = 0;

                dest[i] = (int)((uint)(newAlpha[i] << 24) | (uint)(newRed[i] << 16) | (uint)(newGreen[i] << 8) | (uint)newBlue[i]);
            });

            var image = new Bitmap(_width, _height);
            var rct = new Rectangle(0, 0, image.Width, image.Height);
            var bits2 = image.LockBits(rct, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
            image.UnlockBits(bits2);
            return image;
        }

        private void gaussBlur_4(int[] source, int[] dest, int r)
        {
            var bxs = boxesForGauss(r, 3);
            boxBlur_4(source, dest, _width, _height, (bxs[0] - 1) / 2);
            boxBlur_4(dest, source, _width, _height, (bxs[1] - 1) / 2);
            boxBlur_4(source, dest, _width, _height, (bxs[2] - 1) / 2);
        }

        private int[] boxesForGauss(int sigma, int n)
        {
            var wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
            var wl = (int)Math.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (double)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = Math.Round(mIdeal);

            var sizes = new List<int>();
            for (var i = 0; i < n; i++) sizes.Add(i < m ? wl : wu);
            return sizes.ToArray();
        }

        private void boxBlur_4(int[] source, int[] dest, int w, int h, int r)
        {
            for (var i = 0; i < source.Length; i++) dest[i] = source[i];
            boxBlurH_4(dest, source, w, h, r);
            boxBlurT_4(source, dest, w, h, r);
        }

        private void boxBlurH_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, h, _pOptions, i =>
            {
                var ti = i * w;
                var li = ti;
                var ri = ti + r;
                var fv = source[ti];
                var lv = source[ti + w - 1];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri++] - fv;
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = r + 1; j < w - r; j++)
                {
                    val += source[ri++] - dest[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = w - r; j < w; j++)
                {
                    val += lv - source[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
            });
        }

        private void boxBlurT_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, w, _pOptions, i =>
            {
                var ti = i;
                var li = ti;
                var ri = ti + r * w;
                var fv = source[ti];
                var lv = source[ti + w * (h - 1)];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j * w];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri] - fv;
                    dest[ti] = (int)Math.Round(val * iar);
                    ri += w;
                    ti += w;
                }
                for (var j = r + 1; j < h - r; j++)
                {
                    val += source[ri] - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ri += w;
                    ti += w;
                }
                for (var j = h - r; j < h; j++)
                {
                    val += lv - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ti += w;
                }
            });
        }
    }

    public class NoiseGeneration
    {
        /// <summary>
        /// Type of noise for White noise generation.
        /// </summary>
        public enum NoiseType
        {
            Grayscale, FullColor
        }
        private int seed = 0, count;
        private NoiseType noiseType;
        /// <summary>
        /// Path of noise animation *not individual frames*.
        /// </summary>
        public string Path => ImageEditor.MapsetPath + "/sb/noise/" + noiseType.ToString() + seed.ToString() + "_" + ".jpg";
        /// <summary>
        /// Number of frames.
        /// </summary>
        public int Count => count;

        public void GenerateNoise()
        {

            for (int i = 0; i < count; i++)
            {

                GenerateNoiseFrame(i);
            }
        }
        /// <summary>
        /// Generates random noise based on the parameters you feed in.<br/>
        /// <br/>
        /// All data is generated through Random.NextBytes(char[] buffer) which
        /// gets fed into a Bitmap image. Once the Bitmap has been refilled, if you want
        /// your noise to be grayscale, it then feeds the noise through the same algorithm that
        /// is used in EditedOsbSprite.Grayscale() except with predetermined RGB values.
        /// </summary>
        /// <param name="seed">Random seed for image generation.</param>
        /// <param name="count">Number of images to generate</param>
        /// <param name="noiseType">Either color or grayscale.</param>
        public NoiseGeneration(int seed, int count, NoiseType noiseType)
        {
            this.seed=seed;
            this.count=count;
            this.noiseType=noiseType;

            //just so noise isnt created with a separate command
            GenerateNoise();
        }

        private void GenerateNoiseFrame(int index)
        {
            //generate path for individual frame
            string path = ImageEditor.MapsetPath + "/sb/noise/" + noiseType.ToString() + seed.ToString() + "_" + index.ToString() + ".jpg";
            //ensure proper path name
            path = path.Replace('/', '\\');

            //just so it doesn't get generated twice
            if (File.Exists(path)) { return; }

            //set randoms
            Random rand = new Random(seed + index);

            //generate blank bitmap and bitdata
            Bitmap blank_map = new Bitmap(854, 480);
            BitmapData bitdata = blank_map.LockBits(new Rectangle(0, 0, 854, 480),
                ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //get size of bitmap
            int bytes = bitdata.Stride * bitdata.Height;
            //make byte buffer of same size
            byte[] result = new byte[bytes];
            //fill buffer with bytes
            rand.NextBytes(result);
            //copy the filled buffer to the bitdata location
            Marshal.Copy(result, 0, bitdata.Scan0, bytes);
            //unlocks bitmap with new data
            blank_map.UnlockBits(bitdata);

            //if full color, then everythings done here
            if (noiseType  != NoiseType.Grayscale)
            {
                blank_map.Save(path, ImageFormat.Jpeg);
                return;
            }

            //if not, use same grayscale command as earlier except change r, g, and b
            //such that its a bit darker
            Bitmap bitmap = blank_map;
            //get graphics object
            Graphics graphics = Graphics.FromImage(blank_map);

            float r = 0.25f, g = 0.25f, b = 0.25f;
            float a = 1, w = 1;

            //create and instantiate color matrix
            float[][] colors = new float[][]
            {
                    new float[] {r,r,r,0,0},
                    new float[] {g,g,g,0,0},
                    new float[] {b,b,b,0,0},
                    new float[] {0,0,0,a,0},
                    new float[] {0,0,0,0,w}
            };

            //initialize matrix
            ColorMatrix matrix = new ColorMatrix(colors);

            //initialize attributes
            ImageAttributes attributes = new ImageAttributes();

            //set color matrix
            attributes.SetColorMatrix(matrix);

            //draw original image on !new image! except with grayscale color matrix
            graphics.DrawImage(bitmap,
                     new Rectangle(0, 0, 854, 480), //size
            0, 0, 854, 480, //size
                     GraphicsUnit.Pixel, attributes); //attributes of graphics

            //save new bitmap
            bitmap.Save(path, ImageFormat.Jpeg);
            return;

        }

    }
    /// <summary>
    /// Static class that every class inside of the namespace StorybrewImageEditor uses.<br/><br/>
    /// This is just so new variables don't need to be set every time.
    /// </summary>
    public static class ImageEditor
    {
        /// <summary>
        /// Mapset Path from which all reference images / generated images should stem from.
        /// </summary>
        public static String MapsetPath = StoryboardObjectGenerator.Current.MapsetPath;

    }

}