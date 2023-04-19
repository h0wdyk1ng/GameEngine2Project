using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] clipsToPlay;
    private AudioSource src;
    private int clipIndex;
    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PlayAfterTime(Random.Range(1f, 10f)));
    }

    void PlaySoundAfterRandomTime()
    {
        src.PlayOneShot(clipsToPlay[clipIndex]);
        src.PlayDelayed(Random.Range(1f, 10f));
    }

    IEnumerator PlayAfterTime(float time)
    {
        clipIndex = Random.Range(0, clipsToPlay.Length - 1);
        yield return new WaitForSeconds(time);
        src.PlayOneShot(clipsToPlay[clipIndex]);
    }
}
