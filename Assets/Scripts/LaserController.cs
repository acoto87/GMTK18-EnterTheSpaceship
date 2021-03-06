﻿using UnityEngine;

public class LaserController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            var player = collider.GetComponent<PlayerController2D>();
            player.Respawn();
        }
    }
}
