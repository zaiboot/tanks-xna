using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Proyecto.Tanks.AssetsWrappers;

namespace Proyecto.Tanks
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TanksGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private List<BaseAsset> assetsToDraw = new List<BaseAsset>();

        private List<Tank> tanks;
        private Background obstacles;
        private SoundEffect winGame;

        private const int MAX_NUMBER_TANKS = 4;
        private int tanksAlive = MAX_NUMBER_TANKS;
        private bool GameEnd = false;
        private Tank tankWinner = null;
        private SpriteFont font;
        public TanksGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Assets";
            #region TANK_CREATION

            tanks = new List<Tank>();

            for (int i = 0; i < MAX_NUMBER_TANKS; i++)
            {
                tanks.Add(new Tank(this, (PlayerIndex)i));
            }

            #endregion
            obstacles = new Background(this);

            assetsToDraw.Add(obstacles);
            assetsToDraw.AddRange(tanks);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //t.LoadResources(this.Content);
            foreach (BaseAsset asset in assetsToDraw)
            {
                asset.LoadResources(Content);
            }
            font = Content.Load<SpriteFont>("Courier New");
            winGame = Content.Load<SoundEffect>("WinGame");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            foreach (BaseAsset asset in assetsToDraw)
            {
                asset.Update(gameTime);
            }


            CheckCollisions(gameTime);

            if (tanksAlive <= 1)
            {
                foreach (Tank singleTank in tanks)
                {
                    if (singleTank.isAlive)
                    {
                        tankWinner = singleTank;

                        break;

                    }
                }
                winGame.Play();
                GameEnd = true;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (BaseAsset asset in assetsToDraw)
            {
                asset.Draw(spriteBatch);
            }

            //Someone won the game
            if (this.GameEnd)
            {
                spriteBatch.DrawString(font, "You won the game", new Vector2(0, 0), tankWinner.tankColor);
            }
            spriteBatch.End();
            foreach (Tank tank in tanks)
            {
                if (tank.isAlive)
                {
                    tank.explosion.DrawExplosion(spriteBatch);
                }
            }


            base.Draw(gameTime);
        }

        //Check all collisions
        private void CheckCollisions(GameTime gameTime)
        {
            foreach (Tank playerTank in tanks)
            {
                if (playerTank.isAlive)
                {
                    Vector2 tankTerrainCollisionPoint = CheckTankTerrainCollision(playerTank);

                    if (tankTerrainCollisionPoint.X > -1)
                    {
                        playerTank.terrainCollision = true;
                    }

                    //We do not need to check the collision only when the bullet is visible.
                    if (playerTank.myBullet.IsBulletVisible)
                    {
                        if (playerTank.myBullet.IsBulletVisible)
                        {
                            Vector2 bulletTerrainCollisionPoint = CheckBulletTerrainCollision(playerTank.myBullet);
                            if (bulletTerrainCollisionPoint.X > -1)
                            {
                                //Hit the background
                                playerTank.myBullet.IsBulletVisible = false;
                                Matrix mat = playerTank.explosion.AddExplosion(bulletTerrainCollisionPoint, 4, 30.0f, 1000.0f, gameTime);
                                AddCrater(playerTank.explosion.explosionColorArray, mat);
                                obstacles.hitTerrain.Play();
                                obstacles.CreateForeground();
                            }

                            Vector2 bulletTankCollisionPoint = CheckBulletTankCollision(playerTank);
                            if (bulletTankCollisionPoint.X > -1)
                            {
                                playerTank.myBullet.IsBulletVisible = false;
                                Matrix mat = playerTank.explosion.AddExplosion(bulletTankCollisionPoint, 4, 30.0f, 1000.0f, gameTime);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a crater to the background.
        /// It needs to be a squared cracter, if not the tank might not be able to move, since it
        /// will be colliding always.
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="mat"></param>
        private void AddCrater(Color[,] tex, Matrix mat)
        {
            int width = tex.GetLength(0);
            int height = tex.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tex[x, y].R > 0)
                    {
                        Vector2 imagePos = new Vector2(x, y);
                        Vector2 screenPos = Vector2.Transform(imagePos, mat);

                        int screenX = (int)screenPos.X;
                        int screenY = (int)screenPos.Y;

                        if ((screenX) > 0 && (screenX < obstacles.width) && (screenY) > 0 && (screenY < obstacles.height))
                            obstacles.terrainContour[screenX, screenY] = 1000;
                    }
                }
            }
        }

        //Check if to textures collide
        private Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return screenPos;
                                }
                            }
                        }
                    }
                }
            }

            return new Vector2(-1, -1);
        }

        /// <summary>
        /// Checks if a bullte has been hit the ground.
        /// </summary>
        /// <param name="bullet">The bullet to check if it hits the background</param>
        /// <returns>
        /// A vector 2, if the x contains -1 means no collision.
        /// </returns>
        private Vector2 CheckBulletTerrainCollision(Tank.Bullet bullet)
        {
            Matrix bulletMat = Matrix.CreateRotationZ(bullet.rotationAngle) * Matrix.CreateTranslation(bullet.bulletPosition.X, bullet.bulletPosition.Y, 0);
            Matrix obstaclesMat = Matrix.Identity;
            Vector2 terrainCollisionPoint = TexturesCollide(bullet.bulletColorArray, bulletMat, obstacles.backgroundColorArray, obstaclesMat);
            return terrainCollisionPoint;
        }

        //Check collisions between the Terrain and a Tank
        private Vector2 CheckTankTerrainCollision(Tank tank)
        {
            Matrix tankMat = Matrix.CreateRotationZ(tank.tankRotationAngle) * Matrix.CreateTranslation(tank.tankPosition.X, tank.tankPosition.Y, 0);
            Matrix obstaclesMat = Matrix.Identity;
            Vector2 terrainCollisionPoint = TexturesCollide(tank.myBullet.bulletColorArray, tankMat, obstacles.backgroundColorArray, obstaclesMat);
            return terrainCollisionPoint;
        }

        private Vector2 CheckBulletTankCollision(Tank tank)
        {
            Matrix bulletMat = Matrix.CreateRotationZ(tank.myBullet.rotationAngle) * Matrix.CreateTranslation(tank.myBullet.bulletPosition.X, tank.myBullet.bulletPosition.Y, 0);

            foreach (Tank tankEnemy in tanks)
            {
                if (tankEnemy.playerIndex != tank.playerIndex && tankEnemy.isAlive)
                {
                    int xPos = (int)tankEnemy.tankPosition.X;
                    int yPos = (int)tankEnemy.tankPosition.Y;

                    Matrix tankEnemyMat = Matrix.CreateTranslation(0, -20, 0) * Matrix.CreateTranslation(xPos, yPos, 0);
                    Vector2 tankEnemCollisionPoint = TexturesCollide(tankEnemy.tankColorArray, tankEnemyMat, tank.myBullet.bulletColorArray, bulletMat);

                    if (tankEnemCollisionPoint.X != -1)
                    {
                        tankEnemy.isAlive = false;
                        tankEnemy.hitTank.Play();
                        tanksAlive--;

                        return tankEnemCollisionPoint;
                    }

                }
            }

            return new Vector2(-1, -1);
        }

    }
}

