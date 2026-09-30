using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameOver {  get; private set; }
    public GameObject winPanel;
    public GameObject losePanel;

    void Awake()
    {
        Instance = this;
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }
    public void Win()
    {
        if (IsGameOver) return; // cegah dipanggil dobel
        IsGameOver = true;
        if (winPanel != null) winPanel.gameObject.SetActive(true);
        Time.timeScale = 0f; // hentikan game
    }
    public void Lose()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        if (losePanel != null) losePanel.gameObject.SetActive(true);
        Time.timeScale = 0f; // hentikan game
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // penting: kembalikan dulu sebelum reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
