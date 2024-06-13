using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Variables
    public float speed;
    public float rotationSpeed;
    public float life;
    public int damage;
    public int coins;

    protected int currentCorner;
    protected List<Vector2> corners;
    protected GameManager manager;
    public GameObject smoke;


    public virtual void Init(List<Vector2> corners, GameManager manager)
    {
        this.corners = corners;
        this.manager = manager;
        currentCorner = 0;
        transform.localPosition = corners[0];
    }

    public virtual void GetDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            //Imprimir particulas
            Instantiate(smoke, transform.position, transform.rotation);

            PlayerValues.AddCoins(coins);
            manager.DestroyEnemy(gameObject);
        }
    }
}
