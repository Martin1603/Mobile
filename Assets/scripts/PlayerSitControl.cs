using System.Collections;
using UnityEngine;

public class PlayerSitControl : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody rb;

    [Header("Estados")]
    public bool isSitting = false;          // estado real
    private bool forcedSitting = false;     // estado forzado temporal

    [Header("Animaciones")]
    public Animator animator;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (playerMovement != null)
            playerMovement.enabled = !IsSitting(); // desactiva movimiento si está sentado o forzado
    }

    public void SitDown(Vector3 seatPosition)
    {
        if (isSitting) return;

        isSitting = true;
        transform.position = seatPosition;
        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (animator != null)
            animator.SetBool("sentado", true);
    }

    public void StandUp()
    {
        if (!isSitting) return;

        isSitting = false;

        if (rb != null)
            rb.constraints = RigidbodyConstraints.None;

        if (animator != null)
            animator.SetBool("sentado", forcedSitting ? true : false);
    }

    public bool IsSitting()
    {
        // Devuelve true si está sentado realmente o si se forzó el estado
        return isSitting || forcedSitting;
    }

    public void SetForcedSitting(bool value)
    {
        forcedSitting = value;

        if (animator != null)
            animator.SetBool("sentado", value || isSitting);
    }

    public void Morir()
    {
        // Si tiene animador, ejecutar animación de muerte antes de desaparecer
        if (animator != null)
        {
            animator.SetTrigger("morir"); // asegúrate de tener este trigger en tu Animator
            StartCoroutine(DesactivarDespuesDeMorir());
        }
        else
        {
            // Si no hay animador, desaparecer inmediatamente
            gameObject.SetActive(false);
        }
    }

    private IEnumerator DesactivarDespuesDeMorir()
    {
        if (animator != null)
        {
            // Esperar hasta que la animación de muerte realmente empiece
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName("morir")) // cambia "morir" por el nombre exacto del clip
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            // Esperar toda la duración de la animación
            float duracion = stateInfo.length;
            yield return new WaitForSeconds(duracion + 0.2f);
        }
        else
        {
            // Tiempo fijo si no hay animador
            yield return new WaitForSeconds(3f);
        }

        gameObject.SetActive(false);
    }
}
