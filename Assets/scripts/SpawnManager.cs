using System.Collections.Generic;
using UnityEngine;

public class SpawnerPersonajes : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabNPC;
    public GameObject prefabPlayer;

    [Header("Configuracion")]
    public int cantidadNPCs = 8;
    public float radio = 5f;
    public float alturaAjuste = 1f; // Ajuste para evitar que se genere bajo el terreno

    [HideInInspector] public List<Transform> posicionesIniciales = new List<Transform>();

    void Start()
    {
        GenerarPersonajes();
    }

    void GenerarPersonajes()
    {
        if (prefabNPC == null || prefabPlayer == null)
        {
            Debug.LogError("Faltan prefabs en el SpawnerPersonajes.");
            return;
        }

        // Borrar posiciones anteriores si existen
        foreach (Transform t in posicionesIniciales)
        {
            if (t != null) Destroy(t.gameObject);
        }
        posicionesIniciales.Clear();

        // Generar NPCs en círculo
        for (int i = 0; i < cantidadNPCs; i++)
        {
            float angulo = i * Mathf.PI * 2 / (cantidadNPCs + 1);
            Vector3 posicion = new Vector3(Mathf.Cos(angulo), 0, Mathf.Sin(angulo)) * radio;
            posicion += transform.position;
            posicion.y = ObtenerAlturaTerreno(posicion) + alturaAjuste;

            GameObject npc = Instantiate(prefabNPC, posicion, Quaternion.identity);
            npc.name = "NPC_" + (i + 1);

            Transform punto = new GameObject("PuntoNPC_" + (i + 1)).transform;
            punto.position = posicion;
            posicionesIniciales.Add(punto);
        }

        // Generar jugador (Player)
        float anguloPlayer = Mathf.PI * 2 / (cantidadNPCs + 1) * cantidadNPCs;
        Vector3 posicionPlayer = new Vector3(Mathf.Cos(anguloPlayer), 0, Mathf.Sin(anguloPlayer)) * radio;
        posicionPlayer += transform.position;
        posicionPlayer.y = ObtenerAlturaTerreno(posicionPlayer) + alturaAjuste;

        GameObject player = Instantiate(prefabPlayer, posicionPlayer, Quaternion.identity);
        player.name = "Player";

        Transform puntoPlayer = new GameObject("PuntoPlayer").transform;
        puntoPlayer.position = posicionPlayer;
        posicionesIniciales.Add(puntoPlayer);

        Debug.Log("Se generaron todos los personajes correctamente.");
    }

    float ObtenerAlturaTerreno(Vector3 posicion)
    {
        RaycastHit hit;
        if (Physics.Raycast(posicion + Vector3.up * 50, Vector3.down, out hit, 100f, LayerMask.GetMask("Default")))
        {
            return hit.point.y;
        }
        return transform.position.y; // Si no golpea el terreno, usa el Y del spawner
    }
}
