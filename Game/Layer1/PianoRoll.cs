using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    public class PianoRoll {
        public PianoRoll(int start, int end) {
            _start = start;
            _end = end;
        }

        public void Draw(SpriteBatch s) {
            for (int i = _start; i < _end; i++) {
                var whiteIndex = Utility.Mod(i, _whiteNotes.Length);
                if (_whiteNotes[whiteIndex]) {
                    var octave = (int)MathF.Floor((float)i / _whiteNotes.Length) * 7;
                    var rect = new RectangleF(X, Y - (_whiteDownCount[whiteIndex] + octave) * _whiteHeight + Core.LineSize / 2, _whiteWidth, _whiteHeight);
                    s.FillRectangle(rect, Color.White);
                    s.DrawRectangle(rect, Color.Gray, 2);
                }
            }
            for (int i = _start; i < _end; i++) {
                var blackIndex = Utility.Mod(i, _blackNotes.Length);
                if (_blackNotes[blackIndex]) {
                    var rect = new RectangleF(X, Y - (i + 1) * Core.NoteHeight - _blackDiff + Core.LineSize / 2, _blackWidth, _blackHeight);
                    s.FillRectangle(rect, Color.Black);
                    s.DrawRectangle(rect, Color.Gray, 2);
                }
            }
        }

        int[] _whiteDownCount = new int[] {1, 1, 2, 2, 3, 4, 4, 5, 5, 6, 6, 7};
        bool[] _whiteNotes = new bool[] {true, false, true, false, true, true, false, true, false, true, false, true};
        bool[] _blackNotes = new bool[] {false, true, false, true, false, false, true, false, true, false, true, false};

        int X = -1000;
        int Y = 0;

        int _start;
        int _end;

        float _whiteWidth = Core.NoteHeight * 10;
        float _whiteHeight = (Core.NoteHeight * 12) / 7;

        float _blackWidth = Core.NoteHeight * 8;
        float _blackHeight = Core.NoteHeight * 12 / 7 * 0.75f;
        float _blackDiff = ((Core.NoteHeight * 12 / 7 * 0.75f) - Core.NoteHeight) / 2;
    }
}