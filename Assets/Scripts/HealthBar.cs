using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject character;
    private AbstractCharacter characterScript;
    public Slider slider;
    public Image fill;

    private Color fullHealthColor = Color.green;
    private Color zeroHealthColor = Color.red;
    // Start is called before the first frame update
    private void Awake()
    {
        /*Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;*/
    }
    void Start()
    {
        characterScript = character.GetComponent<AbstractCharacter>();
        slider.maxValue = characterScript.MaxHP;
        slider.value = characterScript.CurrentHP;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            // set the position of the health bar to the position of the character
            Vector3 position = character.transform.position;
            position.y += 1.5f;
            transform.position = position;

            float healthPercentage = (float)characterScript.CurrentHP / (float)characterScript.MaxHP;
            fill.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
            slider.value = characterScript.CurrentHP;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
