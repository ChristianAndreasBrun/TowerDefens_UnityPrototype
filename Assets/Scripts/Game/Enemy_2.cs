using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy_2 : EnemyManager
{
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteRight;
    public Sprite spriteLeft;

    SpriteRenderer spriteRenderer;
    Vector2 moveDirection;


    public override void Init(List<Vector2> corners, GameManager manager)
    {
        base.Init(corners, manager);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Mueve el enemigo
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, corners[currentCorner], speed * Time.deltaTime);

        moveDirection = (corners[currentCorner] - (Vector2)transform.localPosition).normalized;

        UpdateSpriteDirection();

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

    void UpdateSpriteDirection()
    {
        //Direccion de la mirada
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            spriteRenderer.sprite = (moveDirection.x > 0) ? spriteRight : spriteLeft;
        }
        else
        {
            spriteRenderer.sprite = (moveDirection.y > 0) ? spriteUp : spriteDown;
        }

        spriteRenderer.flipX = (moveDirection.x > 0);
    }
}
