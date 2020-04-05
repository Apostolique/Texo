using Dcrew.MonoGame._2D_Spatial_Partition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    public class Note : IAABB {
        public Note(int x, int y, int width, int height) {
            _note = new Rectangle(x, y, width, height);
        }

        public Point Position {
            get => _note.Location;
            set {
                _note.Location = value;
            }
        }

        public int Start => _note.Left;
        public int End => _note.Right;

        public void Draw(SpriteBatch s, Color c) {
            s.FillRectangle(_note, c);
        }

        private Rectangle _note;

        public Rectangle AABB => _note;
    }
}