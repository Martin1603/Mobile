using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chairs : MonoBehaviour
{
    public int nSillas;                // Cantidad de sillas que se van a generar
    public float radio;                // Radio del círculo donde se ubicarán las sillas
    public Transform padre;            // Objeto padre para organizar las sillas en la jerarquía
    public GameObject prefabSillas;    // Prefab de la silla a instanciar
    private List<GameObject> sillas = new List<GameObject>(); // Lista que almacena las sillas creadas

    // 🔹 Crea las sillas en círculo alrededor del punto (0,0,0) o del objeto padre si existe
    public void CrearSillas()
    {
        for (int i = 0; i < nSillas; i++)
        {
            // Calcula una posición en círculo
            Vector3 p = new Vector3(
                Mathf.Sin(i * 2 * Mathf.PI / nSillas),
                -0.1f,
                Mathf.Cos(i * 2 * Mathf.PI / nSillas)
            ) * radio;

            // Instancia la silla
            GameObject s = Instantiate(prefabSillas, p, Quaternion.identity);

            // Si hay un padre definido, la hace hija de él
            if (padre != null)
                s.transform.SetParent(padre);

            // Añade la silla a la lista
            sillas.Add(s);

            // Orienta la silla hacia el centro
            s.transform.forward = p;
            s.transform.Rotate(0, 90, 0);
        }
    }

    // 🔹 NUEVA FUNCIÓN: elimina todas las sillas creadas y limpia la lista
    public void EliminarSillas()
    {
        // Recorre todas las sillas y las destruye
        foreach (GameObject s in sillas)
        {
            if (s != null)
                Destroy(s);
        }

        // Limpia completamente la lista
        sillas.Clear();
    }

    // 🔹 Crea las sillas automáticamente al iniciar el objeto
    private void Awake()
    {
        CrearSillas();
    }
    public List<GameObject> GetSillas()
    {
        return sillas;
    }
}

