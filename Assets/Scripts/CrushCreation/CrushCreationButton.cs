using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrushCreationButton : MonoBehaviour
{
    public Image previewImage;

    // public List<Sprite> hairList;
    // public List<Sprite> accessoriesList;
    // public List<Sprite> facesList;
    // public List<Sprite> clothesList;

    // public Sprite chosen;
    public int spriteInt;
    
    private PlayerNetwork playerNetwork;
    
    public GameManagerNetwork gameManagerNetwork;

    public void Select(int spriteNumber)
    {
        // chosen = sprite;
        // previewImage.sprite = chosen;
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
        playerNetwork = FindFirstObjectByType<PlayerNetwork>();
        playerNetwork.SendCrushServerRpc(spriteInt);
        playerNetwork.gameManager.HideCrush();
    }
}
