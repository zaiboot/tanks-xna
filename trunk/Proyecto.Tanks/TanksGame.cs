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
        private Tank singleTank;
        private List<Tank> tanks;
        private Background obstacles;
        
        private const int MAX_NUMBER_TANKS = 4;

        public TanksGame()
        {


            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Assets";
            #region TANK_CREATION

            tanks = new List<Tank>();
            singleTank = new Tank(this, 0);
            tanks.Add(singleTank);
            for (int i = 1; i < MAX_NUMBER_TANKS; i++)
            {
                tanks.Add(new Tank(this, (PlayerIndex)i));
            }
            #endregion
            obstacles = new Background(this);
            
            assetsToDraw.Add(obstacles);
            assetsToDraw.Add(singleTank);

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
            spriteBatch.End();

            singleTank.explosion.DrawExplosion(spriteBatch);

            base.Draw(gameTime);
        }

        private void CheckCollisions(GameTime gameTime)
        {
            Vector2 bulletTerrainCollisionPoint = CheckBulletTerrainCollision();
            Vector2 tankTerrainCollisionPoint = CheckTankTerrainCollision();

            if (singleTank.myBullet.IsBulletVisible)
            {
                if (bulletTerrainCollisionPoint.X > -1)
                {
                    singleTank.myBullet.IsBulletVisible = false;
                    singleTank.explosion.AddExplosion(bulletTerrainCollisionPoint, 4, 30.0f, 1000.0f, gameTime);
                }
            }

            if (tankTerrainCollisionPoint.X > -1)
            {
                singleTank.terrainCollision = true;
            }
        }

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

        private Vector2 CheckBulletTerrainCollision()
        {
            Matrix bulletMat = Matrix.CreateRotationZ(singleTank.myBullet.rotationAngle) * Matrix.CreateTranslation(singleTank.myBullet.bulletPosition.X, singleTank.myBullet.bulletPosition.Y, 0);
            Matrix obstaclesMat = Matrix.Identity;
            Vector2 terrainCollisionPoint = TexturesCollide(singleTank.myBullet.bulletColorArray, bulletMat, obstacles.backgroundColorArray, obstaclesMat);
            return terrainCollisionPoint;
        }

        private Vector2 CheckTankTerrainCollision()
        {
            Matrix tankMat = Matrix.CreateRotationZ(singleTank.tankRotationAngle) * Matrix.CreateTranslation(singleTank.tankPosition.X, singleTank.tankPosition.Y, 0);
            Matrix obstaclesMat = Matrix.Identity;
            Vector2 terrainCollisionPoint = TexturesCollide(singleTank.myBullet.bulletColorArray, tankMat, obstacles.backgroundColorArray, obstaclesMat);
            return terrainCollisionPoint;
        }

    }
}

