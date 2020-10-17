using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientEffect : MonoBehaviour
{
    public List<AudioClip> audioClips;
    private AudioSource[] sources;
    public float frequency = 0.04f;
    public float radius = 10.0f;
    public float minVolume = 0.5f;
    public float maxVolume = 1.0f;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    void Start()
    {
        sources = new AudioSource[audioClips.Count];
        for(int i=0; i<audioClips.Count; i++) {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].spatialBlend = 1.0f;
            sources[i].clip = audioClips[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: change this equation to trigger frequency per second
        if(Random.value <= frequency && NonePlaying(sources)) {
            Vector3 cameraPosition = Camera.main.transform.position;
            AudioSource randomSrc = sources[Random.Range(0, sources.Length)];
            Vector2 randomPos = Random.insideUnitCircle * radius;
            Vector3 playPosition = new Vector3(randomPos.x + cameraPosition.x, 0, randomPos.y + cameraPosition.z);
            transform.position = playPosition;
            randomSrc.volume = Random.Range(minVolume, maxVolume);
            randomSrc.pitch = Random.Range(minPitch, maxPitch);
            randomSrc.Play();
        }
    }

    private bool NonePlaying(AudioSource[] srcs) {
        for(int i=0; i<srcs.Length; i++) {
            if(srcs[i].isPlaying) {
                return false;
            }
        }
        return true;
    }
}
