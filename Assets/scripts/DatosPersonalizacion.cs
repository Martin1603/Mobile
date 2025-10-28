using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatosPersonalizacion : MonoBehaviour
{
    public accesorios gestorAccesorios;
    public GestorDeMateriales gestorMateriales;

    private static DatosPersonalizacion instancia;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        CargarDatos();
    }

    private void OnSceneLoaded(Scene escena, LoadSceneMode modo)
    {
        if (gestorAccesorios == null)
            gestorAccesorios = FindObjectOfType<accesorios>();

        if (gestorMateriales == null)
            gestorMateriales = FindObjectOfType<GestorDeMateriales>();

        CargarDatos();
    }

    // GUARDAR DATOS

    public void GuardarDatos()
    {
        if (gestorAccesorios != null)
        {
            for (int i = 0; i < gestorAccesorios.partes.Length; i++)
            {
                PlayerPrefs.SetInt($"Accesorio_{i}", gestorAccesorios.partes[i].indice);
            }
        }

        if (gestorMateriales != null)
        {
            for (int i = 0; i < gestorMateriales.grupos.Count; i++)
            {
                PlayerPrefs.SetInt($"Material_{i}", ObtenerIndiceMaterialActual(gestorMateriales.grupos[i]));
            }
        }

        PlayerPrefs.Save();
    }


    // CARGAR DATOS

    public void CargarDatos()
    {
        if (gestorAccesorios != null)
        {
            for (int i = 0; i < gestorAccesorios.partes.Length; i++)
            {
                int indiceGuardado = PlayerPrefs.GetInt($"Accesorio_{i}", -1);
                if (indiceGuardado >= 0 && indiceGuardado < gestorAccesorios.partes[i].objetos.Length)
                {
                    gestorAccesorios.partes[i].indice = indiceGuardado;
                    gestorAccesorios.partes[i].Activar();
                }
            }
        }

        if (gestorMateriales != null)
        {
            for (int i = 0; i < gestorMateriales.grupos.Count; i++)
            {
                int indiceGuardado = PlayerPrefs.GetInt($"Material_{i}", -1);
                if (indiceGuardado >= 0 && indiceGuardado < gestorMateriales.grupos[i].materiales.Length)
                {
                    gestorMateriales.CambiarMaterial(gestorMateriales.grupos[i].nombreGrupo, indiceGuardado);
                }
            }
        }
    }

    // FUNCIONES AUXILIARES
   
    int ObtenerIndiceMaterialActual(GrupoDeMateriales grupo)
    {
        if (grupo.objetos.Length == 0 || grupo.materiales.Length == 0) return 0;

        Renderer r = grupo.objetos[0].GetComponent<Renderer>();
        if (r == null || r.material == null) return 0;

        for (int i = 0; i < grupo.materiales.Length; i++)
        {
            if (r.material.name.Contains(grupo.materiales[i].name))
                return i;
        }

        return 0;
    }

    // Guardado automático cada vez que el objeto se desactiva o cambia de escena
    void OnApplicationQuit() => GuardarDatos();
    void OnDisable() => GuardarDatos();
}
