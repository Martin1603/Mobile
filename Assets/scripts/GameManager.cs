using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public Chairs chairsScript;
    public SpawnerPersonajes spawner;
    public Transform player;
    public List<Transform> npcs;
    public Transform playerSpawn;
    public List<Transform> npcSpawns;
    public AudioSource musicaRonda; // musica durante la rotacion

    [Header("Configuración de Rondas")]
    public int totalRondas = 8;
    public float radioInicial = 5f;
    public int sillasIniciales = 8;
    public float incrementoRadio = 2f;
    public float tiempoEntreRondas = 3f;
    public float tiempoInicioPrimeraRonda = 7f; // solo primera ronda
    public float tiempoInicioRondasPosteriores = 4f; // rondas siguientes

    private int rondaActual = 1;
    private bool rondaEnCurso = false;

    private void Start()
    {
        StartCoroutine(EsperarYComenzarPrimeraRonda());
    }

    private IEnumerator EsperarYComenzarPrimeraRonda()
    {
        yield return new WaitForSeconds(0.5f);
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

        LevantarTodos();

        chairsScript.EliminarSillas();
        chairsScript.nSillas = sillasIniciales - (rondaActual - 1);
        chairsScript.radio = radioInicial + (rondaActual - 1) * incrementoRadio;
        chairsScript.CrearSillas();

        ReiniciarPosiciones();

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.ResetForNewRound();
        }

        GirarPersonajesAlCentro();

        // Determinar tiempo de espera según la ronda
        float tiempoEspera = rondaActual == 1 ? tiempoInicioPrimeraRonda : tiempoInicioRondasPosteriores;
        StartCoroutine(EsperarAntesDeGirar(tiempoEspera));
    }

    private IEnumerator EsperarAntesDeGirar(float tiempoEspera)
    {
        // ?? Bloquea movimiento del jugador mientras espera y gira
        var sitControl = player.GetComponent<PlayerSitControl>();
        if (sitControl != null)
            sitControl.SetForcedSitting(true);

        // Activa la música si no está sonando
        if (musicaRonda != null && !musicaRonda.isPlaying)
            musicaRonda.Play();

        // Desactiva movimiento de NPCs también
        DesactivarMovimientoPersonajes();

        yield return new WaitForSeconds(tiempoEspera);

        // Empieza la rotación
        StartCoroutine(RotarAntesDeLiberar(sitControl));
    }

    private IEnumerator RotarAntesDeLiberar(PlayerSitControl sitControl)
    {
        Vector3 centro = chairsScript.transform.position;
        float tiempoRotacion = Random.Range(2f, 5f);
        float tiempoTranscurrido = 0f;
        float velocidadRotacion = 15f;

        if (musicaRonda != null && !musicaRonda.isPlaying)
            musicaRonda.Play();

        List<NavMeshAgent> agentes = new List<NavMeshAgent>();

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;
            var agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agentes.Add(agent);
                agent.enabled = false;
            }
        }

        while (tiempoTranscurrido < tiempoRotacion)
        {
            float step = velocidadRotacion * Time.deltaTime;

            if (player != null)
                player.RotateAround(centro, Vector3.up, step);

            foreach (Transform npc in npcs)
            {
                if (npc != null)
                    npc.RotateAround(centro, Vector3.up, step);
            }

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // ?? Detener música cuando termina de girar
        if (musicaRonda != null && musicaRonda.isPlaying)
            musicaRonda.Stop();

        // ? Reactivar movimiento
        if (playerMovement != null)
            playerMovement.enabled = true;

        foreach (var a in agentes)
        {
            if (a != null)
                a.enabled = true;
        }

        // ?? Desbloquear al jugador después de girar
        if (sitControl != null)
            sitControl.SetForcedSitting(false);

        // NPCs empiezan a buscar sillas
        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;
            var seeker = npc.GetComponent<NPCChairSeeker>();
            if (seeker != null)
                seeker.BeginSearch();
        }
    }

    public void TerminarRonda()
    {
        if (!rondaEnCurso)
            return;

        rondaEnCurso = false;
        Debug.Log($"Ronda {rondaActual} terminada.");

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
        if (player != null)
        {
            var sit = player.GetComponent<PlayerSitControl>();
            if (sit != null)
                sit.StandUp();
        }

        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;
            var sit = npc.GetComponent<NPCSitControl>();
            if (sit != null)
                sit.StandUp();
        }
    }

    private void ReiniciarPosiciones()
    {
        if (player != null && playerSpawn != null)
        {
            player.position = playerSpawn.position;
            player.rotation = playerSpawn.rotation;
        }

        for (int i = 0; i < npcs.Count; i++)
        {
            if (i < npcSpawns.Count && npcSpawns[i] != null && npcs[i] != null)
            {
                npcs[i].position = npcSpawns[i].position;
                npcs[i].rotation = npcSpawns[i].rotation;
            }
        }
    }

    private void GirarPersonajesAlCentro()
    {
        Vector3 centro = chairsScript.transform.position;

        if (player != null)
        {
            Vector3 dir = (centro - player.position);
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                player.rotation = Quaternion.LookRotation(dir);
        }

        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;
            Vector3 dir = (centro - npc.position);
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                npc.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void DesactivarMovimientoPersonajes()
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;
            var agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.enabled = false;
        }
    }
}
