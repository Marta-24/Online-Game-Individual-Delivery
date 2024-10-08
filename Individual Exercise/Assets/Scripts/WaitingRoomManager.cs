using UnityEngine;
using TMPro;

public class WaitingRoomManager : MonoBehaviour
{
    public TextMeshProUGUI playerInfoText;

    // Call this method to display the player's information
    public void DisplayPlayerInfo(string playerName)
    {
        playerInfoText.text = "Welcome, " + playerName + "! Waiting for other players...";
    }
}
