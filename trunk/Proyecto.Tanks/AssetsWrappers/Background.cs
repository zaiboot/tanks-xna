using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Proyecto.Tanks.AssetsWrappers
{
    /// <summary>
    /// The foreground that will be created dinamically
    /// </summary>
    public class Background : BaseAsset
    {
        public int[,] terrainContour;
        /// <summary>
        /// The width of the game
        /// </summary>
        public int width;

        /// <summary>
        /// The height of the game
        /// </summary>
        public int height;

        private Game owner;
        private Random randomizer = new Random();


        Texture2D brick;
        Texture2D backgroundTexture;
        public Color[,] backgroundColorArray;

        public Background(Game owner)
        {
            this.owner = owner;
        }

        public override void LoadResources(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            brick = content.Load<Texture2D>("brick");
            width = owner.Window.ClientBounds.Width;
            height = owner.Window.ClientBounds.Height;

            GenerateTerrainContour();
            CreateForeground();

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Rectangle screenRectangle = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
        }

        public void CreateForeground()
        {
            Color[,] groundColors = Utils.TextureTo2DArray(brick);
            Color[] backgroundColors = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y > terrainContour[x, y])
                        backgroundColors[x + y * width] = groundColors[x % brick.Width, y % brick.Height];
                    else
                        backgroundColors[x + y * width] = Color.Transparent;
                }
            }

            backgroundTexture = new Texture2D(owner.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            backgroundTexture.SetData(backgroundColors);

            backgroundColorArray = Utils.TextureTo2DArray(backgroundTexture);
        }

        private void GenerateTerrainContour()
        {
            terrainContour = new int[width, height];

            for (int x = 0; x < width; x++)
            {

                for (int y = 0; y < height; y++)
                {
                    if ((x < 60 && y < 60) || (x > width - 60 && y < 60) || (x < 60 && y > height - 60) || (x > width - 60 && y > height - 60))
                    {

                        terrainContour[x, y] =  1000;
                    }
                }
            }
            //Generates random wholes on the foreground.
            int numberOfHoles = randomizer.Next(5, 100);
            int holeWidth = randomizer.Next(60, 100);
            int holeHeight = holeWidth;

            for (int singleHole = 0; singleHole < numberOfHoles; singleHole++)
            {
                //Random positions where to place the holes.
                int xRandom = randomizer.Next(0, width - 60);
                int yRandom = randomizer.Next(0, height - 60);
                
                for (int x = xRandom; x < xRandom + holeWidth && x < width ; x++)
                {
                    for (int y = yRandom; y < (yRandom + holeHeight)  && y < height ; y++)
                    {
                        terrainContour[x, y] = 1000;
                    }
                }
            }
        }
    }
}
