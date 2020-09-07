using System;
using System.Collections.Generic;
using Apos.Gui;
using Dcrew.Spatial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpriteFontPlus;

namespace GameProject {
    public class Note : IAABB {
        public Note(int x, int y, int width, int height) {
            _note = new Rectangle(x, y, width, height);
        }

        public Note Parent {
            get;
            set;
        }
        public List<Note> Children {
            get;
            set;
        }

        public Point XY {
            get => _note.Location;
            set {
                _note.Location = value;
            }
        }

        public Rectangle AABB => _note;
        public float Angle => 0;
        public Vector2 Origin => Vector2.Zero;

        public int Start => _note.Left;
        public int End => _note.Right;

        public int Number => Math.Min(Math.Max(-(int)Math.Floor(_note.Y / (float)Core.NoteHeight), 0), 127);

        public void Draw(SpriteBatch s, Color c) {
            s.FillRectangle(_note, c);
            s.DrawRectangle(_note, Color.Black * 0.2f, 1);


            s.DrawString(GuiHelper.Font, $"{Number}", _note.Location.ToVector2(), Color.Black);
        }

        private Rectangle _note;
    }
}
