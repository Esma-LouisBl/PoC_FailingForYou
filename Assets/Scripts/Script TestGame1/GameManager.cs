using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public GameObject playerUI, serverUI, connectionUI, playerCrushUI, serverCrushUI, playerNameUI, playerCharacterUI, waitingUI;
    public GameObject crushHair, crushAccessories, crushFaces, crushClothes; //Relative to Crush Creation
    private List<string> crushParts = new List<string>();
    public TextMeshProUGUI myNumberAsPlayerText;
    
    private GameObject startCrushButton;

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
            startCrushButton = GameObject.FindWithTag("StartCrushButton");
            // startCrushButton.GetComponent<Button>().interactable = false;
            startCrushButton.SetActive(false);
            crushParts.Add("Hair");
            crushParts.Add("Accessories");
            crushParts.Add("Faces");
            crushParts.Add("Clothes");
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
        int randomNumber = Random.Range(0, crushParts.Count);
        string partToShow =  crushParts[randomNumber];
        switch (partToShow)
        {
            case "Hair":
                crushHair.SetActive(true);
                break;
            case "Accessories":
                crushAccessories.SetActive(true);
                break;
            case "Faces":
                crushFaces.SetActive(true);
                break;
            case "Clothes":
                crushClothes.SetActive(true);
                break;
        }
        playerCrushUI.SetActive(true);
        crushParts.Remove(partToShow);
    }

    public void HideCrush()
    {
        playerCrushUI.SetActive(false);
        waitingUI.SetActive(true);
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
        
        myPlayer.CheckVipServerRpc();
        StartCoroutine(WaitBeforeAction());
    }

    public void ShowCrushButton()
    {
        if (myPlayer.isVip)
        {
            startCrushButton.SetActive(true);
        }
    }

    private IEnumerator WaitBeforeAction()
    {
        yield return new WaitForSeconds(1);
        ShowCrushButton();
    }

}
