using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ServerUDP : MonoBehaviour
{
    Socket socket;

    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string serverText;

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
        startServer();
    }

    public void startServer()
    {
        serverText = "Starting UDP Server...\n";

        // Get the local IP address
        string localIP = GetLocalIPAddress();
        serverText += $"Server IP Address: {localIP}\n";

        // Create a UDP socket and bind it to port 9050
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Bind the socket to the local endpoint
        socket.Bind(ipep);

        serverText += "\nUDP Server bound to port 9050";

        // Start a new thread to receive incoming UDP messages
        Thread newConnection = new Thread(Receive);
        newConnection.Start();
    }

    void Update()
    {
        UItext.text = serverText;
    }

    void Receive()
    {
        int recv;
        byte[] data = new byte[1024];

        serverText = serverText + "\n" + "Waiting for new Client...";

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)(sender);

        while (true)
        {
            recv = socket.ReceiveFrom(data, ref remote);
            if (recv > 0)
            {
                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                serverText = serverText + "\n" + $"Message received from {remote.ToString()}: {receivedMessage}";

                // Send confirmation message back to the client
                Thread sendPingThread = new Thread(() => Send(remote, receivedMessage));
                sendPingThread.Start();
            }
        }
    }

    void Send(EndPoint remote, string receivedMessage)
    {
        // Extract player's name from the received message
        string name = receivedMessage.Split(' ')[0];
        string pingMessage = $"Welcome {name}, connection confirmed.";
        byte[] data = Encoding.ASCII.GetBytes(pingMessage);

        socket.SendTo(data, remote);
        serverText = serverText + "\n" + $"Ping sent to {remote.ToString()}";
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
