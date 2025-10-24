using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoHorizontal : MonoBehaviour
{
    
    public float velocidad;

    public float amplitud;

 
    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float desplazamiento = Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = new Vector3(posicionInicial.x + desplazamiento, posicionInicial.y, posicionInicial.z);
    }
}
