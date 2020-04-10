using Apos.Input;
using Microsoft.Xna.Framework.Input;

namespace GameProject {
    public static class Triggers {
        public static ConditionComposite PlayInteraction =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.Space)),
                new ConditionSet(new ConditionGamePad(GamePadButton.A, 0))
            );

        public static ConditionComposite RotateLeft =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.OemComma))
            );
        public static ConditionComposite RotateRight =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.OemPeriod))
            );

        public static ConditionComposite CameraDrag =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.MiddleButton))
            );

        public static ConditionComposite ToggleSelectAll =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.A))
            );
        public static ConditionComposite SelectionDrag =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.LeftButton))
            );
        public static ConditionComposite SelectionDragAdd =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.LeftButton), new ConditionKeyboard(Keys.LeftShift))
            );
        public static ConditionComposite SelectionDragExclude =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.LeftButton), new ConditionKeyboard(Keys.LeftControl))
            );

        public static ConditionComposite ModeSwitch =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.MiddleButton))
            );

        public static ConditionComposite Grab =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.G))
            );
        public static ConditionComposite GrabConfirm =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.LeftButton))
            );
        public static ConditionComposite GrabCancel =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.RightButton))
            );

        public static ConditionComposite CreateNote =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.A), new ConditionKeyboard(Keys.LeftShift))
            );

        public static ConditionComposite DoNote =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.Enter))
            );

        public static ConditionComposite RemoveStart =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.F5))
            );
        public static ConditionComposite AddStart =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.F6))
            );

        public static ConditionComposite RemoveEnd =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.F7))
            );
        public static ConditionComposite AddEnd =
            new ConditionComposite(
                new ConditionSet(new ConditionKeyboard(Keys.F8))
            );
    }
}