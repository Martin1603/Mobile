using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Configuracion")]
    public float tiempoAntesDeMatar = 2f;
    public float tiempoAntesDeReiniciar = 3f;

    [Header("Referencias")]
    public Animator animator; // <-- Nuevo: referencia al Animator del enemigo

    private GameManager gameManager;
    private Chairs chairsScript;
    private bool rondaEnProgreso = false;
    private bool yaEjecutadoEstaRonda = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        chairsScript = FindObjectOfType<Chairs>();

        // Buscar el Animator autom�ticamente si no se asigna
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (rondaEnProgreso || yaEjecutadoEstaRonda) return;

        if (TodasLasSillasOcupadas())
        {
            StartCoroutine(MatarYReiniciar());
        }
    }

    private bool TodasLasSillasOcupadas()
    {
        if (chairsScript == null) return false;

        foreach (GameObject silla in chairsScript.GetSillas())
        {
            ChairSeat seat = silla.GetComponent<ChairSeat>();
            if (seat != null && !seat.IsOccupied())
                return false;
        }
        return true;
    }

    private IEnumerator MatarYReiniciar()
    {
        rondaEnProgreso = true;

        // ?? Reproducir animaci�n antes de matar
        if (animator != null)
        {
            animator.SetTrigger("matar");
        }

        // Espera antes de matar (permite que se vea la animaci�n)
        yield return new WaitForSeconds(tiempoAntesDeMatar);

        NPCSitControl[] npcs = FindObjectsOfType<NPCSitControl>();
        PlayerSitControl player = FindObjectOfType<PlayerSitControl>();

        // Matar (desactivar) solo a los que NO estaban sentados
        foreach (var npc in npcs)
        {
            if (!npc.IsSitting())
                npc.Morir();
        }

        if (player != null && !player.IsSitting())
            player.Morir();

        // Avisar al GameManager inmediatamente (una sola vez)
        if (gameManager != null)
            gameManager.TerminarRonda();

        yaEjecutadoEstaRonda = true;

        // Esperar y luego levantar sobrevivientes para la pr�xima ronda
        yield return new WaitForSeconds(tiempoAntesDeReiniciar);

        foreach (var npc in npcs)
        {
            if (npc != null && npc.gameObject.activeSelf && npc.IsSitting())
                npc.StandUp();
        }

        if (player != null && player.gameObject.activeSelf)
        {
            var ps = player.GetComponent<PlayerSitControl>();
            if (ps != null && ps.IsSitting())
                ps.StandUp();
        }

        // Reiniciar b�squeda de sillas
        NPCChairSeeker[] seekers = FindObjectsOfType<NPCChairSeeker>();
        foreach (var seeker in seekers)
        {
            if (seeker != null && seeker.gameObject.activeSelf)
                seeker.ResetChairSearch();
        }

        rondaEnProgreso = false;
    }

    public void ResetForNewRound()
    {
        yaEjecutadoEstaRonda = false;
        rondaEnProgreso = false;
    }
}
