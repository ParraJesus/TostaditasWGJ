using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.5f;
    
    [Header("Menu Sound Effects")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private float buttonClickVolume = 1f;
    
    // Singleton para acceder desde cualquier lugar
    public static AudioManager Instance { get; private set; }
    
    private void Awake()
    {
        // Implementar Singleton que persiste entre escenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager: Instancia creada y configurada como DontDestroyOnLoad");
            SetupAudioSources();
            StartBackgroundMusic();
        }
        else
        {
            Debug.Log("AudioManager: Instancia duplicada detectada, destruyendo...");
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Configurar los AudioSources
    /// </summary>
    private void SetupAudioSources()
    {
        // Configurar AudioSource para música si no está asignado
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioManager: AudioSource para música creado automáticamente");
        }
        
        // Configurar AudioSource para efectos de sonido si no está asignado
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioManager: AudioSource para efectos creado automáticamente");
        }
        
        // Configurar música de fondo
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f; // 2D
        
        // Configurar efectos de sonido del menú
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = buttonClickVolume;
        sfxSource.spatialBlend = 0f; // 2D
        
        Debug.Log("AudioManager: AudioSources configurados correctamente");
    }
    
    /// <summary>
    /// Iniciar música de fondo
    /// </summary>
    private void StartBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
            Debug.Log("AudioManager: Música de fondo iniciada y se repetirá automáticamente");
        }
        else
        {
            if (backgroundMusic == null)
                Debug.LogWarning("AudioManager: backgroundMusic no está asignado");
            if (musicSource == null)
                Debug.LogWarning("AudioManager: musicSource es null");
        }
    }
    
    /// <summary>
    /// Reproducir sonido de click de botón del menú
    /// </summary>
    public void PlayButtonClick()
    {
        Debug.Log("=== AudioManager.PlayButtonClick() LLAMADO ===");
        
        if (buttonClickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSound, buttonClickVolume);
            Debug.Log($"AudioManager: Sonido de click REPRODUCIDO - Volume: {buttonClickVolume}");
        }
        else
        {
            if (buttonClickSound == null)
                Debug.LogWarning("AudioManager: buttonClickSound no está asignado");
            if (sfxSource == null)
                Debug.LogWarning("AudioManager: sfxSource es null");
        }
    }
    
    /// <summary>
    /// Cambiar volumen de la música
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            Debug.Log($"AudioManager: Volumen de música cambiado a {musicVolume}");
        }
    }
    
    /// <summary>
    /// Cambiar volumen de efectos de sonido
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        buttonClickVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = buttonClickVolume;
            Debug.Log($"AudioManager: Volumen de efectos cambiado a {buttonClickVolume}");
        }
    }
    
    /// <summary>
    /// Pausar/Reanudar música
    /// </summary>
    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
                Debug.Log("AudioManager: Música pausada");
            }
            else
            {
                musicSource.UnPause();
                Debug.Log("AudioManager: Música reanudada");
            }
        }
    }
    
    /// <summary>
    /// Detener música completamente
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            Debug.Log("AudioManager: Música detenida");
        }
    }
    
    /// <summary>
    /// Reiniciar música desde el principio
    /// </summary>
    public void RestartMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = backgroundMusic;
            musicSource.Play();
            Debug.Log("AudioManager: Música reiniciada");
        }
    }
}