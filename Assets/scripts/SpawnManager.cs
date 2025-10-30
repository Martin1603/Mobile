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
    public float alturaAjuste = 1f;

    [HideInInspector] public List<Transform> posicionesIniciales = new List<Transform>();
    [HideInInspector] public Dictionary<Transform, float> alturasIniciales = new Dictionary<Transform, float>();

    [Header("Referencia al GameManager")]
    public GameManager gameManager;

    void Start()
    {
        GenerarPersonajes();
    }

    public void GenerarPersonajes()
    {
        if (prefabNPC == null || prefabPlayer == null)
        {
            Debug.LogError("Faltan prefabs en el SpawnerPersonajes.");
            return;
        }

        // Borrar anteriores
        foreach (Transform t in posicionesIniciales)
        {
            if (t != null) Destroy(t.gameObject);
        }
        posicionesIniciales.Clear();
        alturasIniciales.Clear();

        List<Transform> npcSpawns = new List<Transform>();

        // Generar NPCs
        for (int i = 0; i < cantidadNPCs; i++)
        {
            float angulo = i * Mathf.PI * 2 / (cantidadNPCs + 1);
            Vector3 posicion = new Vector3(Mathf.Cos(angulo), 0, Mathf.Sin(angulo)) * radio + transform.position;
            posicion.y = ObtenerAlturaTerreno(posicion) + alturaAjuste;

            GameObject npc = Instantiate(prefabNPC, posicion, Quaternion.identity);
            npc.name = "NPC_" + (i + 1);

            Transform punto = new GameObject("PuntoNPC_" + (i + 1)).transform;
            punto.position = npc.transform.position;
            npcSpawns.Add(punto);
            posicionesIniciales.Add(punto);

            alturasIniciales[npc.transform] = npc.transform.position.y; // Guardar altura inicial
        }

        // Generar jugador
        float anguloPlayer = Mathf.PI * 2 / (cantidadNPCs + 1) * cantidadNPCs;
        Vector3 posicionPlayer = new Vector3(Mathf.Cos(anguloPlayer), 0, Mathf.Sin(anguloPlayer)) * radio + transform.position;
        posicionPlayer.y = ObtenerAlturaTerreno(posicionPlayer) + alturaAjuste;

        GameObject player = Instantiate(prefabPlayer, posicionPlayer, Quaternion.identity);
        player.name = "Player";

        Transform puntoPlayer = new GameObject("PuntoPlayer").transform;
        puntoPlayer.position = player.transform.position;
        posicionesIniciales.Add(puntoPlayer);

        alturasIniciales[player.transform] = player.transform.position.y;

        // Pasar los datos al GameManager
        if (gameManager != null)
        {
            gameManager.player = player.transform;
            gameManager.playerSpawn = puntoPlayer;
            gameManager.npcs = new List<Transform>();
            gameManager.npcSpawns = npcSpawns;
            gameManager.alturasIniciales = alturasIniciales; // pasar alturas iniciales

            foreach (Transform npcTransform in npcSpawns)
            {
                GameObject npcGO = GameObject.Find(npcTransform.name.Replace("Punto", ""));
                if (npcGO != null)
                    gameManager.npcs.Add(npcGO.transform);
            }

            gameManager.RegistrarPosicionesIniciales();
        }

        Debug.Log("Se generaron todos los personajes y se asignaron al GameManager.");
    }

    float ObtenerAlturaTerreno(Vector3 posicion)
    {
        RaycastHit hit;
        if (Physics.Raycast(posicion + Vector3.up * 50, Vector3.down, out hit, 100f, LayerMask.GetMask("Default")))
        {
            return hit.point.y;
        }
        return transform.position.y;
    }
}
