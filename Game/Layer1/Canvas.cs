using System;
using Apos.Input;
using Dcrew.MonoGame._2D_Spatial_Partition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    public class Canvas {
        public Canvas() {
            Quadtree<Note>.Add(new Note(0, 0, 100, 30));
            Quadtree<Note>.Add(new Note(200, 0, 100, 30));

            Quadtree<Note>.Add(new Note(0, 100, 100, 30));
            Quadtree<Note>.Add(new Note(200, 100, 100, 30));

            Quadtree<Note>.Add(new Note(400, 0, 100, 30));
            Quadtree<Note>.Add(new Note(600, 0, 100, 30));

            Quadtree<Note>.Add(new Note(400, 100, 100, 30));
            Quadtree<Note>.Add(new Note(600, 100, 100, 30));

            Quadtree<Note>.Add(new Note(0, 200, 100, 30));
            Quadtree<Note>.Add(new Note(200, 200, 100, 30));

            Quadtree<Note>.Add(new Note(0, 400, 100, 30));
            Quadtree<Note>.Add(new Note(200, 400, 100, 30));

            Quadtree<Note>.Add(new Note(400, 200, 100, 30));
            Quadtree<Note>.Add(new Note(600, 200, 100, 30));

            Quadtree<Note>.Add(new Note(400, 400, 100, 30));
            Quadtree<Note>.Add(new Note(600, 400, 100, 30));

            Quadtree<Note>.Add(new Note(800, -200, 100, 30));
            Quadtree<Note>.Add(new Note(1500, -1000, 100, 30));
        }

        public void Update(GameTime gameTime) {
            if (_currentMode == Modes.camera) {
                if (Triggers.PlayInteraction.Pressed()) {
                    _play = !_play;
                }
                if (Triggers.RotateLeft.Pressed()) {
                    Core.Camera.Angle += MathHelper.PiOver4;
                }
                if (Triggers.RotateRight.Pressed()) {
                    Core.Camera.Angle -= MathHelper.PiOver4;
                }
                if (InputHelper.NewMouse.ScrollWheelValue - InputHelper.OldMouse.ScrollWheelValue != 0) {
                    var diff = InputHelper.NewMouse.ScrollWheelValue - InputHelper.OldMouse.ScrollWheelValue;
                    // TODO: The diff multiplier should be a setting.
                    Core.Linear = Math.Max(Core.Linear - diff * 0.0005f, 0.1f);
                    Core.Depth = Core.LinearToDepth(Core.Linear);
                    Core.Camera.Scale = new Vector2(Core.DepthToZoom(Core.Depth, 0));
                }

                if (Triggers.Drag.Pressed()) {
                    Core.MouseAnchor = Core.MouseWorld;
                }
                if (Triggers.Drag.Held()) {
                    Core.Camera.Pos += Core.MouseAnchor - Core.MouseWorld;
                } else if (_play) {
                    Core.Camera.Pos += new Vector2(300 * gameTime.GetElapsedSeconds(), 0);
                }
            }
        }

        public void Draw(SpriteBatch s) {
            foreach (var n in Quadtree<Note>.Items) {
                n.Item.Draw(s);
            }

            foreach (var n in Quadtree<Note>.Nodes)
                s.DrawRectangle(n, Color.White * 0.2f, 4);

            foreach (var e in Quadtree<Note>.Items)
                s.DrawLine(e.Item.AABB.Center.ToVector2(), e.Node.Center.ToVector2(), Color.White * .5f, 4);
        }

        enum Modes {
            camera,
            selection,
        }

        Modes _currentMode = Modes.camera;
        bool _play = false;
    }
}