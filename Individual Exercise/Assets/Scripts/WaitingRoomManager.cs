using UnityEngine;
using TMPro;

public class WaitingRoomManager : MonoBehaviour
{
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI chatText; // UI text element for chat display
    public TMP_InputField chatInputField; // Input field for typing messages

    void Start()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        DisplayPlayerInfo(playerName);
    }

    public void DisplayPlayerInfo(string playerName)
    {
        playerInfoText.text = $"Welcome, {playerName}! Waiting for other players...";
    }

    public void DisplayChatMessage(string message)
    {
        chatText.text += message + "\n"; // Append new messages to the chat display
    }

    // This method will be called when the player presses Enter or sends a message
    public void OnSendMessage()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            // Send the message to the server (you'll need to call the ClientTCP to handle this)
            FindObjectOfType<ClientTCP>().SendChatMessage(message);

            // Clear the input field after sending
            chatInputField.text = "";
        }
    }
}
