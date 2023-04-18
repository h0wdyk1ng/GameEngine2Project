using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Shake (float duration, float magnitude)
    {
        Vector3 originalPose = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPose.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPose;
    }
}
