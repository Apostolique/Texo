using System;
using System.Collections.Generic;
using System.Linq;
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
            // TODO: Figure out the order that things need to be. Right now we need to invalidate the mouse cache multiple times.

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

            Core.Update();

            if (Triggers.CameraDrag.Pressed()) {
                _mouseAnchor = Core.MouseWorld;
            }
            if (Triggers.CameraDrag.Held()) {
                Core.Camera.Pos += _mouseAnchor - Core.MouseWorld;
            } else if (_play) {
                Core.Camera.Pos += new Vector2(300 * gameTime.GetElapsedSeconds(), 0);
            }

            Core.Update();

            if (_currentMode == Modes.selection) {
                if (Triggers.ToggleSelectAll.Pressed()) {
                    if (_selectedNotes.Count < Quadtree<Note>.Items.Count()) {
                        _selectedNotes.UnionWith(Quadtree<Note>.Query(Quadtree<Note>.Bounds));
                    } else {
                        _selectedNotes.Clear();
                    }
                }
                if (Triggers.SelectionDrag.Pressed()) {
                    _selectionStart = Core.MouseWorld;
                    _selectionEnd = Core.MouseWorld;
                    _selection = Utility.CreateRect(_selectionStart, _selectionEnd);

                    _isSelecting = true;
                }
                if (_isSelecting && Triggers.SelectionDrag.Held()) {
                    _selectionEnd = Core.MouseWorld;
                    _selection = Utility.CreateRect(_selectionStart, _selectionEnd);
                    _selectedNotesTemp.Clear();

                    if (Triggers.SelectionDragAdd.Held()) {
                        _selectedNotesTemp.UnionWith(_selectedNotes);
                        _selectedNotesTemp.UnionWith(Quadtree<Note>.Query(_selection));
                    } else if (Triggers.SelectionDragExclude.Held()) {
                        _selectedNotesTemp.UnionWith(_selectedNotes);
                        _selectedNotesTemp.ExceptWith(Quadtree<Note>.Query(_selection));
                    } else {
                        _selectedNotesTemp.UnionWith(Quadtree<Note>.Query(_selection));
                    }
                }
                if (_isSelecting && Triggers.SelectionDrag.Released()) {
                    _selectedNotes.Clear();
                    _selectedNotes.UnionWith(_selectedNotesTemp);
                    _selectedNotesTemp.Clear();

                    _selection = Utility.CreateRect(Vector2.Zero, Vector2.Zero);

                    _isSelecting = false;
                }

                if (!_isSelecting && Triggers.CreateNote.Pressed()) {
                    _currentMode = Modes.grab;
                    _grabAnchor = Core.MouseWorld;
                    _grabAnchorInitial = Core.MouseWorld;

                    Note newNote = new Note((int)Core.MouseWorld.X, (int)Core.MouseWorld.Y, 100, 30);
                    Quadtree<Note>.Add(newNote);

                    _selectedNotes.Clear();
                    _selectedNotes.Add(newNote);
                }
                if (!_isSelecting && Triggers.Grab.Pressed()) {
                    _currentMode = Modes.grab;
                    _grabAnchor = Core.MouseWorld;
                    _grabAnchorInitial = Core.MouseWorld;
                }
            } else if (_currentMode == Modes.grab) {
                Point diff = (Core.MouseWorld - _grabAnchor).ToPoint();

                if (Triggers.GrabConfirm.Pressed()) {
                    _currentMode = Modes.selection;
                }
                if (Triggers.GrabCancel.Pressed()) {
                    _currentMode = Modes.selection;
                    diff = (_grabAnchorInitial - _grabAnchor).ToPoint();
                }

                foreach (var n in _selectedNotes) {
                    n.Position += diff;
                    Quadtree<Note>.Update(n);
                }
                _grabAnchor = Core.MouseWorld;
            }
        }

        public void Draw(SpriteBatch s) {
            foreach (var n in Quadtree<Note>.Items) {
                n.Item.Draw(s, Color.White);
            }

            foreach (var n in Quadtree<Note>.Nodes)
                s.DrawRectangle(n, Color.White * 0.2f, 4);

            foreach (var e in Quadtree<Note>.Items)
                s.DrawLine(e.Item.AABB.Center.ToVector2(), e.Node.Center.ToVector2(), Color.White * .5f, 4);

            if (_isSelecting) {
                foreach (var n in _selectedNotesTemp)
                    s.DrawRectangle(n.AABB, Color.Red * 0.8f, 4);
            } else {
                foreach (var n in _selectedNotes)
                    s.DrawRectangle(n.AABB, Color.Red * 0.8f, 4);
            }

            if (_currentMode == Modes.selection && _selection.Width > 0 && _selection.Height > 0) {
                s.DrawRectangle(_selection, Color.Red, 4 / Core.Camera.ScreenToWorldScale());
            }
        }

        enum Modes {
            selection,
            grab,
        }

        Modes _currentMode = Modes.selection;
        bool _play = false;

        Vector2 _mouseAnchor = Vector2.Zero;
        Vector2 _cameraAnchor = Vector2.Zero;

        Vector2 _grabAnchorInitial = Vector2.Zero;
        Vector2 _grabAnchor = Vector2.Zero;

        Rectangle _selection = new Rectangle(0, 0, 0, 0);
        Vector2 _selectionStart = Vector2.Zero;
        Vector2 _selectionEnd = Vector2.Zero;
        bool _isSelecting = false;

        HashSet<Note> _selectedNotesTemp = new HashSet<Note>();
        HashSet<Note> _selectedNotes = new HashSet<Note>();
    }
}