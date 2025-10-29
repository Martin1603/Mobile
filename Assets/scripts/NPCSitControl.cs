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
        // Inicia la animación de muerte si existe
        if (animator != null)
        {
            animator.SetTrigger("morir");
        }

        // Inicia una corrutina para esperar que termine la animación antes de desactivar el objeto
        StartCoroutine(DesactivarDespuesDeMorir());
    }

    private IEnumerator DesactivarDespuesDeMorir()
    {
        if (animator != null)
        {
            // Esperar hasta que la animación de muerte realmente empiece
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName("morir")) // ?? cambia "morir" por el nombre exacto de tu animación
            {
                yield return null; // esperar un frame
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            // Ahora sí estamos en la animación de muerte
            float duracion = stateInfo.length;

            // Esperar el tiempo real más un pequeño margen de seguridad
            yield return new WaitForSeconds(duracion + 0.2f);
        }
        else
        {
            // Si no hay animator, usar tiempo fijo
            yield return new WaitForSeconds(3f);
        }

        gameObject.SetActive(false);
    }
}
