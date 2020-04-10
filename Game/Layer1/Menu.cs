using Apos.Input;
using Apos.Gui;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    public class Menu {
        public Menu() {
            _grabFocus = c => {
                _menuFocus.Focus = c;
            };

            MenuPanel mp = new MenuPanel();
            mp.Layout = new LayoutVerticalCenter();
            mp.Add(deviceSelectMenu());

            _menuFocus = new ComponentFocus(mp, Default.ConditionPreviousFocus, Default.ConditionNextFocus);
        }

        ComponentFocus _menuFocus;
        Action<Component> _grabFocus;

        public void UpdateSetup() {
            _menuFocus.UpdateSetup();
        }

        public void Update() {
            _menuFocus.Update();
        }

        public void UpdateInput() {
            _menuFocus.UpdateInput();
        }

        public void DrawUI() {
            _menuFocus.Draw();
        }

        private Component deviceSelectMenu() {
            Panel p = new Panel();
            p.Layout = new LayoutVerticalCenter();
            p.AddHoverCondition(Default.ConditionHoverMouse);
            p.AddAction(Default.IsScrolled, Default.ScrollVertically);

            p.Add(Default.CreateButton(
                "Resume",
                c => {
                    Core.ShowMenu = !Core.ShowMenu;
                    return true;
                },
                _grabFocus));

            p.Add(createDynamicText(() => $"Current device: {Core.Midi.Device.name}"));

            p.Add(createTitle("Choose a midi device:"));

            foreach (var device in Midi.Devices) {
                p.Add(Default.CreateButton(
                    device.name,
                    c => {
                        Core.Midi.Dispose();
                        Core.Midi = new Midi(device.index);
                        return true;
                    },
                    _grabFocus));
            }

            p.Add(Default.CreateButton(
                "Quit",
                c => {
                    Core.Game.Exit();
                    return true;
                },
                _grabFocus));

            return p;
        }

        private Component createTitle(string title) {
            Label l = new Label(title);
            Border b = new Border(l, 20, 20, 20, 50);

            return b;
        }
        private Component createDynamicText(Func<string> ld) {
            LabelDynamic l = new LabelDynamic(ld);
            Border b = new Border(l, 20, 20, 20, 20);

            return b;
        }

        private class MenuPanel : ScreenPanel {
            public override void Draw() {
                SetScissor();
                _s.FillRectangle(BoundingRect, Color.Black * 0.6f);

                _s.DrawLine(Left, Top, Right, Top, Color.Black, 2);
                _s.DrawLine(Right, Top, Right, Bottom, Color.Black, 2);
                _s.DrawLine(Left, Bottom, Right, Bottom, Color.Black, 2);
                _s.DrawLine(Left, Top, Left, Bottom, Color.Black, 2);

                base.Draw();
                ResetScissor();
            }
        }
    }
}