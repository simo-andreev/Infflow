using System;
using System.Numerics;
using Emotion.Primitives;

namespace Infflow
{
    public class Model
    {
        public sealed class Tile : Transform
        {
            public static readonly Vector2 TileSize = new Vector2(32, 32);

            public bool _hasStation = false;

            public Vector2 Vertex0;
            public Vector2 Vertex1;
            public Vector2 Vertex2;
            public Vector2 Vertex3;

            private float _influence;

            public float Influence
            {
                get => _influence;
                set
                {
                    if (value <= Constants.EVAPORATION_LEVEL)
                    {
                        value = 0;
                        owner = null;
                    }
                    else
                    {
                        _influence = value;
                    }
                }
            }

            public Player? owner;

            public Tile(Vector2 position, Player? owner = null, float influence = 0f)
            {
                Vertex0 = new Vector2(position.X, position.Y);
                Vertex1 = new Vector2(position.X + TileSize.X, position.Y);
                Vertex2 = new Vector2(position.X + TileSize.X, position.Y + TileSize.Y);
                Vertex3 = new Vector2(position.X, position.Y + TileSize.Y);

                this.owner = owner;
                this.Influence = influence;

                Bounds = Rectangle.BoundsFromPolygonPoints(new[] {Vertex0, Vertex1, Vertex2, Vertex3});
            }
        }

        public enum Player : uint
        {
            Player1 = Constants.COLOR_PINK,
            Player2 = Constants.COLOR_CORN
        }
    }
}