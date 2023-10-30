using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SoulLauncher : MonoBehaviour
{
    public GameObject soul;
    public GameObject SwordAiController;
    public GameObject GunAiController;

    private bool canLaunch  = true;
    private PlayerController playerController;
    protected AbstractCharacter _Character { get; set; }

    private float timeMousePressed = 0f;
    private bool mousePressed = false;

    private float timeMousePressedMax = 3f;

    private float maxScale = 1f;
    private float timeToMaxScale = 1f;
    
    private float maxRightMovement = 5;
    private Vector3 initialPosition;


    private void Awake()
    {
        playerController = GetComponent <PlayerController>();
        _Character = GetComponentInChildren<AbstractCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject soul = GameObject.Find("Soul(Clone)");
        bool soulExist = soul != null;

        if (Input.GetMouseButtonDown(0) && !soulExist)
        {
            // Mouse button is pressed down
            mousePressed = true;
        }

        if (mousePressed)
        {
            // Increment the timeMousePressed while the mouse button is held down
            timeMousePressed += Time.deltaTime;
            Transform halo = playerController.GetHalo();

            // Calculate the fraction of timeMousePressed in relation to timeMousePressedMax
            float lerpFactor = Mathf.Clamp(timeMousePressed / timeMousePressedMax, 0f, 1f);

            // Interpolate the color from white to blue based on the lerpFactor
            Color interpolatedColor = Color.Lerp(Color.white, Color.blue, lerpFactor);
            interpolatedColor.a = 0.23f;

            //Change the mesh renderer color to the interpolated color
            halo.GetComponent<MeshRenderer>().material.SetColor("_TintColor", interpolatedColor);

            // Ajuster la taille du halo
            float scale = Mathf.Lerp(maxScale, 0.5f, lerpFactor);
            halo.transform.localScale = new Vector3(scale, scale, scale);

            // Déplacer le halo vers la droite
            float rightMovement = Mathf.Lerp(0f, maxRightMovement, lerpFactor);
            halo.transform.position += new Vector3(rightMovement, 0f, 0f);


            //halo.GetComponent<MeshRenderer>().material.color = interpolatedColor;
            Debug.Log("timeMousePressed : " + timeMousePressed);
            Debug.Log("interpolatedColor : " + interpolatedColor);

        }

        if (Input.GetMouseButtonUp(0) && !soulExist && mousePressed)
        {
            Debug.Log("LaunchSoul with charged power: " + timeMousePressed);

            Animator anim = _Character._Anim;
            anim.SetTrigger("Pain");
            LaunchSoul(); // Launch the soul based on the timeMousePressed for charged power

            // Reset the mousePressed and timeMousePressed
            mousePressed = false;
            timeMousePressed = 0f;
        }

    }

    private void LaunchSoul()
    {
        canLaunch = false;

        Vector3 soulPosition = _Character.Position;
        soulPosition.y += 0.5f;

        Vector3 direction = Utils.GetMouseWorldDirection(_Character.Position + Vector3.up * 0.5f);

        soulPosition += direction * 0.5f;


        GameObject controller;
        if (_Character.GetComponentInChildren<GunCharacter>() != null)
        {
            controller = GunAiController;
        }else if (_Character.GetComponentInChildren<SwordCharacter>() != null)
        {
            controller = SwordAiController;
        }else { 
            throw new System.Exception("The character of the player is not recognise");
        }

        GameObject newEnemy = Instantiate(controller, transform.position, transform.rotation);
        Debug.Log("tempEnemy : " + newEnemy);
        _Character.transform.parent = newEnemy.transform;

        GameObject soulLaunched = Instantiate(soul, soulPosition, transform.rotation);
        soulLaunched.GetComponent<Soul>().characterLaunchFrom = _Character.gameObject;

        soulLaunched.transform.parent = gameObject.transform;
        playerController.updateCharacter();
        newEnemy.GetComponentInChildren<AbstractController>().updateCharacter();
        soulLaunched.GetComponent<Soul>().SetTimeCharged(timeMousePressed);
        soulLaunched.GetComponent<Soul>().SetDirection(direction);
    }

    public void updateCharacter()
    {
        _Character = GetComponentInChildren<AbstractCharacter>();
    }
}
