using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSitControl : MonoBehaviour
{
    private Rigidbody rb;
    public bool isSitting = false;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SitDown(Vector3 seatPosition)
    {
        // Si ya estaba sentado o el objeto no esta activo, salir
        if (isSitting || !gameObject.activeInHierarchy) return;

        isSitting = true;

        transform.position = seatPosition;
        transform.rotation = Quaternion.identity;

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

        animator.SetBool("sentado", true);
    }

    public void StandUp()
    {
        // Forzar liberacion sin return prematuro
        isSitting = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None;
        }

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            // Asegurarse de activar el agente y despausar
            agent.enabled = true;
            agent.isStopped = false;
        }

        animator.SetBool("sentado", false);
    }

    public bool IsSitting()
    {
        return isSitting;
    }

    public void Morir()
    {
        // Desactivar el objeto (comportamiento actual)
        gameObject.SetActive(false);
    }
}
