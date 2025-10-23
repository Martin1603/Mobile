using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIpersonalizarObjetos : MonoBehaviour
{

    public accesorios cuerpoPersonalizable;
    public Text txtUnidades;
    public int indice;





    // Start is called before the first frame update
    void Start()
    {
        ActualizarTexto();
    }

    public void Siguiente()
    {
        cuerpoPersonalizable.partes[indice].Siguiente();
        ActualizarTexto();

    }

    public void Anterior()
    {
        cuerpoPersonalizable.partes[indice].Anterior();
        ActualizarTexto();
    }

    void ActualizarTexto()
    {
        txtUnidades.text = (cuerpoPersonalizable.partes[indice].indice + 1).ToString() + "/" + cuerpoPersonalizable.partes[indice].objetos.Length;
    }
}
