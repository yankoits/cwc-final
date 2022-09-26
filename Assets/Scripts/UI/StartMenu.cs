using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_InputField playerName;
    [SerializeField] Button startButton;

    private void OnEnable()
    {
        Messenger.AddListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
    }
    public void OnStartButton()
    {
        string name = playerName.text;
        if (name == "")
            name = "anonymous";
        GameManager.Instance.Score.SetPlayerName(name);
        SceneManager.LoadScene("Level1");
    }
    void Start()
    {
        startButton.interactable = false;
        string highScorer = PlayerPrefs.GetString("Player");
        if (highScorer != "")
            highScorer = $" / {highScorer}";
        highScoreText.text = $"High score: {PlayerPrefs.GetInt("HighScore")}{highScorer}";
    }

    private void OnManagersStarted()
    {
        startButton.interactable = true;
    }
}
