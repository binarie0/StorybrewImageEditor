using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;

namespace StorybrewImageLib
{
    public static class BAndGExtensions
    {
        private static Vector2 CENTER = new Vector2(320, 240);
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer,
                                        string Base_Path,
                                        OsbOrigin Origin,
                                        Vector2 position,
                                        BlurEffect Blur = null,
                                        GrayscaleEffect Grayscale = null,
                                        bool Inverse = false)
        {
            EditedOsbSprite spr = EditedOsbSprite.NewSprite(Base_Path, Blur, Grayscale, Inverse);
            spr.Export();


            return (Layer.CreateSprite(spr.Path, Origin, position));

        }
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer,
                                        string Base_Path,
                                        OsbOrigin Origin,
                                        
                                        BlurEffect Blur = null,
                                        GrayscaleEffect Grayscale = null,
                                        bool Inverse = false)
        {
            EditedOsbSprite spr = EditedOsbSprite.NewSprite(Base_Path, Blur, Grayscale, Inverse);
            spr.Export();


            return (Layer.CreateSprite(spr.Path, Origin, CENTER));

        }
        public static OsbSprite GenerateSprite(this StoryboardLayer Layer,
                                        string Base_Path,
                                        
                                        BlurEffect Blur = null,
                                        GrayscaleEffect Grayscale = null,
                                        bool Inverse = false)
        {
            EditedOsbSprite spr = EditedOsbSprite.NewSprite(Base_Path, Blur, Grayscale, Inverse);
            spr.Export();


            return (Layer.CreateSprite(spr.Path, OsbOrigin.Centre, CENTER));

        }
        

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
