using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // For using TextMeshPro UI elements

public class GameManager : MonoBehaviour
{
    private bool gameWon = false;  // To ensure the game ends after the first collision
    public TextMeshProUGUI winText;  // UI element to display the winner

    // This method is called when the object this script is attached to collides with another object
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a player and if the game is not already won
        if (!gameWon)
        {
            if (collision.gameObject.CompareTag("Player1"))
            {
                DisplayWinner("Player 1");
            }
            else if (collision.gameObject.CompareTag("Player2"))
            {
                DisplayWinner("Player 2");
            }
            else if (collision.gameObject.CompareTag("Player3"))
            {
                DisplayWinner("Player 3");
            }
            else if (collision.gameObject.CompareTag("Player4"))
            {
                DisplayWinner("Player 4");
            }
        }
    }

    // Method to display the winner and set gameWon to true
    void DisplayWinner(string player)
    {
        winText.text = player + " wins!";
        gameWon = true;
    }
}
