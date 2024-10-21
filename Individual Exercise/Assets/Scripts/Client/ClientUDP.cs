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
    public GameObject usernameInputField; // New field for username
    TextMeshProUGUI UItext;
    string clientText;
    string playerName; // Store the player's name
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
        if (ipInputField == null || usernameInputField == null)
        {
            Debug.LogError("ipInputField or usernameInputField is not assigned in the Inspector!");
            return;
        }

        TMP_InputField inputField = ipInputField.GetComponent<TMP_InputField>();
        TMP_InputField usernameField = usernameInputField.GetComponent<TMP_InputField>();

        if (inputField == null || usernameField == null)
        {
            Debug.LogError("ipInputField or usernameInputField does not contain a TMP_InputField component!");
            return;
        }

        string serverIP = inputField.text;
        playerName = usernameField.text; // Get the entered username

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Username cannot be empty!");
            return;
        }

        if (!IPAddress.TryParse(serverIP, out IPAddress ipAddress))
        {
            Debug.LogError("Invalid IP address!");
            return;
        }

        Thread mainThread = new Thread(() => Send(ipAddress));
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

    void Send(IPAddress serverIP)
    {
        IPEndPoint ipep = new IPEndPoint(serverIP, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        string handshake = playerName + " has joined the game."; // Use the player's name
        byte[] data = Encoding.ASCII.GetBytes(handshake);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);

        clientText = "Sent handshake to server: " + handshake;

        // Start a thread to receive the response
        Thread receive = new Thread(Receive);
        receive.Start();

        // Wait for a short period to check if a response is received
        if (!receive.Join(5000)) // Wait for 5 seconds
        {
            clientText = "Server is not responding. Unable to join the waiting room.";
            socket.Close();
            return;
        }

        // Set the flag to load the waiting room scene if server is reachable
        loadWaitingRoom = true;
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

            // Notify the Waiting Room Manager about the player's name
            if (receivedMessage.StartsWith("Welcome"))
            {
                WaitingRoomManager waitingRoomManager = FindObjectOfType<WaitingRoomManager>();
                if (waitingRoomManager != null)
                {
                    waitingRoomManager.DisplayPlayerInfo(playerName);
                }
            }
        }
        catch (SocketException e)
        {
            clientText = "Error receiving message: " + e.Message;
            socket.Close();
        }
    }

}
