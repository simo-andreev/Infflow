using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Emotion.Common;
using Emotion.Game.QuadTree;
using Emotion.Graphics;
using Emotion.Graphics.Objects;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Utility;

namespace Infflow
{
    public class TestScene : IScene
    {
        private readonly Random _random = new Random();

        private const int MAP_WIDTH = 64; // X axis tile count
        private const int MAP_HEIGHT = 64; // Y axis tile count

        private Model.Tile[,] tiles = new Model.Tile[MAP_WIDTH, MAP_HEIGHT];
        private QuadTree<Model.Tile> quadTree;

        private List<Model.Tile> _drawMemory = new List<Model.Tile>(64);
        private FrameBuffer _frameBuffer;

        private byte _lastMouseX = 0;
        private byte _lastMouseY = 0;
        private Model.Tile _lastMouseOver = null;

        public void Load()
        {
            GenerateTileMap();

            Rectangle boundsOfMap = Rectangle.BoundsFromPolygonPoints(new[]
            {
                tiles[0, 0].Vertex0,
                tiles[MAP_WIDTH - 1, 0].Vertex1,
                tiles[MAP_WIDTH - 1, MAP_HEIGHT - 1].Vertex2,
                tiles[0, MAP_HEIGHT - 1].Vertex3,
            });

            quadTree = new QuadTree<Model.Tile>(boundsOfMap, 100);
            foreach (var tile in tiles) quadTree.Add(tile);
        }

        public void Update()
        {
            /* do noting */
        }

        public void Draw(RenderComposer composer)
        {
            RenderMap(composer);
        }

        public void Unload()
        {
            /* do noting */
        }


        private void GenerateTileMap()
        {
            for (var x = 0; x < MAP_WIDTH; x++)
            for (var y = 0; y < MAP_HEIGHT; y++)
            {
                tiles[x, y] = new Model.Tile(new Vector2(x, y) * Model.Tile.TileSize);
                switch (_random.Next(5))
                {
                    case 1:
                        tiles[x, y].owner = Model.Player.Player1;
                        break;
                    case 2:
                        tiles[x, y].owner = Model.Player.Player2;
                        break;
                }

                tiles[x, y].Influence = (float) (_random.NextDouble() * 255.0f);
            }
        }

        private void RenderMap(RenderComposer composer)
        {
            _drawMemory.Clear();
            var rect = new Rectangle(
                Engine.Renderer.Camera.ScreenToWorld(Vector2.Zero),
                Engine.Configuration.RenderSize * (Engine.Renderer.Scale - (Engine.Renderer.IntScale - 1)) / Engine.Renderer.Camera.Zoom
            );

            quadTree.GetObjects(rect, ref _drawMemory);

            for (var i = 0; i < _drawMemory.Count; i++)
            {
                var tile = _drawMemory[i];


                uint tileColor;
                if (tile.owner == null)
                {
                    tileColor = Color.White.ToUint();
                }
                else
                {
                    var ownerColor = new Color((uint) tile.owner);
                    var baseTileColor = new Color(ownerColor, (byte) tile.Influence);
                    
                    tileColor = Color.White.Overlay(baseTileColor).ToUint();
                }

                var a = composer.GetBatch();
                var data = a.GetData(null);
                data[0].Vertex = tile.Vertex0.ToVec3();
                data[0].Color = tileColor;
                data[0].UV = Vector2.Zero;

                data[1].Vertex = tile.Vertex1.ToVec3();
                data[1].Color = tileColor;
                data[1].UV = new Vector2(1, 0);

                data[2].Vertex = tile.Vertex2.ToVec3();
                data[2].Color = tileColor;
                data[2].UV = new Vector2(1, 1);

                data[3].Vertex = tile.Vertex3.ToVec3();
                data[3].Color = tileColor;
                data[3].UV = new Vector2(0, 1);
            }
        }
    }
}