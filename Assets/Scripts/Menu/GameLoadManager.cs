using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoadManager : MonoBehaviour
{
    public void GoMenu()    //Funcion publica para verla en el Inspector
    {
        SceneManager.LoadScene(0);  //Cambio escena 0, que es el menu
    }
}
