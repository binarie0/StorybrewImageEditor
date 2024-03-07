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
using StorybrewImageLib;
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
            #region Sprite Fragmentation and Grouping

            //to create a sprite fragment array, use this
            SpriteFragment[] fragments = {layer.CreateSpriteFragment(Beatmap.BackgroundPath, OsbOrigin.Centre, 0.1, 0.5, 320, 240),
                            layer.CreateSpriteFragment(Beatmap.BackgroundPath, OsbOrigin.Centre, 0.5, 0.2, 200, 40)
            };
            //instantiation of a sprite fragment is as follows
            //layer.CreateSpriteFragment([path], [origin], [relative scale], [initial scale - does not affect relative scale], [x], [y])
            //there are shorter ways to instantiate where x and y are ignored

            //sprite fragments will not work unless inside a group, as at that point using a sprite would be enough

            //to initialize the sprite group, use this
            OsbSpriteGroup group = layer.CreateSpriteGroup(fragments);

            //Now, all commands will be relative
            group.Move(OsbEasing.InOutSine, 15000, 16000, 80, 100);

            //all relative commands can also be turned absolute by adding -true- to the end of the method
            //group.Move(OsbEasing.InOutSine, 15000, 16000, 80, 100, true);

            //fading works a bit differently as fading is overlayed, meaning that a normal fade would ruin the immersion
            //instead, if you want to fade the group, use this
            group.FadeAll(OsbEasing.InExpo, 19000, 20000, 0.5);

            //if you want to still fade individual sprites, use this
            group.FadeIndividual(0, OsbEasing.InBack, 21000, 24000, 0.2);
            //where 0 is the index in the fragment array

            //if you want a fragment to stay completely still for a time, use this
            group.LockFragment(0);
            //and to unlock use this
            group.UnlockFragment(0);

            //OsbSpriteGroup supports move, fade, rotate, scale, scalevec. Additive and Color can be applied on individual sprites.
            #endregion

        }
    }
}
