using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public Chairs chairsScript;              // Referencia al script de las sillas
    public Transform player;                 // Referencia al jugador
    public List<Transform> npcs;             // Lista de NPCs a reiniciar
    public Transform playerSpawn;            // Punto de reinicio del jugador
    public List<Transform> npcSpawns;        // Puntos de reinicio de cada NPC

    [Header("Configuración de Rondas")]
    public int totalRondas = 7;              // Total de rondas del juego
    public float radioInicial = 5f;          // Radio base para la primera ronda
    public int sillasIniciales = 8;          // Número de sillas en la primera ronda
    public float incrementoRadio = 2f;       // Cuánto aumenta el radio por ronda
    public float tiempoEntreRondas = 3f;     // Tiempo antes de iniciar una nueva ronda

    private int rondaActual = 1;
    private bool rondaEnCurso = false;

    private void Start()
    {
        IniciarRonda();
    }

    // 🔹 Inicia una nueva ronda
    public void IniciarRonda()
    {
        if (rondaActual > totalRondas)
        {
            Debug.Log("Juego terminado. Todas las rondas completadas.");
            return;
        }

        rondaEnCurso = true;

        // Configurar sillas para esta ronda
        chairsScript.EliminarSillas();
        chairsScript.nSillas = sillasIniciales - (rondaActual - 1);
        chairsScript.radio = radioInicial + (rondaActual - 1) * incrementoRadio;
        chairsScript.CrearSillas();

        // Reiniciar posiciones
        ReiniciarPosiciones();

        Debug.Log("▶️ Ronda " + rondaActual + " iniciada con " + chairsScript.nSillas + " sillas.");
    }

    // 🔹 Termina la ronda y programa la siguiente
    public void TerminarRonda()
    {
        if (!rondaEnCurso)
            return;

        rondaEnCurso = false;

        Debug.Log("🏁 Ronda " + rondaActual + " terminada.");
        rondaActual++;

        StartCoroutine(SiguienteRonda());
    }

    // 🔹 Espera un tiempo antes de comenzar la siguiente ronda
    private IEnumerator SiguienteRonda()
    {
        yield return new WaitForSeconds(tiempoEntreRondas);
        IniciarRonda();
    }

    // 🔹 Reinicia al jugador y NPCs a sus posiciones originales
    private void ReiniciarPosiciones()
    {
        // Reinicia el jugador
        if (player != null && playerSpawn != null)
        {
            player.position = playerSpawn.position;
            player.rotation = playerSpawn.rotation;
        }

        // Reinicia los NPCs
        for (int i = 0; i < npcs.Count; i++)
        {
            if (npcs[i] != null && i < npcSpawns.Count && npcSpawns[i] != null)
            {
                npcs[i].position = npcSpawns[i].position;
                npcs[i].rotation = npcSpawns[i].rotation;

                // Si tienen scripts de comportamiento, los reinicia
                var seeker = npcs[i].GetComponent<NPCChairSeeker>();
                if (seeker != null)
                    seeker.ResetChairSearch();
            }
        }
    }
}
