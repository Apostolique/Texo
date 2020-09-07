using Apos.Input;
using Microsoft.Xna.Framework.Input;

namespace GameProject {
    public static class Triggers {
        public static ICondition PlayInteraction =
            new AnyCondition(
                new KeyboardCondition(Keys.Space),
                new GamePadCondition(GamePadButton.A, 0)
            );

        public static ICondition RotateLeft = new KeyboardCondition(Keys.OemComma);
        public static ICondition RotateRight = new KeyboardCondition(Keys.OemPeriod);

        public static ICondition CameraDrag = new MouseCondition(MouseButton.MiddleButton);

        public static ICondition ToggleSelectAll = new KeyboardCondition(Keys.A);
        public static ICondition SelectionDrag = new MouseCondition(MouseButton.LeftButton);
        public static ICondition SelectionDragAdd =
            new AllCondition(
                new MouseCondition(MouseButton.LeftButton),
                new KeyboardCondition(Keys.LeftShift)
            );
        public static ICondition SelectionDragExclude =
            new AllCondition(
                new MouseCondition(MouseButton.LeftButton),
                new KeyboardCondition(Keys.LeftControl)
            );

        public static ICondition ModeSwitch = new MouseCondition(MouseButton.MiddleButton);

        public static ICondition Grab = new KeyboardCondition(Keys.G);
        public static ICondition GrabConfirm = new MouseCondition(MouseButton.LeftButton);
        public static ICondition GrabCancel = new MouseCondition(MouseButton.RightButton);

        public static ICondition CreateNote =
            new AllCondition(
                new KeyboardCondition(Keys.A),
                new KeyboardCondition(Keys.LeftShift)
            );
        public static ICondition DeleteNote =
            new AnyCondition(
                new KeyboardCondition(Keys.Delete),
                new KeyboardCondition(Keys.Back),
                new KeyboardCondition(Keys.X)
            );

        public static ICondition DoNote = new KeyboardCondition(Keys.Enter);

        public static ICondition ShrinkQuadtree = new KeyboardCondition(Keys.F5);

        public static ICondition RemoveEnd = new KeyboardCondition(Keys.F7);
        public static ICondition AddEnd = new KeyboardCondition(Keys.F8);
    }
}
