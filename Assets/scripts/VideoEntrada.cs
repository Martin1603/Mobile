using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoEntrada : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer; // El componente del VideoPlayer
    public Image fadeImage;         // Imagen negra que hará el fade
    public Text texto;              // Texto normal de la UI

    [Header("Configuración")]
    public float delayAntesVideo = 2f;   // Tiempo antes de que empiece el video
    public float duracionFade = 1.5f;    // Duración del fade-in
    public float duracionTexto = 2f;     // Tiempo que el texto permanece visible

    void Start()
    {
        // Inicia con la pantalla completamente opaca
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        // Asegura que el texto esté invisible al principio
        if (texto != null)
        {
            Color txtColor = texto.color;
            txtColor.a = 0f;
            texto.color = txtColor;
        }

        // Detiene el video al inicio
        if (videoPlayer != null)
            videoPlayer.Stop();

        // Inicia la secuencia
        StartCoroutine(SecuenciaVideo());
    }

    IEnumerator SecuenciaVideo()
    {
        // Espera antes de iniciar el video
        yield return new WaitForSeconds(delayAntesVideo);

        // Inicia el video
        if (videoPlayer != null)
            videoPlayer.Play();

        // Fade-in del fondo negro (desaparece)
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duracionFade);

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }

            yield return null;
        }

        // Asegura transparencia total del fade
        if (fadeImage != null)
        {
            Color final = fadeImage.color;
            final.a = 0f;
            fadeImage.color = final;
        }

        // Mostrar texto con fade-in
        if (texto != null)
        {
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                Color c = texto.color;
                c.a = Mathf.Lerp(0f, 1f, t);
                texto.color = c;
                yield return null;
            }

         
        }
    }
}
