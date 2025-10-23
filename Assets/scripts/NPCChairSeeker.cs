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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        sitControl = GetComponent<NPCSitControl>();

        if (agent == null)
        {
            enabled = false;
            return;
        }

        StartCoroutine(FindAndGoToChair());
    }

    private IEnumerator FindAndGoToChair()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

        GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");
        if (chairs.Length == 0) yield break;

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

        StartCoroutine(FindAndGoToChair());
    }
}
