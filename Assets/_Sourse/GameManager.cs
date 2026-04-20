using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button restartButton;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    public void PlayerWin()
    {
        if (gameEnded) return;
        gameEnded = true;
        gameOverPanel.SetActive(true);
        winnerText.text = "Победа";
        Time.timeScale = 0f;
    }

    public void EnemyWin()
    {
        if (gameEnded) return;
        gameEnded = true;
        gameOverPanel.SetActive(true);
        winnerText.text = "Поражение";
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}