using StorybrewCommon;
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
        public static Rectangle FullImage = new Rectangle(0, 0, -1, -1);
        #region Safe
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin = OsbOrigin.Centre)
        {
            return (Layer.CreateSprite(Path, Origin));
        }
        #endregion
       
        

        #region Main Command
        /// <summary>
        /// [Extension] Generates a new sprite based on a base sprite.
        /// </summary>
        /// <param name="Layer">The layer the sprite is to be generated on.</param>
        /// <param name="Path">The base sprite's path.</param>
        /// <param name="Origin">The origin of the new sprite</param>
        /// <param name="Params">The parameters of the generation.</param>
        /// <param name="ExportFolderOverride">Change if you want the export folder to be changed (simplicity or other reasons).</param>
        /// <returns></returns>
        public static OsbSprite? GenerateSprite(this StoryboardLayer Layer, string Path, OsbOrigin Origin, ImageEditParams Params, string ExportFolderOverride = null)
        {
            //nullchecks
            if (Layer == null) return null;
            if (Params.IsUnutilized()) return null;

            //transfer variables so it's easier to read
            int BlurStrength = Params.BlurStrength;
            float r = Params.R;
            float g = Params.G;
            float b = Params.B;
            bool inversed = Params.Inversed;
            bool allocateBlur = Params.AllocateSpaceForBlur;
            Rectangle cropBounds = Params.CropBounds;


            string fullpath = ImageEditor.GetFullExportPath(Path);

            //get export info
            string filename = ImageEditor.GetFileName(Path);
            string? ext = (BlurStrength != 0 && allocateBlur)? "png" : ImageEditor.GetExtension(Path);
            ImageCodecInfo codec = ImageEditor.GetCodec(ext);

            string newID = ImageEditor.GenerateID(BlurStrength, r, g, b, inversed, cropBounds);

            string exportfolder = (ExportFolderOverride != null) ? ExportFolderOverride: ExportFolder;
            //double check folder's been created
            ImageEditor.CreateFolder(exportfolder);

            string spritePath = exportfolder + (exportfolder.EndsWith("/") ? "": "/") + filename + "_" + newID + "." + ext;

            string ExportPath = ImageEditor.GetFullExportPath(spritePath);
            Bitmap? transfer = null, bmap = null;
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
                    if (allocateBlur)
                    {
                        transfer = new Bitmap(bmap.Width + BlurStrength*2, bmap.Height + BlurStrength*2);
                        Graphics gr = Graphics.FromImage(transfer);
                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        gr.DrawImage(bmap, new Rectangle(BlurStrength, BlurStrength, bmap.Width, bmap.Height));

                        bmap = transfer;
                    }
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
    /// <summary>
    /// The input type for an image edit. Utilize the following for the desired effect:<br/>
    /// ------<br/>
    /// For Blurs:<br/>
    ///         BlurStrength (int) -> The strength of the gaussian blur (in px).<br/>
    ///         AllocateSpaceForBlur (bool) -> Whether the image will need space allocated for the blur.<br/>
    /// ------<br/>
    /// For Monochrome:<br/>
    ///         R, G, B (floats) -> The strengths of each individual color (from 0 to 1).<br/>
    /// ------<br/>
    /// For Inverse:<br/>
    ///         Inversed (bool) -> Whether the image should be inversed<br/>
    /// ------<br/>
    /// For Crop:<br/>
    ///         CropBounds (Rectangle) -> The bounds for the image crop.<br/>
    /// </summary>
    public class ImageEditParams
    {
        /// <summary>
        /// The strength of the gaussian blur effect (in pixels)
        /// </summary>
        public int BlurStrength = 0;
        /// <summary>
        /// If a blurred object is a PNG that needs to allocate more space, set this to true.
        /// This will not work if BlurStrength is equal to 0.
        /// </summary>
        public bool AllocateSpaceForBlur = false;
        /// <summary>
        /// R, G, and B represent the strength of the monochrome effect for each color value.
        /// </summary>
        public float R = 0f, G = 0f, B = 0f;

        /// <summary>
        /// Whether the image should be inversed
        /// </summary>
        public bool Inversed = false;

        /// <summary>
        /// If the image should be cropped, these are the bounds.
        /// </summary>
        public Rectangle CropBounds = ImageHandler.FullImage;

        public bool IsUnutilized()
        {
            return (BlurStrength == 0 && !AllocateSpaceForBlur && R == 0f && G == 0f && B == 0f
                && !Inversed && ImageEditor.CheckBounds(CropBounds, ImageHandler.FullImage)) ;
        }
    }
}
