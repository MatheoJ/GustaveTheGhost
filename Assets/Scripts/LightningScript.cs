using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    private List<GameObject> children = new();
    private List<Vector3> initialPostions = new();
    public float shakeIntensity = 0.05f; // Intensité de la vibration
    public float shakeDuration = 0.5f;   // Durée de la vibration

    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        // Commencer à vibrer tous les enfants dès le début
        foreach (GameObject child in children)
        {
            StartCoroutine(Shake(child));
        }
    }

    private void OnDisable()
    {
        // Remettre les enfants à leur position initiale
        for (int i = 0; i < children.Count; i++)
        {
            children[i].transform.localPosition = initialPostions[i];
        }
        children.Clear();
    }
    

    IEnumerator Shake(GameObject obj)
    {
        Vector3 originalPosition = obj.transform.localPosition;
        initialPostions.Add(originalPosition);

        while (true) // Boucle infinie
        {
            float x = 0f;
            float z = 0f; // Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            obj.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z + z);

            yield return new WaitForSeconds(0.05f); // Vous pouvez ajuster cette valeur pour contrôler la rapidité des vibrations

            obj.transform.localPosition = originalPosition;

            yield return new WaitForSeconds(0.05f); // Temps de pause avant le prochain shake
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
