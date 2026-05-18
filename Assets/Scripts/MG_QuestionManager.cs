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
    public TextMeshProUGUI questionTMP, aspectTMP, winnerTMP, answer1TMP, answer2TMP, answer3TMP, answer4TMP;
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
        r = Random.Range(0, aspectsList.Count);
        aspectTMP.text = "Répondez de manière " + aspectsList[r] + " !";
    }

    public void ConfirmAnswer(TextMeshProUGUI answer)   //Fonction appelée par le client quand bouton cliqué
    {
        gameManager.PlayerSentAnswer(answer.text);
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

    public void PrintWinner(string winner)
    {
        winnerTMP.text = "Le gagnant est";
        winnerTMP.enabled = true;
        StartCoroutine(Suspense(winner));
    }

    private void EndPrintWinner(string winner)
    {
        winnerTMP.text += " " + winner + " !";
    }

    private IEnumerator Suspense(string winner)
    {
        yield return new WaitForSeconds(0.6f);
        winnerTMP.text += ".";
        if (coroutineCounter < 2)
        {
            coroutineCounter++;
            StartCoroutine(Suspense(winner));
        }
        else
        {
            EndPrintWinner(winner);
        }
    }
}
