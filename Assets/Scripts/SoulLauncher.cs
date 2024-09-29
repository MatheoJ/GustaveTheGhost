using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SoulLauncher : MonoBehaviour
{
    const string SOUL_ASSET_PATH = "Prefabs/projectiles/Soul";
    const string SWORD_AI_ASSET_PATH = "Prefabs/controllers/Sword Enemy";
    const string GUN_AI_ASSET_PATH = "Prefabs/controllers/Gun Enemy";
    const string SHIELD_AI_ASSET_PATH = "Prefabs/controllers/Shield Enemy";

    private PlayerController playerController;
    protected AbstractCharacter _Character { get; set; }

    public float TimeMaxOfPossession = 0.0f;
    private float timeOfPossession = 0.0f;

    private float timeMousePressed = 0f;
    private bool mousePressed = false;

    private float timeMousePressedMax = 3f;

    private float maxScale = 1f;
    //private float timeToMaxScale = 1f;
    
    private float maxRightMovement = 0.5f;
    //private Vector3 initialPosition;

    private float nextBlinkTime = 0f;
    private float blinkRate = 0.5f;
    private bool tic = true;
    public Transform halo;
    public bool ShouldDrawArrow = false;


    private void Awake()
    {
        playerController = GetComponent <PlayerController>();
        _Character = GetComponentInChildren<AbstractCharacter>();
        halo = playerController.GetHalo();

        if (Skills.HasPossession) TimeMaxOfPossession += 20f;
        if (Skills.HasFiveSecPossession) TimeMaxOfPossession += 5f;
        if(Skills.HasTenSecPossession) TimeMaxOfPossession += 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        if (playerController.initialCameraTour) return;

        if(FindObjectOfType<Game>().IsGameOver) return;

        GameObject soul = GameObject.Find("Soul(Clone)");
        bool soulExist = soul != null;

        if (soulExist)
        {
            timeOfPossession = 0.0f;
            timeMousePressed = 0f;
            FindObjectOfType<Game>().PossessionTimeLeft = null;
            return;
        }

        if (playerController.IsDead) {
            FindObjectOfType<Game>().PossessionTimeLeft = null;
            return;
        } 

        FindObjectOfType<Game>().PossessionTimeLeft = (TimeMaxOfPossession - timeOfPossession) / TimeMaxOfPossession;

        if (Input.GetMouseButtonDown(0))
        {
            // Mouse button is pressed down
            mousePressed = true;
            ShouldDrawArrow = true;
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

            /*// D�placer le halo vers la droite ou la gauche
            float rightMovement = Mathf.Lerp(0f, maxRightMovement, lerpFactor);
            Vector3 direction = _Character.ForwardDirection.normalized;
            Debug.Log("direction : " + direction);
            Debug.Log("rightMovement : " + rightMovement);
            halo.transform.position = _Character.Position + direction * rightMovement;*/


            /*//halo.GetComponent<MeshRenderer>().material.color = interpolatedColor;
            Debug.Log("timeMousePressed : " + timeMousePressed);
            Debug.Log("interpolatedColor : " + interpolatedColor);*/

        }

        if (Input.GetMouseButtonUp(0) && mousePressed)
        {
            ShouldDrawArrow = false;
            Debug.Log("LaunchSoul with charged power: " + timeMousePressed);
            LaunchSoul(); // Launch the soul based on the timeMousePressed for charged power

            // Reset the mousePressed and timeMousePressed
            mousePressed = false;
            timeMousePressed = 0f;
            playerController.GetHalo().transform.localScale = new Vector3(maxScale, maxScale, maxScale);

            return;
        }
        timeOfPossession += Time.deltaTime;

        if (timeOfPossession >= TimeMaxOfPossession)
        {
            ShouldDrawArrow = false;
            LaunchSoul();
        }
        else
        {
            float timeLeft = TimeMaxOfPossession - timeOfPossession;
            if (timeLeft < TimeMaxOfPossession / 4)
            {
                blinkRate = Mathf.Max(0.1f, timeLeft / 5f); 
                if (Time.time >= nextBlinkTime)
                {
                    StartCoroutine(BlinkHalo());
                    nextBlinkTime = Time.time + blinkRate; // Set next blink time
                }
                ShouldDrawArrow = true;
            }
        }
    }

    IEnumerator BlinkHalo()
    {
        AudioManager.Instance.PlaySound(tic?"tic":"tac", 0.5f);
        halo.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.06f);
        halo.gameObject.SetActive(true);
        tic = !tic;
    }


    private void LaunchSoul()
    {
        StopAllCoroutines();
        Animator anim = _Character._Anim;
        anim.SetTrigger("Pain");

        Vector3 soulPosition = _Character.Position;
        soulPosition.y += 0.5f;

        Vector3 direction = Utils.GetMouseWorldDirection(_Character.Position + Vector3.up * 0.5f);

        soulPosition += direction * 0.5f;

        EnemyAI ai = CreateAI();
        ai.AttachToBody(_Character);
        _Character.transform.SetParent(ai.transform);
        ai.updateCharacter();
        _Character.updateController();

        Soul soul = CreateSoul(soulPosition);
        playerController.DetachBody();
        playerController.updateCharacter();
        soul.Owner = playerController;
        soul.characterLaunchFrom = _Character.gameObject;
        soul.transform.parent = gameObject.transform;
        soul.SetTimeCharged(timeMousePressed);
        soul.SetDirection(direction);
        AudioManager.Instance.PlaySound("dash", 0.5f);
        
    }

    public void updateCharacter()
    {
        _Character = GetComponentInChildren<AbstractCharacter>();
    }

    private Soul CreateSoul(Vector3 position)
    {
        GameObject asset = Resources.Load<GameObject>(SOUL_ASSET_PATH);
        if (asset == null) throw new System.Exception("Could not find Soul asset '" + SOUL_ASSET_PATH + "'");

        GameObject o = Instantiate(asset, position, transform.rotation);
        return o.GetComponent<Soul>();
    }

    private EnemyAI CreateAI()
    {
        GameObject asset = FindEnemyAIAsset();
        if (asset == null) throw new System.Exception("Could not find AI asset for character.");

        GameObject o = Instantiate(asset, transform.position, transform.rotation);
        return o.GetComponent<EnemyAI>();
    }

    private GameObject FindEnemyAIAsset()
    {
        if (_Character.GetComponentInChildren<SwordCharacter>() != null)
            return Resources.Load<GameObject>(SWORD_AI_ASSET_PATH);

        if (_Character.GetComponentInChildren<GunCharacter>() != null)
            return Resources.Load<GameObject>(GUN_AI_ASSET_PATH);

        if (_Character.GetComponentInChildren<ShieldCharacter>() != null)
            return Resources.Load<GameObject>(SHIELD_AI_ASSET_PATH);

        return null;
    }
}
