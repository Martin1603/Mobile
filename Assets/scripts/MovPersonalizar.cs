using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovimientoHorizontal : MonoBehaviour
{
    
    public float velocidad;

    public float amplitud;

    public string escenaDesactivar = "EscenaMartin";
    private Vector3 posicionInicial;

    void Start()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        // Si la escena coincide, desactiva este componente
        if (escenaActual == escenaDesactivar)
        {
            this.enabled = false;
            return;
        }
        posicionInicial = transform.position;
    }

    void Update()
    {
        float desplazamiento = Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = new Vector3(posicionInicial.x + desplazamiento, posicionInicial.y, posicionInicial.z);
    }
}
