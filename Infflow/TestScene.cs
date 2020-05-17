using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Emotion.Common;
using Emotion.Game.QuadTree;
using Emotion.Graphics;
using Emotion.Graphics.Data;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Tools.Windows;
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

        private Vector2 towerSize = Model.Tile.TileSize;
        private TextureAsset towerTextureAsset;

        public void Load()
        {
            towerTextureAsset = Engine.AssetLoader.Get<TextureAsset>("image/tower.png");

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

        }

        public void Draw(RenderComposer composer)
        {
            RenderMap(composer);
            composer.RenderToolsMenu();
        }

        public void Unload()
        {
            Engine.AssetLoader.Destroy(towerTextureAsset.Name);
        }

        private void RenderMap(RenderComposer composer)
        {
            _drawMemory.Clear();
            var rect = new Rectangle(
                Engine.Renderer.Camera.ScreenToWorld(Vector2.Zero),
                Engine.Configuration.RenderSize * (Engine.Renderer.Scale - (Engine.Renderer.IntScale - 1)) / Engine.Renderer.Camera.Zoom
            );

            quadTree.GetObjects(rect, ref _drawMemory);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _drawMemory.Count; i++)
            {
                var tile = _drawMemory[i];
                var span = composer.GetBatch().GetData(null);
                SetTileRenderData(tile, span);
                if (tile._hasStation)
                    composer.RenderSprite(tile.Position, towerSize, towerTextureAsset.Texture); // render tower
            }
        }

        private void SetTileRenderData(Model.Tile tile, in Span<VertexData> span)
        {
            uint tileColor;
            var owner = tile.GetOwner();
            if (owner == null)
            {
                tileColor = Color.White.ToUint();
            }
            else
            {
                var ownerColor = new Color((uint) owner);
                var baseTileColor = new Color(ownerColor, (byte) tile.Influence[(Model.Player) owner]);

                tileColor = Color.White.Overlay(baseTileColor).ToUint();
            }

            span[0].Vertex = tile.Vertex0.ToVec3();
            span[0].Color = tileColor;
            span[0].UV = Vector2.Zero;

            span[1].Vertex = tile.Vertex1.ToVec3();
            span[1].Color = tileColor;
            span[1].UV = new Vector2(1, 0);

            span[2].Vertex = tile.Vertex2.ToVec3();
            span[2].Color = tileColor;
            span[2].UV = new Vector2(1, 1);

            span[3].Vertex = tile.Vertex3.ToVec3();
            span[3].Color = tileColor;
            span[3].UV = new Vector2(0, 1);
        }


        private void GenerateTileMap()
        {
            for (var x = 0; x < MAP_WIDTH; x++)
            for (var y = 0; y < MAP_HEIGHT; y++)
            {
                var tile = new Model.Tile(new Vector2(x, y) * Model.Tile.TileSize);
                switch (_random.Next(20))
                {
                    case 1:
                        if (_random.Next(100) > 50)
                            tile._hasStation = true;
                        tile.Influence[Model.Player.Player1] = 100f;
                        break;

                    case 2:
                        tile.Influence[Model.Player.Player2] = 100f;
                        if (_random.Next(100) > 50)
                            tile._hasStation = true;
                        break;
                }

                tiles[x, y] = tile;
            }
        }
    }
}