using System;
using Microsoft.Xna.Framework;

namespace GameProject {
    public static class Utility {
        public static Rectangle CreateRect(Vector2 start, Vector2 end) {
            Vector2 topLeft = new Vector2(MathF.Min(start.X, end.X), MathF.Min(start.Y, end.Y));
            Vector2 bottomRight = new Vector2(MathF.Max(start.X, end.X), MathF.Max(start.Y, end.Y));
            return new Rectangle(topLeft.ToPoint(), (bottomRight - topLeft).ToPoint());
        }

        public static T Clamp<T>(this T val, T min, T max)where T : IComparable<T> {
            if (val.CompareTo(min) < 0)return min;
            else if (val.CompareTo(max) > 0)return max;
            else return val;
        }

        public static int Mod(int x, int m) {
            if (m == 0) {
                return x;
            }
            return (x % m + m) % m;
        }
    }
}