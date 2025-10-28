using UnityEngine;
using UnityEngine.SceneManagement;

public class cambioEscena : MonoBehaviour
{
    // Nombre de la escena a la que se cambiará
    public string nombreDeEscena;

    // Este método se llamará desde el botón
    public void Cambiar()
    {
        if (!string.IsNullOrEmpty(nombreDeEscena))
        {
            SceneManager.LoadScene(nombreDeEscena);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el nombre de la escena en el botón.");
        }
    }
}
