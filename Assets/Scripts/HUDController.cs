using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Health")]
    [SerializeField] private TMP_Text healthText;

    private void Update()
    {
        UpdateScore();
        UpdateHealth();
    }

    private void UpdateScore()
    {
        if (ScoreManager.Instance == null || scoreText == null)
            return;

        scoreText.text = "Score: " + ScoreManager.Instance.Score;
    }

    private void UpdateHealth()
    {
        if (ScoreManager.Instance == null || healthText == null)
            return;

        healthText.text = "Health: " + ScoreManager.Instance.Health;
    }
}