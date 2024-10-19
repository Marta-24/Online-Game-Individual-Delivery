using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System.Collections.Generic;

public class ServerUDP : MonoBehaviour
{
    Socket socket;
    Thread receiveThread = null;
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string serverText;

    private List<IPEndPoint> connectedClients = new List<IPEndPoint>();

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
        StartServer();
    }

    void Update()
    {
        UItext.text = serverText;
    }

    public void StartServer()
    {
        serverText = "Starting UDP Server...\n";

        // Get the local IP address
        string localIP = GetLocalIPAddress();
        serverText += $"Server IP Address: {localIP}\n";

        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 9050);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(localEp);

        serverText += "\nServer listening on port 9050...";
        receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();
    }

    void ReceiveMessages()
    {
        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;

        while (true)
        {
            data = new byte[1024];
            int recv = socket.ReceiveFrom(data, ref remote);
            string message = Encoding.ASCII.GetString(data, 0, recv);

            // If it's the first message, consider it as a "login" (username)
            if (!connectedClients.Contains((IPEndPoint)remote))
            {
                connectedClients.Add((IPEndPoint)remote);
                serverText += $"\nNew user connected: {message} from {remote}";
                BroadcastMessage($"{message} has joined the waiting room.", (IPEndPoint)remote);
            }
            else
            {
                // Broadcast the received message to all other clients
                BroadcastMessage(message, (IPEndPoint)remote);
            }
        }
    }

    void BroadcastMessage(string message, IPEndPoint sender)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        foreach (IPEndPoint client in connectedClients)
        {
            if (!client.Equals(sender)) // Don't send the message to the sender itself
            {
                socket.SendTo(data, client);
            }
        }
    }

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
