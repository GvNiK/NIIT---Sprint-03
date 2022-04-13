using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientAudioController 
{
	private AudioZone currentAudioZone;
	private List<AudioZone> audioZones;
	private AudioMixer masterMixer;
	private AudioMixerGroup mixerGroup;

	private float ambienceTransitionTime = 0.5f;

    public AmbientAudioController(AudioMixer masterMixer, Transform ambientAudioContainer)
    {
        this.masterMixer = masterMixer;

        // Cache off all audioZones found in scene in case we need to do any initialisation or functions on all audioZones at once.
        FindAllAudioZonesInScene(ambientAudioContainer);
        mixerGroup = masterMixer.FindMatchingGroups("Ambience")[0];
    }

    private void FindAllAudioZonesInScene(Transform ambientAudioZones)
    {
        audioZones = new List<AudioZone>();

        if(ambientAudioZones != null)
        {
            foreach (Transform audioTrans in ambientAudioZones)
            {
                AudioZone audioZoneComponent = audioTrans.GetComponent<AudioZone>();
                audioZones.Add(audioZoneComponent);
                audioZoneComponent.OnAudioZoneEntered += () => HandleAmbientZoneChange(audioZoneComponent);
                audioZoneComponent.masterMixer = masterMixer;
            }
        }
    }

    private void HandleAmbientZoneChange(AudioZone newAudioZone)
    {
        if(currentAudioZone == null)
        {
            currentAudioZone = newAudioZone;
            currentAudioZone.FadeIn(ambienceTransitionTime);
        }
        else
        {
            currentAudioZone.FadeOut(ambienceTransitionTime,
                () =>
                {
                    currentAudioZone = newAudioZone;
                    currentAudioZone.FadeIn(ambienceTransitionTime);
                });
        }
    }
}
