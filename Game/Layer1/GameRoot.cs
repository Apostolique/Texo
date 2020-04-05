﻿using Microsoft.Xna.Framework;
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
            _graphics.PreferredBackBufferWidth = 1700;
            _graphics.PreferredBackBufferHeight = 900;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            IsFixedTimeStep = true;
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

            Core.Setup(this);
            _canvas = new Canvas();

            _pixel = Content.Load<Texture2D>("Pixel");

            _grid = Content.Load<Effect>("Grid");
            _grid.Parameters["BackgroundColor"].SetValue(new Color(10, 10, 10).ToVector4());
            _grid.Parameters["GridColor"].SetValue(new Color(30, 30, 30).ToVector4());
            _grid.Parameters["GridSize"].SetValue(new Vector2(200, 200));
            _grid.Parameters["LineSize"].SetValue(new Vector2(4, 4));

            Core.Midi = new Midi();
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            Core.Midi.Dispose();
        }

        protected override void Update(GameTime gameTime) {
            InputHelper.UpdateSetup();

            if (_quit.Pressed())
                Exit();

            if (Triggers.DoNote.Pressed()) {
                Core.Midi.PlayNote(50);
            }
            if (Triggers.DoNote.Released()) {
                Core.Midi.StopNote(50);
            }

            Core.Update();
            _canvas.Update(gameTime);

            float scale = 1f;
            Vector2 size = new Vector2(_pixel.Width, _pixel.Height);
            Vector2 posOffset = new Vector2(0, 0);

            Matrix m =
                Matrix.CreateScale(scale) *
                Matrix.CreateScale(size.X, size.Y, 1) *
                Matrix.CreateTranslation(posOffset.X, posOffset.Y, 1) *
                Core.Camera.View *
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

            _s.Begin(transformMatrix: Core.Camera.View);
            _canvas.Draw(_s);
            _s.End();

            base.Draw(gameTime);
        }

        GraphicsDeviceManager _graphics;
        SpriteBatch _s;
        Texture2D _pixel;
        Effect _grid;
        Canvas _canvas;

        ConditionComposite _quit =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.Escape)),
                new ConditionSet(new ConditionGamePad(GamePadButton.Back, 0))
            );
    }
}