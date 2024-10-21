using UnityEngine;
using UnityEngine.SceneManagement;

public class UDPMenu : MonoBehaviour
{
    public void LoadUDPServer()
    {
        // Load the TCP scene
        SceneManager.LoadScene("UDPCreateGame");
    }

    public void LoadUDPClient()
    {
        // Load the UDP scene
        SceneManager.LoadScene("UDPJoinGame");
    }
}
