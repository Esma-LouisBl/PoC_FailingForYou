using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MG_QuestionManager : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI questionTMP, aspectTMP, winnerTMP, answer1TMP, answer2TMP, answer3TMP, answer4TMP, answerToIlluminate;
    public Image chara1, chara2, chara3, chara4;
    
    public List<string> questionsList;
    public List<string> aspectsList;
    
    private int coroutineCounter = 0;

    private void Start()
    {
        winnerTMP.enabled = false;
        answer1TMP.enabled = false;
        answer2TMP.enabled = false;
        answer3TMP.enabled = false;
        answer4TMP.enabled = false;
    }

    public void ChooseQuestion()
    {
        int r = Random.Range(0, questionsList.Count);
        questionTMP.text = questionsList[r];

        questionsList.RemoveAt(r);
            
        r = Random.Range(0, aspectsList.Count);
        aspectTMP.text = "Répondez de manière " + aspectsList[r] + " !";
    }

    public void ConfirmAnswer()   //Fonction appelée par le client quand bouton cliqué
    {
        // gameManager.PlayerSentAnswer(answer.text);
        gameManager.PlayerSentAnswer(gameManager.answerArea.text);
    }

    public void ShowAnswer(int answerNumber)
    {
        switch (answerNumber)
        {
            case 1:
                answer1TMP.enabled = true;
                break;
            case 2:
                answer2TMP.enabled = true;
                break;
            case 3:
                answer3TMP.enabled = true;
                break;
            case 4:
                answer4TMP.enabled = true;
                break;
        }
    }

    public void PrintWinner(string winner, string winnerAnswer)
    {
        winnerTMP.text = "Le gagnant est";
        winnerTMP.enabled = true;
        StartCoroutine(Suspense(winner, winnerAnswer));
    }

    private void EndPrintWinner(string winner, string winnerAnswer)
    {
        coroutineCounter = 0;
        winnerTMP.text += " " + winner + " !";

        if (winnerAnswer == answer1TMP.text)
        {
            answerToIlluminate = answer1TMP;
        }
        else if (winnerAnswer == answer2TMP.text)
        {
            answerToIlluminate = answer2TMP;
        }
        else if (winnerAnswer == answer3TMP.text)
        {
            answerToIlluminate = answer3TMP;
        }
        else if (winnerAnswer == answer4TMP.text)
        {
            answerToIlluminate = answer4TMP;
        }
        
        StartCoroutine(IlluminateWinnerAnswer(answerToIlluminate));
        
        gameManager.gameObject.GetComponent<GameManagerNetwork>().AskToShowNextMGButtonClientRpc();
    }

    private IEnumerator Suspense(string winner, string winnerAnswer)
    {
        yield return new WaitForSeconds(0.6f);
        winnerTMP.text += ".";
        if (coroutineCounter < 2)
        {
            coroutineCounter++;
            StartCoroutine(Suspense(winner, winnerAnswer));
        }
        else
        {
            EndPrintWinner(winner, winnerAnswer);
        }
    }

    private IEnumerator IlluminateWinnerAnswer(TextMeshProUGUI answer)
    {
        yield return new WaitForSeconds(0.05f);
        answer.fontSize++;
        if (answer.fontSize <60)
        {
            StartCoroutine(IlluminateWinnerAnswer(answer));
        }
        else
        {
            if (coroutineCounter < 4)  //Pour caper à 4 le nombre de fois que les coroutines s'opèrent
            {
                StartCoroutine(InIlluminateWinnerAnswer(answer));
            }
        }
    }

    private IEnumerator InIlluminateWinnerAnswer(TextMeshProUGUI answer)
    {
        yield return new WaitForSeconds(0.05f);
        answer.fontSize--;
        if (answer.fontSize < 45)
        {
            StartCoroutine(IlluminateWinnerAnswer(answer));
            coroutineCounter++;
        }
        else
        {
            StartCoroutine(InIlluminateWinnerAnswer(answer));
        }
    }
}
