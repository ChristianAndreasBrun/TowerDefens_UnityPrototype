using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy_3 : EnemyManager
{
    Vector2 moveDirection;


    public override void Init(List<Vector2> corners, GameManager manager)
    {
        base.Init(corners, manager);
    }

    void Update()
    {
        //Mueve el enemigo
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, corners[currentCorner], speed * Time.deltaTime);

        moveDirection = (corners[currentCorner] - (Vector2)transform.localPosition).normalized;

        //Destuir objeto
        if ((Vector2)transform.localPosition == corners[currentCorner])
        {
            currentCorner++;
            if (currentCorner >= corners.Count)
            {
                manager.DestroyEnemy(gameObject);
                PlayerValues.RemoveLifes(damage);
                manager.UpdateHeart();
            }
        }
    }
}
