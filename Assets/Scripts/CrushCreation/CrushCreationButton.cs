using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrushCreationButton : MonoBehaviour
{
    public Image previewImage;

    public int spriteInt;
    
    private GameManager gameManager;
    
    public GameManagerNetwork gameManagerNetwork;

    public void Select(int spriteNumber)
    {
        spriteInt = spriteNumber;
        if (spriteInt <= 6)
        {
            previewImage.sprite = gameManagerNetwork.allCrushHairSprites[spriteInt-1];
        }
        else if (spriteInt <= 12)
        {
            previewImage.sprite = gameManagerNetwork.allCrushFacesSprites[spriteInt - 7];
        }
        else if (spriteInt <= 18)
        {
            previewImage.sprite = gameManagerNetwork.allCrushAccessoriesSprites[spriteInt - 13];
        }
        else
        {
            previewImage.sprite = gameManagerNetwork.allCrushClothesSprites[spriteInt - 19];
        }
    }
    
    public void Confirm()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.PlayerSendCrush(spriteInt);
        gameManager.HideCrush();

    }
}
