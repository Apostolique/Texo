using System;
using Microsoft.Xna.Framework;

namespace GameProject {
    public static class Utility {
        public static Rectangle CreateRect(Vector2 start, Vector2 end) {
            Vector2 topLeft = new Vector2(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Vector2 bottomRight = new Vector2(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
            return new Rectangle(topLeft.ToPoint(), (bottomRight - topLeft).ToPoint());
        }

        public static T Clamp<T>(this T val, T min, T max)where T : IComparable<T> {
            if (val.CompareTo(min) < 0)return min;
            else if (val.CompareTo(max) > 0)return max;
            else return val;
        }
    }
}