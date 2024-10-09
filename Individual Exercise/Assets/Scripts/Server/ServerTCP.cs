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
        public string playerName;
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
        serverText = "Starting TCP Server...\n";

        // Get the local IP address
        string localIP = GetLocalIPAddress();
        serverText += $"Server IP Address: {localIP}\n";

        // Create and bind the socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9050);
        socket.Bind(localEndPoint);

        // Put the socket in listening mode
        socket.Listen(10);
        serverText += "\nServer listening on port 9050...";

        mainThread = new Thread(CheckNewConnections);
        mainThread.Start();
    }

    void CheckNewConnections()
    {
        while (true)
        {
            User newUser = new User();
            newUser.playerName = "";

            newUser.socket = socket.Accept();

            IPEndPoint clientEP = (IPEndPoint)newUser.socket.RemoteEndPoint;
            serverText += "\nConnected with " + clientEP.Address.ToString() + " at port " + clientEP.Port.ToString();

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

                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                serverText += "\nReceived: " + receivedMessage;

                Send(user);
            }
            catch (SocketException)
            {
                break;
            }
        }

        user.socket.Close();
    }

    void Send(User user)
    {
        string pingMessage = "Ping";
        byte[] data = Encoding.ASCII.GetBytes(pingMessage);
        user.socket.Send(data);
        serverText += "\nSent ping to client: " + pingMessage;
    }

    // Function to get the local IP address
    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1"; // Default local address if no valid IP is found
    }
}
