using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInImage : MonoBehaviour
{
    [Header("Configuración del Fade")]
    public float retraso = 2f;         // Tiempo antes de iniciar el fade (en segundos)
    public float duracion = 1.5f;      // Duración del fade in
    public Image imagen;               // Referencia a la imagen del Canvas

    void Start()
    {
        if (imagen == null)
            imagen = GetComponent<Image>();

        // Asegurar que empiece invisible
        Color c = imagen.color;
        c.a = 0f;
        imagen.color = c;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Esperar el tiempo antes de iniciar
        yield return new WaitForSeconds(retraso);

        float tiempo = 0f;
        Color c = imagen.color;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Clamp01(tiempo / duracion);
            c.a = alpha;
            imagen.color = c;
            yield return null;
        }

        // Asegurar que termine completamente visible
        c.a = 1f;
        imagen.color = c;
    }
}
