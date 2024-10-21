using UnityEngine;
using UnityEngine.SceneManagement;

public class TCPMenu : MonoBehaviour
{
    public void LoadTCPServer()
    {
        // Load the TCP scene
        SceneManager.LoadScene("TCPCreateGame");
    }

    public void LoadTCPClient()
    {
        // Load the UDP scene
        SceneManager.LoadScene("TCPJoinGame");
    }
}
