using System;
using System.Collections.Generic;
using ToolShed.UITKTools.BasicControls;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public class Grid
    {
        public enum Location
        {
            Center,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left
        }
        
        public Vector2Int numTiles { get; private set; }
        public Vector2 tileSize { get; private set; }

        public List<VisualElement> columns = new List<VisualElement>();
        public List<VisualElement> tiles = new List<VisualElement>();

        private Action<VisualElement, Location> OnTileCreate;

        public Grid(VisualElement baseElement, Vector2Int numTiles, Action<VisualElement, Location> OnTileCreate = null)
        {
            this.numTiles = numTiles;
            baseElement.RegisterCallback<GeometryChangedEvent>((eve) =>
            {
                updateScale(eve, baseElement);
            });

            for (int i = 0; i < numTiles.y; i++)
            {
                VisualElement column = new VisualElement();
                column.style.flexDirection = FlexDirection.Row;

                columns.Add(column);
                baseElement.Add(column);

                for (int j = 0; j < numTiles.x; j++)
                {
                    VisualElement tile = new VisualElement();

                    tiles.Add(tile);
                    column.Add(tile);

                    Location location = Location.Center;
                    if (i == 0 && j == 0) location = Location.TopLeft;
                    else if (i == 0 && j != 0 && j != numTiles.x - 1) location = Location.Top;
                    else if (i == 0 && j == numTiles.x - 1) location = Location.TopRight;
                    else if (i > 0 && i < numTiles.y - 1 && j == numTiles.x - 1) location = Location.Right;
                    else if (i == numTiles.y - 1 && j == numTiles.x - 1) location = Location.BottomRight;
                    else if (i == numTiles.y - 1 && j > 0 && j < numTiles.x) location = Location.Bottom;
                    else if (i == numTiles.y - 1 && j == 0) location = Location.BottomLeft;
                    else if (i > 0 && i < numTiles.y - 1 && j == 0) location = Location.Left;

                    OnTileCreate?.Invoke(tile, location);
                }
            }
            
            Debug.Log(tileSize);
        }

        public void updateScale(GeometryChangedEvent eve, VisualElement baseElement)
        {
            this.tileSize = new Vector2(eve.newRect.width / numTiles.x, eve.newRect.width / numTiles.x);

            foreach (var column in columns)
            {
                column.style.width = tileSize.x * numTiles.x;
                column.style.height = tileSize.y;
            }

            foreach (var tile in tiles)
            {
                tile.style.width = tileSize.x;
                tile.style.height = tileSize.y;
            }
        }
    }
}