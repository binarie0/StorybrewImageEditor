using Microsoft.SqlServer.Server;
using StorybrewCommon.Storyboarding;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SBImageLib
{
    public enum NoiseType
    {
        FullColor, Monochrome
    }
    public static class NoiseGeneration
    {
        public static OsbAnimation GenerateNoise(this StoryboardLayer layer, int seed, int count, float delay, NoiseType type = NoiseType.Monochrome)
        {
            NoiseGenerator noise = new NoiseGenerator(layer, seed, count, delay, type);

            return noise.Generate();
        }
    }
    internal class NoiseGenerator
    {
        private static string ExportFolder = "sb/noise";
        private string AnimPath;
        private StoryboardLayer layer;
        private int seed, count;
        private float delay;
        private string seedstr, noisetypestr;
        private NoiseType noiseType;
        internal NoiseGenerator(StoryboardLayer layer, int seed, int count, float delay, NoiseType type)
        {
            this.layer = layer;
            this.seed = seed;
            this.count = count;
            this.noiseType = type;
            this.delay = delay;
            seedstr = seed.ToString();
            noisetypestr = type == NoiseType.FullColor ? "fc" : "m";
            AnimPath = ExportFolder + "/" + seedstr + noisetypestr + ".jpg";
        }
        internal OsbAnimation Generate()
        {
            ImageEditor.CreateFolder(ExportFolder);
            string s;
            string exportpath;
            for (int i = 0; i < count; i++)
            {
                s = GetPath(i);
                exportpath = ImageEditor.GetFullExportPath(s);
                if (!File.Exists(exportpath))
                {
                    Random rand = new Random(seed + i);

                    //generate blank bitmap and bitdata
                    Bitmap blank_map = new Bitmap(854, 480),
                        transfer;
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

                    //if not full color, re-edit
                    if (noiseType == NoiseType.Monochrome)
                    {
                        GrayscaleEffect gscale = new GrayscaleEffect(blank_map, 0.5f, 0.5f, 0.5f);
                        

                        transfer = gscale.Process();

                        blank_map = transfer;
                        
                    }

                    ImageEditor.ExportImage(blank_map, exportpath, ImageEditor.JpegEncoder);

                }
            }

            return (layer.CreateAnimation(AnimPath, count, delay, OsbLoopType.LoopForever, OsbOrigin.Centre));
        }
        internal string GetPath(int index)
        => ExportFolder + "/" + seedstr + noisetypestr + index.ToString() + ".jpg";

        

    }
}
