using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCChairSeeker : MonoBehaviour
{
    private NavMeshAgent agent;
    private NPCSitControl sitControl;

    private ChairSeat currentTargetChair;
    private bool hasChair = false;
    private bool buscandoSilla = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        sitControl = GetComponent<NPCSitControl>();

        if (agent == null)
        {
            enabled = false;
            return;
        }

        // NO iniciar la búsqueda aquí. Esperar a que GameManager llame BeginSearch().
    }

    // Nuevo método público para que GameManager inicie la búsqueda cuando convenga
    public void BeginSearch()
    {
        if (!buscandoSilla && !sitControl.IsSitting())
        {
            StartCoroutine(FindAndGoToChair());
        }
    }

    private IEnumerator FindAndGoToChair()
    {
        buscandoSilla = true;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

            if (sitControl.IsSitting())
            {
                buscandoSilla = false;
                yield break;
            }

            GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");

            if (chairs.Length == 0)
            {
                // No hay sillas aún, volver a intentar
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            GameObject closestChair = null;
            float shortestDistance = Mathf.Infinity;

            foreach (GameObject chair in chairs)
            {
                float distance = Vector3.Distance(transform.position, chair.transform.position);
                var chairSeat = chair.GetComponent<ChairSeat>();

                if (chairSeat != null && !chairSeat.IsOccupied())
                {
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestChair = chair;
                    }
                }
            }

            if (closestChair != null)
            {
                hasChair = true;
                currentTargetChair = closestChair.GetComponent<ChairSeat>();
                agent.SetDestination(currentTargetChair.transform.position);
                buscandoSilla = false;
                yield break;
            }

            // Si no encontró ninguna disponible, reintenta
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {
        if (!hasChair || currentTargetChair == null || sitControl.IsSitting())
            return;

        if (currentTargetChair.IsOccupied())
        {
            hasChair = false;
            currentTargetChair = null;
            agent.isStopped = false;

            if (!buscandoSilla)
                StartCoroutine(FindAndGoToChair());
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTargetChair.transform.position);

        if (distance < 1.5f)
        {
            agent.isStopped = true;
            currentTargetChair.SendMessage("OnTriggerEnter", GetComponent<Collider>(), SendMessageOptions.DontRequireReceiver);
            hasChair = false;
        }
    }

    public void ResetChairSearch()
    {
        if (sitControl.IsSitting())
            sitControl.StandUp();

        if (agent != null)
            agent.isStopped = false;

        hasChair = false;
        currentTargetChair = null;

        if (!buscandoSilla)
            StartCoroutine(FindAndGoToChair());
    }
}
