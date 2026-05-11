using UnityEngine;
using UnityEngine.UI;

public class CrushCreationButton : MonoBehaviour
{
    public Image previewImage;

    public Sprite sprite;

    public void Select()
    {
        previewImage.sprite = sprite;
    }
    
    public void Confirm()
    {
        Debug.Log("Confirm");
    }
}
