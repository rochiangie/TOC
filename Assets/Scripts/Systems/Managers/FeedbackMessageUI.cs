using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Sistema de mensajes de feedback para el jugador.
/// Muestra mensajes temporales en pantalla (errores, éxitos, etc.)
/// </summary>
public class FeedbackMessageUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("El TextMeshProUGUI que mostrará los mensajes")]
    public TextMeshProUGUI messageText;

    [Header("Settings")]
    [Tooltip("Duración por defecto de los mensajes en segundos")]
    public float defaultDuration = 3f;

    [Header("Colors")]
    public Color errorColor = new Color(1f, 0.3f, 0.3f); // Rojo suave
    public Color successColor = new Color(0.3f, 1f, 0.3f); // Verde suave
    public Color infoColor = new Color(1f, 1f, 1f); // Blanco
    public Color warningColor = new Color(1f, 0.8f, 0.2f); // Amarillo/Naranja

    private Coroutine currentMessageCoroutine;

    // Singleton para fácil acceso
    public static FeedbackMessageUI Instance { get; private set; }

    private void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Ya existe una instancia de FeedbackMessageUI. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }

        // Ocultar el mensaje al inicio
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Muestra un mensaje de error (rojo)
    /// </summary>
    public void ShowError(string message, float duration = -1f)
    {
        ShowMessage(message, errorColor, duration);
    }

    /// <summary>
    /// Muestra un mensaje de éxito (verde)
    /// </summary>
    public void ShowSuccess(string message, float duration = -1f)
    {
        ShowMessage(message, successColor, duration);
    }

    /// <summary>
    /// Muestra un mensaje informativo (blanco)
    /// </summary>
    public void ShowInfo(string message, float duration = -1f)
    {
        ShowMessage(message, infoColor, duration);
    }

    /// <summary>
    /// Muestra un mensaje de advertencia (amarillo)
    /// </summary>
    public void ShowWarning(string message, float duration = -1f)
    {
        ShowMessage(message, warningColor, duration);
    }

    /// <summary>
    /// Muestra un mensaje con color personalizado
    /// </summary>
    public void ShowMessage(string message, Color color, float duration = -1f)
    {
        if (messageText == null)
        {
            Debug.LogWarning("FeedbackMessageUI: No hay TextMeshProUGUI asignado!");
            return;
        }

        // Si hay un mensaje activo, detenerlo
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        // Usar duración por defecto si no se especifica
        if (duration < 0)
        {
            duration = defaultDuration;
        }

        // Iniciar nueva corrutina de mensaje
        currentMessageCoroutine = StartCoroutine(DisplayMessageCoroutine(message, color, duration));
    }

    private IEnumerator DisplayMessageCoroutine(string message, Color color, float duration)
    {
        // Configurar el texto
        messageText.text = message;
        messageText.color = color;
        messageText.gameObject.SetActive(true);

        // Animación de entrada (fade in rápido)
        float fadeInTime = 0.2f;
        float elapsed = 0f;
        Color startColor = color;
        startColor.a = 0f;

        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            Color currentColor = color;
            currentColor.a = alpha;
            messageText.color = currentColor;
            yield return null;
        }

        // Asegurar que esté completamente visible
        messageText.color = color;

        // Esperar la duración especificada
        yield return new WaitForSeconds(duration);

        // Animación de salida (fade out)
        float fadeOutTime = 0.5f;
        elapsed = 0f;

        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            Color currentColor = color;
            currentColor.a = alpha;
            messageText.color = currentColor;
            yield return null;
        }

        // Ocultar el mensaje
        messageText.gameObject.SetActive(false);
        currentMessageCoroutine = null;
    }

    /// <summary>
    /// Oculta el mensaje actual inmediatamente
    /// </summary>
    public void HideMessage()
    {
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
            currentMessageCoroutine = null;
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}
