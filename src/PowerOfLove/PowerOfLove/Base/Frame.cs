using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Base
{
    /// <summary>
    /// Represents a single image inside a sprite sheet.
    /// </summary>
    public class Frame
    {
        public Frame(Texture2D texture, Rectangle? rectangle = null)
        {
            Texture = texture;
            Rectangle = rectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
        }

        /// <summary>
        /// The sprite sheet's texture.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// The frame's position inside the sprite sheet.
        /// </summary>
        public Rectangle Rectangle { get; private set; }

        /// <summary>
        /// The Width of this frame.
        /// </summary>
        public int Width { get { return Rectangle.Width; } }

        /// <summary>
        /// The Height of this frame.
        /// </summary>
        public int Height { get { return Rectangle.Height; } }
    }
}
