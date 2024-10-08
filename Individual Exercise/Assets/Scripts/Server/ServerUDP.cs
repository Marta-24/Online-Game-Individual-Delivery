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
        serverText = "Starting UDP Server...";

        //TO DO 1
        // Create a UDP socket and bind it to port 9050
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Bind the socket to the local endpoint
        socket.Bind(ipep);

        serverText += "\nUDP Server bound to port 9050";

        //TO DO 3
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

        //TO DO 3
        // We don't know the remote endpoint yet, so we create a general one
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)(sender);

        while (true)
        {
            // Receive message from a remote client
            recv = socket.ReceiveFrom(data, ref remote);
            if (recv > 0)
            {
                // Convert the message to a string and update serverText
                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                serverText = serverText + "\n" + $"Message received from {remote.ToString()}: {receivedMessage}";

                //TO DO 4
                // After receiving a message, send a ping back to the client
                Thread sendPingThread = new Thread(() => Send(remote));
                sendPingThread.Start();
            }
        }
    }

    void Send(EndPoint remote)
    {
        //TO DO 4
        // Send a "Ping" message back to the client using the stored remote endpoint
        string pingMessage = "Ping";
        byte[] data = Encoding.ASCII.GetBytes(pingMessage);

        socket.SendTo(data, remote);
        serverText = serverText + "\n" + $"Ping sent to {remote.ToString()}";
    }
}
