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
    
    [Header("Audio Settings")]
    [SerializeField] private float desplazamientoVolume = 1f;
    [SerializeField] private float floresVolume = 1f;
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
            Debug.Log("PlayerSounds: Sistema de sonidos inicializado - Desplazamiento y Flores");
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
    
    void Update()
    {
        // Ya no necesitamos detectar cambios de color aquí
        // PlayerTileInteraction se encarga de llamar PlayFlores() directamente
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
            
            if (debugMode)
            {
                Debug.Log($"PlayerSounds: Input de movimiento detectado: {movement}");
            }
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
        Debug.Log("=== INTENT REPRODUCIR SONIDO DE FLORES ===");
        
        if (floresSound == null)
        {
            Debug.LogError("PlayerSounds: floresSound es NULL! Asigna un AudioClip en el inspector.");
            return;
        }
        
        if (audioSource == null)
        {
            Debug.LogError("PlayerSounds: audioSource es NULL!");
            return;
        }
        
        // Verificar si el AudioSource está en otro GameObject
        if (audioSource.gameObject != this.gameObject)
        {
            Debug.LogWarning($"AudioSource está en {audioSource.gameObject.name}, no en {this.gameObject.name}");
        }
        
        Debug.Log($"GameObject actual: {this.gameObject.name}");
        Debug.Log($"AudioSource en: {audioSource.gameObject.name}");
        Debug.Log($"AudioSource activo: {audioSource.gameObject.activeInHierarchy}");
        Debug.Log($"AudioSource enabled: {audioSource.enabled}");
        Debug.Log($"AudioSource mute: {audioSource.mute}");
        Debug.Log($"AudioSource volume: {audioSource.volume}");
        Debug.Log($"floresVolume: {floresVolume}");
        Debug.Log($"AudioClip nombre: {floresSound.name}");
        Debug.Log($"AudioClip length: {floresSound.length} segundos");
        
        // Intentar reproducir el sonido
        try
        {
            audioSource.PlayOneShot(floresSound, floresVolume);
            Debug.Log("PlayerSounds: ¡¡¡ COMANDO PlayOneShot EJECUTADO !!!");
            
            // Verificar si realmente está reproduciendo
            if (audioSource.isPlaying)
            {
                Debug.Log("AudioSource confirmado: ESTÁ REPRODUCIENDO");
            }
            else
            {
                Debug.LogWarning("AudioSource NO está reproduciendo después de PlayOneShot");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al reproducir sonido: {e.Message}");
        }
    }
    
    #region Public Methods (para llamar desde otros scripts si es necesario)
    
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
    
    #endregion
}