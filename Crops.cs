using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using StorybrewCommon.Storyboarding;
using OpenTK;
namespace StorybrewImageLib
{
    public static class CropSpriteExtensions
    {
        private static Vector2 CENTER = new Vector2(320, 240);
        public static OsbSprite CreateCropSprite(this StoryboardLayer layer, string path, OsbOrigin origin, Vector2 position,
                                    Vector2 TopLeft, Vector2 BottomRight)
        {
            Rectangle rect = new Rectangle((int)TopLeft.X, (int)TopLeft.Y,
                                            (int)(BottomRight.X - TopLeft.X),
                                            (int)(BottomRight.Y - TopLeft.Y));

            CropSprite crop = new CropSprite(path, rect);
            return (layer.CreateSprite(crop.Path, origin, position));
        }
        public static OsbSprite CreateCropSprite(this StoryboardLayer layer, string path, OsbOrigin origin,
                                    Vector2 TopLeft, Vector2 BottomRight)
        {
            return (layer.CreateCropSprite(path, origin, CENTER, TopLeft, BottomRight));
        }
        public static OsbSprite CreateCropSprite(this StoryboardLayer layer, string path,
                                    Vector2 TopLeft, Vector2 BottomRight)
        {
            return (layer.CreateCropSprite(path, OsbOrigin.Centre, CENTER, TopLeft, BottomRight));
        }
        public static OsbSprite GenerateSpriteCrop(this StoryboardLayer Layer,
                                        string Base_Path,
                                        OsbOrigin Origin,
                                        Vector2 position,
                                        Vector2 TopLeft, Vector2 BottomRight,
                                        BlurEffect Blur = null,
                                        GrayscaleEffect Grayscale = null,
                                        bool Inverse = false
                                        )
        {
            //creates both the edited sprite and the crop sprite

            //if one wants to reference the other sprite they can just generate another sprite and it'll
            //automatically pick that
            EditedOsbSprite spr = EditedOsbSprite.NewSprite(Base_Path, Blur, Grayscale, Inverse);
            spr.Export();

            Rectangle rect = new Rectangle((int)TopLeft.X, (int)TopLeft.Y,
                                            (int)(BottomRight.X - TopLeft.X),
                                            (int)(BottomRight.Y - TopLeft.Y));

            CropSprite crop = new CropSprite(spr.Path, rect);

            return (Layer.CreateSprite(crop.Path, Origin, position));
        }
        public static OsbSprite GenerateSpriteCrop(this StoryboardLayer Layer,
                                        string Base_Path,
                                        OsbOrigin Origin,
                                        
                                        Vector2 TopLeft, Vector2 BottomRight,
                                        BlurEffect Blur = null,
                                        GrayscaleEffect Grayscale = null,
                                        bool Inverse = false
                                        )
        {
            return (Layer.GenerateSpriteCrop(Base_Path, Origin, CENTER,
                TopLeft, BottomRight, Blur, Grayscale, Inverse));
        }
        public static OsbSprite GenerateSpriteCrop(this StoryboardLayer Layer,
                                        string Base_Path,
                                        

                                        Vector2 TopLeft, Vector2 BottomRight,
                                        BlurEffect Blur = null,
                                        GrayscaleEffect Grayscale = null,
                                        bool Inverse = false
                                        )
        {
            return (Layer.GenerateSpriteCrop(Base_Path, OsbOrigin.Centre, CENTER,
                TopLeft, BottomRight, Blur, Grayscale, Inverse));
        }
    }
    public class CropSprite
    {
        public string Path;
        private string BasePath;
        private string FullBasePath;
        private string ExportPath;
        private string FileExtension;
        private Rectangle bounds;
        private int width, height;
        internal CropSprite(string BasePath, Rectangle bounds)
        {
            this.BasePath = BasePath;
            this.bounds = bounds;
            width = bounds.Width;
            height = bounds.Height;

            SetFileExports();
            if (File.Exists(ExportPath)) return; //no need to generate twice

            Crop(); 
        }
        private void Crop()
        {
            //make image available to read
            ImageEditor.SetReadOnly(FullBasePath, false);
            Bitmap b = new Bitmap(FullBasePath);
            Bitmap nb = new Bitmap(width, height);
            nb.SetResolution(96, 96);

            //auto dispose
            using (Graphics g = Graphics.FromImage(nb))
            {
                //draw image again
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                //bounds are negative bc stackoverflow told me to
                g.DrawImage(b, -bounds.X, -bounds.Y);

                
            }


            //save
            nb.Save(ExportPath);

            //dispose extra bitmaps
            b.Dispose();
            nb.Dispose();

            //reset read avail
            ImageEditor.SetReadOnly(FullBasePath, true);
            ImageEditor.SetReadOnly(ExportPath, true);
        }
        
        private void SetFileExports()
        {
            string[] e = BasePath.Split('/');
            string[] o = e[e.Length - 1].Split('.');

            
            FileExtension = "." + o[1];
            Path = "sb/" + o[0] + "_" + width.ToString() + "-" + height.ToString() + FileExtension;
            FullBasePath = System.IO.Path.Combine(ImageEditor.MapsetPath, BasePath);
            ExportPath = System.IO.Path.Combine(ImageEditor.MapsetPath, Path)
                                .Replace('/', '\\');


        }
    }
}
