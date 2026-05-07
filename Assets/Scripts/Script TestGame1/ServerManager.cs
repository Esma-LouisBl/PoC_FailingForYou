using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public class ServerManager : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI codeText;
    public TMP_InputField joinInput;
    
    async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    
    public async void StartServer()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        codeText.text = "ENTER THE CODE [" + joinCode + "]";

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );
        
        NetworkManager.Singleton.StartServer();
        gameManager.SetServer();
    }

    public async void StartClient()
    {
        string code = joinInput.text;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetClientRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );
        
        NetworkManager.Singleton.StartClient();
        gameManager.SetPlayer();
    }
}
