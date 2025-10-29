using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; // ? Necesario para cambiar escenas

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public Chairs chairsScript;
    public SpawnerPersonajes spawner;
    public Transform player;
    public List<Transform> npcs;
    public Transform playerSpawn;
    public List<Transform> npcSpawns;
    public AudioSource musicaRonda; // música durante la rotación
    public Animator baflesAnimator;  // Bafle 1
    public Animator baflesAnimator2; // Bafle 2

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
    private bool juegoTerminado = false; // ? Evita que se llame la escena más de una vez

    private void Start()
    {
        StartCoroutine(EsperarYComenzarPrimeraRonda());
    }

    private void Update()
    {
        // ? Si el jugador se desactiva (muere), ir a la escena "perder"
        if (!juegoTerminado && (player == null || !player.gameObject.activeInHierarchy))
        {
            juegoTerminado = true;
            SceneManager.LoadScene("perder");
        }
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
            // ? Si completó todas las rondas, gana
            if (!juegoTerminado)
            {
                juegoTerminado = true;
                SceneManager.LoadScene("ganar");
            }
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

        float tiempoEspera = rondaActual == 1 ? tiempoInicioPrimeraRonda : tiempoInicioRondasPosteriores;
        StartCoroutine(EsperarAntesDeGirar(tiempoEspera));
    }

    private IEnumerator EsperarAntesDeGirar(float tiempoEspera)
    {
        var sitControl = player.GetComponent<PlayerSitControl>();
        if (sitControl != null)
            sitControl.SetForcedSitting(true);

        // ?? Encender música y animaciones de ambos bafles
        if (musicaRonda != null && !musicaRonda.isPlaying)
        {
            musicaRonda.Play();
            if (baflesAnimator != null)
                baflesAnimator.SetBool("Encendidos", true);
            if (baflesAnimator2 != null)
                baflesAnimator2.SetBool("Encendidos", true);
        }

        DesactivarMovimientoPersonajes();

        yield return new WaitForSeconds(tiempoEspera);

        StartCoroutine(RotarAntesDeLiberar(sitControl));
    }

    private IEnumerator RotarAntesDeLiberar(PlayerSitControl sitControl)
    {
        Vector3 centro = chairsScript.transform.position;
        float tiempoRotacion = Random.Range(2f, 5f);
        float tiempoTranscurrido = 0f;
        float velocidadRotacion = 15f;

        if (musicaRonda != null && !musicaRonda.isPlaying)
        {
            musicaRonda.Play();
            if (baflesAnimator != null)
                baflesAnimator.SetBool("Encendidos", true);
            if (baflesAnimator2 != null)
                baflesAnimator2.SetBool("Encendidos", true);
        }

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

        // ?? Apagar música y ambos bafles al detenerse
        if (musicaRonda != null && musicaRonda.isPlaying)
        {
            musicaRonda.Stop();
            if (baflesAnimator != null)
                baflesAnimator.SetBool("Encendidos", false);
            if (baflesAnimator2 != null)
                baflesAnimator2.SetBool("Encendidos", false);
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        foreach (var a in agentes)
        {
            if (a != null)
                a.enabled = true;
        }

        if (sitControl != null)
            sitControl.SetForcedSitting(false);

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
