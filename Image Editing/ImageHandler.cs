
using StorybrewCommon.Mapset;
using StorybrewCommon.Storyboarding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBImageLib
{
    public static class ImageHandler
    {
        internal static string ExportFolder = "sb/Generated-Sprites";
        public static Rectangle FullImage = new Rectangle(0, 0, 0, 0);
        #region Safe
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin = OsbOrigin.Centre)
        {
            return (Layer.CreateSprite(Path, Origin));
        }
        #endregion
        #region Only Crop
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds
                            )
        {
            return (Layer.GenerateSprite(Path, Origin, cropBounds, 0, 0, 0, 0));
        }
        #endregion
        #region One Command Present
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds,
                            float r = 0f, float g = 0f, float b = 0f)
        {
            return (Layer.GenerateSprite(Path, Origin, cropBounds, 0, r, g, b, false));
        }
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds,
                            bool inversed = false)
        {
            return (Layer.GenerateSprite(Path, Origin, cropBounds, 0, 0, 0, 0, inversed));
        }
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds,
                            int BlurStrength = 0)
        {
            return (Layer.GenerateSprite(Path, Origin, cropBounds, BlurStrength, 0, 0, 0, false));
        }

        #endregion
        #region Two Commands Present
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds,
                            int BlurStrength = 0, bool inversed = false)
        {
            return (Layer.GenerateSprite(Path, Origin, cropBounds, BlurStrength, 0, 0, 0, inversed));
        }
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds,
                            int BlurStrength = 0, float r = 0f, float g = 0f, float b = 0f)
        {
            return (Layer.GenerateSprite(Path, Origin, cropBounds, BlurStrength, r, g, b, false));
        }
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds,
                            float r = 0f, float g = 0f, float b = 0f, bool inversed = false)
        {
            return (Layer.GenerateSprite(Path, Origin, cropBounds, 0, r, g, b, inversed));
        }
        #endregion

        #region Main Command
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, Rectangle cropBounds, 
                            int BlurStrength = 0, float r = 0f, float g = 0f, float b = 0f, bool inversed = false, string ExportFolderOverride = null)
        {
            //nullchecks
            if (Layer == null) return null;
            if (BlurStrength <= 0 && r < 0 && g < 0 && b < 0) return null;

            string fullpath = ImageEditor.GetFullExportPath(Path);

            //get export info
            string filename = ImageEditor.GetFileName(Path);
            string ext = ImageEditor.GetExtension(Path);
            ImageCodecInfo codec = ImageEditor.GetCodec(ext);

            string newID = ImageEditor.GenerateID(BlurStrength, r, g, b, inversed, cropBounds);

            string exportfolder = (ExportFolderOverride != null) ? ExportFolderOverride: ExportFolder;
            //double check folder's been created
            ImageEditor.CreateFolder(exportfolder);

            string spritePath = exportfolder + (exportfolder.EndsWith("/") ? "": "/") + filename + "_" + newID + "." + ext;

            string ExportPath = ImageEditor.GetFullExportPath(spritePath);
            Bitmap transfer = null, bmap = null;
            if (!File.Exists(ExportPath))   //skip over bitmap creations
            {
                //set original bitmap
                bmap = new Bitmap(fullpath);
                transfer = new Bitmap(1, 1);
               
                //check if crop so that new effects don't need to be on big big bitmap
                if (!ImageEditor.CheckBounds(cropBounds, FullImage))
                {
                    transfer = new Bitmap(cropBounds.Width, cropBounds.Height);
                    Graphics gr = Graphics.FromImage(transfer);
                    gr.DrawImage(bmap, new Rectangle(0, 0, cropBounds.Width, cropBounds.Height),
                                    cropBounds, GraphicsUnit.Pixel);

                    bmap = transfer;

                }
                if (BlurStrength >= 0)
                {
                    BlurEffect blur = new BlurEffect(bmap, BlurStrength);


                    transfer = blur.Process();

                    bmap = transfer;
                }
                if (r > 0 || g > 0 || b > 0)
                {
                    GrayscaleEffect gscale = new GrayscaleEffect(bmap, r, g, b);


                    transfer = gscale.Process();
                    bmap = transfer;

                }

                //iterate through effects
                if (inversed)
                {
                    InverseEffect inv = new InverseEffect(bmap);


                    transfer  = inv.Process();
                    bmap = transfer;
                }
                
                
                
                

                ImageEditor.ExportImage(bmap, ExportPath, codec);
                
                bmap.Dispose();
                transfer.Dispose();
                


            }

            return (Layer.CreateSprite(spritePath, Origin));
        }
        
        
        #endregion

        
    }
}
