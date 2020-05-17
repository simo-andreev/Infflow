using System;
using System.Collections.Generic;
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

            public Dictionary<Player, float> Influence = new Dictionary<Player, float>(2)
            {
                {Player.Player1, 0f},
                {Player.Player2, 0f}
            };

            public Tile(Vector2 position, Player? owner = null)
            {
                Vertex0 = new Vector2(position.X, position.Y);
                Vertex1 = new Vector2(position.X + TileSize.X, position.Y);
                Vertex2 = new Vector2(position.X + TileSize.X, position.Y + TileSize.Y);
                Vertex3 = new Vector2(position.X, position.Y + TileSize.Y);

                Bounds = Rectangle.BoundsFromPolygonPoints(new[] {Vertex0, Vertex1, Vertex2, Vertex3});
            }

            public Player? GetOwner()
            {
                Player? owner = null;
                float highest = 0;
                foreach (var (player, influence) in Influence)
                    if (highest < influence)
                        owner = player;

                return owner;
            }
        }

        public enum Player : uint
        {
            Player1 = Constants.COLOR_PINK,
            Player2 = Constants.COLOR_CORN
        }
    }
}