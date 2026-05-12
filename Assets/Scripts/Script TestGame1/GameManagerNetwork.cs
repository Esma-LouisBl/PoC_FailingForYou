using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class GameManagerNetwork : NetworkBehaviour
{
    private List<PlayerNetwork> players = new List<PlayerNetwork>();
    public NetworkVariable<int> numberOfPlayers;
    
    
    public List<PlayerScriptableObject> playerObjects;
    public List<Sprite> allPlayerSprites;
    
    
    //RELATIVE TO CRUSH CREATION
    public List<Sprite> allCrushHairSprites;
    public List<Sprite> allCrushFacesSprites;
    public List<Sprite> allCrushAccessoriesSprites;
    public List<Sprite> allCrushClothesSprites;
    
    public Sprite crushHair;
    public Sprite crushFace;
    public Sprite crushAccessory;
    public Sprite crushClothes;

    public Image crushHairSlot;
    public Image crushFaceSlot;
    public Image crushAccessorySlot;
    public Image crushClothesSlot;

    public Image crushCurtain;
    
    public int crushPartsChosen;

    public List<string> crushNamePropositions;
    public string crushName;

    // public CrushCreation crushManager;

    private bool readyToShowCrush;
    
    private bool canJump = true;
    private float playerHeight = 0.51f;
    private Vector3 jumpVector = new Vector3(0, 2f, 0);
    [SerializeField]
    private List<TextMeshProUGUI> joinSlots = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> nameSlots = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<Image> spriteSlots = new List<Image>();
    
    public void RegisterPlayer(PlayerNetwork player)
    {
        if (IsServer)
        {
            //player.GetComponentInChildren<Button>().gameObject.SetActive(false);
            players.Add(player);
            numberOfPlayers.Value = players.Count;
            //Debug.Log("Joueur ajouté. Total : " + players.Count);
        }
        //Note : Si un joueur s'en va, ça ne l'enlève pas de la liste
    }

    public void ReceiveInput(PlayerNetwork player, int input)
    {
        //Debug.Log($"Input reçu de {player} : {input}");
        switch (input)
        {
            case 1: //Move
                player.GetComponent<Transform>().position += Vector3.right;
                break;
            case 2: //Jump
                StartCoroutine(Jump(player));
                break;
            case 3: //Init Player
                if (gameObject.GetComponent<GameManager>().myNumberAsPlayer == 0 && !IsOwner)
                {
                    gameObject.GetComponent<GameManager>().myNumberAsPlayer = numberOfPlayers.Value;
                }
                playerHeight = 0.51f;
                player.gameManager.myNumberAsPlayerText.text = "Player : " + player.gameManager.myNumberAsPlayer;
                player.transform.position = new Vector3(numberOfPlayers.Value+0.2f, 0.5f, 0);
                player.GetComponentInChildren<TextMeshPro>().text = numberOfPlayers.Value.ToString();

                if (IsServer)
                {
                    //Debug.Log(player.gameManager.myNumberAsPlayer);
                    joinSlots[numberOfPlayers.Value-1].text = "RESERVED";
                    joinSlots[numberOfPlayers.Value-1].fontSize = 20;
                    if (players.Count > 1 && readyToShowCrush)
                    {
                        gameObject.GetComponent<GameManager>().ShowCrush();
                        FindFirstObjectByType<SpawnerBehavior>().numberOfPlayers = players.Count;
                    }
                }
                break;
            
            //Relative to Crush Creation
            // case 4:
            //     crushManager.ChangeHair(true);
            //     break;
            // case 5:
            //     crushManager.ChangeHair(false);
            //     break;
            // case 6:
            //     crushManager.ChangeFace(true);
            //     break;
            // case 7:
            //     crushManager.ChangeFace(false);
            //     break;
            // case 8:
            //     crushManager.ChangeBody(true);
            //     break;
            // case 9:
            //     crushManager.ChangeBody(false);
            //     break;
            // case 10:
            //     crushManager.ChangeAccessories(true);
            //     break;
            // case 11:
            //     crushManager.ChangeAccessories(false);
            //     break;
            
            //Relative to Player ScriptableObject
            case 12:
                PlayerScriptableObject playerObject = PlayerScriptableObject.CreateInstance<PlayerScriptableObject>();
                playerObject.playerNetwork = player;
                playerObject.playerId = playerObjects.Count;
        
                playerObjects.Add(playerObject);
                break;
            case 13:    //Assignation du nom du joueur dans son ScriptableObject
                // foreach (PlayerScriptableObject playerScriptableObject in playerObjects)
                // {
                //     if (player == playerScriptableObject.playerNetwork)
                //     {
                //         playerScriptableObject.playerName = player.playerName;
                //     }
                // }
                break;
        }
    }
    
    
    public void ReceiveName(PlayerNetwork player, string pName)
    {
        foreach (PlayerScriptableObject playerScriptableObject in playerObjects)
        {
            if (playerScriptableObject.playerNetwork == player)
            {
                playerScriptableObject.playerName = pName;
                nameSlots[playerScriptableObject.playerId].text = pName;
            }
        }
    }

    public void ReceiveCharacterSprite(PlayerNetwork player, int sprite)
    {
        foreach (PlayerScriptableObject playerScriptableObject in playerObjects)
        {
            if (playerScriptableObject.playerNetwork == player)     //Affichage du sprite + retire le texte
            {
                playerScriptableObject.playerSprite = allPlayerSprites[sprite-1];
                spriteSlots[playerScriptableObject.playerId].enabled = true;
                spriteSlots[playerScriptableObject.playerId].sprite = playerScriptableObject.playerSprite;
                joinSlots[playerScriptableObject.playerId].text = "";
            }
        }
    }

    public void ReceiveCrush(PlayerNetwork player, int sprite)
    {
        if (sprite <= 6)
        {
            crushHair = allCrushHairSprites[sprite-1];
        }
        else if (sprite <= 12)
        {
            crushFace = allCrushFacesSprites[sprite - 7];
        }
        else if (sprite <= 18)
        {
            crushAccessory = allCrushAccessoriesSprites[sprite - 13];
        }
        else
        {
            crushClothes = allCrushClothesSprites[sprite - 19];
        }

        crushPartsChosen++;     //On incrémente ce nombre pour vérifier si tous les joueurs ont choisi leur partie

        if (crushPartsChosen == playerObjects.Count)
        {
            crushHairSlot.sprite = crushHair;
            crushFaceSlot.sprite = crushFace;
            crushAccessorySlot.sprite = crushAccessory;
            crushClothesSlot.sprite = crushClothes;
            crushCurtain.gameObject.SetActive(false);
            Debug.Log("crush terminé (enfin)");
            
            AskForCrushNameClientRpc();
        }
    }

    [ClientRpc]
    public void AskForCrushNameClientRpc()
    {
        gameObject.GetComponent<GameManager>().ShowCrushName();
    }

    public void ReceiveCheckVip(PlayerNetwork player)
    {
        SendVipToClientRpc();
    }

    [ClientRpc]
    public void SendVipToClientRpc()
    {
        Debug.Log("fonction vip lancée");
        if (gameObject.GetComponent<GameManager>().myNumberAsPlayer == 1)
        {
            gameObject.GetComponent<GameManager>().myPlayer.isVip = true;
            Debug.Log("passé vip");
        }
    }
    
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            print(numberOfPlayers.Value.ToString());
        }
    }
    */

    private IEnumerator Jump(PlayerNetwork player)
    {
        if (playerHeight > player.transform.position.y)
        {
            canJump = false;
            for (int i = 0; i < 130; i++)
            {
                player.GetComponent<Transform>().position += (jumpVector/100);
                yield return new WaitForSeconds(0.001f*(i+1)/40);
            }
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < 130; i++)
            {
                player.GetComponent<Transform>().position -= (jumpVector/100);
                yield return new WaitForSeconds(0.001f*(100-i+1)/100);
            }
            canJump = true;
        }
    }
}
