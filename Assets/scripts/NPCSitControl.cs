using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSitControl : MonoBehaviour
{
    private Rigidbody rb;
    private bool isSitting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SitDown(Vector3 seatPosition)
    {
        if (isSitting) return;
        isSitting = true;

        transform.position = seatPosition;
        transform.rotation = Quaternion.identity;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }

        public void StandUp()
    {
        if (!isSitting) return;

        isSitting = false;

        if (rb != null)
            rb.constraints = RigidbodyConstraints.None;
    }

    public bool IsSitting()
    {
        return isSitting;
    }
}
