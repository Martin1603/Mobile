using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Configuraci�n")]
    public float tiempoAntesDeMatar = 2f; // Tiempo de espera antes de eliminar a los que no se sentaron
    public float tiempoAntesDeReiniciar = 3f; // Tiempo antes de que se reinicie la ronda

    private GameManager gameManager;
    private Chairs chairsScript;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        chairsScript = FindObjectOfType<Chairs>();
    }

    void Update()
    {
        // Si todas las sillas est�n ocupadas, empieza el proceso de eliminaci�n
        if (TodasLasSillasOcupadas())
        {
            StartCoroutine(MatarYReiniciar());
            enabled = false; // Desactiva este script hasta la pr�xima ronda
        }
    }

    private bool TodasLasSillasOcupadas()
    {
        if (chairsScript == null) return false;

        foreach (GameObject silla in chairsScript.GetSillas())
        {
            ChairSeat seat = silla.GetComponent<ChairSeat>();
            if (seat != null && !seat.IsOccupied())
            {
                return false; // A�n hay sillas disponibles
            }
        }
        return true;
    }

    private IEnumerator MatarYReiniciar()
    {
        yield return new WaitForSeconds(tiempoAntesDeMatar);

        // Buscar todos los NPCs y el Player que no est�n sentados
        NPCSitControl[] npcs = FindObjectsOfType<NPCSitControl>();
        PlayerSitControl player = FindObjectOfType<PlayerSitControl>();

        foreach (var npc in npcs)
        {
            if (!npc.IsSitting())
            {
                npc.Morir();
            }
        }

        if (player != null && !player.IsSitting())
        {
            player.Morir();
        }

        yield return new WaitForSeconds(tiempoAntesDeReiniciar);

        // Llamar al GameManager para terminar la ronda
        if (gameManager != null)
            gameManager.TerminarRonda();

        // Reactivar el script para la pr�xima ronda
        yield return new WaitForSeconds(1f);
        enabled = true;
    }
}
