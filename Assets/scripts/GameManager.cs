using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public Chairs chairsScript;
    public Transform player;
    public List<Transform> npcs;
    public Transform playerSpawn;
    public List<Transform> npcSpawns;

    [Header("Configuracion de Rondas")]
    public int totalRondas = 7;
    public float radioInicial = 5f;
    public int sillasIniciales = 8;
    public float incrementoRadio = 2f;
    public float tiempoEntreRondas = 3f;

    private int rondaActual = 1;
    private bool rondaEnCurso = false;

    private void Start()
    {
        IniciarRonda();
    }

    public void IniciarRonda()
    {
        if (rondaActual > totalRondas)
        {
            Debug.Log("Juego terminado. Todas las rondas completadas.");
            return;
        }

        rondaEnCurso = true;

        // Asegurar que todos se levanten
        LevantarTodos();

        // Limpiar y crear nuevas sillas
        chairsScript.EliminarSillas();
        chairsScript.nSillas = sillasIniciales - (rondaActual - 1);
        chairsScript.radio = radioInicial + (rondaActual - 1) * incrementoRadio;
        chairsScript.CrearSillas();

        // Reiniciar posiciones
        ReiniciarPosiciones();

        // **Reiniciar la bandera de los Enemy para permitir ejecutar la corutina nuevamente**
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.ReiniciarRonda();
            enemy.StopAllCoroutines(); // opcional, para asegurarnos que no quede nada corriendo de rondas previas
        }

        Debug.Log("Ronda " + rondaActual + " iniciada con " + chairsScript.nSillas + " sillas.");
    }


    public void TerminarRonda()
    {
        if (!rondaEnCurso)
            return;

        rondaEnCurso = false;
        Debug.Log("Ronda " + rondaActual + " terminada.");

        rondaActual++;
        StartCoroutine(SiguienteRonda());
    }

    private IEnumerator SiguienteRonda()
    {
        yield return new WaitForSeconds(tiempoEntreRondas);
        IniciarRonda();
    }

    private void LevantarTodos()
    {
        // Jugador
        if (player != null)
        {
            var playerSit = player.GetComponent<PlayerSitControl>();
            if (playerSit != null)
                playerSit.StandUp();

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
                rb.constraints = RigidbodyConstraints.None;
        }

        // NPCs
        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;

            var npcSit = npc.GetComponent<NPCSitControl>();
            if (npcSit != null)
                npcSit.StandUp();

            var agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
                agent.isStopped = false;
            }

            var rb = npc.GetComponent<Rigidbody>();
            if (rb != null)
                rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void ReiniciarPosiciones()
    {
        if (player != null && playerSpawn != null)
        {
            player.position = playerSpawn.position;
            player.rotation = playerSpawn.rotation;

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        for (int i = 0; i < npcs.Count; i++)
        {
            if (npcs[i] != null && i < npcSpawns.Count && npcSpawns[i] != null)
            {
                npcs[i].position = npcSpawns[i].position;
                npcs[i].rotation = npcSpawns[i].rotation;

                var rb = npcs[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                var seeker = npcs[i].GetComponent<NPCChairSeeker>();
                if (seeker != null)
                    seeker.ResetChairSearch();
            }
        }
    }
}
