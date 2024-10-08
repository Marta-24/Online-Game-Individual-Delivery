using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.Text;

public class ServerTCP : MonoBehaviour
{
    Socket socket;
    Thread mainThread = null;

    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string serverText;

    public struct User
    {
        public string name;
        public Socket socket;
    }

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = serverText;
    }

    public void startServer()
    {
        serverText = "Starting TCP Server...";

        // Create and bind the socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to any IP address on port 9050
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9050);
        socket.Bind(localEndPoint);

        // Put the socket in listening mode
        socket.Listen(10);
        serverText += "\nServer listening on port 9050...";

        // Start a thread to check for new connections
        mainThread = new Thread(CheckNewConnections);
        mainThread.Start();
    }

    void CheckNewConnections()
    {
        while (true)
        {
            User newUser = new User();
            newUser.name = "";

            // Accept any incoming clients
            newUser.socket = socket.Accept();

            // Get the remote endpoint (client info)
            IPEndPoint clientEP = (IPEndPoint)newUser.socket.RemoteEndPoint;
            serverText += "\nConnected with " + clientEP.Address.ToString() + " at port " + clientEP.Port.ToString();

            // Start a new thread to handle messages from this client
            Thread newConnection = new Thread(() => Receive(newUser));
            newConnection.Start();
        }
    }

    void Receive(User user)
    {
        byte[] data = new byte[1024];
        int recv;

        while (true)
        {
            try
            {
                recv = user.socket.Receive(data);
                if (recv == 0)
                    break;

                // Convert received data to string
                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                serverText += "\nReceived: " + receivedMessage;

                // Send a ping back to the client
                Send(user);
            }
            catch (SocketException)
            {
                // Handle socket exceptions
                break;
            }
        }

        // Close the socket when done
        user.socket.Close();
    }

    void Send(User user)
    {
        string pingMessage = "Ping";
        byte[] data = Encoding.ASCII.GetBytes(pingMessage);
        user.socket.Send(data); // Send to the client directly via their socket
        serverText += "\nSent ping to client: " + pingMessage;
    }
}
