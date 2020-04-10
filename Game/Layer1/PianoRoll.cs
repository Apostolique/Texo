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
            for (int i = _start; i <= _end; i++) {
                var whiteIndex = Utility.Mod(i, _whiteNotes.Length);
                var octave = (int)MathF.Floor((float)i / _whiteNotes.Length) * _whiteDownCount.Last();
                if (_whiteNotes[whiteIndex]) {
                    s.FillRectangle(X, Y - (_whiteDownCount[whiteIndex] + octave) * _whiteHeight, _whiteWidth, _whiteHeight, Color.White);
                    s.DrawRectangle(X, Y - (_whiteDownCount[whiteIndex] + octave) * _whiteHeight, _whiteWidth, _whiteHeight, Color.Gray, 2);
                }
            }
            for (int i = _start; i <= _end; i++) {
                var blackIndex = Utility.Mod(i, _blackNotes.Length);
                var octave = (int)MathF.Floor((float)i / _blackNotes.Length) * 12;
                if (_blackNotes[blackIndex]) {
                    s.FillRectangle(X, Y - (blackIndex + octave) * Core.NoteHeight - _blackHeight + _blackDiff, _blackWidth, _blackHeight, Color.Black);
                    s.DrawRectangle(X, Y - (blackIndex + octave) * Core.NoteHeight - _blackHeight + _blackDiff, _blackWidth, _blackHeight, Color.Gray, 2);
                }
            }
        }

        int[] _whiteDownCount = new int[] {1, 1, 2, 2, 3, 4, 4, 5, 5, 6, 6, 7};
        bool[] _whiteNotes = new bool[] {true, false, true, false, true, true, false, true, false, true, false, true};
        bool[] _blackNotes = new bool[] {false, true, false, true, false, false, true, false, true, false, true, false};

        int X = -2000;
        int Y = 0;

        int _start;
        int _end;

        int _whiteWidth = Core.NoteHeight * 10;
        int _whiteHeight = Core.NoteHeight * 12 / 7;

        int _blackWidth = Core.NoteHeight * 8;
        int _blackHeight = (int)(Core.NoteHeight * 12 / 7 * 0.75f);
        int _blackDiff = (int)((Core.NoteHeight * 12 / 7 * 0.75f) - Core.NoteHeight) / 2;
    }
}