using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("MainMenu: Script iniciado en la escena");
    }
    
    public void PlayGame() 
    {
        Debug.Log("=== MainMenu.PlayGame() LLAMADO ===");
        
        // Verificar si AudioManager existe
        if (AudioManager.Instance != null)
        {
            Debug.Log("MainMenu: AudioManager encontrado, reproduciendo sonido...");
            AudioManager.Instance.PlayButtonClick();
        }
        else
        {
            Debug.LogError("MainMenu: AudioManager.Instance es NULL!");
        }
        
        // Pequeño delay para asegurar que el sonido se reproduzca antes del cambio de escena
        Debug.Log("MainMenu: Cargando escena del juego...");
        SceneManager.LoadSceneAsync(1);
    }
    
    /// <summary>
    /// Método alternativo para probar el sonido sin cambiar de escena
    /// </summary>
    public void TestButtonSound()
    {
        Debug.Log("=== MainMenu.TestButtonSound() LLAMADO ===");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
            Debug.Log("MainMenu: Sonido de prueba reproducido");
        }
        else
        {
            Debug.LogError("MainMenu: AudioManager no encontrado para la prueba");
        }
    }
    
    /// <summary>
    /// Método para salir del juego (opcional)
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("MainMenu: Saliendo del juego...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}