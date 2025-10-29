using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSitControl : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody rb;

    [Header("Estados")]
    public bool isSitting = false;          // estado real
    private bool forcedSitting = false;     // estado forzado temporal

    public Animator animator;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (playerMovement != null)
            playerMovement.enabled = !IsSitting(); // ahora usa IsSitting() (real o forzado)
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
        // mantiene el bool activo si aún está forzado
    }

    public bool IsSitting()
    {
        // Devuelve true si está sentado realmente o si se forzó el estado
        return isSitting || forcedSitting;
    }

    public void SetForcedSitting(bool value)
    {
        forcedSitting = value;

        // Refleja visualmente el estado forzado
        if (animator != null)
            animator.SetBool("sentado", value || isSitting);
    }

    public void Morir()
    {
        Debug.LogWarning($"MORIR() llamado desde: {new System.Diagnostics.StackTrace()}");
        gameObject.SetActive(false);
    }
}
