using System;
using Apos.Input;
using Dcrew.Camera;
using Microsoft.Xna.Framework;

namespace GameProject {
    public static class Core {
        public static Game Game;
        public static GameWindow Window;
        public static Camera Camera;
        public static Vector2 MouseWorld = Vector2.Zero;

        public static bool ShowMenu = true;
        public static Menu Menu;

        public static Midi Midi;

        public static float Zoom {
            get => MathF.Sqrt(Camera.ZFromScale(Camera.Scale.X, 0f));
            set {
                Camera.Scale = new Vector2(Camera.ScaleFromZ(value * value, 0f));
            }
        }
        public static int NoteWidth = 50;
        public static int NoteHeight = 30;

        public static int LineSize = 2;

        public static void Setup(Game game) {
            Game = game;
            Window = game.Window;

            Camera = new Camera(new Vector2(-200, -1800), 0, new Vector2(1f));
        }
    }
}
