using Dcrew.MonoGame._2D_Spatial_Partition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    public class Canvas {
        public Canvas() {
            Quadtree<Note>.Add(new Note(0, 0, 100, 30));
            Quadtree<Note>.Add(new Note(200, 0, 100, 30));

            Quadtree<Note>.Add(new Note(0, 100, 100, 30));
            Quadtree<Note>.Add(new Note(200, 100, 100, 30));

            Quadtree<Note>.Add(new Note(400, 0, 100, 30));
            Quadtree<Note>.Add(new Note(600, 0, 100, 30));

            Quadtree<Note>.Add(new Note(400, 100, 100, 30));
            Quadtree<Note>.Add(new Note(600, 100, 100, 30));

            Quadtree<Note>.Add(new Note(0, 200, 100, 30));
            Quadtree<Note>.Add(new Note(200, 200, 100, 30));

            Quadtree<Note>.Add(new Note(0, 400, 100, 30));
            Quadtree<Note>.Add(new Note(200, 400, 100, 30));

            Quadtree<Note>.Add(new Note(400, 200, 100, 30));
            Quadtree<Note>.Add(new Note(600, 200, 100, 30));

            Quadtree<Note>.Add(new Note(400, 400, 100, 30));
            Quadtree<Note>.Add(new Note(600, 400, 100, 30));

            Quadtree<Note>.Add(new Note(800, -200, 100, 30));
            Quadtree<Note>.Add(new Note(1500, -1000, 100, 30));
        }

        public void Draw(SpriteBatch s) {
            foreach (var n in Quadtree<Note>.Items) {
                n.Item.Draw(s);
            }

            foreach (var n in Quadtree<Note>.Nodes)
                s.DrawRectangle(n, Color.White * 0.2f, 4);

            foreach (var e in Quadtree<Note>.Items)
                s.DrawLine(e.Item.AABB.Center.ToVector2(), e.Node.Center.ToVector2(), Color.White * .5f, 4);
        }
    }
}