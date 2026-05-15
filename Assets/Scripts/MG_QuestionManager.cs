using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MG_QuestionManager : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI questionTMP, aspectTMP;
    
    public List<string> questionsList;
    public List<string> aspectsList;
    
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
}
