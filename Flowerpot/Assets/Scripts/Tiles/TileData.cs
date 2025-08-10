using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/Custom Tile")]
public class TileData : Tile
{
    public TileType tileType;
    public TileColor tileColor;

    [Header("Animation Settings")]
    public bool isAnimated = false;
    public Sprite[] animationFrames;
    public float animationSpeed = 2.5f;

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

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        if (isAnimated && animationFrames != null && animationFrames.Length > 0)
        {
            tileAnimationData.animatedSprites = animationFrames;
            tileAnimationData.animationSpeed = animationSpeed;
            tileAnimationData.animationStartTime = 0f;
            return true;
        }

        return false;
    }


}
