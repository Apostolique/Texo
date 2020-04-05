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
                new ConditionSet(new ConditionMouse(MouseButton.RightButton))
            );

        public static ConditionComposite SelectionDrag =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.LeftButton))
            );

        public static ConditionComposite ModeSwitch =
            new ConditionComposite(
                new ConditionSet(new ConditionMouse(MouseButton.MiddleButton))
            );
    }
}