using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // necesario para cambiar de escena

public class PlayerSitControl : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody rb;

    [Header("Estados")]
    public bool isSitting = false;          // estado real
    private bool forcedSitting = false;     // estado forzado temporal

    [Header("Animaciones")]
    public Animator animator;

    [Header("Sonido de Muerte")]
    public AudioSource audioSource;
    public AudioClip sonidoMuerte;

    [Header("Referencia GameManager")]
    public GameManager gameManager; // asigna desde el Inspector

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
        // ?? Reproducir sonido de muerte
        if (audioSource != null && sonidoMuerte != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }

        // Ejecutar animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("morir");
            StartCoroutine(AlTerminarMuerte());
        }
        else
        {
            // Si no hay animador, ir directo a perder
            IrEscenaPerder();
        }
    }

    private IEnumerator AlTerminarMuerte()
    {
        // Esperar a que la animación de morir termine
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("morir"))
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        float duracion = stateInfo.length;
        yield return new WaitForSeconds(duracion + 0.2f);

        IrEscenaPerder();
    }

    private void IrEscenaPerder()
    {
        SceneManager.LoadScene("perder");
    }
}
