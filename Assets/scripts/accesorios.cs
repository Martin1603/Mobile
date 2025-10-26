using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accesorios : MonoBehaviour
{
    public GrupoObjetos[] partes;
    public bool aleatorizar;

    void Start()
    {
        // Activar los objetos iniciales
        foreach (var parte in partes)
            parte.Activar();

        // Aleatorizar si está habilitado
        if (aleatorizar)
            Aleatorizar();
    }

    public void Aleatorizar()
    {
        foreach (var parte in partes)
            parte.Aleatorizar();
    }
}

[System.Serializable]
public class GrupoObjetos
{
    public string nombre = "Default";
    public GameObject[] objetos;
    public int indice;

    public void Activar()
    {
        if (objetos == null || objetos.Length == 0)
            return;

        for (int i = 0; i < objetos.Length; i++)
        {
            if (objetos[i] != null)
                objetos[i].SetActive(i == indice);
        }
    }

    public void Siguiente()
    {
        if (objetos == null || objetos.Length == 0)
            return;

        indice = (indice + 1) % objetos.Length;
        Activar();
    }

    public void Anterior()
    {
        if (objetos == null || objetos.Length == 0)
            return;

        indice = (indice - 1 + objetos.Length) % objetos.Length;
        Activar();
    }

    public void Aleatorizar()
    {
        if (objetos == null || objetos.Length == 0)
            return;

        indice = Random.Range(0, objetos.Length);
        Activar();
    }
}
