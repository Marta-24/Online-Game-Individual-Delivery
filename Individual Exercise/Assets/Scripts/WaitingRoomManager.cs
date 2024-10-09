using UnityEngine;
using TMPro;

public class WaitingRoomManager : MonoBehaviour
{
    public TextMeshProUGUI playerInfoText;

    void Start()
    {
        // Get the player's name from PlayerPrefs or any global storage
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        DisplayPlayerInfo(playerName);
    }


    // Call this method to display the player's information
    public void DisplayPlayerInfo(string playerName)
    {
        playerInfoText.text = $"Welcome, {playerName}! Waiting for other players...";
    }
}
