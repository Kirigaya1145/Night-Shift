using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject tutorialPanel;
    public GameObject creditPanel;
    public string gameSceneName = "SampleScene";
    void Start()
    {
        Time.timeScale = 1f;
        homePanel.SetActive(true);
        tutorialPanel.SetActive(false);
        creditPanel.SetActive(false);
    }
    public void OnStart() { SceneManager.LoadScene(gameSceneName); }
    public void OnTutorial() { tutorialPanel.SetActive(true); }
    public void CloseTutorial() { tutorialPanel.SetActive(false); }
    public void OnCredit()
    {
        homePanel.SetActive(false);
        creditPanel.SetActive(true);
    }
    public void CloseCredit()
    {
        creditPanel.SetActive(false);
        homePanel.SetActive(true);
    }
    public void OnExit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // supaya terasa saat tes di Editor
        #endif
    }
}
