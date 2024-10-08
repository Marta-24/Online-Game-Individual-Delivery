using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using UnityEngine.SceneManagement;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    public GameObject UItextObj;
    public GameObject ipInputField;
    TextMeshProUGUI UItext;
    string clientText;
    bool loadWaitingRoom = false; // Flag to trigger loading the waiting room

    void Start()
    {
        if (UItextObj != null)
        {
            UItext = UItextObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("UItextObj is not assigned in the Inspector!");
        }
    }

    public void StartClient()
    {
        if (ipInputField == null)
        {
            Debug.LogError("ipInputField is not assigned in the Inspector!");
            return;
        }

        TMP_InputField inputField = ipInputField.GetComponent<TMP_InputField>();

        if (inputField == null)
        {
            Debug.LogError("ipInputField does not contain a TMP_InputField component!");
            return;
        }

        string serverIP = inputField.text;
        Thread mainThread = new Thread(Send);
        mainThread.Start();
    }

    void Update()
    {
        UItext.text = clientText;

        // Check if we need to load the waiting room scene
        if (loadWaitingRoom)
        {
            loadWaitingRoom = false; // Reset the flag
            SceneManager.LoadScene("WaitingRoomScene"); // Load the waiting room scene
        }
    }

    void Send()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        string playerName = "Player1"; // Replace with the player's actual name
        string handshake = playerName + " has joined the game.";
        byte[] data = Encoding.ASCII.GetBytes(handshake);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);

        clientText = "Sent handshake to server: " + handshake;

        // Set the flag to load the waiting room scene
        loadWaitingRoom = true;

        Thread receive = new Thread(Receive);
        receive.Start();
    }

    void Receive()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;

        byte[] data = new byte[1024];
        int recv;

        try
        {
            recv = socket.ReceiveFrom(data, ref Remote);
            string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
            clientText = $"Message received from {Remote.ToString()}: {receivedMessage}";
        }
        catch (SocketException e)
        {
            clientText = "Error receiving message: " + e.Message;
        }

        socket.Close();
    }
}
