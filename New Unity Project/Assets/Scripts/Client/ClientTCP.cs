using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
//using UnityEngine.tvOS;

public class ClientTCP : MonoBehaviour
{
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string clientText;
    Socket server;

    // Start is called before the first frame update
    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        UItext.text = clientText;

    }

    public void StartClient()
    {
        Thread connect = new Thread(Connect);
        connect.Start();
    }
    void Connect()
    {
        //TO DO 2
        //Create the server endpoint so we can try to connect to it.
        //You'll need the server's IP and the port we binded it to before
        //Also, initialize our server socket.
        //When calling connect and succeeding, our server socket will create a
        //connection between this endpoint and the server's endpoint

        Debug.Log("connecting to server");
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050); 
        // Aqui crees un Endpoint, es com el final de una conexio, en aquest cas es la IP del servidor on s'ha de conectar 
        // 127.0.0.1 es la adresa local, el teu ordinador, si cambies aquesta a una altre ip et podras conectar al ordinador que vulguis

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Connect(ipep);
        // crees un socket (un EndPoint d'aqui, del client) i poses les definicions que vulguis, 
        // SocketType.Stream i ProtocolType.Tcp son les definicions de TCP, al UDP cambien
        // Si el servidor esta obert server.Connect hauria de funcionar


        if (server.Connected) { 
            Debug.Log("server connected"); 
            clientText = clientText + "\n" + "server connection established";
        } // Checkejant que tot estigui be

        //TO DO 4
        //With an established connection, we want to send a message so the server acknowledges us
        //Start the Send Thread
        

        Thread sendThread = new Thread(Send); //iniciar funcio Send
        sendThread.Start();

        //TO DO 7
        //If the client wants to receive messages, it will have to start another thread. Call Receive()
        Thread receiveThread = new Thread(Receive); 
        receiveThread.Start();
        // un cop enviat el misatge esperes una resposta
        // recorda que tot aixo es fa en una thread per no colapsar el joc ni el codi, aixo es fa a part

    }
    void Send()
    {
        //TO DO 4
        //Using the socket that stores the connection between the 2 endpoints, call the TCP send function with
        //an encoded message
        byte[] data = new byte[1024];
        Debug.Log("sending welcome");
        data = Encoding.ASCII.GetBytes("welcome");
        server.Send(data); // prepares data i la envies utilitzant el socket "server", el codi no hauria d'arribar aqui si no s'ha fet la coneccio
    }

    //TO DO 7
    //Similar to what we already did with the server, we have to call the Receive() method from the socket.
    void Receive()
    {
        byte[] data = new byte[1024];
        int recv = 0;
        recv = server.Receive(data); //funcio per rebre la data de un socket en especific (server), basicament es com escoltar

        clientText = clientText += "\n" + Encoding.ASCII.GetString(data, 0, recv);
    }

}
