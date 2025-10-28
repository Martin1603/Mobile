using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotador : MonoBehaviour
{
    public void Rotar(float cuanto)
    {
        transform.eulerAngles = Vector3.up* cuanto*360;
    }
}
