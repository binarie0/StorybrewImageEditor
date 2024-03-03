using Microsoft.SqlServer.Server;
using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewImageEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace StorybrewImageEditor
{
    public static class Extensions
    {
        private static Vector2 CENTER = new Vector2(320, 240);
        #region Blur
        public static OsbSprite CreateBlurSprite(this StoryboardLayer layer,
                                        string path, 
                                        OsbOrigin origin, 
                                        Vector2 position, 
                                        int strength)
        {
            EditedOsbSprite spr = EditedOsbSprite.NewSprite(path);
            spr.Blur(strength);
            spr.Export();

            return (layer.CreateSprite(spr.Path, origin, position));
        }
        public static OsbSprite CreateBlurSprite(this StoryboardLayer layer,
                                        string path,
                                        OsbOrigin origin,
                                        int strength)
        {
            return layer.CreateBlurSprite(path, origin, CENTER, strength);
        }
        public static OsbSprite CreateBlurSprite(this StoryboardLayer layer,
                                        string path,
                                        int strength)
        {
            return layer.CreateBlurSprite(path, OsbOrigin.Centre, CENTER, strength);
        }
        #endregion
        #region Grayscale
        public static OsbSprite CreateGraySprite(this StoryboardLayer layer,
                                        string path,
                                        OsbOrigin origin,
                                        Vector2 position,
                                        float r, float g, float b
                                        )
        {
            EditedOsbSprite spr = EditedOsbSprite.NewSprite(path);
            spr.Grayscale(r, g, b);
            spr.Export();

            return (layer.CreateSprite(spr.Path, origin, position));

        }
        public static OsbSprite CreateGraySprite(this StoryboardLayer layer,
                                        string path,
                                        OsbOrigin origin,
                                        float r, float g, float b
                                        )
        {
            return layer.CreateGraySprite(path, origin, CENTER, r, g, b);
        }
        public static OsbSprite CreateGraySprite(this StoryboardLayer layer,
                                        string path,
                                        float r, float g, float b
                                        )
        {
            return layer.CreateGraySprite(path, OsbOrigin.Centre, CENTER, r, g, b);
        }
        public static OsbSprite CreateGraySprite(this StoryboardLayer layer,
                                        string path,
                                        OsbOrigin origin,
                                        float colorshift
                                        )
        {
            return layer.CreateGraySprite(path, OsbOrigin.Centre, CENTER, colorshift, colorshift, colorshift);
        }
        #endregion
        #region Blur + Grayscale
        public static OsbSprite CreateBlurPlusGraySprite(this StoryboardLayer layer,
                                        string path,
                                        OsbOrigin origin,
                                        Vector2 position,
                                        int strength,
                                        float r, float g, float b)
        {
            EditedOsbSprite spr = EditedOsbSprite.NewSprite(path);
            spr.Blur(strength);
            spr.Grayscale(r, g, b);
            spr.Export();

            return (layer.CreateSprite(spr.Path, origin, position));
        }
        public static OsbSprite CreateBlurPlusGraySprite(this StoryboardLayer layer,
                                        string path,
                                        OsbOrigin origin,

                                        int strength,
                                        float r, float g, float b)
        {
            return (layer.CreateBlurPlusGraySprite(path, origin, CENTER, strength, r, g, b));
        }
        public static OsbSprite CreateBlurPlusGraySprite(this StoryboardLayer layer,
                                        string path,


                                        int strength,
                                        float r, float g, float b)
        {
            return (layer.CreateBlurPlusGraySprite(path, OsbOrigin.Centre, CENTER, strength, r, g, b));
        }
        public static OsbSprite CreateBlurPlusGraySprite(this StoryboardLayer layer,
                                        string path,
                                        OsbOrigin origin,

                                        int strength,
                                        float colorshift)
        {
            return (layer.CreateBlurPlusGraySprite(path, OsbOrigin.Centre, CENTER, strength, 
                                        colorshift, colorshift, colorshift));
        }

        #endregion


        #region Noise
        public static OsbAnimation CreateNoise(this StoryboardLayer layer,
                                        int count,
                                        NoiseGeneration.NoiseType noiseType,
                                        double interval)
        {
            NoiseGeneration generation = new NoiseGeneration(
                                        StoryboardObjectGenerator.Current.RandomSeed,
                                        count, 
                                        noiseType);

            return (layer.CreateAnimation(generation.Path, count, interval, OsbLoopType.LoopForever));
        }
        public static OsbAnimation CreateNoise(this StoryboardLayer layer,
                                        int count,
                                        double interval)
        {
            NoiseGeneration generation = new NoiseGeneration(
                                        StoryboardObjectGenerator.Current.RandomSeed,
                                        count,
                                        NoiseGeneration.NoiseType.Grayscale);

            return (layer.CreateAnimation(generation.Path, count, interval, OsbLoopType.LoopForever));
        }
        #endregion
    }
}
