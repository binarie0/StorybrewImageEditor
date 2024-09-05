using OpenTK.Graphics.OpenGL;
using StorybrewCommon.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBImageLib.Statics
{
    public static class SpriteGrabber
    {
        public static string[] GetAllSpritePathsFromFolder(this StoryboardObjectGenerator Generator, string folder)
        {
            //get paths
            string mapsetpath = Generator.MapsetPath;
            string fullpath = Path.Combine(mapsetpath, folder).Replace('/', '\\');

            //check existence
            if (Directory.Exists(fullpath))
            {
                //create arrays

                //all file names and paths from folder
                string[] copy = Directory.GetFiles(fullpath);

                //actual return array
                string[] ret = new string[copy.Length];

                //character array
                char[] chararray;

                //length of mapsetpath to format properly
                int len = mapsetpath.Length;
                for (int index = 0; index < copy.Length; index++)
                {
                    //to char array
                    chararray = copy[index].ToCharArray();
                    //reformat output
                    ret[index] = new string(chararray, len + 1, chararray.Length - len - 1).Replace('\\', '/');
                    
                }
                return ret;

            }
            return null;

        }
    }
}
