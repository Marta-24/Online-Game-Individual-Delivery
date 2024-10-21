using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadTCPScene()
    {
        // Load the TCP scene
        SceneManager.LoadScene("TCP");
    }

    public void LoadUDPScene()
    {
        // Load the UDP scene
        SceneManager.LoadScene("UDP");
    }
}
