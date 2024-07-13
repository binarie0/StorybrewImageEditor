using OpenTK;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.CommandValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SBImageLib
{
    /// <summary>
    /// This class provides a safe shell for connecting multiple sprites at the same time.<br/>
    /// To create a sprite group, instantiate the object then add sprites already created using<br>
    /// the StoryboardLayer.CreateSprite() function.
    /// </summary>
    public class SpriteGroup
    {
        public bool Finalized { get; private set; } = false;
        /// <summary>
        /// The current sprite count.
        /// </summary>
        public int SpriteCount { get; private set; } = 0;
            
        public double _StartTime { get; private set; } = -1;

        internal List<SpriteGroupFragment> fragments = null;

        internal List<ScaleFunction> ScaleFunctions = null;
        
        

        /// <summary>
        /// Instantiates a sprite group. This contains no sprites at first.
        /// </summary>
        public SpriteGroup()
        {
            _instantiate();
        }
        /// <summary>
        /// Instantiates a sprite group with an original sprite.
        /// </summary>
        /// <param name="sprite"></param>
        public SpriteGroup(OsbSprite sprite, float movementMultiplier = 1, double scaleX = 1, double scaleY = 1, float opacity = 1)
        {
            _instantiate();

            _add(sprite, movementMultiplier, scaleX, scaleY, opacity);
        }
        private void _instantiate()
        {
            fragments = new List<SpriteGroupFragment>();
            ScaleFunctions = new List<ScaleFunction>();
        }
        #region Sprite Adding
        /// <summary>
        /// Adds a sprite to the group with the accompanying factors.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        /// <param name="movementMultiplier">The movement multiplier applied to Move, Scale, and Rotate commands.</param>
        /// <param name="scaleX">The horizontal scale.</param>
        /// <param name="scaleY">The vertical scale.</param>
        public void AddSprite(OsbSprite sprite, float movementMultiplier = 1, double scaleX = 1, double scaleY = 1, float opacity = 1)
        {
            _add(sprite, movementMultiplier, scaleX, scaleY, opacity);
        }
        /// <summary>
        /// Adds a sprite to the group with the accompanying factors.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        /// <param name="movementMultiplier">The movement multiplier applied to Move, Scale, and Rotate commands.</param>
        /// <param name="scale">The scale.</param>
        public void AddSprite(OsbSprite sprite, float movementMultiplier = 1, double scale = 1, float opacity = 1)
        {
            _add(sprite, movementMultiplier, scale, scale, opacity);
        }
        /// <summary>
        /// Adds a sprite to the group with the accompanying factors.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        /// <param name="initialPosition">The initial position of the sprite.</param>
        /// <param name="movementMultiplier">The movement multiplier applied to Move, Scale, and Rotate commands.</param>
        /// <param name="scaleX">The horizontal scale.</param>
        /// <param name="scaleY">The vertical scale.</param>
        public void AddSprite(OsbSprite sprite, Vector2 initialPosition, float movementMultiplier, double scaleX = 1, double scaleY = 1, float opacity = 1)
        {
            sprite.InitialPosition = initialPosition;
            _add(sprite, movementMultiplier, scaleX, scaleY, opacity);
        }

        /// <summary>
        /// Adds a sprite to the group with the accompanying factors.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        /// <param name="initialPosition">The initial position of the sprite.</param>
        /// <param name="movementMultiplier">The movement multiplier applied to Move, Scale, and Rotate commands.</param>
        /// <param name="scale">The scale.</param>
        public void AddSprite(OsbSprite sprite, Vector2 initialPosition, float movementMultiplier, double scale = 1, float opacity = 1)
        {
            sprite.InitialPosition = initialPosition;
            _add(sprite, movementMultiplier, scale, scale, opacity);
        }
        private void _add(OsbSprite sprite, float movementMultiplier, double scaleX, double scaleY, float opacity)
        {
            fragments.Add(new SpriteGroupFragment(sprite, movementMultiplier, scaleX, scaleY, opacity));

            SpriteCount += 1;
        }
        #endregion
        #region End Sprite Group
        /// <summary>
        /// This finalizes the group and ensures proper scaling for each sprite.
        /// </summary>
        public void FinalizeGroup()
        {
            _end();
        }
        private void _end()
        {
            if (_StartTime != -1 && SpriteCount != 0 && !Finalized)
            {
                ScaleFunction[] scaleFunctions = ScaleFunctions.ToArray();
                for (int i = 0; i < SpriteCount; i++)
                {
                    fragments[i].SetScales(_StartTime, scaleFunctions);
                }
            }
            Finalized = true;
        }
        private void _updateStartTime(double time)
        {
            if (time < _StartTime) _StartTime = time;
        }
        
        #endregion
        #region Move Commands
        #region Public Move Commands
        /// <summary>
        /// Relatively moves the sprites in the group a certain distance away from their original positions.
        /// </summary>
        /// <param name="Easing">The easing.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dPosition">The change in position.</param>
        public void Move(OsbEasing Easing, double StartTime, double EndTime, CommandPosition dPosition)
        {
            _move(Easing, StartTime, EndTime, dPosition);
        }
        /// <summary>
        /// Relatively moves the sprite group with no easing.
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dPosition">The change in position.</param>
        public void Move(double StartTime, double EndTime, CommandPosition dPosition)
        {
            _move(OsbEasing.None, StartTime, EndTime, dPosition);
        }
        /// <summary>
        /// Relatively moves the sprite group with easing.
        /// </summary>
        /// <param name="Easing">The easing type.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dx">The change in X.</param>
        /// <param name="dy">The change in Y.</param>
        public void Move(OsbEasing Easing, double StartTime, double EndTime, double dx, double dy)
        {
            _move(Easing, StartTime, EndTime, dx, dy);
        }

        /// <summary>
        /// Relatively moves the sprite group with no easing.
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dx">The change in X.</param>
        /// <param name="dy">The change in Y.</param>
        public void Move(double StartTime, double EndTime, double dx, double dy)
        {
            _move(OsbEasing.None, StartTime, EndTime, dx, dy);
        }

        /// <summary>
        /// Relatively moves the sprite group instantaneously.
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="dPosition">The change in position.</param>
        public void Move(double StartTime, CommandPosition dPosition)
        {
            _move(OsbEasing.None, StartTime, StartTime, dPosition);
        }

        /// <summary>
        /// Relatively moves the sprite group instantaneously.
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="dx">The change in X.</param>
        /// <param name="dy">The change in Y.</param>
        public void Move(double StartTime, double dx, double dy)
        {
            _move(OsbEasing.None, StartTime, StartTime, dx, dy);
        }

        #endregion
        #region Move X Commands
        /// <summary>
        /// Relatively moves the sprite group horizontally with easing.
        /// </summary>
        /// <param name="Easing">The easing type.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dx">The change in X.</param>
        public void MoveX(OsbEasing Easing, double StartTime, double EndTime, double dx)
        {
            _moveX(Easing, StartTime, EndTime, dx);
        }
        /// <summary>
        /// Relatively moves the sprite group horizontally without easing.
        /// </summary>
        
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dx">The change in X.</param>
        public void MoveX(double StartTime, double EndTime, double dx)
        {
            _moveX(OsbEasing.None, StartTime, EndTime, dx);
        }

        /// <summary>
        /// Relatively moves the sprite group horizontally instantaneously.
        /// </summary>
        
        /// <param name="StartTime">The start time.</param>
        
        /// <param name="dx">The change in X.</param>
        public void MoveX(double StartTime, double dx)
        {
            _moveX(OsbEasing.None, StartTime, StartTime, dx);
        }
        #endregion
        #region Move Y Commands
        /// <summary>
        /// Relatively moves the sprite group horizontally with easing.
        /// </summary>
        /// <param name="Easing">The easing type.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dy">The change in X.</param>
        public void MoveY(OsbEasing Easing, double StartTime, double EndTime, double dy)
        {
            _moveY(Easing, StartTime, EndTime, dy);
        }
        /// <summary>
        /// Relatively moves the sprite group horizontally without easing.
        /// </summary>

        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dy">The change in Y.</param>
        public void MoveY(double StartTime, double EndTime, double dy)
        {
            _moveY(OsbEasing.None, StartTime, EndTime, dy);
        }

        /// <summary>
        /// Relatively moves the sprite group horizontally instantaneously.
        /// </summary>

        /// <param name="StartTime">The start time.</param>

        /// <param name="dy">The change in Y.</param>
        public void MoveY(double StartTime, double dy)
        {
            _moveY(OsbEasing.None, StartTime, StartTime, dy);
        }
        #endregion
        #region Private Move Commands
        //MOVE FUNCTIONS WITH COMMANDPOSITIONS
        private void _move(OsbEasing Easing, double StartTime, double EndTime, CommandPosition dPosition)
        {
            _move(Easing, StartTime, EndTime, dPosition.X, dPosition.Y);
        }

        //MOVE FUNCTIONS WITH DOUBLES
        private void _move(OsbEasing Easing, double StartTime, double EndTime, double dx, double dy)
        {
            _moveX(Easing, StartTime, EndTime, dx);
            _moveY(Easing, StartTime, EndTime, dy);
        }
        private void _moveX(OsbEasing Easing, double StartTime, double EndTime, double dx)
        {
            if (Finalized) return;
            _updateStartTime(StartTime);
            for (int i = 0; i < SpriteCount; i++)
            {
                fragments[i].MoveX(Easing, StartTime, EndTime, dx);
            }
        }
        private void _moveY(OsbEasing Easing, double StartTime, double EndTime, double dy)
        {
            if (Finalized) return;
            _updateStartTime(StartTime);
            for (int i = 0; i < SpriteCount; i++)
            {
                fragments[i].MoveY(Easing, StartTime, EndTime, dy);
            }
        }
        #endregion

        #endregion
        #region Scale Commands
        
        #region Regular Scaling
        /// <summary>
        /// Relatively scales the sprite group with easing.<br/>
        /// NOTE: You will need to finalize the sprite group using the <br/>.FinalizeGroup()
        /// function in order for the scaling to be accurate. This is because in order to set
        /// an initial scale, all scale functions need to be cached until a known start time is set.
        /// 
        /// </summary>
        /// <param name="Easing">The easing.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dscale">The change in scale.</param>
        public void Scale(OsbEasing Easing, double StartTime, double EndTime, double dscale)
        {
            _scalevec(Easing, StartTime, EndTime, dscale, dscale);
        }
        /// <summary>
        /// Relatively scales the sprite group without easing.<br/>
        /// NOTE: You will need to finalize the sprite group using the <br/>.FinalizeGroup()
        /// function in order for the scaling to be accurate. This is because in order to set
        /// an initial scale, all scale functions need to be cached until a known start time is set.
        /// 
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dscale">The change in scale.</param>
        public void Scale(double StartTime, double EndTime, double dscale)
        {
            _scalevec(OsbEasing.None, StartTime, EndTime, dscale, dscale);
        }
        #endregion
        #region Vector Scaling
        /// <summary>
        /// Relatively vector scales the sprite group with easing.<br/>
        /// NOTE: You will need to finalize the sprite group using the <br/>.FinalizeGroup()
        /// function in order for the scaling to be accurate. This is because in order to set
        /// an initial scale, all scale functions need to be cached until a known start time is set.
        /// 
        /// </summary>
        /// <param name="Easing">The easing.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dscaleX">The change in x scale.</param>
        /// <param name="dscaleY">The change in y scale.</param>
        public void ScaleVec(OsbEasing Easing, double StartTime, double EndTime, double dscaleX, double dscaleY)
        {
            _scalevec(Easing, StartTime, EndTime, dscaleX, dscaleY);
        }
        /// <summary>
        /// Relatively vector scales the sprite group without easing.<br/>
        /// NOTE: You will need to finalize the sprite group using the <br/>.FinalizeGroup()
        /// function in order for the scaling to be accurate. This is because in order to set
        /// an initial scale, all scale functions need to be cached until a known start time is set.
        /// 
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="dscaleX">The change in x scale.</param>
        /// <param name="dscaleY">The change in y scale.</param>
        public void ScaleVec(double StartTime, double EndTime, double dscaleX, double dscaleY)
        {

            _scalevec(OsbEasing.None, StartTime, EndTime, dscaleX, dscaleY);
        }
        #endregion
        #region Private Scale Command
        private void _scalevec(OsbEasing Easing, double StartTime, double EndTime, double dscaleX, double dscaleY)
        {
            if (Finalized) return;
            ScaleFunctions.Add(new ScaleFunction(Easing, StartTime, EndTime, dscaleX, dscaleY));
        }
        #endregion
        #endregion

        #region Setting Visible Bounds
        public void SetVisibleBounds(double StartTime, double EndTime)
        {
            if (Finalized) return;
            _updateStartTime(StartTime);
            for (int i = 0; i < SpriteCount; i++)
            {
                fragments[i].SetVisibleBounds(StartTime, EndTime);
            }
        }
        #endregion
        #region Rotation Commands
        #region Public Commands
        /// <summary>
        /// Relatively rotates the group with easing.
        /// </summary>
        /// <param name="Easing"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="dr"></param>
        public void Rotate(OsbEasing Easing, double StartTime, double EndTime, double dr)
        {
            _rotate(Easing, StartTime, EndTime, dr);
        }
        /// <summary>
        /// Relatively rotates the group without easing.
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="dr"></param>
        public void Rotate(double StartTime, double EndTime, double dr)
        {
            _rotate(OsbEasing.None, StartTime, EndTime, dr);
        }
        /// <summary>
        /// Relatively rotates the group instantly.
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="dr"></param>
        public void Rotate(double StartTime, double dr)
        {
            _rotate(OsbEasing.None, StartTime, StartTime, dr);
        }
        #endregion
        #region Private Commands
        private void _rotate(OsbEasing Easing, double StartTime, double EndTime, double dr)
        {
            if (Finalized) return;
            _updateStartTime(StartTime);
            for (int i = 0; i < SpriteCount; i++)
            {
                fragments[i].Rotate(Easing, StartTime, EndTime, dr);
            }

        }
        #endregion
        #endregion
    }

    internal class ScaleFunction
    {
        internal OsbEasing easing;
        internal double StartTime, EndTime;
        internal double dScaleX, dScaleY;
        internal ScaleFunction(OsbEasing easing, double startTime, double endTime, double dScaleX, double dScaleY)
        {
            this.easing=easing;
            this.StartTime=startTime;
            this.EndTime=endTime;
            this.dScaleX=dScaleX;
            this.dScaleY=dScaleY;
        }
    }
    
}
