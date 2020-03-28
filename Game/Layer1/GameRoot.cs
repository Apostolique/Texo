using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Dcrew.MonoGame._2D_Camera;
using Apos.Input;

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
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;

            InputHelper.Setup(this);

            base.Initialize();
        }

        protected override void LoadContent() {
            _s = new SpriteBatch(GraphicsDevice);

            _camera = new Camera(new Vector2(0, 0), 0, Vector2.One);
            _canvas = new Canvas();
        }

        protected override void Update(GameTime gameTime) {
            InputHelper.UpdateSetup();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_playInteraction.Pressed()) {
                _play = !_play;
            }

            if (_play) {
                _camera.X += 300 * gameTime.GetElapsedSeconds();

                if (_camera.X > 1000) {
                    _camera.X = -1000;
                }
            }

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _s.Begin(transformMatrix: _camera.View);
            _canvas.Draw(_s);
            _s.End();

            base.Draw(gameTime);
        }

        GraphicsDeviceManager _graphics;
        SpriteBatch _s;
        Camera _camera;
        Canvas _canvas;
        bool _play = true;

        ConditionComposite _playInteraction =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.Space)),
                new ConditionSet(new ConditionGamePad(GamePadButton.A, 0))
            );
    }
}