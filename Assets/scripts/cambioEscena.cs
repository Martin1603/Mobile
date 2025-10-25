using UnityEngine;
using UnityEngine.SceneManagement;

public class cambioEscena : MonoBehaviour
{
    // Nombre de la escena a la que se cambiar�
    public string nombreDeEscena;

    // Este m�todo se llamar� desde el bot�n
    public void Cambiar()
    {
        if (!string.IsNullOrEmpty(nombreDeEscena))
        {
            SceneManager.LoadScene(nombreDeEscena);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el nombre de la escena en el bot�n.");
        }
    }
}
