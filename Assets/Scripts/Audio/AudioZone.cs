using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CollisionCallbacks))]
public class AudioZone : MonoBehaviour
{
    public AudioClip clip;
    public MixerGroupType mixerGroupType;
    [Range(0, 1)]
    public float maxVolume;
    public Action OnAudioZoneEntered = delegate { };

    private float currentVolume = 0;
    private AudioSource source;
    public AudioMixer masterMixer;
    private CollisionCallbacks collisionCallbacks;

    public void Start()
    {
        SetupAudioSource();
    }

    private void SetupAudioSource()
    {
        source = GetComponent<AudioSource>();
        collisionCallbacks = GetComponent<CollisionCallbacks>();
        collisionCallbacks.OnTriggerEntered += (collision) => RelayCollision(collision);

        source.clip = clip;
        source.outputAudioMixerGroup = GetMixerGroup();
        source.volume = currentVolume;
    }

    private AudioMixerGroup GetMixerGroup()
    {
        AudioMixerGroup mixerGroupReturn;

        switch(mixerGroupType)
        {
            case MixerGroupType.SFX:
                mixerGroupReturn = masterMixer.FindMatchingGroups("SFX")[0];
                break;
            case MixerGroupType.Music:
                mixerGroupReturn = masterMixer.FindMatchingGroups("Music")[0];
                break;
            case MixerGroupType.Ambience:
                mixerGroupReturn = masterMixer.FindMatchingGroups("Ambience")[0];
                break;
            default:
                mixerGroupReturn = masterMixer.FindMatchingGroups("Master")[0];
                break;
        }

        return mixerGroupReturn;
    }

    public void FadeIn(float time, Action OnComplete = null)
    {
        StartCoroutine(FadeSourceIn(time, OnComplete));
    }

    public void FadeOut(float time, Action OnComplete = null)
    {
        StartCoroutine(FadeSourceOut(time, OnComplete));
    }

    private void RelayCollision(Collider collider)
    {
        if(collider.transform.tag.Equals("Player"))
        {
            OnAudioZoneEntered.Invoke();
        }
    }

    private IEnumerator FadeSourceIn(float time, Action OnComplete)
    {
        source.Play();

        float timer = 0;
        while(timer <= time)
        {
            timer += Time.deltaTime;
            currentVolume = (timer / time) * maxVolume;
            source.volume = currentVolume;
            yield return null;
        }

        source.volume = maxVolume;
        OnComplete?.Invoke();
        yield break;

    }

    private IEnumerator FadeSourceOut(float time, Action OnComplete)
    {
        float timer = 0;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            currentVolume = (1 - (timer / time)) * maxVolume;
            source.volume = currentVolume;
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
        OnComplete?.Invoke();
        yield break;
    }

}


