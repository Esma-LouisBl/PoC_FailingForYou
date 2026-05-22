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
    public GameObject miniGameServerUI, miniGamePlayerUI, votingServerUI, votingPlayerUI, tieServerUI, tieAccuratePlayerUI, nextMiniGamePlayerUI;
    public GameObject crushHair, crushAccessories, crushFaces, crushClothes; //Relative to Crush Creation
    private List<string> crushParts = new List<string>();
    public TextMeshProUGUI myNumberAsPlayerText;
    
    private GameObject startCrushButton;
    
    //RELATIVE TO MINIGAME
    public MG_QuestionManager questionManager;
    private int answerNumber = 1;
    public int totalAnswers;
    public GameObject voteButton1, voteButton2, voteButton3, voteButton4;
    public TextMeshProUGUI tieText;
    public TMP_InputField answerArea, accurateAnswerArea;
    public List<AccurateQuestion> accurateQuestions;
    public AccurateQuestion currentAccurateQuestion;

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

    public void ShowCrushPart1() //Appelé par le bouton du VIP
    {
        // StartCoroutine(DelayCrushLaunching());
        myPlayer.ShowCrushUIPlayerServerRpc();
    }

    // private IEnumerator DelayCrushLaunching()
    // {
    //     yield return new WaitForSeconds(myNumberAsPlayer);
    //     myPlayer.ShowCrushUIPlayerServerRpc();
    // }
    public IEnumerator ShowCrushPart2()    //Exécuté sur chaque téléphone
    {
        float timeToWait = myNumberAsPlayer;
        timeToWait /= 2.5f;
        // Debug.Log("temps : " + timeToWait);
        yield return new WaitForSeconds(timeToWait);
        
        int randomNumber = Random.Range(0, crushParts.Count);
        // int randomNumber = System.DateTime.Now.Second;
        // Debug.Log(randomNumber);
        // randomNumber *= myNumberAsPlayer;
        // Debug.Log($"Randomnumber = {randomNumber}");
        string partToShow =  crushParts[randomNumber];

        foreach (var part in crushParts)
        {
            // Debug.Log(part);
        }
        
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
        
        myPlayer.RemoveCrushPartServerRpc(partToShow);
    }

    public void RemoveCrushPart(string partToRemove)
    {
        crushParts.Remove(partToRemove);
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
        Debug.Log("on passe par la fonction");
        if (myPlayer.isVip)
        {
            startCrushButton.SetActive(true);
            Debug.Log("envie de caner");
        }
    }

    public void AllowCrushButton()
    {
        startCrushButton.GetComponent<Button>().interactable = true;
    }

    public void DisableCrushButton()
    {
        startCrushButton.GetComponent<Button>().interactable = false;
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
        answerArea.text = "Entrez votre réponse";   //ça joue le rôle d'un placeholder sinon la dernière réponse reste présente dans l'input field
        waitingUI.SetActive(false);
        tieAccuratePlayerUI.SetActive(false);
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
            answerNumber = 1;
        }
    }

    public void PlayerCanVote()
    {
        gameObject.GetComponent<GameManagerNetwork>().VotingPhaseClientRpc();
    }

    public void UpdateTotalAnswers()
    {
        totalAnswers = gameObject.GetComponent<GameManagerNetwork>().numberOfPlayers.Value;
    }
    public void PlayerCanVotePhase2()   //Côté client
    {
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

    public void Tie(PlayerNetwork player1, PlayerNetwork player2)
    {
        tieText.text = "C'est une égalité !";
        tieServerUI.SetActive(true);
        votingServerUI.SetActive(false);
        
        // 50-50 : soit au pif, soit question qui départage objectivement
        int r = Random.Range(0, 2);
        if (r == 0)
        {
            RandomTie(player1, player2);
        }
        else
        {
            AccurateQuestionTie(player1, player2);
        }
    }

    public void AccurateQuestionTie(PlayerNetwork player1, PlayerNetwork player2)
    {
        currentAccurateQuestion = accurateQuestions[Random.Range(0, accurateQuestions.Count)];
        tieText.text = $"Égalité ! Pour vous départager, répondez à cette question\n(le résultat est un nombre) :\n{currentAccurateQuestion.question}";
        gameObject.GetComponent<GameManagerNetwork>().InitDisplayAnswersAccurateTie(player1, player2);
    }

    public void ReceiveAccurateQuestion(int p1, int p2)
    {
        if (myNumberAsPlayer == p1+1 || myNumberAsPlayer == p2+1)
        {
            tieAccuratePlayerUI.SetActive(true);
            waitingUI.SetActive(false);
        }
    }

    public void ConfirmAccurateAnswer()
    {
        waitingUI.SetActive(true);
        tieAccuratePlayerUI.SetActive(false);
        myPlayer.SendAccurateAnswerServerRpc(int.Parse(accurateAnswerArea.text));
        accurateAnswerArea.text = "";
    }

    public void PublishAccurateWinner(PlayerNetwork winner)
    {
        tieText.text =
            $"Le joueur le plus proche est {winner.playerName} !\nLa réponse exacte est {currentAccurateQuestion.answer}";
        gameObject.GetComponent<GameManagerNetwork>().AskToShowNextMGButtonClientRpc();
    }
    
    public void RandomTie(PlayerNetwork player1, PlayerNetwork player2)
    {
        int r =  Random.Range(0, 2);
        if (r == 0)
        {
            StartCoroutine(RandomTieUnfolding(player1.playerName, player2.playerName, player1.playerName));
        }
        else
        {
            StartCoroutine(RandomTieUnfolding(player1.playerName, player2.playerName, player2.playerName));
        }
    }


    private IEnumerator RandomTieUnfolding(string name1, string name2, string winnerName)
    {
        yield return new WaitForSeconds(2);
        tieText.text = $"Le gagnant va être tiré au sort entre {name1} et {name2}.";
        yield return new WaitForSeconds(2);
        tieText.text = ".";
        yield return new WaitForSeconds(0.5f);
        tieText.text = "..";
        yield return new WaitForSeconds(0.5f);
        tieText.text = "...";
        yield return new WaitForSeconds(1);
        tieText.text = $"{winnerName} a remporté cette manche !";
        
        gameObject.GetComponent<GameManagerNetwork>().AskToShowNextMGButtonClientRpc();
    }

    public void ShowNextMGButton()  //Lancée uniquement sur le tel du VIP
    {
        nextMiniGamePlayerUI.SetActive(true);
        waitingUI.SetActive(false);
    }

    public void NextMGButtonPressed()   //Quand le bouton Lancer le mini-jeu pressé par VIP
    {
        myPlayer.RestartMiniGameServerRpc();
        nextMiniGamePlayerUI.SetActive(false);
    }

    private IEnumerator WaitBeforeAction()
    {
        yield return new WaitForSeconds(1);
        ShowCrushButton();
    }

}
