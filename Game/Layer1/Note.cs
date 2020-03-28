using Dcrew.MonoGame._2D_Spatial_Partition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    public class Note : IAABB {
        public Note(int x, int y, int width, int height) {
            _note = new Rectangle(x, y, width, height);
        }

        public void Draw(SpriteBatch s) {
            s.FillRectangle(_note, Color.White);
        }

        private Rectangle _note;

        public Rectangle AABB => _note;
    }
}