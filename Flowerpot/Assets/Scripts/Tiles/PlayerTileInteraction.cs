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
    
    // Referencia al sistema de sonidos
    private PlayerSounds playerSounds;

    private void Start()
    {
        // Buscar PlayerSounds en el mismo GameObject
        playerSounds = GetComponent<PlayerSounds>();
        
        // Si no está en el mismo GameObject, buscar en toda la escena
        if (playerSounds == null)
        {
            playerSounds = FindObjectOfType<PlayerSounds>();
            if (playerSounds != null)
            {
                Debug.LogWarning($"PlayerTileInteraction: PlayerSounds encontrado en {playerSounds.gameObject.name} (no en el mismo GameObject)");
            }
        }
        
        if (playerSounds == null)
        {
            Debug.LogError("PlayerTileInteraction: NO se encontró PlayerSounds en ningún lugar de la escena!");
        }
        else
        {
            Debug.Log($"PlayerTileInteraction: PlayerSounds configurado correctamente en {playerSounds.gameObject.name}");
        }
        
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

        // DEBUG: Mostrar información del tile actual
        if (floorTile != null)
        {
            Debug.Log($"Tile actual: Tipo={floorTile.tileType}, Color={floorTile.tileColor}, CurrentColor={currentColor}");
        }

        if (floorTile != null && floorTile.tileType == TileType.ColorChange && floorTile.tileColor != currentColor)
        {
            // Guardar color anterior para referencia
            TileColor previousColor = currentColor;
            currentColor = floorTile.tileColor;

            Debug.Log($"DETECTADO: Cambio de color de {previousColor} a {currentColor}");

            int index = (int)currentColor;
            if (index >= 0 && index < colorSprites.Count && colorSprites[index] != null)
            {
                playerSprite.sprite = colorSprites[index];
                Debug.Log("Se ha cambiado de color a " + currentColor);
            }

            // Actualizar paredes y objetivos
            UpdateAllWalls();
            UpdateAllGoals();
            
            // DEBUG: Verificar si playerSounds existe
            if (playerSounds == null)
            {
                Debug.LogError("PlayerSounds es NULL! No se puede reproducir sonido.");
            }
            else
            {
                Debug.Log("PlayerSounds encontrado, intentando reproducir sonido...");
                
                // Reproducir sonido de flores cuando se activa cualquier color
                playerSounds.PlayFlores();
                Debug.Log($"Sonido de flores: Color {currentColor} activado");
            }
        }
        
        // TEST MANUAL: Presiona T para probar sonido de flores
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=== TEST MANUAL SONIDO FLORES ===");
            if (playerSounds != null)
            {
                playerSounds.PlayFlores();
            }
        }
    }

    private void UpdateAllWalls()
    {
        int wallsUnlocked = 0;
        
        foreach (var kvp in wallTiles)
        {
            Vector3Int pos = kvp.Key;
            TileData tile = kvp.Value;

            bool wasVisible = collisionTilemap.HasTile(pos);
            bool visible = tile.tileColor != currentColor;
            
            // Contar paredes que se desbloquean (se vuelven invisibles)
            if (wasVisible && !visible && tile.tileColor == currentColor)
            {
                wallsUnlocked++;
            }
            
            UpdateTileVisibility(pos, visible);
        }
        
        // Si se desbloquearon paredes, mostrar debug
        if (wallsUnlocked > 0)
        {
            Debug.Log($"PlayerTileInteraction: {wallsUnlocked} paredes {currentColor} desbloqueadas");
        }
    }

    private void UpdateAllGoals()
    {
        int goalsActivated = 0;
        
        foreach (var kvp in GoalTiles)
        {
            Vector3Int pos = kvp.Key;
            TileData tile = kvp.Value;

            bool wasVisible = goalTilemap.HasTile(pos);
            bool visible = (tile.tileColor == TileColor.None) || (tile.tileColor == currentColor);
            
            // Contar objetivos que se activan
            if (!wasVisible && visible && tile.tileColor == currentColor)
            {
                goalsActivated++;
            }
            
            UpdateGoalVisibility(pos, visible);
        }
        
        // Si se activaron objetivos, mostrar debug
        if (goalsActivated > 0)
        {
            Debug.Log($"PlayerTileInteraction: {goalsActivated} objetivos {currentColor} activados");
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
    
    // Propiedades públicas para acceder al estado (si es necesario)
    public TileColor CurrentColor => currentColor;
}