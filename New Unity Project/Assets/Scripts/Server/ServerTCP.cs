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
        Debug.Log("Server started");

    }


    void Update()
    {
        UItext.text = serverText;

    }


    public void startServer()
    {
        serverText = "Starting TCP Server...";
        Debug.Log("initializing startserver()");

        //TO DO 1
        //Create and bind the socket
        //Any IP that wants to connect to the port 9050 with TCP, will communicate with this socket
        //Don't forget to set the socket in listening mode

        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 9050); 
        // crees un endpoint (explicació a ClientTCP.cs) sense cap IP ja que el server vol escoltar, quan rebi un misatge ja tindra tota aquesta info

        Debug.Log("IPEndpointworked");

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // creacio d'un socket, explicació a ClientTCP.cs
        socket.Bind(localEp);
        socket.Listen(10); // Aquestes dos funcions serveixen per posar el servidor a escoltar si algu envia un misatge a aquesta direccio ho rebra

        Debug.Log("Socket worked");


        //TO DO 3
        //TIme to check for connections, start a thread using CheckNewConnections
        CheckNewConnections();


       //mainThread = new Thread(CheckNewConnections);
        //mainThread.Start();
    }

    void CheckNewConnections()
    {
        //while(true)
       // {
            int recv = 0;
            Debug.Log("starting checkNewConnections()");
            User newUser = new User();
            newUser.name = "";

            //TO DO 3
            //TCP makes it so easy to manage conections, so we are going
            //to put it to use
            //Accept any incoming clients and store them in this user.
            //When accepting, we can now store a copy of our server socket
            //who has established a communication between a
            //local endpoint (server) and the remote endpoint(client)
            //If you want to check their ports and adresses, you can acces
            //the socket's RemoteEndpoint and LocalEndPoint
            //try printing them on the console

            Debug.Log("starting socket.Accept()");
            
            newUser.socket = socket.Accept(); // acceptar la nova conecció amb el client, ell la demana i aqui s'accepta
            // aqui es guarda la info del client

            Debug.Log("starting clientEP");
            IPEndPoint clientep = (IPEndPoint)socket.RemoteEndPoint;


            //TO DO 5
            //For every client, we call a new thread to receive their messages. 
            //Here we have to send our user as a parameter so we can use it's socket.
            Debug.Log("starting newconnection");
            Thread newConnection = new Thread(() => Receive(newUser)); // Recieve() call en una thread
            newConnection.Start();
        //}
        //This users could be stored in the future on a list
        //in case you want to manage your connections

    }

    void Receive(User user)
    {
        //TO DO 5
        //Create an infinite loop to start receiving messages for this user
        //You'll have to use the socket function receive to be able to get them.
        Debug.Log("starting recieve function");

        byte[] data = new byte[1024];
        int recv = 0;

        while (true)
        {
            data = new byte[1024];
            
            recv = user.socket.Receive(data); // aqui es rep el misatge

            if (recv == 0)
            {
                Debug.Log("recv was null"); // per si hi han errors
                break;
            }
            else
            {
                Debug.Log("recv recieved: " + Encoding.ASCII.GetString(data, 0, recv));
                serverText = serverText + "\n" + Encoding.ASCII.GetString(data, 0, recv); // printing mesage
            }

            //TO DO 6
            //We'll send a ping back every time a message is received
            //Start another thread to send a message, same parameters as this one.
            Thread answer = new Thread(() => Send(user)); // creem una thread per enviar una resposta, user s'ha guardat abans i tenim el EndPoint del client
            answer.Start();
        }
    }

    //TO DO 6
    //Now, we'll use this user socket to send a "ping".
    //Just call the socket's send function and encode the string.
    void Send(User user)
    {
        byte[] data = new byte[1024];
        Debug.Log("sending ping");
        data = Encoding.ASCII.GetBytes("ping");
        user.socket.Send(data); // funcio per enviar un ping
    }
}
