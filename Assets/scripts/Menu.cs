using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("EscenaMartin");
    }

    public void Personalizar()
    {
        SceneManager.LoadScene("Personalizar");
    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit(); 
    }
}
