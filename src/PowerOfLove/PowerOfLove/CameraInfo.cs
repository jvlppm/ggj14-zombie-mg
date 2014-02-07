using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PowerOfLove
{
    public class CameraInfo
    {
        public Vector2 Position { get; private set; }
        public float ZoomFactor { get; private set; }
        public Viewport Viewport { get; private set; }

        public CameraInfo(Vector2 position, float zoomFactor, Viewport viewport)
        {
            Position = position;
            ZoomFactor = zoomFactor;
            Viewport = viewport;
        }

        public Vector2 GetTopLeftPosition()
        {
            var screenWidth = Viewport.Width / ZoomFactor;
            var screenHeight = Viewport.Height / ZoomFactor;

            return new Vector2(Position.X - screenWidth / 2, Position.Y - screenHeight / 2);
        }

        public Vector2 ScreenToMapPosition(Point screenPosition)
        {
            return GetTopLeftPosition() + new Vector2(screenPosition.X, screenPosition.Y) / ZoomFactor;
        }
    }
}
