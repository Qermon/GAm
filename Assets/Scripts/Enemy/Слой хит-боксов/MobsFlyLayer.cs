using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsFlyLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Присваиваем слой "Mobs" объекту моба
        gameObject.layer = LayerMask.NameToLayer("MobsFly");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
