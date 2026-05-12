using UnityEngine;

public class InputSender : MonoBehaviour
{
    private PlayerNetwork player;
    
    // public GameObject uiPrefab;

    void Start()
    {
        player = GetComponent<PlayerNetwork>();

        // if (player.IsOwner)
        // {
        //     Instantiate(uiPrefab);
        // }
    }

    void Update()
    {
        if (!player.IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.SendInputServerRpc(2);
        }
    }

    //OUTDATED
    public void Move()
    {
        if (!player.IsOwner) return;

        player.SendInputServerRpc(1);
    }
    
    public void Jump()
    {
        if (!player.IsOwner) return;
        
        player.SendInputServerRpc(2);
    }
    
    public void SabotageWithButton(int targetNumber)
    {
        player.SendSabotageServerRpc(targetNumber, gameObject.GetComponent<PlayerNetwork>().playerNumber);
    }
}
