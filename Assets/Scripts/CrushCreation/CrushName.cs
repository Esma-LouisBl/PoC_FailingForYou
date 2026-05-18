using TMPro;
using UnityEngine;

public class CrushName : MonoBehaviour
{
    // public TextMeshProUGUI crushNameTMP;
    // private PlayerNetwork player;
    
    private GameManager gameManager;

    public void ConfirmName(TextMeshProUGUI tmp)
    {
        // player = FindFirstObjectByType<PlayerNetwork>();
        // player.SendCrushNameServerRpc(tmp.text);
        // player.gameManager.HideCrushName();
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.PlayerSendCrushName(tmp.text);
        gameManager.HideCrushName();
    }
}
