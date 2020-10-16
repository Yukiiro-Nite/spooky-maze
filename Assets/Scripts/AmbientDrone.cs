using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientDrone : MonoBehaviour
{
    public List<AudioClip> audioClips;
    public float fadeTime = 5.0f;
    private AudioSource[] sources;
    private int srcIndex = 0;
    private int clipIndex = 0;
    private double nextEventTime;

    void Start()
    {
        sources = GetComponents<AudioSource>();
        nextEventTime = AudioSettings.dspTime + 1.0f;
    }

    void Update()
    {
        double time = AudioSettings.dspTime;
        if(time + 1.0f > nextEventTime) {
            ScheduleNextClip();
        }
    }

    private void ScheduleNextClip()
    {
        AudioClip nextClip = GetNextClip();
        sources[srcIndex].clip = nextClip;
        sources[srcIndex].PlayScheduled(nextEventTime);

        nextEventTime += nextClip.length - fadeTime;
        srcIndex = (srcIndex + 1) % sources.Length;
    }

    private AudioClip GetNextClip()
    {
        int nextIndex = Random.Range(0, audioClips.Count - 1);
        clipIndex = nextIndex < clipIndex
            ? nextIndex
            : nextIndex + 1;
        return audioClips[clipIndex];
    }
}
