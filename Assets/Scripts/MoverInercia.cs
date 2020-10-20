﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverInercia : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D;
    public float Velocidade;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Rigidbody2D.velocity = new Vector2(Velocidade * GameController.instance.GameSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D.velocity = new Vector2(Velocidade * GameController.instance.GameSpeed, 0);
    }
}
