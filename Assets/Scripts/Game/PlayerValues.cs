using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

static public class PlayerValues
{
    static public int lifes;


    static public void InitLife(int idMap)
    {
        switch (idMap)
        {
            case 0: lifes = 5; coins = 150; break;
            case 1: lifes = 5; coins = 180; break;
            case 2: lifes = 5; coins = 240; break;
            case 3: lifes = 5; coins = 280; break;
            case 4: lifes = 5; coins = 320; break;
        }
    }

    static public void RemoveLifes(int damage = 1)
    {
        lifes -= damage;
        if (lifes <= 0)
        {
            SceneManager.LoadScene(2);
        }
    }

    static public int coins;
    static public void RemoveCoins(int ruby)
    {
        coins -= ruby;
    }

    static public void AddCoins(int ruby)
    {
        coins += ruby;
    }
}
