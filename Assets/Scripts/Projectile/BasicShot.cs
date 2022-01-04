using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class BasicShot : Projectile
{
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        transform.Translate(travelDir * dt * travelSpeed);
    }
}