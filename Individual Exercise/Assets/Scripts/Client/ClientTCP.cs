using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientTCP : MonoBehaviour
{
    public GameObject UItextObj;
    public TMP_InputField ipInputField;  // Input for IP address
    public TMP_InputField nameInputField;  // Input for username
    TextMeshProUGUI UItext;
    string clientText;
    Socket server;
    public TMP_InputField chatInputField;

    // Add the loadWaitingRoom flag
    bool loadWaitingRoom = false;

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;

        // Check for sending chat messages
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(chatInputField.text))
        {
            // Pass the chat input text as an argument to SendChatMessage
            SendChatMessage(chatInputField.text);
            chatInputField.text = ""; // Clear the input field
        }

        if (loadWaitingRoom)
        {
            loadWaitingRoom = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoomScene");
        }
    }


    public void StartClient()
    {
        // Store the player name in PlayerPrefs
        string playerName = nameInputField.text;
        PlayerPrefs.SetString("PlayerName", playerName);

        // Start a new thread to connect to the server
        Thread connect = new Thread(Connect);
        connect.Start();
    }

    void Connect()
    {
        // Get IP from the input field
        string ipAddress = ipInputField.text;

        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ipAddress), 9050); // Get IP dynamically
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            Debug.Log("Attempting to connect to the server...");
            server.Connect(ipep);
            clientText = "Connected to server!";
            Debug.Log("Server connection successful!");

            // Start the send and receive threads
            Thread sendThread = new Thread(Send);
            sendThread.Start();

            Thread receiveThread = new Thread(Receive);
            receiveThread.Start();

            loadWaitingRoom = true; // Load the waiting room after successful connection
        }
        catch (SocketException e)
        {
            clientText = "Error connecting to server: " + e.Message;
            loadWaitingRoom = false;
            Debug.LogError("Failed to connect to the server: " + e.Message);
        }
    }

    void Send()
    {
        // Send player name to the server
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        byte[] data = Encoding.ASCII.GetBytes(playerName);

        try
        {
            server.Send(data);
            clientText += "\nSent player name: " + playerName;
            Debug.Log("Player name sent to server: " + playerName);
        }
        catch (SocketException e)
        {
            clientText += "\nError sending player name: " + e.Message;
            Debug.LogError("Error sending player name: " + e.Message);
        }
    }

    public void SendChatMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            try
            {
                server.Send(data);
                Debug.Log("Chat message sent to server: " + message);
            }
            catch (SocketException e)
            {
                Debug.LogError("Error sending chat message: " + e.Message);
            }
        }
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        int recv;

        while (true)
        {
            try
            {
                recv = server.Receive(data);
                if (recv == 0) break;

                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);

                // Display the received chat message in the chat box
                FindObjectOfType<WaitingRoomManager>().DisplayChatMessage(receivedMessage);
                Debug.Log("Message received from server: " + receivedMessage);
            }
            catch (SocketException e)
            {
                Debug.LogError("Error receiving message: " + e.Message);
                break;
            }
        }

        server.Close();
        Debug.Log("Disconnected from the server.");
    }
}
