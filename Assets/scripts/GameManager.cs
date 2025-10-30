using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public Chairs chairsScript;
    public SpawnerPersonajes spawner;
    public Transform player;
    public List<Transform> npcs;
    public Transform playerSpawn;
    public List<Transform> npcSpawns;
    public AudioSource musicaRonda;
    public Animator baflesAnimator;
    public Animator baflesAnimator2;

    [Header("Configuración de Rondas")]
    public int totalRondas = 8;
    public float radioInicial = 5f;
    public int sillasIniciales = 8;
    public float incrementoRadio = 2f;
    public float tiempoEntreRondas = 3f;
    public float tiempoInicioPrimeraRonda = 7f;
    public float tiempoInicioRondasPosteriores = 4f;

    [HideInInspector]
    public Dictionary<Transform, float> alturasIniciales = new Dictionary<Transform, float>();

    private int rondaActual = 1;
    private bool rondaEnCurso = false;
    private bool juegoTerminado = false;

    private Dictionary<Transform, float> radiosOriginales = new Dictionary<Transform, float>();
    private Dictionary<Transform, float> angulosOriginales = new Dictionary<Transform, float>();

    public void RegistrarPosicionesIniciales()
    {
        Vector3 centro = chairsScript.transform.position;

        // Guardar radio y ángulo de NPCs
        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;
            Vector3 offset = npc.position - centro;
            radiosOriginales[npc] = new Vector2(offset.x, offset.z).magnitude;
            angulosOriginales[npc] = Mathf.Atan2(offset.z, offset.x);
        }

        // Guardar radio y ángulo del player
        if (player != null)
        {
            Vector3 offset = player.position - centro;
            radiosOriginales[player] = new Vector2(offset.x, offset.z).magnitude;
            angulosOriginales[player] = Mathf.Atan2(offset.z, offset.x);
        }
    }

    private void Start()
    {
        StartCoroutine(EsperarYComenzarPrimeraRonda());
    }

    private void Update()
    {
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
            if (!juegoTerminado)
            {
                juegoTerminado = true;
                SceneManager.LoadScene("VICTORIA");
            }
            return;
        }

        rondaEnCurso = true;

        LevantarTodos();

        // Configurar sillas (solo afecta a las sillas)
        chairsScript.EliminarSillas();
        chairsScript.nSillas = sillasIniciales - (rondaActual - 1);
        chairsScript.radio = radioInicial + (rondaActual - 1) * incrementoRadio; // radio dinámico solo para sillas
        chairsScript.CrearSillas();

        // Reorganizar personajes usando su radio original fijo
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
        float tiempoRotacion = Random.Range(2f, 10f);
        float tiempoTranscurrido = 0f;
        float velocidadRotacion = 15f;

        List<NavMeshAgent> agentes = new List<NavMeshAgent>();
        List<Rigidbody> rigidbodies = new List<Rigidbody>();

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Desactivar los NavMeshAgents y congelar rotaciones
        foreach (Transform npc in npcs)
        {
            if (npc == null) continue;

            var agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agentes.Add(agent);
                agent.enabled = false;
            }

            var rb = npc.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rigidbodies.Add(rb);
                rb.constraints = RigidbodyConstraints.FreezeRotation; // ? congela la rotación
            }
        }

        // Congelar rotación del jugador si tiene Rigidbody
        var playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
            playerRb.constraints = RigidbodyConstraints.FreezeRotation;

        // --- Inicio del giro ---
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
        // --- Fin del giro ---

        // Apagar música y luces
        if (musicaRonda != null && musicaRonda.isPlaying)
        {
            musicaRonda.Stop();
            if (baflesAnimator != null)
                baflesAnimator.SetBool("Encendidos", false);
            if (baflesAnimator2 != null)
                baflesAnimator2.SetBool("Encendidos", false);
        }

        // Reactivar movimiento
        if (playerMovement != null)
            playerMovement.enabled = true;

        foreach (var a in agentes)
        {
            if (a != null)
                a.enabled = true;
        }

        // Restaurar constraints de Rigidbody
        foreach (var rb in rigidbodies)
        {
            if (rb != null)
                rb.constraints = RigidbodyConstraints.None; // ? libera nuevamente
        }

        if (playerRb != null)
            playerRb.constraints = RigidbodyConstraints.None;

        if (sitControl != null)
            sitControl.SetForcedSitting(false);

        // Iniciar búsqueda de sillas
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
        Vector3 centro = chairsScript.transform.position;

        // Crear lista de personajes vivos (NPCs activos + player)
        List<Transform> vivos = new List<Transform>();
        if (player != null && player.gameObject.activeInHierarchy)
            vivos.Add(player);

        foreach (Transform npc in npcs)
        {
            if (npc != null && npc.gameObject.activeInHierarchy)
                vivos.Add(npc);
        }

        int cantidad = vivos.Count;
        if (cantidad == 0) return;

        for (int i = 0; i < cantidad; i++)
        {
            Transform personaje = vivos[i];

            float radio = radiosOriginales.ContainsKey(personaje) ? radiosOriginales[personaje] : radioInicial;
            float altura = alturasIniciales.ContainsKey(personaje) ? alturasIniciales[personaje] : personaje.position.y;

            // Recalcular ángulo equitativo
            float angulo = i * Mathf.PI * 2f / cantidad;

            Vector3 nuevaPos = new Vector3(
                centro.x + radio * Mathf.Cos(angulo),
                altura,
                centro.z + radio * Mathf.Sin(angulo)
            );

            personaje.position = nuevaPos;

            Vector3 dir = (centro - personaje.position);
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                personaje.rotation = Quaternion.LookRotation(dir);
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

    public void ExcluirNPCMuerto(Transform npcMuerto)
    {
        if (npcs.Contains(npcMuerto))
        {
            npcs.Remove(npcMuerto);
            Debug.Log($"{npcMuerto.name} eliminado de la lista de NPC activos.");
        }
    }
}
