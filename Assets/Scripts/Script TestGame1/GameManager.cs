using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public GameObject playerUI, serverUI, connectionUI, crushUI, playerNameUI, playerCharacterUI, isSabotageUI;
    public TextMeshProUGUI myNumberAsPlayerText;

    public NetworkVariable<int> numberOfPlayers;
    public int myNumberAsPlayer;
    public SpawnerBehavior spawner;
    
    public PlayerNetwork myPlayer;
    
    public void SetPlayer()
    {
        if(!IsOwner)
        {
            connectionUI.SetActive(false);
            playerUI.SetActive(true);
            playerNameUI.SetActive(true);
            playerCharacterUI.SetActive(false);
            //numberOfPlayers.Value++;
            //myNumberAsPlayer = numberOfPlayers.Value;
            //myNumberAsPlayerText.text = myNumberAsPlayer.ToString();
        }
    }
    
    public void SetServer()
    {
        if(IsOwner)
        {
            connectionUI.SetActive(false);
            serverUI.SetActive(true);
        }
    }

    public void ShowCrush()
    {
        crushUI.SetActive(true);
    }
    
    public void StartMiniGame()
    {
        spawner.StartSpawning();
    }

    public void PlayerNameButton(TextMeshProUGUI playerName)    //Used when Player click on the button to send their name
    {
        string absoluteName = playerName.text;
        myPlayer.SendPlayerNameServerRpc(absoluteName);
        playerNameUI.SetActive(false);
        playerCharacterUI.SetActive(true);
    }

    //Impossible de passer un Sprite dans un ServerRPC donc on attribue un numéro à chaque sprite pour le retrouver par la suite
    public void PlayerCharacterSpriteButton(int characterSprite)
    {
        myPlayer.SendPlayerCharacterSpriteServerRpc(characterSprite);
        playerCharacterUI.SetActive(false);
    }

    public void Sabotage()
    {
        StartCoroutine(SabotageRoutine());
    }

    private IEnumerator SabotageRoutine()
    {
        for (int i = 0; i < 10; i++)
        {
            isSabotageUI.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            isSabotageUI.SetActive(false);
        }
    }
}
