using System;
using System.Collections.Generic;
using Apos.Gui;
using Dcrew.Spatial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpriteFontPlus;

namespace GameProject {
    public class Note : IBounds {
        public Note(int x, int y, int width, int height) {
            _bounds = new RotRect(x, y, width, height);
        }

        public Note Parent {
            get;
            set;
        }
        public List<Note> Children {
            get;
            set;
        }

        public Vector2 XY {
            get => _bounds.XY;
            set {
                _bounds.XY = value;
            }
        }

        public RotRect Bounds => _bounds;
        public float Angle => 0;
        public Vector2 Origin => Vector2.Zero;

        public int Start => _bounds.AABB.Left;
        public int End => _bounds.AABB.Right;

        public int Number => Math.Min(Math.Max(-(int)Math.Floor(_bounds.AABB.Y / (float)Core.NoteHeight), 0), 127);

        public void Draw(SpriteBatch s, Color c) {
            s.FillRectangle(_bounds.AABB, c);
            s.DrawRectangle(_bounds.AABB, Color.Black * 0.2f, 1);

            s.DrawString(GuiHelper.Font, $"{Number}", _bounds.XY, Color.Black);
        }

        private RotRect _bounds;
    }
}
