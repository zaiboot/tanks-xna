using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Proyecto.Tanks.AssetsWrappers
{
    internal enum Orientation
    {
        DOWN,
        UP,
        LEFT,
        RIGHT

    };

    /// <summary>
    /// This is a tank.
    /// What the player controls and shoots bullets, but only one at the time.
    /// Until the bullet is out of bounds or hit some obstacle, the thank cannot shoot another bullet.
    /// </summary>
    public class Tank : BaseAsset
    {
        /// <summary>
        /// Represents a single bullet
        /// </summary>
        private class Bullet
        {
            /// <summary>
            /// The image of the bullet to draw
            /// </summary>
            internal Texture2D bullet;
            /// <summary>
            /// Where the bullet will be placed.
            /// </summary>
            internal Rectangle bulletPosition;
            /// <summary>
            /// Where the bullet will be pointing. 
            /// <see cref="Proyecto.Tanks.AssetsWrappers.Orientation"/>
            /// </summary>
            private Orientation currentOrientation;
            /// <summary>
            /// The point to use as a reference to rotate the bullet if necessary
            /// </summary>
            internal Vector2 rotationPoint;
            /// <summary>
            /// The angle to rotate the bullet. Remember that this comes on radians
            /// instead degrees.
            /// </summary>
            internal float rotationAngle;
            /// <summary>
            /// How many pixels the bullet will move per iteration
            /// </summary>
            private const int SPEED = 1;

            public Bullet()
            {
                bulletPosition = new Rectangle() { Width = 15, Height = 10 };
            }

            /// <summary>
            /// Defines if the bullet is visible
            /// </summary>
            public bool IsBulletVisible { get; private set; }

            /// <summary>
            /// Moves the bullet by using the direction, speed in this class
            /// </summary>
            public void Move(Rectangle windowSize)
            {
                //Bullet out of bounds
                if (bulletPosition.Y < 0 || bulletPosition.X < 0 ||
                    bulletPosition.Y > windowSize.Height || bulletPosition.X > windowSize.Width)
                {
                    IsBulletVisible = false;
                    return;
                }


                switch (currentOrientation)
                {
                    case Orientation.DOWN:
                        bulletPosition.Y += SPEED;
                        break;
                    case Orientation.UP:
                        bulletPosition.Y -= SPEED;
                        break;
                    case Orientation.LEFT:
                        bulletPosition.X -= SPEED;
                        break;
                    case Orientation.RIGHT:
                        bulletPosition.X += SPEED;
                        break;

                }
            }

            /// <summary>
            /// Starts moving the bullet.
            /// </summary>
            /// <param name="theTank">The thank that contains the bullet.</param>
            public void Start(Tank theTank)
            {
                IsBulletVisible = true;

                currentOrientation = theTank.currentOrientation;
                rotationPoint.X = bullet.Width / 2;
                rotationPoint.Y = bullet.Height / 2;
                switch (currentOrientation)
                {
                    case Orientation.DOWN:
                        rotationAngle = MathHelper.PiOver2;
                        bulletPosition.X = (int)theTank.tankPosition.X + (theTank.spriteToDraw.Width / 2);
                        bulletPosition.Y = (int)theTank.tankPosition.Y + theTank.spriteToDraw.Height;
                        break;
                    case Orientation.UP:
                        rotationAngle = MathHelper.PiOver2 * -1;
                        bulletPosition.X = (int)theTank.tankPosition.X + (theTank.spriteToDraw.Height / 2) - 4;
                        bulletPosition.Y = (int)theTank.tankPosition.Y;
                        break;
                    case Orientation.LEFT:
                        rotationAngle = MathHelper.Pi;//180 Degrees
                        bulletPosition.X = (int)theTank.tankPosition.X;
                        bulletPosition.Y = (int)theTank.tankPosition.Y + (theTank.spriteToDraw.Height / 2) - 4;
                        break;
                    case Orientation.RIGHT:
                        rotationAngle = 0;
                        bulletPosition.X = (int)theTank.tankPosition.X + theTank.spriteToDraw.Width;
                        bulletPosition.Y = (int)theTank.tankPosition.Y + (theTank.spriteToDraw.Height / 2) - 4;
                        break;

                }
            }
        }

        #region UI_ASSETS
        /// <summary>
        /// The name of the asset to load.
        /// </summary>
        private string assetName = "tank";
        private string bulletAssetName = "bullet";
        private Color tankColor;
        private Texture2D tank;
        private Rectangle spriteToDraw;
        /// <summary>
        /// The direction of the tank.
        /// </summary>
        private Orientation currentOrientation = Orientation.RIGHT;
        #endregion

        #region MOVEMENT
        private bool canMove = true;
        private Bullet myBullet = new Bullet();
        private Vector2 tankPosition;

        /// <summary>
        /// Returns the tankPosition of this tank.
        /// </summary>
        public IList<Vector2> TankBoundaries { get; private set; }
        private ushort tankSpeed = 3;//the speed of the tank. 
        #endregion

        private Game owner;
        #region INTERACTION_KEYS
        private Keys DOWN_KEY;
        private Keys UP_KEY;
        private Keys LEFT_KEY;
        private Keys RIGHT_KEY;
        private Keys FIRE_KEY;
        #endregion

        public Tank(Game owner)
        {
            Random r = new Random(owner.TargetElapsedTime.Milliseconds);
            TankBoundaries = new List<Vector2>();
            spriteToDraw = new Rectangle(0, 0, 46, 48);
            tankPosition = new Vector2(0, 0);

            this.owner = owner;
            DOWN_KEY = Keys.Down;
            UP_KEY = Keys.Up;
            LEFT_KEY = Keys.Left;
            RIGHT_KEY = Keys.Right;
            FIRE_KEY = Keys.LeftControl;
            tankColor = Color.FromNonPremultiplied(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255), 255);

        }

        public override void LoadResources(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            tank = content.Load<Texture2D>(assetName);
            myBullet.bullet = content.Load<Texture2D>(bulletAssetName);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var keyStroke = Keyboard.GetState();
            Keys[] pressedKeys = keyStroke.GetPressedKeys();
            bool tankMoved = false;
            if (myBullet.IsBulletVisible) //we gotta move the bullet
            {
                myBullet.Move(owner.Window.ClientBounds);
            }
            else
            {
                //Shoot only one bullet at the time
                if (pressedKeys.Contains(FIRE_KEY))
                {

                    myBullet.Start(this);
                }
            }


            if (canMove)
            {
                //the tank can move freely

                if (pressedKeys.Contains(DOWN_KEY))
                {
                    if (currentOrientation != Orientation.DOWN)
                    {
                        //Down key, rotate it if necessary and move it down in the next frame
                        spriteToDraw.X = 147;
                        currentOrientation = Orientation.DOWN;
                    }
                    else
                    {
                        if (tankPosition.Y < owner.Window.ClientBounds.Height - 53)
                        {
                            //Make the tank move down.
                            tankPosition.Y += tankSpeed;
                            tankMoved = true;
                        }
                    }
                }

                if (pressedKeys.Contains(UP_KEY))
                {
                    if (currentOrientation != Orientation.UP)
                    {
                        //Down key, rotate it if necessary and move it down in the next frame
                        spriteToDraw.X = 96;
                        currentOrientation = Orientation.UP;
                    }
                    else
                    {
                        //Make the tank move up.
                        if (tankPosition.Y > tankSpeed)
                        {
                            // avoid the out of bounds.
                            tankPosition.Y -= tankSpeed;
                            tankMoved = true;
                        }

                    }
                }

                if (pressedKeys.Contains(LEFT_KEY))
                {
                    if (currentOrientation != Orientation.LEFT)
                    {
                        //Down key, rotate it if necessary and move it down in the next frame
                        spriteToDraw.X = 48;
                        currentOrientation = Orientation.LEFT;
                    }
                    else
                    {
                        //Make the tank move left.
                        if (tankPosition.X > tankSpeed)
                        {
                            tankPosition.X -= tankSpeed;
                            tankMoved = true;
                        }

                    }
                }

                if (pressedKeys.Contains(RIGHT_KEY))
                {
                    if (currentOrientation != Orientation.RIGHT)
                    {
                        //Down key, rotate it if necessary and move it down in the next frame
                        spriteToDraw.X = 0;
                        currentOrientation = Orientation.RIGHT;
                    }
                    else
                    {

                        //Make the tank move right.
                        if (tankPosition.X < owner.Window.ClientBounds.Width - 53)
                        {
                            tankPosition.X += tankSpeed;
                            tankMoved = true;
                        }
                    }
                }
                if (tankMoved)
                {
                    UpdateTankBoundaries(); 
                }
            }
            

        }

        private void UpdateTankBoundaries()
        {
            TankBoundaries.Clear();
            TankBoundaries.Add(tankPosition);
            for (int i = (int)tankPosition.X; i < tank.Width; i++)
            {
                TankBoundaries.Add(new Vector2(i,1));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tank, tankPosition, spriteToDraw, tankColor);

            if (myBullet.IsBulletVisible)
            {
                spriteBatch.Draw(myBullet.bullet, myBullet.bulletPosition, null, tankColor, myBullet.rotationAngle, myBullet.rotationPoint, SpriteEffects.None, 0f);
            }
        }


        internal void StopMoving(bool stopMoving)
        {
            this.canMove = !stopMoving;
        }
    }
}
