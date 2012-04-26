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
        private Background obstacles;


        public TanksGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Assets";
            singleTank = new Tank(this);
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
            CheckCollissionWithBackground();
            // TODO: Add your update logic here
            foreach (BaseAsset asset in assetsToDraw)
            {
                asset.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private void CheckCollissionWithBackground()
        {

            //bool tankHitsBackground = false;
            //foreach (Vector2 tankPosition in singleTank.TankBoundaries)
            //{
            //    tankHitsBackground = obstacles.NonTransparentPoints.Contains(tankPosition);
            //    if (tankHitsBackground)
            //    {
            //        break;
            //    }
            //}

            //if (tankHitsBackground)
            //{
            //    singleTank.StopMoving(true);

            //}
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
            base.Draw(gameTime);
        }
    }
}
