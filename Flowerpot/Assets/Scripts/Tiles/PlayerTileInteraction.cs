using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using static TileData;
using UnityEngine.SceneManagement;

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

    [Header("Audio Settings")]
    [SerializeField] private float delayBeforeNextLevel = 1.5f; // Tiempo para escuchar el sonido de victoria

    private Dictionary<Vector3Int, TileData> wallTiles = new Dictionary<Vector3Int, TileData>();
    private Dictionary<Vector3Int, TileData> GoalTiles = new Dictionary<Vector3Int, TileData>();
    
    // Referencia al sistema de sonidos
    private PlayerSounds playerSounds;
    private bool levelCompleted = false; // Para evitar múltiples activaciones

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
        // Si el nivel ya se completó, no hacer nada más
        if (levelCompleted) return;

        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);
        TileData floorTile = groundTilemap.GetTile<TileData>(cellPos);
        TileData goalTile = goalTilemap.GetTile<TileData>(cellPos);

        // Verificar si llegó a la meta
        if (goalTile != null && goalTile.tileType == TileType.Goal)
        {
            levelCompleted = true; // Marcar nivel como completado
            
            // Reproducir sonido de victoria
            if (playerSounds != null)
            {
                playerSounds.PlayNextLevel();
            }
            
            // Cargar siguiente nivel con delay
            StartCoroutine(LoadNextLevelWithDelay());
        }

        // Verificar cambio de color
        if (floorTile != null && floorTile.tileType == TileType.ColorChange && floorTile.tileColor != currentColor)
        {
            currentColor = floorTile.tileColor;

            int index = (int)currentColor;
            if (index >= 0 && index < colorSprites.Count && colorSprites[index] != null)
            {
                playerSprite.sprite = colorSprites[index];
            }

            // Actualizar paredes y objetivos
            UpdateAllWalls();
            UpdateAllGoals();
            
            // Reproducir sonido de flores cuando se activa cualquier color
            if (playerSounds != null)
            {
                Debug.Log("PlayerTileInteraction: Reproduciendo sonido de flores...");
                playerSounds.PlayFlores();
            }
            else
            {
                Debug.LogError("PlayerTileInteraction: PlayerSounds es NULL! No se puede reproducir sonido.");
            }
        }
    }

    /// <summary>
    /// Cargar el siguiente nivel con un pequeño delay para escuchar los sonidos
    /// </summary>
    private IEnumerator LoadNextLevelWithDelay()
    {
        Debug.Log($"PlayerTileInteraction: Nivel completado! Cargando siguiente nivel en {delayBeforeNextLevel} segundos...");
        
        yield return new WaitForSeconds(delayBeforeNextLevel);
        
        LoadNextLevel();
    }

    private void UpdateAllWalls()
    {
        foreach (var kvp in wallTiles)
        {
            Vector3Int pos = kvp.Key;
            TileData tile = kvp.Value;

            bool wasVisible = collisionTilemap.HasTile(pos);
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

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        // Verificar si existe el siguiente nivel
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"PlayerTileInteraction: Cargando nivel {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("PlayerTileInteraction: ¡Juego completado! No hay más niveles.");
            // Aquí puedes cargar una escena de victoria o volver al menú principal
            SceneManager.LoadScene(0); // Volver al menú principal
        }
    }

    // Propiedades públicas para acceder al estado (si es necesario)
    public TileColor CurrentColor => currentColor;
}