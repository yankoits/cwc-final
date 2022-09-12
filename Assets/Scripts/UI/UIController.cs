using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] Image[] healthBar;
    [SerializeField] Image informationBlock;
    [SerializeField] TMP_Text informationText;

    private void OnEnable()
    {
        Messenger.AddListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
        Messenger.AddListener(GameEvent.GAME_OVER, OnGameOver);
        Messenger.AddListener(GameEvent.LEVEL_BEATEN, OnLevelBeaten);
        Messenger.AddListener(GameEvent.PAUSE, OnPause);
        Messenger.AddListener(GameEvent.UNPAUSE, OnUnpause);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
        Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
        Messenger.RemoveListener(GameEvent.LEVEL_BEATEN, OnLevelBeaten);
        Messenger.RemoveListener(GameEvent.PAUSE, OnPause);
        Messenger.RemoveListener(GameEvent.UNPAUSE, OnUnpause);
    }

    void Start()
    {
        OnHealthUpdated();
        informationBlock.gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public void OnHealthUpdated()
    {
        int health = GameManager.Instance.Player.health;

        for (int i = 0; i < health; i++)
            healthBar[i].gameObject.SetActive(true);
        for (int i = health; i < healthBar.Length; i++)
            healthBar[i].gameObject.SetActive(false);
    }

    private void OnGameOver()
    {
        informationText.text = "game over.";
        informationBlock.gameObject.SetActive(true);
    }

    private void OnPause()
    {
        informationText.text = "pause.";
        informationBlock.gameObject.SetActive(true);
    }
    private void OnUnpause()
    {
        informationBlock.gameObject.SetActive(false);
    }
    private void OnLevelBeaten()
    {
        informationText.text = "level completed.";
        informationBlock.gameObject.SetActive(true);
    }
}
