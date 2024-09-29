using UnityEngine;
using System.Collections;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private string fullText;
    public float typingSpeed = 0.05f;

    private Color color;
    private string currentText = "";

    void Start()
    {
        fullText = Utils.GetInstructions();
        if (fullText.Length == 0) return;

        // split the text with "-"
        string[] splitText = fullText.Split('-');
        fullText = splitText[1];
        switch (splitText[0])
        {
            case "W":
                color = Color.white;
                break;
            case "B":
                color = Color.black;
                break;
            case "R":
                color = Color.red;
                break;
        }

        StartCoroutine(ShowText());

    }

    IEnumerator ShowText()
    {
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            textComponent.text = currentText;
            textComponent.color = color;
            AudioManager.Instance.SetTime(0.08f);
            if (fullText[i] == '\n')
            {
                AudioManager.Instance.PlaySound("ding", 0.1f);
            }
            else
            {
                AudioManager.Instance.PlaySound("type", 0.3f);
            }

            yield return new WaitForSeconds(Random.Range(typingSpeed - typingSpeed/2, typingSpeed + typingSpeed/2));
        }
        yield return new WaitForSeconds(5f);
        textComponent.text = "";
    }
}
