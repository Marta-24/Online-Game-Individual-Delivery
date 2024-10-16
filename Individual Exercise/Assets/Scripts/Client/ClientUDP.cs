using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    public GameObject UItextObj;
    public TMP_InputField ipInputField;
    public TMP_InputField nameInputField;
    public TMP_InputField chatInputField;
    public TextMeshProUGUI chatDisplay;
    TextMeshProUGUI UItext;
    string clientText;
    IPEndPoint serverEndPoint;
    bool loadWaitingRoom = false;

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;

        // Check if we should load the waiting room scene
        if (loadWaitingRoom)
        {
            loadWaitingRoom = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoomScene");
        }

        // Check if Enter is pressed to send a chat message
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(chatInputField.text))
        {
            SendMessage(chatInputField.text);
            chatInputField.text = ""; // Clear input field after sending
        }
    }

    public void StartClient()
    {
        // Get the server IP and username
        string ipAddress = ipInputField.text;
        string username = nameInputField.text;

        serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 9050);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Send username as the first message to the server
        SendMessage(username);

        // Start receiving messages
        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        // After sending the username, load the waiting room scene
        loadWaitingRoom = true;
    }

    void SendMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        socket.SendTo(data, serverEndPoint);
        clientText += "\nSent message: " + message;
    }

    void ReceiveMessages()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;
        byte[] data = new byte[1024];

        while (true)
        {
            int recv = socket.ReceiveFrom(data, ref remote);
            string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);

            // Display received message in the chat box
            chatDisplay.text += "\n" + receivedMessage;
            clientText += "\nReceived: " + receivedMessage;
        }
    }
}
