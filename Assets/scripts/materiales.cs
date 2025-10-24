using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materiales : MonoBehaviour
{
    public Renderer personajeRenderer;
    public Material[] materialesDePiel;

    public bool aleatorio = false;

    void Start()
    {
        if (aleatorio)
        {
            CambiarColorAleatorio();
        }
    }


    public void CambiarMaterial(int indice)
    {
        if (personajeRenderer != null && indice >= 0 && indice < materialesDePiel.Length)
        {
            personajeRenderer.material = materialesDePiel[indice];
        }
        else
        {
            Debug.LogWarning("Índice de material fuera de rango o Renderer no asignado.");
        }
    }

    /// <summary>
    /// Asigna un material aleatorio de la lista.
    /// </summary>
    public void CambiarColorAleatorio()
    {
        if (personajeRenderer != null && materialesDePiel.Length > 0)
        {
            int indiceAleatorio = Random.Range(0, materialesDePiel.Length);
            personajeRenderer.material = materialesDePiel[indiceAleatorio];
        }
        else
        {
            Debug.LogWarning("No hay materiales disponibles o Renderer no asignado.");
        }
    }
}
