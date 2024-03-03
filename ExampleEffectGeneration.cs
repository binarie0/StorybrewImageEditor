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
            
            
            string SpritePath = Beatmap.BackgroundPath;
            #region EditedOsbSprite Showcase
            
            //to generate a blurred sprite, just use the following function
            OsbSprite sprite = layer.CreateBlurSprite(Beatmap.BackgroundPath, OsbOrigin.Centre, 10);
            sprite.Fade(1000, 2000, 1, 0);
        
            //to generate a grayscaled sprite, use this
            OsbSprite sprite2 = layer.CreateGraySprite(Beatmap.BackgroundPath, OsbOrigin.Centre, 0.3f, 0.4f, 0.6f);
            sprite2.Fade(4000, 5000, 1, 0);

            //to generate a blurred and grayscaled sprite, use this
            OsbSprite sprite3 = layer.CreateBlurPlusGraySprite(Beatmap.BackgroundPath, OsbOrigin.Centre, 30, 0.2f);
            sprite3.Fade(5000, 6000, 1, 0);
            #endregion
            #region NoiseGeneration Showcase

            //to generate noise, all you need to do is use this
            OsbAnimation animation = layer.CreateNoise(6, Beatmap.GetTimingPointAt(0).BeatDuration * 0.25d);
            animation.Fade(8000, 9000, 1, 0);
            #endregion

            
            
        }
    }
}
