using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCSitControl : MonoBehaviour
{
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Collider col;

    [Header("Estados")]
    public bool isSitting = false;
    [HideInInspector] public bool isDead = false;

    [Header("Referencias")]
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip sonidoMuerte;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();
    }

    public void SitDown(Vector3 seatPosition)
    {
        if (isSitting || isDead || !gameObject.activeInHierarchy) return;

        isSitting = true;
        transform.position = seatPosition;
        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animator != null)
            animator.SetBool("sentado", true);
    }

    public void StandUp()
    {
        if (isDead) return;

        isSitting = false;

        if (rb != null)
            rb.constraints = RigidbodyConstraints.None;

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        if (animator != null)
            animator.SetBool("sentado", false);
    }

    public bool IsSitting() => isSitting;
    public bool EstaMuerto() => isDead;

    public void Morir()
    {
        if (isDead) return;
        isDead = true;

        // ?? Sonido de muerte
        if (audioSource != null && sonidoMuerte != null)
            audioSource.PlayOneShot(sonidoMuerte);

        // ?? Congelar movimiento
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (animator != null)
            animator.SetTrigger("morir");

        // ?? Desactivar colisionador para no interferir con sillas
        if (col != null)
            col.enabled = false;

        // ??? Eliminar tag
        gameObject.tag = "Untagged";

        // ?? Buscar y eliminar cualquier componente llamado "LegsAnimator" en hijos
        Component[] legsAnimators = GetComponentsInChildren(typeof(MonoBehaviour), true);
        foreach (var comp in legsAnimators)
        {
            if (comp != null && comp.GetType().Name == "LegsAnimator")
            {
                Destroy(comp);
                Debug.Log($"{name}: LegsAnimator eliminado correctamente");
            }
        }

        // ?? Buscar y eliminar cualquier componente llamado "TailAnimator2" en hijos
        Component[] tailAnimators = GetComponentsInChildren(typeof(MonoBehaviour), true);
        foreach (var comp in tailAnimators)
        {
            if (comp != null && comp.GetType().Name == "TailAnimator2")
            {
                Destroy(comp);
                Debug.Log($"{name}: TailAnimator2 eliminado correctamente");
            }
        }

        // ?? Avisar al GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.ExcluirNPCMuerto(transform);

        StartCoroutine(EsperarAnimacionMuerte());
    }

    private IEnumerator EsperarAnimacionMuerte()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName("morir"))
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            float duracion = stateInfo.length;
            yield return new WaitForSeconds(duracion + 0.3f);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
