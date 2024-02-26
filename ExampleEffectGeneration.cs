using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
//This is the necessary *using* statement to access these classes, methods, and functions.
using StorybrewImageEditor;
namespace StorybrewScripts
{
    public class ExampleEffectGeneration : StoryboardObjectGenerator
    {
        internal StoryboardLayer layer;
        public override void Generate()
        {

            //this effect shows how to use StorybrewImageEditor

		    layer = GetLayer("ExampleEffectGeneration");

            //set sprite path
            string SpritePath = Beatmap.BackgroundPath;
            #region EditedOsbSprite Showcase
            //need to set mapset path
            ImageEditor.MapsetPath = MapsetPath;

            EditedOsbSprite sprite = EditedOsbSprite.NewSprite(SpritePath);
            
            //to blur a sprite, just call this method
            sprite.Blur(10); //10 = strength in pixels (this is with gaussian blur)

            //to make a sprite black and white, just call this method
            sprite.Grayscale(0.2f, 0.2f, 0.2f); //strength of red, green, blue

            //and yes, these can be run concurrently!
            
            //to finalize the sprite generation, just call this method
            sprite.Export(); // or sprite.Export(EditedOsbSprite.JpegEncoder); | sprite.Export(EditedOsbSprite.PngEncoder);

            //to reference the sprite, just reference its variables in a CreateSprite function
            OsbSprite spr = layer.CreateSprite(sprite.Path, OsbOrigin.Centre);

            //if you need to scale it down, sprite also stores the objects native width and height
            spr.Scale(0, 1000, 1, 854.0d/sprite.Width); // or spr.Scale(0, 1000, 1, 480.0d/sprite.Height);
            
            //all sprites generated have their own *unique-ish???* hash codes so that they don't get generated over and over with each update (unless values change)
            #endregion
            #region NoiseGeneration Showcase
            //to generate noise, all you need to do is just call the constructor of the class
            NoiseGeneration noise = new NoiseGeneration(this.RandomSeed, 4, NoiseGeneration.NoiseType.Grayscale); //seed, count, noise type
            //you can also generate noise in full color
            NoiseGeneration noise2 = new NoiseGeneration(this.RandomSeed, 4, NoiseGeneration.NoiseType.FullColor);

            //all noise created will be stored inside of "sb/noise" and be separated by noise type and seed such that generations are fully unique and not overlapping
            
            //to reference the generated noise, just reference its variables in a CreateAnimation function
            OsbAnimation anim = layer.CreateAnimation(noise.Path, noise.Count, 100, OsbLoopType.LoopForever);
            #endregion
        }
    }
}
