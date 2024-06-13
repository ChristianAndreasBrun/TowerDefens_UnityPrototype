using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public float projectileSpeed = 10f;
    private int damaged = 1;
    private float areaEffect;
    public GameObject thunder;

    [SerializeField] private AudioSource thunderStrikeSFX;


    private void Update()
    {
        transform.Translate(Vector2.right * projectileSpeed * Time.deltaTime);
    }

    public void SetDirection(Vector2 direction, int damage, float areaEffect)
    {
        transform.right = direction.normalized;
        this.damaged = damage;
        this.areaEffect = areaEffect;

        Destroy(gameObject, 2f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el proyectil colisiona con un enemigo
        EnemyManager enemy = collision.GetComponent<EnemyManager>();
        if (enemy != null)
        {
            // Aplica da�o al enemigo
            thunderStrikeSFX.Play();
            enemy.GetDamage(damaged);

            // Aplica da�o en �rea a otros enemigos cercanos
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, areaEffect);

            //Debug.Log("Numero de colisiones: " + colliders.Length);

            foreach (Collider2D col in colliders)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Enemy") && col.gameObject != collision.gameObject)
                {
                    EnemyManager nearbyEnemy = col.GetComponent<EnemyManager>();
                    if (nearbyEnemy != null)
                    {
                        Instantiate(thunder, transform.position, transform.rotation);
                        nearbyEnemy.GetDamage(damaged);
                        //Debug.Log("Da�o aplicado");
                    }
                }
            }

            // Destruye el proyectil despu�s de impactar
            Destroy(gameObject);
        }
    }
}
