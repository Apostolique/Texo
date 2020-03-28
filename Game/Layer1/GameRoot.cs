using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Dcrew.MonoGame._2D_Camera;
using Apos.Input;
using System;

namespace GameProject {
    public class GameRoot : Game {
        public GameRoot() {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;
            _graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize() {
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowSizeChanged;

            InputHelper.Setup(this);

            base.Initialize();
        }

        private void WindowSizeChanged(object sender, EventArgs e) {
            _grid.Parameters["ViewportSize"].SetValue(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));
        }

        protected override void LoadContent() {
            _s = new SpriteBatch(GraphicsDevice);

            _camera = new Camera(new Vector2(0, 0), 0, new Vector2(_depth));
            _canvas = new Canvas();

            _pixel = Content.Load<Texture2D>("Pixel");

            _grid = Content.Load<Effect>("Grid");
            _grid.Parameters["BackgroundColor"].SetValue(new Color(10, 10, 10).ToVector4());
            _grid.Parameters["GridColor"].SetValue(new Color(30, 30, 30).ToVector4());
            _grid.Parameters["GridSize"].SetValue(new Vector2(200, 200));
            _grid.Parameters["LineSize"].SetValue(new Vector2(4, 4));
        }

        protected override void Update(GameTime gameTime) {
            InputHelper.UpdateSetup();

            if (_quit.Pressed())
                Exit();

            if (_playInteraction.Pressed()) {
                _play = !_play;
            }
            if (_rotateLeft.Pressed()) {
                _camera.Angle += MathHelper.PiOver4;
            }
            if (_rotateRight.Pressed()) {
                _camera.Angle -= MathHelper.PiOver4;
            }
            if (InputHelper.NewMouse.ScrollWheelValue - InputHelper.OldMouse.ScrollWheelValue != 0) {
                var diff = InputHelper.NewMouse.ScrollWheelValue - InputHelper.OldMouse.ScrollWheelValue;
                // TODO: The diff multiplier should be a setting.
                _linear = Math.Max(_linear - diff * 0.0005f, 0.1f);
                _depth = LinearToDepth(_linear);
                _camera.Scale = new Vector2(DepthToZoom(_depth, 0));
            }

            _camera.UpdateMousePos(InputHelper.NewMouse);
            _mouseWorld = _camera.MousePos;

            if (_drag.Pressed()) {
                _mouseAnchor = _mouseWorld;
            }
            if (_drag.Held()) {
                _camera.Pos += _mouseAnchor - _mouseWorld;
            } else if (_play) {
                _camera.X += 300 * gameTime.GetElapsedSeconds();
            }

            float scale = 1f;
            Vector2 size = new Vector2(_pixel.Width, _pixel.Height);
            Vector2 posOffset = new Vector2(0, 0);

            Matrix m =
                Matrix.CreateScale(scale) *
                Matrix.CreateScale(size.X, size.Y, 1) *
                Matrix.CreateTranslation(posOffset.X, posOffset.Y, 1) *
                _camera.View *
                Matrix.CreateScale(1f / size.X, 1f / size.Y, 1);

            _grid.Parameters["ScrollMatrix"].SetValue(Matrix.Invert(m));
            _grid.Parameters["ViewportSize"].SetValue(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _s.Begin(samplerState: SamplerState.LinearWrap, effect: _grid);
            _s.Draw(_pixel, Vector2.Zero, _s.GraphicsDevice.Viewport.Bounds, Color.Red);
            _s.End();

            _s.Begin(transformMatrix: _camera.View);
            _canvas.Draw(_s);
            _s.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Outputs a Z dolly value.
        /// Let's say you want a layer with a depth of 2 to be drawn at a scale of 0.5.
        /// You'd call this function with 0.5 as the zoom parameter and 2 as the target depth parameter:
        ///
        /// GetDepthFromZoom(0.5, 2);
        ///
        /// The result would be 4. In other words, you'd have to set the camera's depth to 4.
        /// <seealso cref="DepthToZoom(float, float)"/>
        /// </summary>
        private float ZoomToDepth(float zoom, float targetDepth) {
            return 1 / zoom + targetDepth;
        }
        /// <summary>
        /// Outputs a zoom value.
        /// This is the sister function to GetDepthFromZoom.
        /// Finds a layer's zoom value relative to an other layer.
        /// <seealso cref="ZoomToDepth(float, float)"/>
        /// </summary>
        private float DepthToZoom(float depth, float targetDepth) {
            if (depth - targetDepth == 0) {
                return 0;
            }
            return 1 / (depth - targetDepth);
        }
        private float LinearToDepth(float linear) {
            return linear * linear;
        }
        private float DepthToLinear(float depth) {
            return (float)Math.Sqrt(depth);
        }

        GraphicsDeviceManager _graphics;
        SpriteBatch _s;
        Texture2D _pixel;
        Effect _grid;
        Camera _camera;
        Canvas _canvas;
        bool _play = false;

        float _depth = 1;
        float _linear = 1;

        Vector2 _mouseWorld = Vector2.Zero;
        Vector2 _mouseAnchor = Vector2.Zero;

        ConditionComposite _playInteraction =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.Space)),
                new ConditionSet(new ConditionGamePad(GamePadButton.A, 0))
            );

        ConditionComposite _rotateLeft =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.OemComma))
            );
        ConditionComposite _rotateRight =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.OemPeriod))
            );

        ConditionComposite _drag =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.LeftButton))
            );

        ConditionComposite _quit =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.Escape)),
                new ConditionSet(new ConditionGamePad(GamePadButton.Back, 0))
            );
    }
}