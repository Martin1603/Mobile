using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CambiarImagen : MonoBehaviour
{
    [Header("Referencias a las imágenes del Canvas")]
    public GameObject imagenA;
    public GameObject imagenB;

    // Este método se llamará al presionar el botón
    public void Cambiar()
    {
        // Activa la imagen A y desactiva la imagen B
        if (imagenA != null && imagenB != null)
        {
            imagenA.SetActive(true);
            imagenB.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Faltan referencias a las imágenes en el inspector.");
        }
    }
}
