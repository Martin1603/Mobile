using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrupoDeMateriales
{
    public string nombreGrupo = "Nuevo Grupo";
    public GameObject[] objetos;          // Los objetos (mallas, partes, etc.)
    public Material[] materiales;         // Materiales que se pueden asignar
}

public class GestorDeMateriales : MonoBehaviour
{
    [Header("Grupos de objetos y materiales")]
    public List<GrupoDeMateriales> grupos = new List<GrupoDeMateriales>();

    [Header("Configuración")]
    public bool aleatorioAlIniciar = false;

    void Start()
    {
        if (aleatorioAlIniciar)
        {
            AsignarMaterialesAleatorios();
        }
    }

    /// <summary>
    /// Cambia el material de un grupo por índice.
    /// </summary>
    public void CambiarMaterial(string nombreGrupo, int indiceMaterial)
    {
        GrupoDeMateriales grupo = grupos.Find(g => g.nombreGrupo == nombreGrupo);

        if (grupo == null)
        {
            Debug.LogWarning($"No se encontró el grupo '{nombreGrupo}'.");
            return;
        }

        if (indiceMaterial < 0 || indiceMaterial >= grupo.materiales.Length)
        {
            Debug.LogWarning($"Índice de material fuera de rango en el grupo '{nombreGrupo}'.");
            return;
        }

        foreach (GameObject obj in grupo.objetos)
        {
            if (obj != null)
            {
                Renderer r = obj.GetComponent<Renderer>();
                if (r != null)
                    r.material = grupo.materiales[indiceMaterial];
            }
        }
    }

    /// <summary>
    /// Cambia los materiales de todos los grupos de forma aleatoria.
    /// </summary>
    public void AsignarMaterialesAleatorios()
    {
        foreach (GrupoDeMateriales grupo in grupos)
        {
            if (grupo.materiales.Length == 0) continue;

            int randomIndex = Random.Range(0, grupo.materiales.Length);
            CambiarMaterial(grupo.nombreGrupo, randomIndex);
        }
    }
}
