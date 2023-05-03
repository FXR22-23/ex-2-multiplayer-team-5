using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManagement : NetworkBehaviour
{
    public GameManager gameManager;
    public PlayerController playerController;
    public Vector3 initialHostPosition = new Vector3(-20,0,20);
    public Vector3 initialClientPosition = new Vector3(20,0,-20);
    public String title;
    private void Start()
    {
        
        playerController = GetComponent<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        if (IsHost)
        {
            title = "Host";
        }
        else
        {
            title = "Client";
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!gameManager)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        gameManager.isGameInProgress.OnValueChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(bool previous, bool current)
    {
        if (current)
        {
            playerController.Enable();
        }
        else
        {
            playerController.Disable();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        gameManager.PlacePlayersServerRpc();
    }
}
