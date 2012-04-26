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
        /// <summary>
        /// The game that we are running
        /// </summary>
        private Game owner;

        /// <summary>
        /// The image that we will repeat over
        /// </summary>
        private Texture2D backgroundShape;

        /// <summary>
        /// Where the background starts
        /// </summary>
        private Vector2 position;

        private List<Vector2> nonTransparentPoints = new List<Vector2>();

        public IList<Vector2> NonTransparentPoints { get { return nonTransparentPoints; } }

        public Background(Game owner)
        {
            this.owner = owner;
            position = new Vector2(0, 0);
        }


        public override void LoadResources(Microsoft.Xna.Framework.Content.ContentManager content)
        {

            backgroundShape = new Texture2D(owner.GraphicsDevice, owner.Window.ClientBounds.Width, owner.Window.ClientBounds.Height);
            int width = owner.Window.ClientBounds.Width;
            int height = owner.Window.ClientBounds.Height;
            Color[] backgroundData = new Color[width * height];

            // Colour the entire texture transparent first.
            for (int i = 0; i < backgroundData.Length; i++)
                backgroundData[i] = Color.Transparent;

            Texture2D brick = content.Load<Texture2D>("brick");
            Color[,] brickContent = Utils.TextureTo2DArray(content.Load<Texture2D>("brick"));
            Random randomObject = new Random();

            // this will use same smoke radomizer function we use for smoke generating.
            double rand1 = randomObject.NextDouble() + 1;
            double rand2 = randomObject.NextDouble() + 2;
            double rand3 = randomObject.NextDouble() + 3;

            float offset = height / 2;
            float peakheight = height;
            float flatness = 70;

            // Ulacit: For each wave, we first draw a random value between 0 and 1. We also offset it a bit, so the first one becomes between the [0,1] range,
            // the second between the [1,2] range and the last one between the [2,3] range. For our 3 waves, these values will divide the peakheight
            // and the flatness, so wave 3 will be lower and shorter than waves 1 and 2. Furthermore, you see we’re also adding the random values inside
            // the Sine method. Otherwise, all 3 waves would start exactly at the Y coordinate specified by ‘offset’, making this point fixed each time
            // we would restart the game.
            
            
            


            for (int x = 0; x < width; x++)
            {
                
                for (int y = 0; y < height; y++)
                {
                    //Areas that cannot have brick colors at all, 
                    //these are the spaces for the 4 players
                    if ((x > 60 || y > 60)
                        && (x < width - 60 || y > 60)
                        && (x < width - 60 || y < height - 60)
                        && (x > 60 || y < height - 60)

                        )
                    {

                        double height1 = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                        height1 += peakheight / rand2 * Math.Cos((float)y / flatness * rand2 + rand2);
                        height1 += peakheight / rand3 * Math.Tanh((float)x / flatness * rand3 + rand3);
                        height1 += offset;
                        
                        //Define wheter we add the brick content or it will be transparent
                        if (height1 > y)
                        {

                            backgroundData[x + y * width] = brickContent[x % brick.Width, y % brick.Height];
                            nonTransparentPoints.Add(new Vector2(x, y));
                        }
                    }

                }
            }
            backgroundShape.SetData(backgroundData);


            //Here we need to calculate the convex hull for this one also update it every time
            //that the background is hit by a bullet, since we it will change all.

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundShape, position, Color.White);
        }
    }
}
