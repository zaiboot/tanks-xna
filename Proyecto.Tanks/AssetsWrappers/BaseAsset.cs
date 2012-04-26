using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Proyecto.Tanks.AssetsWrappers
{
    /// <summary>
    /// This is the base asset wrapper.
    /// </summary>
    public abstract class BaseAsset
    {
        /// <summary>
        /// You load the resources here
        /// </summary>
        /// <param name="content">Where the resources come from</param>
        public abstract void LoadResources(ContentManager content);

        /// <summary>
        /// You do the calcualtions here
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// You draw or paint here.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(SpriteBatch spriteBatch);

    }
}
