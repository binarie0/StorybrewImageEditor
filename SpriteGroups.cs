using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.CommandValues;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StorybrewImageLib
{
    public static class OsbSpriteGroup_Extensions
    {
        internal static OsbOrigin DEFAULT_ORIGIN = OsbOrigin.Centre;
        internal static float DEFAULT_X = 320, DEFAULT_Y = 240;
        internal static double DEFAULT_MOVESCALE = 1, DEFAULT_INITSCALE = 1;

        public static SpriteFragment CreateSpriteFragment(this StoryboardLayer layer, string path,
                                            OsbOrigin origin = OsbOrigin.Centre,
                                            double movementScale = 1,
                                            double initialScale = 1,
                                            float x = 320f, float y = 240f)
        {
            OsbSprite spr = layer.CreateSprite(path, origin, new Vector2(x, y));
            return (new SpriteFragment(spr, movementScale, initialScale));
        }
        public static SpriteFragment CreateSpriteFragment(this StoryboardLayer layer, string path,
                                            double movementScale = 1,
                                            float x = 320f, float y = 240f)
        {
            return layer.CreateSpriteFragment(path, DEFAULT_ORIGIN, movementScale, DEFAULT_INITSCALE, x, y);
        }
        public static SpriteFragment CreateSpriteFragment(this StoryboardLayer layer, string path,
                                            double movementScale = 1, double initialScale = 1
                                            )
        {
            return layer.CreateSpriteFragment(path, DEFAULT_ORIGIN, movementScale, DEFAULT_INITSCALE, DEFAULT_X, DEFAULT_Y);
        }
        public static SpriteFragment CreateSpriteFragment(this StoryboardLayer layer, string path,
                                            float x = 320f, float y = 240f)
        {
            return layer.CreateSpriteFragment(path, DEFAULT_ORIGIN, DEFAULT_MOVESCALE, DEFAULT_INITSCALE, x, y);
        }
        public static SpriteFragment CreateSpriteFragment(this StoryboardLayer layer, string path
                                            )
        {
            return layer.CreateSpriteFragment(path, DEFAULT_ORIGIN, DEFAULT_MOVESCALE, DEFAULT_INITSCALE,
                                            DEFAULT_X, DEFAULT_Y);
        }
        public static OsbSpriteGroup CreateSpriteGroup(this StoryboardLayer layer, params SpriteFragment[] fragments)
        {
            OnePX.GenerateOnePX();

            OsbSpriteGroup group = new OsbSpriteGroup(fragments);
            group.cover = layer.CreateSprite("sb/1px.png", OsbOrigin.Centre);

            return group;
        }
    }
    public class OsbSpriteGroup
    {
        internal SpriteFragment[] sprites;
        internal OsbSprite cover;
        bool uninitialized = true;
        
        
        internal OsbSpriteGroup(SpriteFragment[] sprites)
        {
            this.sprites = sprites;
            GroupSprites();
        }
        internal void GroupSprites()
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].iswithingroup = true;
            }
        }
        public SpriteFragment GetFragment(int index) => sprites[index];
        public void LockFragment(SpriteFragment fragment)
        {
            fragment.Lock();
        }
        public void UnlockFragment(SpriteFragment fragment)
        {
            fragment.Unlock();
        }
        public void LockFragment(int index)
        {
            if (index < sprites.Length)
                sprites[index].Lock();
        }
        public void UnlockFragment(int index)
        {
            if (index < sprites.Length)
                sprites[index].Unlock();
        }
        internal void InitializeRelativities(double startTime)
        {
            double iscale;
            for (int i = 0; i < sprites.Length; i++)
            {
                iscale = sprites[i].initial_scale;
                sprites[i].Scale(OsbEasing.None, startTime, startTime, iscale, iscale, true);
            }
            //initialize cover as well
            cover.ScaleVec(startTime, 854, 480);
            cover.Color(startTime, Color4.Black);
            cover.Fade(startTime, startTime, 0, 0);
            uninitialized = false;
        }
        /// <summary>
        /// Moves the group relatively by a set amount of pixels (relative to a sprite of move scale 1)
        /// </summary>
        /// <param name="easing">easing of movement</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        /// <param name="dx">change in x</param>
        /// <param name="dy">change in y</param>
        /// <param name="IgnoreScaling">ignore movement scaling and move everything the same distance</param>
        public void Move(OsbEasing easing, double startTime, double endTime, 
                                        double dx, double dy,
                                        bool IgnoreScaling = false)
        {
            if (uninitialized) InitializeRelativities(startTime);
            int index;
            double x, y;
            if (dx != 0)
            {
                for (index = 0; index < sprites.Length; index++)
                {
                    x = sprites[index].XPositionAt(startTime);
                    sprites[index].MoveX(easing, startTime, endTime, x, x + dx, IgnoreScaling);
                }
            }
            if (dy != 0)
            {
                for (index = 0; index < sprites.Length; index++)
                {
                    y = sprites[index].YPositionAt(startTime);
                    sprites[index].MoveY(easing, startTime, endTime, y, y + dy, IgnoreScaling);
                }
            }
        }
        public void MoveX(OsbEasing easing, double startTime, double endTime,
                                double dx, bool IgnoreScaling = false)
        {
            if (uninitialized) InitializeRelativities(startTime);
            int index;
            double x;
            for (index = 0; index < sprites.Length; index++)
            {
                x = sprites[index].XPositionAt(startTime);
                sprites[index].MoveX(easing, startTime, endTime, x, x + dx, IgnoreScaling);
            }
        }
        public void MoveY(OsbEasing easing, double startTime, double endTime,
                                double dy, bool IgnoreScaling = false)
        {
            if (uninitialized) InitializeRelativities(startTime);
            int index;
            double y;
            for (index = 0; index < sprites.Length; index++)
            {
                y = sprites[index].XPositionAt(startTime);
                sprites[index].MoveY(easing, startTime, endTime, y, y + dy, IgnoreScaling);
            }
        }
        /// <summary>
        /// Scales all sprites relatively by a set amount (relative to a sprite of move scale 1)
        /// </summary>
        /// <param name="easing">easing of movement</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        /// <param name="dscale">change in scale</param>
        /// <param name="IgnoreScaling">if sprites should all be scaled by the same amount</param>
        public void Scale(OsbEasing easing, double startTime, double endTime,
                                double dscale, bool IgnoreScaling = false)
        {
            ScaleVec(easing, startTime, endTime, dscale, dscale, IgnoreScaling);
        }
        /// <summary>
        /// Vector scales all sprites relatively by a set amount (relative to a sprite of move scale 1)
        /// </summary>
        /// <param name="easing">easing of movement</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        /// <param name="dscaleX">change in x scale</param>
        /// <param name="dscaleY">change in y scale</param>
        /// <param name="IgnoreScaling">if sprites should all be scaled by the same amount</param>
        public void ScaleVec(OsbEasing easing, double startTime, double endTime,
                                double dscaleX, double dscaleY, bool IgnoreScaling = false)
        {
            if (uninitialized) InitializeRelativities(startTime);
            int index;
            double cscaleX, cscaleY;
            for (index = 0; index < sprites.Length; index++)
            {
                cscaleX = sprites[index].XScaleAt(startTime);
                cscaleY = sprites[index].YScaleAt(startTime);
                sprites[index].ScaleVec(easing, startTime, endTime, cscaleX, cscaleY, cscaleX + dscaleX, cscaleY + dscaleY, IgnoreScaling);
            }
        }
        /// <summary>
        /// Fades the group via a cover sprite by a certain amount
        /// </summary>
        /// <param name="easing">easing of movement</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        /// <param name="dopacity">change in opacity</param>
        public void FadeAll(OsbEasing easing, double startTime, double endTime,
                                double dopacity)
        {
            if (uninitialized) InitializeRelativities(startTime);
            double s = cover.OpacityAt(startTime);
            double f = s + dopacity;
            if (f < 0) f = 0; if (f > 1) f = 1;
            cover.Fade(easing, startTime, endTime, s, f);

        }
        /// <summary>
        /// Fades an individual sprite by a certain amount
        /// </summary>
        /// <param name="index">index of sprite</param>
        /// <param name="easing">easing of movement</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        /// <param name="dopacity">change in opacity</param>
        public void FadeIndividual(int index, OsbEasing easing, double startTime, double endTime,
                                double dopacity)
        {
            if (uninitialized) InitializeRelativities(startTime);

            if (index < sprites.Length)
            { 
                double s = sprites[index].OpacityAt(startTime);
                double f = s + dopacity;
                if (f < 0) f = 0; if (f > 1) f = 1;
                sprites[index].Fade(easing, startTime, endTime, s, f);
            }
        }
        /// <summary>
        /// Rotates the group relatively by the change in angle (relative to a sprite of move scale 1)
        /// </summary>
        /// <param name="easing">easing of movement</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        /// <param name="drotation">change in rotation</param>
        /// <param name="IgnoreScaling">if sprites should all be rotated by the same amount</param>
        public void Rotate(OsbEasing easing, double startTime, double endTime,
            double drotation, bool IgnoreScaling = false)
        {
            if (uninitialized) InitializeRelativities(startTime);
            int index;
            double rotation;
            for (index = 0; index < sprites.Length; index++)
            {
                rotation = sprites[index].RotationAt(startTime);
                sprites[index].Rotate(easing, startTime, endTime, rotation, rotation + drotation, IgnoreScaling);
            }

        }
        /// <summary>
        /// Sets the color of one sprite in the group
        /// </summary>
        /// <param name="index">index of sprite in fragment array</param>
        /// <param name="easing">easing of movement</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        /// <param name="startColor">start color</param>
        /// <param name="endColor">end color</param>
        public void SetColor(int index, OsbEasing easing, double startTime, double endTime,
             Color4 startColor, Color4 endColor)
        {
            if (uninitialized) InitializeRelativities(startTime);

            if (index < sprites.Length)
            {
                sprites[index].Color(easing, startTime, endTime, startColor, endColor);
            }
        }
        public void SetAdditive(int index, double startTime, double endTime)
        {
            if (uninitialized) InitializeRelativities(startTime);

            if (index < sprites.Length)
            {
                sprites[index].Additive(startTime, endTime);
            }
        }
        
        

    }
    public class SpriteFragment
        {
        internal static string groupError = "Sprite fragment must be in group" +
                                            "in order to function properly. If this" +
                                            "is unintended, use the CreateSprite() function" +
                                            " instead";
            internal int index;
            internal double initial_scale;
            internal double phys_scale;
            internal double store_phys_scale = 0;
            internal OsbSprite sprite;
            internal bool iswithingroup = false;
            internal SpriteFragment(OsbSprite sprite)
            {
                this.sprite = sprite;
                phys_scale = 1;
                this.initial_scale = 1;
            }
            internal SpriteFragment(OsbSprite sprite, double phys_scale)
            {
                this.sprite = sprite;
                this.phys_scale = phys_scale;
                this.initial_scale = 1;
            }
            internal SpriteFragment(OsbSprite sprite, double phys_scale, double initial_scale)
            {
                this.sprite = sprite;
                this.initial_scale = initial_scale;
                this.phys_scale = phys_scale;
            }
            public void Lock()
            {
                store_phys_scale = phys_scale;
                phys_scale = 0;
            }
            public void Unlock()
            {
                phys_scale = store_phys_scale;
                store_phys_scale = 0;
            }
            public void Rotate(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                CommandDecimal startAngle,
                                CommandDecimal endAngle,
                                bool IgnoreScaling = false)
            {
            if (!iswithingroup) throw new ArgumentException(groupError);
            if (phys_scale == 0) return;

            double differenceRotation = endAngle - startAngle;
            sprite.Rotate(easing, startTime, endTime, startAngle, 
                                IgnoreScaling ? endAngle: startAngle + differenceRotation*phys_scale);
            }
            public double RotationAt(double time) => sprite.RotationAt(time);
            public void Fade(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                CommandDecimal startOpacity,
                                CommandDecimal endOpacity)
            {
            if (!iswithingroup) throw new ArgumentException(groupError);
                sprite.Fade(easing, startTime, endTime, startOpacity, endOpacity);
            }
            public double OpacityAt(double time) => sprite.OpacityAt(time);
            public void Move(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                double startX, double startY,
                                double endX, double endY,
                                bool IgnoreScaling = false)
            {
            if (!iswithingroup) throw new ArgumentException(groupError);

            if (phys_scale == 0) return;
                double differenceX = endX - startX;
                double differenceY = endY - startY;
                if (startX != endX)
                    sprite.MoveX(easing, startTime, endTime, startX,
                                    IgnoreScaling ? endX : startX + differenceX*phys_scale);
                if (startY != endY)
                    sprite.MoveY(easing, startTime, endTime, startY,
                                    IgnoreScaling ? endY : startY + differenceY*phys_scale);
            }
            public Vector2 PositionAt(double time) => sprite.PositionAt(time);
            public void MoveX(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                double startX, double endX,
                                bool IgnoreScaling = false)
            {
            if (!iswithingroup) throw new ArgumentException(groupError);

            if (phys_scale == 0) return;
                double differenceX = endX - startX;
                sprite.MoveX(easing, startTime, endTime, startX,
                                    IgnoreScaling ? endX : startX + differenceX*phys_scale);
            }
            public double XPositionAt(double time) => sprite.PositionAt(time).X;
            public void MoveY(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                double startY, double endY,
                                bool IgnoreScaling = false)
            {
            if (!iswithingroup) throw new ArgumentException(groupError);

            if (phys_scale == 0) return;
                double differenceY = endY - startY;
                sprite.MoveY(easing, startTime, endTime, startY,
                                    IgnoreScaling ? endY : startY + differenceY*phys_scale);
            }
            public double YPositionAt(double time) => sprite.PositionAt(time).Y;
            public void Scale(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                double startScale, double endScale,
                                bool IgnoreScaling = false)
            {
            if (!iswithingroup) throw new ArgumentException(groupError);

            if (phys_scale == 0) return;
                double difference = endScale - startScale;
                double e = IgnoreScaling ? endScale : startScale + difference*phys_scale;
                sprite.ScaleVec(easing, startTime, endTime, startScale, startScale,
                                    e, e);
            }
            public Vector2 ScaleAt(double time) => sprite.ScaleAt(time);
            public void ScaleVec(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                double startScaleX, double startScaleY,
                                double endScaleX, double endScaleY,
                                bool IgnoreScaling = false)
            {
            if (!iswithingroup) throw new ArgumentException(groupError);

            if (phys_scale == 0) return;
                double differenceX = endScaleX - startScaleX,
                    differenceY = endScaleY - startScaleY;
                double eX = IgnoreScaling ? endScaleX : startScaleX + differenceX*phys_scale,
                    eY = IgnoreScaling ? endScaleY : startScaleY + differenceY*phys_scale;
                sprite.ScaleVec(easing, startTime, endTime, startScaleX, startScaleY,
                                    eX, eY);
            }
            public double XScaleAt(double time) => sprite.ScaleAt(time).X;
            public double YScaleAt(double time) => sprite.ScaleAt(time).Y;

        public void Color(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                CommandColor startColor, CommandColor endColor)
        {
            if (!iswithingroup) throw new ArgumentException(groupError);

            sprite.Color(easing, startTime, endTime, startColor, endColor);
        }
        public void Color(OsbEasing easing,
                                CommandDecimal startTime,
                                CommandDecimal endTime,
                                float startr, float startg, float startb,
                                float endr, float endg, float endb)
        {
            if (!iswithingroup) throw new ArgumentException(groupError);

            Color4 startColor = new Color4(startr, startg, startb, 0xff),
                    endColor = new Color4(endr, endg, endb, 0xff);
            sprite.Color(easing, startTime, endTime, startColor, endColor);
        }
        public Color4 ColorAt(double time) => sprite.ColorAt(time);

        public void Additive(CommandDecimal startTime, CommandDecimal endTime)
        {
            if (!iswithingroup) throw new ArgumentException(groupError);
            sprite.Additive(startTime, endTime);
        }
        public bool AdditiveAt(double time) => sprite.AdditiveAt(time);


    }
}
