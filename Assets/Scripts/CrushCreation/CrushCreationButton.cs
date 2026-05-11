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

    public Sprite chosen;
    public int spriteInt;
    
    private PlayerNetwork playerNetwork;

    public void Select(Sprite sprite)
    {
        chosen = sprite;
        previewImage.sprite = chosen;
    }
    
    public void Confirm()
    {
        playerNetwork = FindFirstObjectByType<PlayerNetwork>();
        playerNetwork.SendCrushServerRpc(spriteInt);
    }
}
