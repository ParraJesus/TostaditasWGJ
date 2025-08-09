using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileData;

public class PlayerTileInteraction : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundTilemap;
    [SerializeField]
    private Tilemap collisionTilemap;
    [SerializeField]
    private Tilemap goalTilemap;
    [SerializeField]
    private SpriteRenderer playerSprite;
    [SerializeField]
    private List<Sprite> colorSprites;
    [SerializeField]
    private TileColor currentColor = TileColor.None;

    private Dictionary<Vector3Int, TileData> wallTiles = new Dictionary<Vector3Int, TileData>();
    private Dictionary<Vector3Int, TileData> GoalTiles = new Dictionary<Vector3Int, TileData>();

    private void Start()
    {
        BoundsInt bounds = collisionTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            TileData tile = collisionTilemap.GetTile<TileData>(pos);
            if (tile != null && tile.tileType == TileType.ColorWall)
            {
                        wallTiles[pos] = tile;         
            }
        }

        BoundsInt goalBounds = goalTilemap.cellBounds;
        foreach (var pos in goalBounds.allPositionsWithin)
        {
            TileData tile = goalTilemap.GetTile<TileData>(pos);
            if (tile != null && tile.tileType == TileType.Goal)
            {
                GoalTiles[pos] = tile;
            }
        }

        UpdateAllWalls();
        UpdateAllGoals();
    }

    private void Update()
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);
        TileData floorTile = groundTilemap.GetTile<TileData>(cellPos);

        if (floorTile != null && floorTile.tileType == TileType.ColorChange && floorTile.tileColor != currentColor)
        {
            currentColor = floorTile.tileColor;

            int index = (int)currentColor;
            if (index >= 0 && index < colorSprites.Count && colorSprites[index] != null)
            {
                playerSprite.sprite = colorSprites[index];
                Debug.Log("Se ha cambiado de color a " + currentColor);
            }

            UpdateAllWalls();
            UpdateAllGoals();
        }
    }

    private void UpdateAllWalls()
    {
        foreach (var kvp in wallTiles)
        {
            Vector3Int pos = kvp.Key;
            TileData tile = kvp.Value;

            bool visible = tile.tileColor != currentColor;
            UpdateTileVisibility(pos, visible);
        }
    }

    private void UpdateAllGoals()
    {
        foreach (var kvp in GoalTiles)
        {
            Vector3Int pos = kvp.Key;
            TileData tile = kvp.Value;

            bool visible = (tile.tileColor == TileColor.None) || (tile.tileColor == currentColor);
            UpdateGoalVisibility(pos, visible);
        }
    }

    private void UpdateGoalVisibility(Vector3Int pos, bool visible)
    {
        if (visible)
        {
            goalTilemap.SetTile(pos, GoalTiles[pos]);
        }
        else
        {
            goalTilemap.SetTile(pos, null);
        }
    }

    private void UpdateTileVisibility(Vector3Int pos, bool visible)
    {
        if (visible)
        {
            collisionTilemap.SetTile(pos, wallTiles[pos]);
        }
        else
        {
            collisionTilemap.SetTile(pos, null);
        }

    }

}
