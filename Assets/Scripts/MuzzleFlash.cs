using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{

    private float timeToLive = 0.1f;
    private float starTingScale = 0.01f;
    private float maxScale = 0.06f;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = Vector3.one * starTingScale;
        //gameObject.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), -90, 0);
        gameObject.transform.Rotate(0, -90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToLive > 0)
        {
            timeToLive -= Time.deltaTime;
            float scale = Mathf.Lerp(maxScale, 0, timeToLive / 0.5f);
            gameObject.transform.localScale = Vector3.one * scale;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
