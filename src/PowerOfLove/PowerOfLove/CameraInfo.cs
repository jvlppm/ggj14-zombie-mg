using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PowerOfLove
{
    public class CameraInfo
    {
        public Vector2 Position { get; private set; }
        public float ZoomFactor { get; private set; }

        public CameraInfo(Vector2 position, float zoomFactor)
        {
            Position = position;
            ZoomFactor = zoomFactor;
        }

        public Rectangle GetArea(Viewport viewport)
        {
            var screenWidth = (int)(viewport.Width / ZoomFactor / 2);
            var screenHeight = (int)(viewport.Height / ZoomFactor / 2);

            return new Rectangle(
                x: (int)(Position.X) - screenWidth / 2,
                y: (int)(Position.Y) - screenHeight / 2,
                width: screenWidth,
                height: screenHeight);
        }
    }
}
