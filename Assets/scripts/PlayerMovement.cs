using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveVector;
    public float moveSpeed = 8f;
    public float rotationSpeed = 10f;


    public void InputPlayer(InputAction.CallbackContext _context)
    {
        moveVector = _context.ReadValue<Vector2>();
    }

    
    void Update()
    {
        Vector3 movement = new Vector3(moveVector.x, 0, moveVector.y);
        movement.Normalize();
        if (movement.sqrMagnitude > 0.01f)
        {
            // Calcula la rotación objetivo
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);

            // Rota suavemente hacia la dirección
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Mueve al personaje hacia adelante
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
