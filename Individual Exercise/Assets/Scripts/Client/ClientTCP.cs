using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientTCP : MonoBehaviour
{
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string clientText;
    Socket server;

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;
    }

    public void StartClient()
    {
        // Start a new thread to connect to the server
        Thread connect = new Thread(Connect);
        connect.Start();
    }

    void Connect()
    {
        //TO DO 2
        // Create the server endpoint (IP and port) and initialize the client socket
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);  // Replace with your server's IP
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Connect to the server
            server.Connect(ipep);
            clientText = "Connected to server!";

            //TO DO 4
            // Start the send thread to send a message to the server
            Thread sendThread = new Thread(Send);
            sendThread.Start();

            //TO DO 7
            // Start the receive thread to receive messages from the server
            Thread receiveThread = new Thread(Receive);
            receiveThread.Start();
        }
        catch (SocketException e)
        {
            clientText = "Error connecting to server: " + e.Message;
        }
    }

    void Send()
    {
        //TO DO 4
        // Send a message to the server using the established connection
        string message = "Hello, Server!";
        byte[] data = Encoding.ASCII.GetBytes(message);

        try
        {
            // Send the message through the socket
            server.Send(data);
            clientText += "\nSent message: " + message;
        }
        catch (SocketException e)
        {
            clientText += "\nError sending message: " + e.Message;
        }
    }

    void Receive()
    {
        //TO DO 7
        // Receive messages from the server
        byte[] data = new byte[1024];
        int recv;

        while (true)
        {
            try
            {
                // Receive data from the server
                recv = server.Receive(data);
                if (recv == 0) break; // Connection closed

                // Convert the received data to string and append it to clientText
                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                clientText += "\nReceived: " + receivedMessage;
            }
            catch (SocketException e)
            {
                clientText += "\nError receiving message: " + e.Message;
                break;
            }
        }

        // Close the socket when done
        server.Close();
        clientText += "\nDisconnected from server.";
    }
}
