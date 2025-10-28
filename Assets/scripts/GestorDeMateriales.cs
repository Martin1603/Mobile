using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrupoDeMateriales
{
    public string nombreGrupo = "Nuevo Grupo";
    public GameObject[] objetos;
    public Material[] materiales;
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
            AsignarMaterialesAleatorios();
    }

    public void CambiarMaterial(string nombreGrupo, int indiceMaterial)
    {
        GrupoDeMateriales grupo = grupos.Find(g => g.nombreGrupo == nombreGrupo);

        if (grupo == null)
        {
            Debug.LogWarning($"No se encontró el grupo '{nombreGrupo}'.");
            return;
        }

        if (grupo.materiales == null || grupo.materiales.Length == 0)
        {
            Debug.LogWarning($"El grupo '{nombreGrupo}' no tiene materiales asignados.");
            return;
        }

        if (indiceMaterial < 0 || indiceMaterial >= grupo.materiales.Length)
        {
            Debug.LogWarning($"Índice de material fuera de rango en el grupo '{nombreGrupo}'.");
            return;
        }

        foreach (GameObject obj in grupo.objetos)
        {
            if (obj == null) continue;

            Renderer r = obj.GetComponent<Renderer>();
            if (r != null)
                r.sharedMaterial = grupo.materiales[indiceMaterial];
        }
    }

    public void AsignarMaterialesAleatorios()
    {
        foreach (GrupoDeMateriales grupo in grupos)
        {
            if (grupo.materiales == null || grupo.materiales.Length == 0)
                continue;

            int randomIndex = Random.Range(0, grupo.materiales.Length);
            CambiarMaterial(grupo.nombreGrupo, randomIndex);
        }
    }
}
