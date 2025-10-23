using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairSeat : MonoBehaviour
{
    public Transform seatPoint;
    public float sitSpeed = 3f;

    public bool isOccupied = false;
    private GameObject occupant;

    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return;

        if (other.CompareTag("Player") || other.CompareTag("NPC"))
            SitDown(other.gameObject);
    }

    private void SitDown(GameObject character)
    {
        if (seatPoint == null) return;

        isOccupied = true;
        occupant = character;

        var rb = character.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        var controller = character.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        var sitControl = character.GetComponent<PlayerSitControl>();
        if (sitControl != null)
            sitControl.SitDown(seatPoint.position);

        var npcSitControl = character.GetComponent<NPCSitControl>();
        if (npcSitControl != null)
            npcSitControl.SitDown(seatPoint.position);

        StartCoroutine(MoveToSeat(character));
    }

    private IEnumerator MoveToSeat(GameObject character)
    {
        Vector3 startPos = character.transform.position;
        Quaternion startRot = character.transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * sitSpeed;
            character.transform.position = Vector3.Lerp(startPos, seatPoint.position, t);
            character.transform.rotation = Quaternion.Lerp(startRot, seatPoint.rotation, t);
            yield return null;
        }

        character.transform.position = seatPoint.position;
        character.transform.rotation = seatPoint.rotation;
    }

    public void StandUp()
    {
        if (occupant == null) return;

        var rb = occupant.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        var controller = occupant.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = true;

        var sitControl = occupant.GetComponent<PlayerSitControl>();
        if (sitControl != null)
            sitControl.StandUp();

        var npcSitControl = occupant.GetComponent<NPCSitControl>();
        if (npcSitControl != null)
            npcSitControl.StandUp();

        occupant = null;
        isOccupied = false;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }
}
