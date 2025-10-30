using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscalarObjeto: MonoBehaviour
{
    [Header("Escala inicial y final")]
    public Vector3 escalaInicial = Vector3.one;
    public Vector3 escalaFinal = new Vector3(2, 2, 2);

    [Header("Duración en segundos")]
    public float duracion = 2f;

    private float tiempoTranscurrido = 0f;
    private bool escalando = false;

    void Start()
    {
        transform.localScale = escalaInicial;
        IniciarEscalado(); // Puedes llamar esto manualmente si prefieres
    }

    public void IniciarEscalado()
    {
        tiempoTranscurrido = 0f;
        escalando = true;
    }

    void Update()
    {
        if (!escalando) return;

        tiempoTranscurrido += Time.deltaTime;
        float progreso = Mathf.Clamp01(tiempoTranscurrido / duracion);

        transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progreso);

        if (progreso >= 1f)
        {
            escalando = false;
        }
    }
}


