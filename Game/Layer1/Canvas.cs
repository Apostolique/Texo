using System;
using System.Collections.Generic;
using System.Linq;
using Apos.Input;
using Dcrew.Spatial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    public class Canvas {
        public Canvas() {
            _quadtree.Add(new Note(50, 0, 100, 30));
            _quadtree.Add(new Note(200, 0, 100, 30));

            _quadtree.Add(new Note(50, 100, 100, 30));
            _quadtree.Add(new Note(200, 100, 100, 30));

            _quadtree.Add(new Note(400, 0, 100, 30));
            _quadtree.Add(new Note(600, 0, 100, 30));

            _quadtree.Add(new Note(400, 100, 100, 30));
            _quadtree.Add(new Note(600, 100, 100, 30));

            _quadtree.Add(new Note(50, 200, 100, 30));
            _quadtree.Add(new Note(200, 200, 100, 30));

            _quadtree.Add(new Note(50, 400, 100, 30));
            _quadtree.Add(new Note(200, 400, 100, 30));

            _quadtree.Add(new Note(400, 200, 100, 30));
            _quadtree.Add(new Note(600, 200, 100, 30));

            _quadtree.Add(new Note(400, 400, 100, 30));
            _quadtree.Add(new Note(600, 400, 100, 30));

            _quadtree.Add(new Note(800, -200, 100, 30));
            _quadtree.Add(new Note(1500, -1000, 100, 30));
        }

        public void UpdateInput(GameTime gameTime) {
            // TODO: Figure out the order that things need to be. Right now we need to invalidate the mouse cache multiple times.
            if (Triggers.PlayInteraction.Pressed()) {
                _play = !_play;

                if (!_play) {
                    Core.Midi.StopAll();
                }
            }
            if (Triggers.RotateLeft.Pressed()) {
                cameraRotate(MathHelper.PiOver4);
            }
            if (Triggers.RotateRight.Pressed()) {
                cameraRotate(-MathHelper.PiOver4);
            }
            if (InputHelper.NewMouse.ScrollWheelValue - InputHelper.OldMouse.ScrollWheelValue != 0) {
                cameraScale(InputHelper.NewMouse.ScrollWheelValue - InputHelper.OldMouse.ScrollWheelValue);
            }

            if (Triggers.CameraDrag.Pressed()) {
                _mouseAnchor = Core.MouseWorld;
            }
            if (Triggers.CameraDrag.Held()) {
                cameraTranslate(_mouseAnchor - Core.MouseWorld);
                _playheadNew = Core.Camera.X;
            } else if (_play) {
                cameraTranslate(new Vector2(300 * gameTime.GetElapsedSeconds(), 0));

                _playheadOld = _playheadNew;
                _playheadNew = Core.Camera.X;

                foreach (Note n in _quadtree.Query(_quadtree.Bounds)) {
                    if (_playheadOld < n.Start && _playheadNew > n.Start) {
                        Core.Midi.PlayNote(40);
                    }
                    if (_playheadOld < n.End && _playheadNew > n.End) {
                        Core.Midi.StopNote(40);
                    }
                }

                if (Core.Camera.X > 2000) {
                    Core.Camera.X = 0;
                }
            }

            if (_currentMode == Modes.selection) {
                if (Triggers.ToggleSelectAll.Pressed()) {
                    if (_selectedNotes.Count < _quadtree.ItemCount) {
                        _selectedNotes.UnionWith(_quadtree.Query(_quadtree.Bounds));
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
                        _selectedNotesTemp.UnionWith(_quadtree.Query(_selection));
                    } else if (Triggers.SelectionDragExclude.Held()) {
                        _selectedNotesTemp.UnionWith(_selectedNotes);
                        _selectedNotesTemp.ExceptWith(_quadtree.Query(_selection));
                    } else {
                        _selectedNotesTemp.UnionWith(_quadtree.Query(_selection));
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
                    _quadtree.Add(newNote);

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
                    n.XY += diff;
                    _quadtree.Update(n);
                }
                _grabAnchor = Core.MouseWorld;
            }

            if (Triggers.RemoveStart.Pressed()) {
                _pianoStart--;
                _pianoRoll = new PianoRoll(_pianoStart, _pianoEnd);
            }
            if (Triggers.AddStart.Pressed()) {
                _pianoStart++;
                _pianoRoll = new PianoRoll(_pianoStart, _pianoEnd);
            }
            if (Triggers.RemoveEnd.Pressed()) {
                _pianoEnd--;
                _pianoRoll = new PianoRoll(_pianoStart, _pianoEnd);
            }
            if (Triggers.AddEnd.Pressed()) {
                _pianoEnd++;
                _pianoRoll = new PianoRoll(_pianoStart, _pianoEnd);
            }
        }
        public void Update(GameTime gameTime) {

        }

        public void Draw(SpriteBatch s) {
            _pianoRoll.Draw(s);

            foreach (var n in _quadtree.Query(_quadtree.Bounds))
                n.Draw(s, Color.White);

            foreach (var n in _quadtree.Nodes)
                s.DrawRectangle(n, Color.White * 0.2f, 4);

            foreach (var b in _quadtree.Bundles)
                s.DrawLine(b.Item.AABB.Center.ToVector2(), b.Node.Center.ToVector2(), Color.White * .5f, 4);

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

            //s.DrawLine(_playheadNew, 3000, _playheadNew, -3000, Color.Green, 8);
        }

        private void cameraRotate(float dAngle) {
            Core.Camera.Angle += dAngle;

            Core.UpdateMouseWorld();
        }
        private void cameraScale(float dScale) {
            // TODO: The diff multiplier should be a setting.
            Core.Linear = MathF.Max(Core.Linear - dScale * 0.0005f, 0.1f);
            Core.Depth = Core.LinearToDepth(Core.Linear);
            Core.Camera.Scale = new Vector2(Core.DepthToZoom(Core.Depth, 0));

            Core.UpdateMouseWorld();
        }
        private void cameraTranslate(Vector2 dXY) {
            Core.Camera.XY += dXY;

            Core.UpdateMouseWorld();
        }
        private void cameraXY(Vector2 XY) {
            Core.Camera.XY = XY;

            Core.UpdateMouseWorld();
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

        float _playheadOld = 0;
        float _playheadNew = 0;

        int _pianoStart = 0;
        int _pianoEnd = 36;
        PianoRoll _pianoRoll = new PianoRoll(0, 36);

        Quadtree<Note> _quadtree = new Quadtree<Note>();
    }
}
