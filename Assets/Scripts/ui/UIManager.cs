using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    bool _IsGameOver = false;
    int _SpectralUnitCount = 0;

    [SerializeField]
    GameObject gameOverScreen;
    [SerializeField]
    Text textSpectralUnitCount;
    [SerializeField]
    Text textBounceCounter;

    void Start()
    {
        gameOverScreen.SetActive(false);
    }

    void Update()
    {
        bool playerIsDead = GameObject.Find("Player").GetComponentInChildren<PlayerController>().isDead;        

        if (playerIsDead && !_IsGameOver)
        {
            StartCoroutine(ShowGameOverAfterDelay());
        }

        if (_IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        //TODO: Update Spectral Unit Count
        textSpectralUnitCount.text = _SpectralUnitCount.ToString();

        GameObject soul = GameObject.Find("Soul(Clone)");
        if (soul != null)
        {
            int bounceLeft = soul.GetComponent<Soul>().getBounceLeft();
            textBounceCounter.text = "Bounce Left: " + bounceLeft.ToString();
        }
        else
        {
            textBounceCounter.text = "";
        }

    }

    IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSeconds(2); // Attendez 2 secondes
        gameOverScreen.SetActive(true);
        _IsGameOver = true;
    }
}
