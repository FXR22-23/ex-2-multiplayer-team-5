using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<bool> clientConnected = new();
    public NetworkVariable<bool> isGameInProgress = new();
    public NetworkVariable<bool> hostIsIt = new();
    public Vector3[] playersInitialLocation;
    public GameObject countdownScreen;
    public TextMeshProUGUI canvasText;


    public override void OnNetworkSpawn()
    {
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        isGameInProgress.OnValueChanged += OnGameEnd;

        if (!IsHost)
            return;
        hostIsIt.Value = true;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.ConnectedClients.Count == 2)
        {
            clientConnected.Value = true;
            StartCoroutine(StartGameCountdown());
            CountdownClientRpc();
        }
    }

    [ClientRpc]
    private void CountdownClientRpc()
    {
        StartCoroutine(StartGameCountdown());
    }

    private IEnumerator StartGameCountdown()
    {
        countdownScreen.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            canvasText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        if (hostIsIt.Value)
        {
            canvasText.text = "Host is it! Go!";
        }
        else
        {
            canvasText.text = "Client is it! Go!";
        }
        yield return new WaitForSeconds(1.0f);
        countdownScreen.SetActive(false);
        isGameInProgress.Value = true;
    }

    private void OnGameEnd(bool previous, bool current)
    {
        if (current)
            return;
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        countdownScreen.SetActive(true);
        if (hostIsIt.Value)
        {
            canvasText.text = "Host win!";
        }
        else
        {
            canvasText.text = "Client win!";
        }
        yield return new WaitForSeconds(2f);
        countdownScreen.SetActive(false);
        hostIsIt.Value = !hostIsIt.Value;
        yield return new WaitForSeconds(1f);
        isGameInProgress.Value = true;
        // yield return StartGameCountdown();
    }
    
    [ServerRpc]
    public void PlacePlayersServerRpc()
    {
        int i = 0;
        foreach (NetworkClient client in NetworkManager.ConnectedClients.Values)
        {
            client.PlayerObject.transform.position = playersInitialLocation[i];
            i++;
        }
        isGameInProgress.Value = false;
    }
}
