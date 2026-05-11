using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public GameObject playerUI, serverUI, connectionUI, crushUI, playerNameUI, playerCharacterUI;
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
            startCrushButton.GetComponent<Button>().interactable = false;
            startCrushButton.SetActive(false);
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
        
        myPlayer.CheckVipServerRpc();
        // StartCoroutine(WaitForPlayers());
    }

    private IEnumerator WaitForPlayers()
    {
        Debug.Log("coco");
        yield return new WaitForSeconds(1f);
        if (myPlayer.isVip)
        {
            Debug.Log("normalement on est bon");
            startCrushButton.SetActive(true);
        }

        Debug.Log("bon bah jvais momurir");
    }
}
