using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerNetwork : NetworkBehaviour
{
    public int playerId, playerNumber;
    public bool isVip, readyToCreateCrush, everybodyReady;
    
    public GameManagerNetwork gameManagerNetwork;

    public GameManager gameManager;
    
    public string playerName;
    
    
    // [NotNull] public GameObject canvasJump, canvasHair, canvasFace, canvasBody, canvasAccessories;
    // private GameObject startCrushButton;

    public override void OnNetworkSpawn()
    {
        gameManagerNetwork = FindFirstObjectByType<GameManagerNetwork>();
        gameManager = FindFirstObjectByType<GameManager>();
        
        if (IsServer)
        {
            playerId = OwnerClientId.GetHashCode();
            FindFirstObjectByType<GameManagerNetwork>().RegisterPlayer(this);
            
            // startCrushButton.enabled = false;
        }
        StartCoroutine(InitWithDelay());
        if (IsOwner)
        {
            gameManager.myPlayer = this;
            
            StartCoroutine(GetIdWithDelay());
            // gameManagerNetwork.crushManager.playerRef = this;
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            //
            // startCrushButton = GameObject.FindWithTag("StartCrushButton");
            // startCrushButton.GetComponent<Button>().interactable = false;
            // startCrushButton.SetActive(false);
            
            SendInputServerRpc(12); //Update PlayerObjects List
        }
    }

    // public void LoadCrushCreation()
    // {
    //     if (IsOwner)
    //     {
    //         switch (playerNumber)
    //         {
    //             case 1:
    //                 canvasHair.SetActive(true);
    //                 break;
    //             case 2:
    //                 canvasFace.SetActive(true);
    //                 break;
    //             case 3:
    //                 canvasBody.SetActive(true);
    //                 break;
    //             case 4:
    //                 canvasAccessories.SetActive(true);
    //                 break;
    //         }
    //     }
    // }

    [ServerRpc]
    public void SendInputServerRpc(int input)
    {
        FindFirstObjectByType<GameManagerNetwork>().ReceiveInput(this, input);
    }

    [ServerRpc]
    public void ShowCrushUIServerRpc()
    {
        gameManager.ShowCrushServerPart2();
    }

    [ServerRpc]
    public void ShowCrushUIPlayerServerRpc()
    {
        gameManagerNetwork.ShowCrushClientRpc();
    }
    
    [ServerRpc]
    public void SendCrushServerRpc(int spriteNumber)
    {
        gameManagerNetwork.ReceiveCrush(this, spriteNumber);
    }

    [ServerRpc]
    public void SendCrushNameServerRpc(string enteredName)
    {
        gameManagerNetwork.ReceiveNameProposition(enteredName);
    }

    private IEnumerator InitWithDelay()
    {
        yield return new WaitForSeconds(0.02f);
        gameManagerNetwork.ReceiveInput(this, 3);
    }
    
    private IEnumerator GetIdWithDelay()
    {
        yield return new WaitForSeconds(0.02f);
        playerNumber = gameManager.myNumberAsPlayer;
        
        //canvasStartCrush.SetActive(true); //Je pose ça là pour le moment histoire de l'activer dès le début mais pas chez tout le monde
        // LoadCrushCreation();
        /*
        if (playerNumber >= 2)
        {
            LoadCrushCreation();
        }
        */
    }

    // public void ShowJumpButton()
    // {
    //     canvasJump.SetActive(true);
    // }

    [ServerRpc]
    public void SendPlayerNameServerRpc(string enteredName)
    {
        playerName = enteredName;
        gameManagerNetwork.ReceiveName(this, playerName);
    }

    [ServerRpc]
    public void SendPlayerCharacterSpriteServerRpc(int characterSprite)
    {
        gameManagerNetwork.ReceiveCharacterSprite(this, characterSprite);
        readyToCreateCrush = true;
    }
    
    public void ShowStartCrushButton()
    {
        // startCrushButton.SetActive(true);
    }

    [ServerRpc]
    public void CheckVipServerRpc()
    {
        gameManagerNetwork.ReceiveCheckVip(this);
    }

    [ServerRpc]
    public void AskServerReadyToCreateCrushServerRpc()
    {
        gameManagerNetwork.EverybodyReadyToCreateCrush(this);
    }
}
