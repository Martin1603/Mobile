using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Configuracion")]
    public float tiempoAntesDeMatar = 2f;
    public float tiempoAntesDeReiniciar = 3f;

    private GameManager gameManager;
    private Chairs chairsScript;
    private bool rondaEnProgreso = false; // indica si la corutina está activa
    private bool rondaTerminada = false;  // indica si ya se ejecutó el matar de esta ronda

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        chairsScript = FindObjectOfType<Chairs>();
    }

    private void Update()
    {
        // Solo iniciar la corutina si la ronda no ha terminado
        if (!rondaEnProgreso && !rondaTerminada && TodasLasSillasOcupadas())
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

        // Espera antes de matar
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

        // Espera antes de levantar a los que sobrevivieron (los que estaban sentados)
        yield return new WaitForSeconds(tiempoAntesDeReiniciar);

        foreach (var npc in npcs)
        {
            if (npc != null && npc.gameObject.activeSelf && npc.IsSitting())
                npc.StandUp();
        }

        if (player != null && player.gameObject.activeSelf)
        {
            var playerSit = player.GetComponent<PlayerSitControl>();
            if (playerSit != null && playerSit.IsSitting())
                playerSit.StandUp();
        }

        // Reactivar búsqueda de sillas
        NPCChairSeeker[] seekers = FindObjectsOfType<NPCChairSeeker>();
        foreach (var seeker in seekers)
        {
            if (seeker != null && seeker.gameObject.activeSelf)
                seeker.ResetChairSearch();
        }

        // Llamar a GameManager para terminar la ronda
        if (gameManager != null)
            gameManager.TerminarRonda();

        // Marcar que ya se ejecutó el matar de esta ronda
        rondaTerminada = true;
        rondaEnProgreso = false;
    }

    // Llamar desde GameManager al iniciar nueva ronda para reiniciar la bandera
    public void ReiniciarRonda()
    {
        rondaTerminada = false;
    }
}
