using UnityEngine;
using System.Collections;

public class SonidoTemporizado : MonoBehaviour
{
    public AudioSource audioSource;
    public float duracion = 3f; // segundos que durará el sonido

    void Start()
    {
        StartCoroutine(ReproducirPorTiempo());
    }

    IEnumerator ReproducirPorTiempo()
    {
        audioSource.Play();
        yield return new WaitForSeconds(duracion);
        audioSource.Stop();
    }
}