using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret_1 : TurretManager
{
    private EnemyManager currentEnemy;
    float fireRate;
    public float rateTime;
    public int damage;
    public float areaOfEffectRadius;
    public GameObject projectile;  

    [SerializeField] private AudioSource boltShootSFX;


    private void Start()
    {
        areaRender.color = areaColor;
        areaRender.transform.localScale = new Vector3(area, area, 1);
    }

    private void Update()
    {
        if (active)
        {
            if (currentEnemy == null)
            {
                EnemyDetect();
            }
            else
            {
                AttackEnemy();
            }
        }
    }

    private void OnMouseEnter()
    {
        areaRender.enabled = true;
    }
    private void OnMouseExit()
    {
        areaRender.enabled = false;
    }
    private void OnMouseDown()
    {
        if (evolve != null)
        {
            if (evolve.GetComponent<TurretManager>().rubyCost <= PlayerValues.coins)
            {
                manager.GetTurretEvolve(this);
            }
        }
    }

    void EnemyDetect()
    {
        //Detecta enemigos dibujando un círculo
        Collider2D[] detect = Physics2D.OverlapCircleAll(transform.position, area, detectMask);

        if (detect.Length > 0)
        {
            float distance = Mathf.Infinity;
            EnemyManager tempEnemy = null;

            for (int i = 0; i < detect.Length; i++)
            {
                float currentDistance = Vector2.Distance(transform.position, detect[i].transform.position);

                //Si está más cerca de "distance", actualiza
                if (Vector2.Distance(transform.position, detect[i].transform.position) < distance)
                {
                    distance = currentDistance;
                    tempEnemy = detect[i].GetComponent<EnemyManager>();
                }
            }
            currentEnemy = tempEnemy;
        }
    }

    void AttackEnemy()
    {
        // Dirección
        Vector2 dir = currentEnemy.transform.localPosition - transform.localPosition;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion finalRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // Rota la torreta apuntando al enemigo
        transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, 900 * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentEnemy.transform.position) > area)
        {
            currentEnemy = null;
            return;
        }

        fireRate += Time.deltaTime;
        if (fireRate >= rateTime)
        {
            fireRate = 0;
            // Dispara un proyectil:
            Shoot(dir);

            // Sonido
            boltShootSFX.Play();
        }
    }

    void Shoot(Vector2 direction)
    {
        // Instancia el proyectil usando el ProjectileManager
        GameObject projectilePrefab = Instantiate(projectile, transform.position, Quaternion.identity);

        // Obtiene el componente ProjectileManager del proyectil
        ProjectileManager projectileComponent = projectilePrefab.GetComponent<ProjectileManager>();

        // Configura la dirección y el daño del proyectil
        projectileComponent.SetDirection(direction, damage, areaOfEffectRadius);
    }
}
