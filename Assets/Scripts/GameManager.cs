using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameOver {  get; private set; }
    public GameObject winPanel;
    public GameObject losePanel;

    public AudioSource bgmSource;      // musik latar (loop)
    public AudioSource sfxSource;      // opsional
    public AudioClip winClip;          // opsional
    public AudioClip loseClip;

    void Awake()
    {
        Instance = this;
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }
    void Start()
    {
        PlayBGM();
    }

    void PlayBGM()
    {
        if (bgmSource == null) return;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }
    public void Win()
    {
        if (IsGameOver) return; // cegah dipanggil dobel
        IsGameOver = true;
        StopBGM();
        if (winPanel != null) winPanel.gameObject.SetActive(true);
        Time.timeScale = 0f; // hentikan game
    }
    public void Lose()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        StopBGM();
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
