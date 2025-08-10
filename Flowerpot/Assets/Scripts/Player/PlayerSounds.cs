using UnityEngine;
using UnityEngine.InputSystem;
using static TileData;

public class PlayerSounds : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip desplazamientoSound; // Sonido de movimiento
    [SerializeField] private AudioClip floresSound;         // Sonido de cambio de color/flores
    [SerializeField] private AudioClip nextLevelSound;     // Sonido de pasar de nivel

    [Header("Audio Settings")]
    [SerializeField] private float desplazamientoVolume = 1f;
    [SerializeField] private float floresVolume = 0.2f;
    [SerializeField] private float nextLevelVolume = 0.3f;
    [SerializeField] private float movementCooldown = 0.2f; // Para evitar spam del sonido
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    // Referencias
    private PlayerMovement controls;
    private PlayerTileInteraction tileInteraction;
    private Vector3 lastPosition;
    private TileColor lastKnownColor = TileColor.None;
    private float lastMovementSoundTime;
    
    void Awake()
    {
        // Configurar AudioSource
        SetupAudioSource();
        
        // Buscar PlayerTileInteraction en el mismo GameObject primero
        tileInteraction = GetComponent<PlayerTileInteraction>();
        
        // Si no está en el mismo GameObject, buscar en toda la escena
        if (tileInteraction == null)
        {
            tileInteraction = FindObjectOfType<PlayerTileInteraction>();
            if (tileInteraction != null)
            {
                Debug.LogWarning($"PlayerSounds: PlayerTileInteraction encontrado en {tileInteraction.gameObject.name} (no en el mismo GameObject)");
            }
        }
        
        if (tileInteraction == null)
        {
            Debug.LogError("PlayerSounds: NO se encontró PlayerTileInteraction en ningún lugar de la escena!");
        }
        else
        {
            Debug.Log($"PlayerSounds: PlayerTileInteraction configurado correctamente en {tileInteraction.gameObject.name}");
        }
        
        // Inicializar controles para detectar input
        controls = new PlayerMovement();
        lastPosition = transform.position;
        
        if (debugMode)
        {
            Debug.Log("PlayerSounds: Sistema de sonidos inicializado - Desplazamiento, Flores y Victoria");
        }
    }
    
    void OnEnable()
    {
        controls.Enable();
        controls.Main.Movement.performed += OnMovementInput;
    }
    
    void OnDisable()
    {
        if (controls != null)
        {
            controls.Main.Movement.performed -= OnMovementInput;
            controls.Disable();
        }
    }
    
    void OnDestroy()
    {
        controls?.Dispose();
    }
    
    /// <summary>
    /// Configurar el AudioSource
    /// </summary>
    private void SetupAudioSource()
    {
        // Si no hay AudioSource asignado manualmente, usar el del mismo GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // Crear un AudioSource en este GameObject
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("PlayerSounds: AudioSource creado automáticamente en el Player");
            }
        }
        else
        {
            Debug.Log($"PlayerSounds: Usando AudioSource de {audioSource.gameObject.name}");
        }
        
        // Configurar el AudioSource
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Sonido 2D
        
        // Debug de configuración
        Debug.Log($"AudioSource configurado - Mute: {audioSource.mute}, Volume: {audioSource.volume}");
    }
    
    /// <summary>
    /// Se ejecuta cuando se detecta input de movimiento
    /// </summary>
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        
        if (movement != Vector2.zero && CanPlayMovementSound())
        {
            PlayDesplazamientoSound();
            lastMovementSoundTime = Time.time;
        }
    }
    
    /// <summary>
    /// Verificar si puede reproducir sonido de movimiento (cooldown)
    /// </summary>
    private bool CanPlayMovementSound()
    {
        return Time.time - lastMovementSoundTime >= movementCooldown;
    }
    
    /// <summary>
    /// Reproduce el sonido de desplazamiento
    /// </summary>
    private void PlayDesplazamientoSound()
    {
        if (desplazamientoSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(desplazamientoSound, desplazamientoVolume);
            
            if (debugMode)
            {
                Debug.Log("PlayerSounds: Sonido de desplazamiento reproducido");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning("PlayerSounds: desplazamientoSound no está asignado");
        }
    }
    
    /// <summary>
    /// Reproduce el sonido de flores/cambio de color
    /// </summary>
    private void PlayFloresSound()
    {
        if (floresSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(floresSound, floresVolume);
            
            if (debugMode)
            {
                Debug.Log("PlayerSounds: Sonido de flores reproducido");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning("PlayerSounds: floresSound no está asignado");
        }
    }

    /// <summary>
    /// Reproduce el sonido de pasar de nivel
    /// </summary>
    private void PlayNextLevelSound()
    {
        if (nextLevelSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(nextLevelSound, nextLevelVolume);
            
            if (debugMode)
            {
                Debug.Log("PlayerSounds: Sonido de siguiente nivel reproducido");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning("PlayerSounds: nextLevelSound no está asignado");
        }
    }

    #region Public Methods (para llamar desde otros scripts)

    /// <summary>
    /// Método público para reproducir sonido de desplazamiento
    /// </summary>
    public void PlayDesplazamiento()
    {
        PlayDesplazamientoSound();
    }
    
    /// <summary>
    /// Método público para reproducir sonido de flores
    /// </summary>
    public void PlayFlores()
    {
        PlayFloresSound();
    }
    
    /// <summary>
    /// Método público para reproducir sonido de siguiente nivel
    /// </summary>
    public void PlayNextLevel()
    {
        PlayNextLevelSound();
    }
    
    /// <summary>
    /// Cambiar volumen del sonido de desplazamiento
    /// </summary>
    public void SetDesplazamientoVolume(float volume)
    {
        desplazamientoVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Cambiar volumen del sonido de flores
    /// </summary>
    public void SetFloresVolume(float volume)
    {
        floresVolume = Mathf.Clamp01(volume);
    }
    
    /// <summary>
    /// Cambiar volumen del sonido de siguiente nivel
    /// </summary>
    public void SetNextLevelVolume(float volume)
    {
        nextLevelVolume = Mathf.Clamp01(volume);
    }
    
    #endregion
    
    #region Context Menu Methods (para probar en el editor)
    
    [ContextMenu("Test Desplazamiento Sound")]
    public void TestDesplazamientoSound()
    {
        PlayDesplazamientoSound();
    }
    
    [ContextMenu("Test Flores Sound")]
    public void TestFloresSound()
    {
        PlayFloresSound();
    }
    
    [ContextMenu("Test Next Level Sound")]
    public void TestNextLevelSound()
    {
        PlayNextLevelSound();
    }
    
    #endregion
}