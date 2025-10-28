using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonCambioMaterial : MonoBehaviour
{
    [Header("Referencia al gestor de materiales")]
    public GestorDeMateriales gestor;

    [Header("Configuraci�n del bot�n")]
    public string nombreGrupo;     // Ejemplo: "Cabello", "Ropa", etc.
    public int indiceMaterial = 0; // Qu� material aplicar (0, 1, 2, ...)

    /// <summary>
    /// Llama a esta funci�n desde el bot�n (OnClick).
    /// </summary>
    public void ActivarMaterial()
    {
        if (gestor == null)
        {
            Debug.LogWarning("No se ha asignado el GestorDeMateriales.");
            return;
        }

        gestor.CambiarMaterial(nombreGrupo, indiceMaterial);
    }
}
