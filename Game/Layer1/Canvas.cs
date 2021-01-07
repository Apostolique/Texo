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
            _quadtree.Add(new Note(0, -1800, 100, 30));
            _quadtree.Add(new Note(0, -1890, 100, 30));
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
                Core.Camera.Angle += MathHelper.PiOver4;
            }
            if (Triggers.RotateRight.Pressed()) {
                Core.Camera.Angle -= MathHelper.PiOver4;
            }
            int scrollDelta = InputHelper.NewMouse.ScrollWheelValue - InputHelper.OldMouse.ScrollWheelValue;
            if (scrollDelta != 0) {
                // TODO: The diff multiplier should be a setting.
                Core.Zoom = MathF.Max(Core.Zoom - scrollDelta * 0.0005f, 0.1f);
            }

            Core.MouseWorld = Core.Camera.ScreenToWorld(InputHelper.NewMouse.X, InputHelper.NewMouse.Y);

            if (Triggers.CameraDrag.Pressed()) {
                _mouseAnchor = Core.MouseWorld;
                _isDragging = true;
            }
            if (_isDragging && Triggers.CameraDrag.HeldOnly()) {
                Core.Camera.XY += _mouseAnchor - Core.MouseWorld;
                Core.MouseWorld = _mouseAnchor;
                _playheadNew = Core.Camera.X;
            }
            if (_isDragging && Triggers.CameraDrag.Released()) {
                _isDragging = false;
            }

            if (!_isDragging && _play) {
                Core.Camera.XY += new Vector2(300 * gameTime.GetElapsedSeconds(), 0);
                Core.MouseWorld = Core.Camera.ScreenToWorld(InputHelper.NewMouse.X, InputHelper.NewMouse.Y);

                _playheadOld = _playheadNew;
                _playheadNew = Core.Camera.X;

                // TODO: Do a better search for notes. Can we queue more notes ahead of time?
                foreach (Note n in _quadtree.QueryRect(new RotRect(_playheadOld, _quadtree.Bounds.Top, _playheadNew - _playheadOld, _quadtree.Bounds.Height))) {
                    if (_playheadOld < n.Start && _playheadNew > n.Start) {
                        Core.Midi.PlayNote(n.Number);
                    }
                    if (_playheadOld < n.End && _playheadNew > n.End) {
                        Core.Midi.StopNote(n.Number);
                    }
                }

                if (Core.Camera.X > 2000) {
                    Core.Camera.X = 0;
                }
            }

            if (Triggers.ShrinkQuadtree.Pressed()) {
                _quadtree.Shrink();
            }

            if (_currentMode == Modes.selection) {
                if (Triggers.ToggleSelectAll.Pressed()) {
                    if (_selectedNotes.Count < _quadtree.Count()) {
                        _selectedNotes.UnionWith(_quadtree);
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
                        _selectedNotesTemp.UnionWith(querySelection());
                    } else if (Triggers.SelectionDragExclude.Held()) {
                        _selectedNotesTemp.UnionWith(_selectedNotes);
                        _selectedNotesTemp.ExceptWith(querySelection());
                    } else {
                        _selectedNotesTemp.UnionWith(querySelection());
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
                    _grabStart = mouseToGrid(Core.MouseWorld);

                    Note newNote = new Note((int)_grabStart.X, (int)_grabStart.Y, 100, Core.NoteHeight);
                    _quadtree.Add(newNote);

                    _selectedNotes.Clear();
                    _selectedNotes.Add(newNote);

                    _draggedNotes.Clear();
                    _draggedNotes.Add((newNote, Vector2.Zero));
                }
                if (!_isSelecting && Triggers.DeleteNote.Pressed()) {
                    foreach (Note n in _selectedNotes) {
                        _quadtree.Remove(n);
                    }
                    _selectedNotes.Clear();
                }
                if (!_isSelecting && Triggers.Grab.Pressed()) {
                    _currentMode = Modes.grab;
                    _grabStart = mouseToGrid(Core.MouseWorld);

                    _draggedNotes.Clear();
                    foreach (Note n in _selectedNotes) {
                        _draggedNotes.Add((n, n.XY - _grabStart));
                    }
                }
            } else if (_currentMode == Modes.grab) {
                Vector2 grab = mouseToGrid(Core.MouseWorld);

                if (Triggers.GrabCancel.Pressed()) {
                    _currentMode = Modes.selection;
                    grab = _grabStart;
                }
                if (Triggers.GrabConfirm.Pressed()) {
                    _currentMode = Modes.selection;
                }

                foreach (var n in _draggedNotes) {
                    Vector2 newPosition = grab + n.Offset;
                    n.Note.XY = newPosition;
                    _quadtree.Update(n.Note);
                }

                if (_currentMode != Modes.grab) {
                    _draggedNotes.Clear();
                }
            }
        }

        // Note: Maybe the quadtree should work with rectangles of size 0 and use their locations as a point?
        private IEnumerable<Note> querySelection() {
            if (_selection.Width == 0 || _selection.Height == 0) {
                return _quadtree.QueryPoint(_selection.Location);
            }
            return _quadtree.QueryRect(_selection);
        }

        private Vector2 mouseToGrid(Vector2 v) {
            //Centered because it's centered on the notes based on the note height.

            float x = v.X;
            // float x = (float)Math.Floor(v.X / Core.NoteWidth) * Core.NoteWidth;
            float y = (float)Math.Floor(v.Y / Core.NoteHeight) * Core.NoteHeight;

            return new Vector2(x, y);
        }

        public void Update() {
        }

        public void Draw(SpriteBatch s) {
            foreach (var n in _quadtree.QueryRect(Core.Camera.WorldBounds(), Core.Camera.Angle, Core.Camera.Origin))
                n.Draw(s, Color.White);

            // foreach (var n in _quadtree.Nodes)
            //     s.DrawRectangle(n, Color.White * 0.2f, 4);

            if (_isSelecting) {
                foreach (var n in _selectedNotesTemp)
                    s.DrawRectangle(n.Bounds.AABB, Color.Red * 0.8f, 2);
            } else {
                foreach (var n in _selectedNotes)
                    s.DrawRectangle(n.Bounds.AABB, Color.Red * 0.8f, 2);
            }

            if (_currentMode == Modes.selection && _selection.Width > 0 && _selection.Height > 0) {
                s.DrawRectangle(_selection, Color.Red, 4 / Core.Camera.ScreenToWorldScale());
            }

            if (_play) {
                s.DrawLine(_playheadNew, _quadtree.Bounds.Top, _playheadNew, _quadtree.Bounds.Bottom, Color.Green, 8);

                foreach (var n in _quadtree.QueryRect(new RotRect(_playheadOld, _quadtree.Bounds.Top, _playheadNew - _playheadOld, _quadtree.Bounds.Height))) {
                    n.Draw(s, Color.Red);
                }
            }
        }

        enum Modes {
            selection,
            grab,
        }

        Modes _currentMode = Modes.selection;
        bool _play = false;

        Vector2 _mouseAnchor = Vector2.Zero;
        bool _isDragging = false;

        Rectangle _selection = new Rectangle(0, 0, 0, 0);
        Vector2 _selectionStart = Vector2.Zero;
        Vector2 _selectionEnd = Vector2.Zero;
        bool _isSelecting = false;

        Vector2 _grabStart = Vector2.Zero;

        HashSet<Note> _selectedNotesTemp = new HashSet<Note>();
        HashSet<Note> _selectedNotes = new HashSet<Note>();
        List<(Note Note, Vector2 Offset)> _draggedNotes = new List<(Note, Vector2)>();

        float _playheadOld = 0;
        float _playheadNew = 0;

        Quadtree<Note> _quadtree = new Quadtree<Note>();
    }
}
