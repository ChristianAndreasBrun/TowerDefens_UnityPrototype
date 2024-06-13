using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public int rubyCost;
    public float area;
    public LayerMask detectMask;
    public SpriteRenderer areaRender;
    public Color areaColor;
    public GameObject evolve;

    protected GameManager manager;
    protected bool active;

    public virtual void InitTurret(GameManager manager)
    {
        this.manager = manager;
        active = true;
        areaRender.enabled = false;
    }
}
