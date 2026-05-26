using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManagerNetwork : NetworkBehaviour
{
    private List<PlayerNetwork> players = new List<PlayerNetwork>();
    public NetworkVariable<int> numberOfPlayers;
    
    
    public List<PlayerScriptableObject> playerObjects;
    public List<Sprite> allPlayerSprites;
    
    
    //RELATIVE TO CRUSH CREATION
    public List<Sprite> allCrushHairSprites;
    public List<Sprite> allCrushBackHairSprites;
    public List<Sprite> allCrushFacesSprites;
    public List<Sprite> allCrushAccessoriesSprites;
    public List<Sprite> allCrushClothesSprites;
    
    public Sprite crushHair;
    public Sprite crushBackHair;
    public Sprite crushFace;
    public Sprite crushAccessory;
    public Sprite crushClothes;

    public Image crushHairSlot;
    public Image crushBackHairSlot;
    public Image crushFaceSlot;
    public Image crushAccessorySlot;
    public Image crushClothesSlot;

    public Image crushCurtain;
    
    public int crushPartsChosen;

    public List<string> crushNamePropositions;
    public string crushName;
    public TextMeshProUGUI crushNameUI;
    public GameObject crushNameBg;

    // public CrushCreation crushManager;

    private bool readyToShowCrush; //on utilise plus ce bool

    public NetworkVariable<int> playersReadyForCrush;
    
    // private bool canJump = true;
    // private float playerHeight = 0.51f;
    // private Vector3 jumpVector = new Vector3(0, 2f, 0);
    [SerializeField]
    private List<TextMeshProUGUI> joinSlots = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<TextMeshProUGUI> nameSlots = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<Image> spriteSlots = new List<Image>();
    
    //RELATIVE TO MINIGAME
    public string answer1, answer2, answer3, answer4, bestAnswer;
    public int vote1, vote2, vote3, vote4, collectedAnswers;
    public int numberOfVotes;
    
    public PlayerNetwork nearestPlayerToAccurate;
    public int nearestIntToAccurate;
    
    public RawImage QRCodeImage;
    
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
                // StartCoroutine(Jump(player));
                break;
            case 3: //Init Player
                if (gameObject.GetComponent<GameManager>().myNumberAsPlayer == 0 && !IsOwner)
                {
                    gameObject.GetComponent<GameManager>().myNumberAsPlayer = numberOfPlayers.Value;
                }
                // playerHeight = 0.51f;
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
                        StartCoroutine(gameObject.GetComponent<GameManager>().ShowCrushPart2());
                        FindFirstObjectByType<SpawnerBehavior>().numberOfPlayers = players.Count;
                    }
                    else if (players.Count == 1)
                    {
                        QRCodeImage.enabled = true;
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

    public void IncreasePlayersReady() //Côté serv
    {
        playersReadyForCrush.Value++;
        if (playersReadyForCrush.Value == numberOfPlayers.Value)
        {
            AskToEnableCrushButtonClientRpc();
        }
        else
        {
            AskToDisableCrushButtonClientRpc();
        }
    }

    [ClientRpc]
    public void AskToEnableCrushButtonClientRpc()
    {
        gameObject.GetComponent<GameManager>().AllowCrushButton();
    }

    [ClientRpc]
    public void AskToDisableCrushButtonClientRpc()
    {
        gameObject.GetComponent<GameManager>().DisableCrushButton();
    }

    public void DisableCheckOnConnection()
    {
        AskToDisableCrushButtonClientRpc();
    }
    
    public void ReceiveCrush(PlayerNetwork player, int sprite)
    {
        if (sprite <= 6)
        {
            crushHair = allCrushHairSprites[sprite-1];
            crushBackHair = allCrushBackHairSprites[sprite-1];
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
            crushBackHairSlot.sprite = crushBackHair;
            crushFaceSlot.sprite = crushFace;
            crushAccessorySlot.sprite = crushAccessory;
            crushClothesSlot.sprite = crushClothes;
            crushCurtain.gameObject.SetActive(false);
            
            AskForCrushNameClientRpc();
        }
        //On en profite pour donner le nombre de joueurs au GameManager (côté serv)
        gameObject.GetComponent<GameManager>().totalAnswers = playerObjects.Count;
    }

    [ClientRpc]
    public void ShowCrushClientRpc()
    {
        StartCoroutine(gameObject.GetComponent<GameManager>().ShowCrushPart2());
    }

    [ClientRpc]
    public void RemoveCrushPartClientRpc(string partToRemove)
    {
        gameObject.GetComponent<GameManager>().RemoveCrushPart(partToRemove);
    }

    [ClientRpc]
    public void AskForCrushNameClientRpc()
    {
        gameObject.GetComponent<GameManager>().ShowCrushName();
    }

    public void ReceiveNameProposition(string enteredName)
    {
        crushNamePropositions.Add(enteredName);
        if (crushNamePropositions.Count == playerObjects.Count)
        {
            int r = Random.Range(0, crushNamePropositions.Count);
            crushName = crushNamePropositions[r];
            crushNameUI.text = crushName;
            crushNameBg.SetActive(true);

            StartCoroutine(WaitToAdmireCrushName());
        }
        
    }

    private IEnumerator WaitToAdmireCrushName()
    {
        yield return new WaitForSeconds(2f);
        
        InitializeMiniGameClientRpc(); //Lancement de la 1ère fonction pour launch le minigame
    }

    [ClientRpc]
    public void InitializeMiniGameClientRpc()   //1ère fonction MG Launch
    {
        gameObject.GetComponent<GameManager>().AskPlayerToShowServerMiniGame();
        
        //Actualisation du nombre de players pour le GameManager (côté client)
        gameObject.GetComponent<GameManager>().UpdateTotalAnswers();
    }

    public void ShowMiniGame()  //4ème fonction MG Launch
    {
        gameObject.GetComponent<GameManager>().ShowMiniGameServer();
        
        //J'en profite pour réinitialiser tout ça
        vote1 = 0;
        vote2 = 0;
        vote3 = 0;
        vote4 = 0;
        numberOfVotes = 0;
    }

    public void ReceiveAnswer(PlayerNetwork player, string answer)
    {
        if (player == playerObjects[0].playerNetwork)
        {
            answer1 = answer;
            gameObject.GetComponent<GameManager>().questionManager.answer1TMP.text = answer1;
        }

        if (playerObjects.Count > 1)
        {
            if (player == playerObjects[1].playerNetwork)
            {
                answer2 = answer;
                gameObject.GetComponent<GameManager>().questionManager.answer2TMP.text = answer2;
            }

            if (playerObjects.Count > 2)
            {
                if (player == playerObjects[2].playerNetwork)
                {
                    answer3 = answer;
                    gameObject.GetComponent<GameManager>().questionManager.answer3TMP.text = answer3;
                }

                if (playerObjects.Count == 4)
                {
                    if (player == playerObjects[3].playerNetwork)
                    {
                        answer4 = answer;
                        gameObject.GetComponent<GameManager>().questionManager.answer4TMP.text = answer4;
                    }
                }
            }
        }

        collectedAnswers++;
        if (collectedAnswers == playerObjects.Count)
        {
            collectedAnswers = 0;
            
            // gameObject.GetComponent<GameManager>().questionManager.chara1.sprite = playerObjects[0].playerSprite;
            //
            // if (playerObjects.Count > 1)
            // {
            //     gameObject.GetComponent<GameManager>().questionManager.chara2.sprite = playerObjects[1].playerSprite;
            //     if (playerObjects.Count > 2)
            //     {
            //         gameObject.GetComponent<GameManager>().questionManager.chara3.sprite = playerObjects[2].playerSprite;
            //         if (playerObjects.Count > 3)
            //         {
            //             gameObject.GetComponent<GameManager>().questionManager.chara4.sprite = playerObjects[3].playerSprite;
            //         }
            //     }
            // }
            
            gameObject.GetComponent<GameManager>().InitializeVotes();
        }
    }

    public void ReceiveAccurateAnswer(PlayerNetwork player, int answer)
    {
        if (collectedAnswers == 0)
        {
            collectedAnswers++;
            nearestPlayerToAccurate = player;
            nearestIntToAccurate = Mathf.Abs(gameObject.GetComponent<GameManager>().currentAccurateQuestion.answer - answer);
        }
        else
        {
            collectedAnswers = 0;
            if (Mathf.Abs(gameObject.GetComponent<GameManager>().currentAccurateQuestion.answer - answer) < nearestIntToAccurate )
            {
                nearestPlayerToAccurate = player;
            }
            gameObject.GetComponent<GameManager>().PublishAccurateWinner(nearestPlayerToAccurate);
        }
    }

    [ClientRpc]
    public void VotingPhaseClientRpc()
    {
        gameObject.GetComponent<GameManager>().PlayerCanVotePhase2();
    }

    public void ReceiveVote(int voteNumber)
    {
        switch (voteNumber)
        {
            case 1:
                vote1++;
                break;
            case 2:
                vote2++;
                break;
            case 3:
                vote3++;
                break;
            case 4:
                vote4++;
                break;
        }

        numberOfVotes++;
        if (numberOfVotes == playerObjects.Count)
        {
            string winner = playerObjects[0].playerName;
            if (vote1 > vote2 && vote1 > vote3 && vote1 > vote4)
            {
                winner = playerObjects[0].playerName;
                bestAnswer = playerObjects[0].playerNetwork.lastAnswer;
            }
            else if (vote2 > vote1 && vote2 > vote3 && vote2 > vote4)
            {
                winner = playerObjects[1].playerName;
                bestAnswer = playerObjects[1].playerNetwork.lastAnswer;
            }
            else if (vote3 > vote1 && vote3 > vote2 && vote3 > vote4)
            {
                winner = playerObjects[2].playerName;
                bestAnswer = playerObjects[2].playerNetwork.lastAnswer;
            }
            else if (vote4 > vote1 && vote4 > vote2 && vote4 > vote3)
            {
                winner = playerObjects[3].playerName;
                bestAnswer = playerObjects[3].playerNetwork.lastAnswer;
            }
            else
            {   // CAS D’ÉGALITÉ
                if (vote1 == vote2 && vote2 == vote3 && vote3 == vote4)
                {
                    gameObject.GetComponent<GameManager>().TieBetweenFour(playerObjects[0].playerNetwork, playerObjects[1].playerNetwork, playerObjects[2].playerNetwork, playerObjects[3].playerNetwork);
                }
                else if (vote1 == vote2)
                {
                    gameObject.GetComponent<GameManager>().Tie(playerObjects[0].playerNetwork, playerObjects[1].playerNetwork);
                }
                else if (vote1 == vote3)
                {
                    gameObject.GetComponent<GameManager>().Tie(playerObjects[0].playerNetwork, playerObjects[2].playerNetwork);
                }
                else if (vote1 == vote4)
                {
                    gameObject.GetComponent<GameManager>().Tie(playerObjects[0].playerNetwork, playerObjects[3].playerNetwork);
                }
                else if (vote2 == vote3)
                {
                    gameObject.GetComponent<GameManager>().Tie(playerObjects[1].playerNetwork, playerObjects[2].playerNetwork);
                }
                else if (vote2 == vote4)
                {
                    gameObject.GetComponent<GameManager>().Tie(playerObjects[1].playerNetwork, playerObjects[3].playerNetwork);
                }
                else if (vote3 == vote4)
                {
                    gameObject.GetComponent<GameManager>().Tie(playerObjects[2].playerNetwork, playerObjects[3].playerNetwork);
                }
                return;
            }
            //CAS DE VICTOIRE TOTALE
            gameObject.GetComponent<GameManager>().questionManager.PrintWinner(winner, bestAnswer);
        }
    }

    public void InitDisplayAnswersAccurateTie(PlayerNetwork player1, PlayerNetwork player2)
    {
        int p1 = 0;
        int p2 = 0;
        foreach (PlayerScriptableObject playerObject in playerObjects)
        {
            if (playerObject.playerNetwork == player1)
            {
                p1 = playerObject.playerId;
            }
            else if (playerObject.playerNetwork == player2)
            {
                p2 = playerObject.playerId;
            }
        }
        DisplayAnswersAccurateTieClientRpc(p1, p2);
    }
    [ClientRpc]
    public void DisplayAnswersAccurateTieClientRpc(int player1, int player2)
    {
        gameObject.GetComponent<GameManager>().ReceiveAccurateQuestion(player1, player2);
    }

    [ClientRpc]
    public void AskToShowNextMGButtonClientRpc()    //Appelée après victoire de MG pour lancer le suivant
    {
        if (gameObject.GetComponent<GameManager>().myPlayer.isVip)
        {
            gameObject.GetComponent<GameManager>().ShowNextMGButton();
        }
    }

    public void ReceiveCheckVip(PlayerNetwork player)
    {
        SendVipToClientRpc();
    }

    [ClientRpc]
    public void SendVipToClientRpc()
    {
        if (gameObject.GetComponent<GameManager>().myNumberAsPlayer == 1)
        {
            gameObject.GetComponent<GameManager>().myPlayer.isVip = true;
        }
    }
    
    public void ReceiveSabotage(int targetNumber, int playerNumber) //player selection
    {
        if (targetNumber >= playerNumber)
        {
            targetNumber += 1;
        }
        SabotageTargetPlayerClientRpc(targetNumber);
    }

    [ClientRpc]
    public void SabotageTargetPlayerClientRpc(int targetPlayer)
    {
        if (targetPlayer == gameObject.GetComponent<GameManager>().myNumberAsPlayer)
        {
            gameObject.GetComponent<GameManager>().Sabotage();
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

    // private IEnumerator Jump(PlayerNetwork player)
    // {
    //     if (playerHeight > player.transform.position.y)
    //     {
    //         canJump = false;
    //         for (int i = 0; i < 130; i++)
    //         {
    //             player.GetComponent<Transform>().position += (jumpVector/100);
    //             yield return new WaitForSeconds(0.001f*(i+1)/40);
    //         }
    //         yield return new WaitForSeconds(0.1f);
    //         for (int i = 0; i < 130; i++)
    //         {
    //             player.GetComponent<Transform>().position -= (jumpVector/100);
    //             yield return new WaitForSeconds(0.001f*(100-i+1)/100);
    //         }
    //         canJump = true;
    //     }
    // }
}
