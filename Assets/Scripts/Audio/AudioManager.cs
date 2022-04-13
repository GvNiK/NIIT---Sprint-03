using UnityEngine;
using UnityEngine.Audio;

public class AudioManager
{
    public AudioMixer masterMixer;

    private AmbientAudioController ambientAudioController;
	private SFXController sfxController;

    public AudioManager(Transform levelObjects)
    {
        masterMixer = Resources.Load<AudioMixer>("Audio/MasterMixer");
        ambientAudioController = new AmbientAudioController(masterMixer, levelObjects.Find("AudioMarkup/Ambient"));

		GameObject audio = new GameObject("Audio");
		audio.transform.SetParent(levelObjects);
		sfxController = new SFXController(audio, masterMixer, Resources.Load<SFXLibrary>("Audio/SFXLibrary"));
	}

	public void Update()
	{
		sfxController.Update();
	}

	public SFXController SFX
	{
		get
		{
			return sfxController;
		}
	}
}
