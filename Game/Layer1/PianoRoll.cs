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
                    s.FillRectangle(X + (_whiteDownCount[whiteIndex] - 1 + octave) * _whiteWidth, Y, _whiteWidth, _whiteHeight, Color.White);
                    s.DrawRectangle(X + (_whiteDownCount[whiteIndex] - 1 + octave) * _whiteWidth, Y, _whiteWidth, _whiteHeight, Color.Gray, 2);
                }
            }
            for (int i = _start; i <= _end; i++) {
                var blackIndex = Utility.Mod(i, _blackNotes.Length);
                var octave = (int)MathF.Floor((float)i / _blackNotes.Length) * _whiteDownCount.Last();
                if (_blackNotes[blackIndex]) {
                    s.FillRectangle(X + (_whiteDownCount[blackIndex] + octave) * _whiteWidth - _blackWidth / 2, Y, _blackWidth, _blackHeight, Color.Black);
                    s.DrawRectangle(X + (_whiteDownCount[blackIndex] + octave) * _whiteWidth - _blackWidth / 2, Y, _blackWidth, _blackHeight, Color.Gray, 2);
                }
            }
        }

        int[] _whiteDownCount = new int[] {1, 1, 2, 2, 3, 4, 4, 5, 5, 6, 6, 7};
        bool[] _whiteNotes = new bool[] {true, false, true, false, true, true, false, true, false, true, false, true};
        bool[] _blackNotes = new bool[] {false, true, false, true, false, false, true, false, true, false, true, false};

        int X = 0;
        int Y = 800;

        int _start;
        int _end;

        int _whiteWidth = 20;
        int _whiteHeight = 100;

        int _blackWidth = 10;
        int _blackHeight = 70;
    }
}