using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ����������� ���� "Mobs" ������� ����
        gameObject.layer = LayerMask.NameToLayer("Mobs");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
