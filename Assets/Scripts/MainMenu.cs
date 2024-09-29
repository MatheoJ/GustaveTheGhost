using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void OnClickLevel(int index)
    {
        PlayClickSound();
        ChooseLevel(index);
    }
    public void OnClickBack()
    {
        PlayClickSound();
        SetState(State.NONE);
    }
    public void OnClickExit()
    {
        PlayClickSound();
        Application.Quit();
    }
    public void OnClickPlay()
    {
        PlayClickSound();
        SetState(State.CHOOSE_LEVEL);
    }
    public void OnClickControls()
    {
        PlayClickSound();
        SetState(State.CONTROLS);
    }

    public void OnClickExitInPause()
    {
        PlayClickSound();
        SceneManager.LoadScene("MainMenu");
    }
    private void PlayClickSound() => AudioManager.Instance.PlaySound("select", 0.2f);
    enum State
    {
        NONE,
        CHOOSE_LEVEL,
        CONTROLS
    }

    [SerializeField] GameObject _defaultButtons;
    [SerializeField] GameObject _levelButtons;
    [SerializeField] GameObject _controlPanel;

    State state { get; set; }

    void Start()
    {
        SetState(State.NONE);
        UpdateLevels();
    }
    void UpdateLevels()
    {
        if (_levelButtons == null) return;
        // create list of buttons
        var buttons = new List<Button>();
        foreach (Transform child in _levelButtons.transform)
        {
            if (child.name.Contains("Back")) continue;
            var button = child.GetComponent<Button>();
            if (button != null) buttons.Add(button);
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].interactable = i <= Utils.NextSceneToPlay;
        }
    }
    void SetState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.NONE:
                _defaultButtons.SetActive(true);
                if (_levelButtons != null) _levelButtons.SetActive(false);
                _controlPanel.SetActive(false);
                break;
            case State.CHOOSE_LEVEL:
                _defaultButtons.SetActive(false);
                _levelButtons.SetActive(true);
                _controlPanel.SetActive(false);
                break;
            case State.CONTROLS:
                _defaultButtons.SetActive(false);
                _controlPanel.SetActive(true);
                if (_levelButtons != null) _levelButtons.SetActive(false);
                break;
        }
    }

    void ChooseLevel(int index)
    {
        SceneManager.LoadScene("Level_" + index);
    }
}
