using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SFXController
{
	private GameObject audioObj;
	private SFXLibrary library;
	private List<AudioSource> inUse;
	private List<AudioSource> inPool;
	private AudioMixerGroup mixerGroup;

	public SFXController(GameObject audioObj, 
		AudioMixer mixer, SFXLibrary library)
	{
		this.audioObj = audioObj;
		this.library = library;
		inUse = new List<AudioSource>();
		inPool = new List<AudioSource>();
		mixerGroup = mixer.FindMatchingGroups("SFX")[0];
	}

	public void Play(string ID)
	{
		AudioSource player = StartClipWithPlayer(ID);
		player.spatialBlend = 0f;
	}

	public void Play(string ID, Vector3 position)
	{
		AudioSource player = StartClipWithPlayer(ID);
		player.transform.position = position;
	}

	public void Play(string ID, GameObject emitter)
	{
		AudioSource player = StartClipWithPlayer(ID);
		AttachPlayer(player, emitter);
	}

	public void PlayLooped(string ID, GameObject emitter)
	{
		AudioSource player = StartClipWithPlayer(ID, true);
		AttachPlayer(player, emitter);
		player.loop = true;
	}

	public void StopLooped(GameObject emitter)
	{
		AudioSource source = emitter.transform.Find("AudioContainer").GetComponent<AudioSource>();
		ReturnPlayerToPool(source);
	}

	private AudioSource StartClipWithPlayer(string ID, bool looped = false)
	{
		SFXLibrary.Entry libraryEntry = library.Find(ID);
		AudioSource source = GetPlayerFromPool();
		source.clip = libraryEntry.clip;
		source.volume = libraryEntry.volume;
		source.pitch = 1f + UnityEngine.Random.Range(-libraryEntry.pitchVariance, libraryEntry.pitchVariance);
		source.loop = looped;
		source.Play();
		return source;
	}

	private void AttachPlayer(AudioSource player, GameObject emitter)
	{
		player.transform.SetParent(emitter.transform);
		player.transform.localPosition = Vector3.zero;
	}

	public void Update()
	{
		for(int i = inUse.Count - 1; i >= 0; i--)
		{
			AudioSource playing = inUse[i];
			if(WasPlayerOnObjectThatHasBeenDestroyed(playing))
			{
				inUse[i] = CreatePlayer();
				ReturnPlayerToPool(inUse[i]);
			}
			else if(playing.isPlaying == false)
			{
				ReturnPlayerToPool(playing);
			}
		}
	}

	private bool WasPlayerOnObjectThatHasBeenDestroyed(AudioSource player)
	{
		return player == null;
	}

	private void ReturnPlayerToPool(AudioSource player)
	{
		player.clip = null;
		inUse.Remove(player);
		inPool.Add(player);
		player.transform.SetParent(audioObj.transform);
		player.transform.localPosition = Vector3.zero;
	}

	private AudioSource GetPlayerFromPool()
	{
		if(inPool.Count == 0)
		{
			inPool.Add(CreatePlayer());
		}

		AudioSource player = inPool[0];
		player.spatialBlend = 1f;
		inPool.RemoveAt(0);
		inUse.Add(player);
		return player;
	}

	private AudioSource CreatePlayer()
	{
		GameObject audioContainer = new GameObject("AudioContainer");
		audioContainer.transform.SetParent(audioObj.transform);
		AudioSource newPlayer = audioContainer.AddComponent<AudioSource>();
		newPlayer.outputAudioMixerGroup = mixerGroup;
		return newPlayer;
	}
}
