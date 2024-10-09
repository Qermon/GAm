using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehaviour : ProjectileWeaponBehaviour
{

    KnifeController kc;

    protected override void Start()
    {
        base.Start();
        kc = FindObjectOfType<KnifeController>();
    }

    void Update()
    {
        transform.position += direction * kc.speed * Time.deltaTime;    //Set the movement of the knife
    }

}