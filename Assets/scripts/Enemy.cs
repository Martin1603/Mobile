using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Configuracion")]
    public float tiempoAntesDeMatar = 2f;
    public float tiempoAntesDeReiniciar = 3f;

    private GameManager gameManager;
    private Chairs chairsScript;
    private bool rondaEnProgreso = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        chairsScript = FindObjectOfType<Chairs>();
    }

    void Update()
    {
        if (!rondaEnProgreso && TodasLasSillasOcupadas())
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

        yield return new WaitForSeconds(tiempoAntesDeMatar);

        NPCSitControl[] npcs = FindObjectsOfType<NPCSitControl>();
        PlayerSitControl player = FindObjectOfType<PlayerSitControl>();

        // Matar a los que no esten sentados
        foreach (var npc in npcs)
        {
            if (!npc.IsSitting())
                npc.Morir();
        }

        if (player != null && !player.IsSitting())
            player.Morir();

        yield return new WaitForSeconds(tiempoAntesDeReiniciar);

        // Levantar a los que sobrevivieron
        foreach (var npc in npcs)
        {
            if (npc != null && npc.gameObject.activeSelf && npc.IsSitting())
                npc.StandUp();
        }

        // Reactivar busqueda de silla
        NPCChairSeeker[] seekers = FindObjectsOfType<NPCChairSeeker>();
        foreach (var seeker in seekers)
        {
            if (seeker != null && seeker.gameObject.activeSelf)
                seeker.ResetChairSearch();
        }

        // Llamar al GameManager
        if (gameManager != null)
            gameManager.TerminarRonda();

        // Espera breve antes de reactivar monitoreo
        yield return new WaitForSeconds(1f);
        rondaEnProgreso = false;
    }
}
