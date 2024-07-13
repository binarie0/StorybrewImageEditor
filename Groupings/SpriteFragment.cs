using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.CommandValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBImageLib;
namespace SBImageLib
{
    
    internal class SpriteGroupFragment
    {
        OsbSprite sprite;
        float movementMultiplier;
        double scaleX, scaleY;
        float opacity;
        double rotation;
        
        internal SpriteGroupFragment(OsbSprite sprite, float movementMultiplier = 1f, double scaleX = 1, double scaleY = 1, 
                                float opacity = 1, double rotation = 0)
        {
            _initialize(sprite, movementMultiplier, scaleX, scaleY, opacity, rotation);
        }
        internal void _initialize(OsbSprite sprite, float movementMultiplier, double scaleX, double scaleY, float opacity, double rotation)
        {
            this.sprite = sprite;
            this.movementMultiplier = movementMultiplier;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.opacity = opacity;
            this.rotation = rotation;
        }
        internal void Move(OsbEasing Easing, double StartTime, double EndTime, CommandPosition dPosition)
        {
            MoveX(Easing, StartTime, EndTime, dPosition.X);
            MoveY(Easing, StartTime, EndTime, dPosition.Y);
        }
        internal void Move(OsbEasing Easing, double StartTime, double EndTime, double dx, double dy)
        {
            MoveX(Easing, StartTime, EndTime, dx);
            MoveY(Easing, StartTime, EndTime, dy);
        }
        
        internal void MoveX(OsbEasing Easing, double StartTime, double EndTime, double dx)
        {
            float x = sprite.PositionAt(StartTime).X;
            sprite.MoveX(Easing, StartTime, EndTime, x, x + dx*movementMultiplier);
        }
        internal void MoveY(OsbEasing Easing, double StartTime, double EndTime, double dy)
        {
            float y = sprite.PositionAt(StartTime).Y;
            sprite.MoveY(Easing, StartTime, EndTime, y, y + dy*movementMultiplier);
        }
        internal void SetScales(double StartTime, ScaleFunction[] scaleFunctions)
        {
            sprite.ScaleVec(StartTime, this.scaleX, this.scaleY);
            double sx, sy;
            foreach(ScaleFunction scale in scaleFunctions)
            {
                sx = sprite.ScaleAt(scale.StartTime).X; sy = sprite.ScaleAt(scale.StartTime).Y;
                sprite.ScaleVec(scale.easing, scale.StartTime, scale.EndTime, sx, sy, sx + scale.dScaleX*movementMultiplier, scale.dScaleY*movementMultiplier);
            }
        }
        internal void SetVisibleBounds(double StartTime, double EndTime)
        {
            sprite.Fade(StartTime, StartTime, 0, opacity);
            sprite.Fade(EndTime, EndTime, opacity, 0);
        }
        internal void Rotate(OsbEasing Easing, double StartTime, double EndTime, double dr)
        {
            double r = sprite.RotationAt(StartTime);
            sprite.Rotate(Easing, StartTime, EndTime, r, r + dr*movementMultiplier);
        }
        
        
        
        

    }
}
