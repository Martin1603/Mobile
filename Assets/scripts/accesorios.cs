using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accesorios : MonoBehaviour
{
    public GrupoObjetos[] partes;
    public bool aleatorizar;


    // Start is called before the first frame update
    void Start()
    {
        if (aleatorizar)
        {
            Aleatorizar();
        }
    }
    public void Aleatorizar()
    {
        for (int i = 0; i < partes.Length; i++)
        {
            partes[i].Aleatorizar();
        }
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
        for (int i = 0; i < objetos.Length; i++)
        {
            objetos[i].SetActive(i == indice);
        }
    }

    public void Siguiente()
    {
        indice = (indice+1)%objetos.Length;
        Activar();
    }

    public void Anterior()
    {
        indice = (indice - 1 + objetos.Length) % objetos.Length;
        Activar();
    }

    public void Aleatorizar()
    {
        indice=Random.Range(0,objetos.Length);
        Activar();
    }
}
