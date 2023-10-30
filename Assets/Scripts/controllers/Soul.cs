using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : Entity
{
    [SerializeField]
    private float minMoveSpeedSoul = 8f;

    [SerializeField]
    private float maxMoveSpeedSoul = 20f;

    [SerializeField]
    private float chargingtime = 3.0f;

    private float timeCharged = 0f;

    [SerializeField]
    private float gravityScale = 0.03f;

    private static float globalGravity = -9.81f;

    [SerializeField]
    private int maxNumberOfBounce = 10;
    private int numberOfBounce = 0;

    private bool hit;

    protected override void Start() => base.Start();

    public GameObject characterLaunchFrom;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (hit) return;

        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        Rb.AddForce(gravity, ForceMode.Acceleration);
    }

    public new void Move(float direction){}

    public new void Jump(){}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("SoulCollision");

        if (collision.gameObject.tag == "SoulBouncer")
        {
            numberOfBounce++;
            if (numberOfBounce > maxNumberOfBounce)
            {
                GameObject.Find("Player").GetComponentInChildren<PlayerController>().isDead = true;
                Destroy(gameObject);
            }
        }

        string tagOfParent = collision.gameObject.transform.parent.tag;

        if (tagOfParent == "Enemy" && (collision.gameObject != characterLaunchFrom || numberOfBounce != 0))
        {
            hit = true;
            GameObject characterEnemy = collision.gameObject;
            gameObject.transform.SetParent(null);
            SwapPlayerAndEnemy(characterEnemy);
            Destroy(gameObject);
        }
    }

    private void SwapPlayerAndEnemy( GameObject characterEnemy)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject enemy = characterEnemy.transform.parent.gameObject;

        characterEnemy.transform.SetParent(player.transform, true);

        player.GetComponentInChildren<PlayerController>().updateCharacter();
        player.GetComponentInChildren<SoulLauncher>().updateCharacter();

        Destroy(enemy);
    } 

    private void ReturnToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject tempEnemy = GameObject.Find("TempEnemy");

        GameObject characterPlayer = tempEnemy.transform.GetChild(0).gameObject; 

        characterPlayer.transform.SetParent(player.transform, true);

        player.GetComponentInChildren<PlayerController>().updateCharacter();
        player.GetComponentInChildren<SoulLauncher>().updateCharacter();
        tempEnemy.GetComponentInChildren<AbstractController>().updateCharacter();
    }

    public void SetDirection(Vector3 direction)
    {
        Vector3 directionNormalized = direction.normalized;
        float launchSpeed = Mathf.Lerp(minMoveSpeedSoul, maxMoveSpeedSoul, timeCharged / chargingtime);
        Rb.velocity = directionNormalized * launchSpeed;
        Debug.Log("launchSpeed" + launchSpeed);
    }

    public void SetTimeCharged(float timeCharged)
    {
        this.timeCharged = timeCharged;
    }
    public int getBounceLeft()
    {
        return maxNumberOfBounce - numberOfBounce;
    }
}
