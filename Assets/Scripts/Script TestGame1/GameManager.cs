using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{

    public GameObject playerUI, serverUI, connectionUI, playerCrushUI, serverCrushUI, playerNameUI, playerCharacterUI, playerCrushNameUI, waitingUI, isSabotageUI, qrCodeUI;
    public GameObject miniGameServerUI, miniGamePlayerUI, votingServerUI, votingPlayerUI;
    public GameObject crushHair, crushAccessories, crushFaces, crushClothes; //Relative to Crush Creation
    private List<string> crushParts = new List<string>();
    public TextMeshProUGUI myNumberAsPlayerText;
    
    private GameObject startCrushButton;
    
    public MG_QuestionManager questionManager;
    private int answerNumber = 1;
    public int totalAnswers;
    public GameObject voteButton1, voteButton2, voteButton3, voteButton4;

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
            qrCodeUI.SetActive(false);
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
            gameObject.GetComponent<QRCodeManager>().InitQRCode();
        }
    }

    public void ShowCrushPart1()
    {
        myPlayer.ShowCrushUIPlayerServerRpc();
    }
    public void ShowCrushPart2()
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

    public void ShowCrushServer()
    {
        myPlayer.ShowCrushUIServerRpc();
    }

    public void ShowCrushServerPart2()
    {
        serverCrushUI.SetActive(true);
        serverUI.SetActive(false);
    }

    public void PlayerSendCrush(int spriteNumber)
    {
        myPlayer.SendCrushServerRpc(spriteNumber);
    }

    public void PlayerSendCrushName(string enteredName)
    {
        myPlayer.SendCrushNameServerRpc(enteredName);
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
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void ShowCrushButton()
    {
        if (myPlayer.isVip)
        {
            startCrushButton.SetActive(true);
            CheckIfEverybodyReady();
        }
    }

    public void CheckIfEverybodyReady()
    {
        myPlayer.AskServerReadyToCreateCrushServerRpc();
        if (myPlayer.everybodyReady)
        {
            startCrushButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            startCrushButton.GetComponent<Button>().interactable = false;
            StartCoroutine(RecheckAfterTime());
        }
    }

    public void ShowCrushName()
    {
        waitingUI.SetActive(false);
        playerCrushNameUI.SetActive(true);
    }

    public void HideCrushName()
    {
        waitingUI.SetActive(true);
        playerCrushNameUI.SetActive(false);
    }

    public void ShowMiniGameServer() //5ème fonction MG Launch
    {
        miniGameServerUI.SetActive(true);
        
        questionManager.ChooseQuestion();
    }

    public void AskPlayerToShowServerMiniGame() //2ème fonction MG Launch
    {
        myPlayer.ShowMiniGameServerRpc();
        miniGamePlayerUI.SetActive(true);
        waitingUI.SetActive(false);
    }

    public void PlayerSentAnswer(string answer) //Appelée par QuestionManager quand bouton cliqué
    {
        HideMiniGamePlayer();
        myPlayer.SendAnswerServerRpc(answer);
    }
    public void HideMiniGamePlayer()
    {
        waitingUI.SetActive(true);
        miniGamePlayerUI.SetActive(false);
    }

    public void InitializeVotes()   //Appelée par le serveur quand tout le monde a soumis sa réponse
    {
        miniGameServerUI.SetActive(false);
        votingServerUI.SetActive(true);
        
        // questionManager.PrintWinner("Louis le gros bg");
        StartCoroutine(WaitToShowAnswer());
    }

    private IEnumerator WaitToShowAnswer()
    {
        questionManager.ShowAnswer(answerNumber);
        yield return new WaitForSeconds(5);
        if (answerNumber < totalAnswers)
        {
            answerNumber++;
            StartCoroutine(WaitToShowAnswer());
        }
        else
        {
            PlayerCanVote();
        }
    }

    public void PlayerCanVote()
    {
        gameObject.GetComponent<GameManagerNetwork>().VotingPhaseClientRpc();
    }

    public void UpdateTotalAnswers(int answers)
    {
        Debug.Log("Là on met à jour");
        // Debug.Log(answers);
        totalAnswers = gameObject.GetComponent<GameManagerNetwork>().numberOfPlayers.Value;
        Debug.Log(totalAnswers);
    }
    public void PlayerCanVotePhase2()   //Côté client
    {
        Debug.Log("juste pour checker les totalAnswers du GM");
        Debug.Log(totalAnswers);
        if (totalAnswers > 2)
        {
            voteButton3.SetActive(true);
            if (totalAnswers == 4)
            {
                voteButton4.SetActive(true);
            }
        }

        switch (myNumberAsPlayer)
        {
            case 1:
                voteButton1.GetComponent<Button>().interactable = false;
                break;
            case 2:
                voteButton2.GetComponent<Button>().interactable = false;
                break;
            case 3:
                voteButton3.GetComponent<Button>().interactable = false;
                break;
            case 4:
                voteButton4.GetComponent<Button>().interactable = false;
                break;
        }  //À ne pas utiliser dans le jeu final vu que si l'ordre des réponses ne change pas, on sait de qui elles viennent
        votingPlayerUI.SetActive(true);
        waitingUI.SetActive(false);
    }

    
    public void HasVoted(int answerVotedNumber)
    {
        waitingUI.SetActive(true);
        votingPlayerUI.SetActive(false);
        myPlayer.HasVotedServerRpc(answerVotedNumber);
    }

    private IEnumerator WaitBeforeAction()
    {
        yield return new WaitForSeconds(1);
        ShowCrushButton();
    }

    private IEnumerator RecheckAfterTime()  //pas opti du tout, vaudrait mieux relancer la fonction lorsque chaque joueur qui n'est pas vip entre son nom
    {
        yield return new WaitForSeconds(1);
        CheckIfEverybodyReady();
    }

}
