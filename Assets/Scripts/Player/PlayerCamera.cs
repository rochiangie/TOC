using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private void Start()
    {
        // Cinemachine maneja la cámara, nosotros solo aseguramos que el cursor esté bloqueado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // El resto de la lógica de cámara (rotación, colisión) se elimina 
    // porque Cinemachine FreeLook se encargará de todo eso.
}
