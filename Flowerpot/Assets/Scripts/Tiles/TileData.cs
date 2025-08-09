using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/Custom Tile")]
public class TileData : Tile
{
    public TileType tileType;
    public TileColor tileColor;

    public enum TileColor {
        None,
        Red,
        Blue,
        Purple,
        Yellow,
    }

    public enum TileType
    {
        Normal,
        ColorChange,
        ColorWall,
        Goal
    }

}
