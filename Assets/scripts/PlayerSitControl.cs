using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSitControl : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    public bool isSitting = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (playerMovement != null)
            playerMovement.enabled = !isSitting;
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

    public void Morir()
    {
        Debug.LogWarning($"MORIR() llamado desde: {new System.Diagnostics.StackTrace()}");
        // Aquí puedes poner una animación de muerte o simplemente desactivar el objeto
        gameObject.SetActive(false);
    }
}
