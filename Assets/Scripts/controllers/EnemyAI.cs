using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class EnemyAI : AbstractController
{
    protected float SightDistance = 5.0f;

    public AbstractCharacter SeenPlayer { get; private set; }

    protected float WalkingDirection {  get; set; }
    float AlertCountdown = 0;

    public GameObject Lightnings;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    protected override void Awake()
    {
        base.Awake();
        
        // get first child
        if  (Character != null)
            Lightnings = transform.GetChild(0).gameObject.transform.Find("Lightnings").gameObject;

        Debug.Log("Awake" + Character);
    }

    // Utile pour régler des valeurs aux objets
    protected override void Start()
    {
        base.Start();

        WalkingDirection = 0.5f;
    }

    // Vérifie les entrées de commandes du joueur
    protected override void Update()
    {
        base.Update();

        //if (isDead) Destroy(gameObject);

        if (WalkingDirection != 0) Character.Move(WalkingDirection);

        if (SeenPlayer) OnAlert();
        else
        {
            // Switch direction every 2.0 seconds
            float time = Time.time;
            if (time % 2.0f <= Time.deltaTime) WalkingDirection = -WalkingDirection;
        }
    }

    void FixedUpdate()
    {
        if (Character == null) return;
        if (Lightnings == null)
        {
            Lightnings = transform.GetChild(0).gameObject.transform.Find("Lightnings").gameObject;
        }

        AbstractCharacter player = CheckForPlayer();
        if (player)
        {
            AlertCountdown = 5.0f;
            SeenPlayer = player;
            Lightnings.SetActive(true);
        }
        else
        {
            AlertCountdown -= Time.deltaTime;
            if (AlertCountdown <= 0)
            {
                AlertCountdown = 0;
                SeenPlayer = null;
                Lightnings.SetActive(false);
            }
        }
    }

    protected abstract void OnAlert();

    AbstractCharacter CheckForPlayer()
    {
        int layerMask = LayerMask.GetMask("Character") | LayerMask.GetMask("Floor");
        float[] angles = { -30, -15, 0, 15, 30 };

        AbstractCharacter seenPlayer = null;

        foreach (float angle in angles)
        {
            GameObject obstacle = GetRaycast(layerMask, angle);
            if (obstacle == null) continue;

            PlayerController controller = obstacle.GetComponentInParent<PlayerController>();
            if (controller != null) seenPlayer = controller.Character;
        }

        return seenPlayer;
    }

    GameObject GetRaycast(int layerMask, float angleX)
    {
        if (Character.Flipped) angleX *= -1;
        Vector3 direction = Quaternion.Euler(angleX, 0, 0) * Character.ForwardDirection;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Character.EyePosition, direction, out hit, SightDistance, layerMask))
        {

            Color color = hit.transform.gameObject.GetComponentInParent<PlayerController>() ? Color.red : Color.yellow;
            Debug.DrawRay(Character.EyePosition, direction * hit.distance, color);
            return hit.transform.gameObject;
        }
        else
        {
            Debug.DrawRay(Character.EyePosition, direction * SightDistance, Color.white);
            return null;
        }
    }

    public new void updateCharacter()
    {
        base.updateCharacter();
        if (Character == null) return;

        Debug.Log("updateCharacter" + Character);

        Lightnings = transform.GetChild(0).gameObject.transform.Find("Lightnings").gameObject;
    }
}