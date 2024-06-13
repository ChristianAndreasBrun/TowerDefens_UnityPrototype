using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public MenuControl[] menuControls;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();

        for (int i = 0; i < menuControls.Length; i++)
        {
            //Comprobar los datos guardados del PlayerPrefs
            menuControls[i].isCompleted = PlayerPrefs.GetInt("LevelComplete_" + i) == 1;
            print(PlayerPrefs.GetFloat("Timer_" + i) + "   " + PlayerPrefs.GetInt("Lifes_" + i));

            menuControls[i].btn.transform.Find("TimeText").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("Timer_" + i).ToString();
            menuControls[i].btn.transform.Find("LifeText").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Lifes_" + i).ToString();


            //Desbloquear la siguente pantalla:
            if (i > 0)
            {
                menuControls[i].btn.GetComponent<Button>().interactable = menuControls[i - 1].isCompleted;
            }
        }
    }

    public void ChangeScene(int idGame)
    {
        GameManager.idMap = idGame;
        SceneManager.LoadScene(1);
    }
}

[System.Serializable]
public class MenuControl
{
    public bool isCompleted;
    public string title;
    public GameObject btn;
}
