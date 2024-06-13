using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletManager : MonoBehaviour
{
    
    public float bulletSpeed = 10f;
    int damaged = 1;
    
    void Update()
    {
        transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);
    }


    public void SetDirection(Vector2 direction, int damage)
    {
        transform.right = direction.normalized;
        this.damaged = damage;

        Destroy(gameObject, 2f);
    }

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyManager enemy = collision.GetComponent<EnemyManager>();
        if (enemy != null)
        {
            //Debug.Log("He TOCADO!");
            enemy.GetDamage(damaged);
            Destroy(gameObject);
        }
    }
}
