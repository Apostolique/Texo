using System;
using Apos.Input;
using Dcrew.MonoGame._2D_Camera;
using Microsoft.Xna.Framework;

namespace GameProject {
    public static class Core {
        public static GameWindow Window;
        public static Camera Camera;
        public static Vector2 MouseWorld = Vector2.Zero;

        public static float Depth = 1;
        public static float Linear = 1;

        public static void Setup(Game game) {
            Window = game.Window;

            Camera = new Camera(new Vector2(0, 0), 0, new Vector2(Depth));
        }
        public static void Update() {
            MouseWorld = Camera.ScreenToWorld(InputHelper.NewMouse.X, InputHelper.NewMouse.Y);
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
        public static float ZoomToDepth(float zoom, float targetDepth) {
            return 1 / zoom + targetDepth;
        }
        /// <summary>
        /// Outputs a zoom value.
        /// This is the sister function to GetDepthFromZoom.
        /// Finds a layer's zoom value relative to an other layer.
        /// <seealso cref="ZoomToDepth(float, float)"/>
        /// </summary>
        public static float DepthToZoom(float depth, float targetDepth) {
            if (depth - targetDepth == 0) {
                return 0;
            }
            return 1 / (depth - targetDepth);
        }
        public static float LinearToDepth(float linear) {
            return linear * linear;
        }
        public static float DepthToLinear(float depth) {
            return (float)Math.Sqrt(depth);
        }
    }
}