using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundTilemap;
    [SerializeField]
    private Tilemap collisionTilemap;
    private PlayerMovement controls;

    private void Awake()
    {
        controls = new PlayerMovement();
        
        controls.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        controls.Main.Reload.performed += ctx => ReloadScene();
    }

    private void OnEnable()
    {
        // Verificar que controls no sea null antes de habilitarlo
        if (controls != null)
        {
            controls.Enable();
        }
    }

    private void OnDisable()
    {
        // Verificar que controls no sea null antes de deshabilitarlo
        if (controls != null)
        {
            controls.Disable();
        }
    }

    private void OnDestroy()
    {
        // Limpiar recursos del Input System
        if (controls != null)
        {
            controls.Dispose();
        }
    }

    private void Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            transform.position += (Vector3)direction;
        }
    }

    private bool CanMove(Vector2 direction) 
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
        {
            return false;
        }
        return true;
    }

    private void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}