//==============================================================
// Audio Manager
//==============================================================

using UnityEngine;
using System.Collections;

namespace BrawlShooter
{
	public class AudioManager : Singleton<AudioManager>
	{
		public enum AudioChannel { Master, Music, fx };

		[Range(0, 1)]
		public float masterVolume = 1; // Overall volume
		[Range(0, 1)]
		public float musicVolume = 1f; // Music volume
		[Range(0, 1)]
		public float fxVolume = 1; // FX volume

		public bool MusicIsLooping = true;
		public bool CoroutineRun; // Used in demo.

		//==============================================================
		// Seperate audiosources
		//==============================================================
		AudioSource musicSource;
		AudioSource fxSource;

		//==============================================================
		// Sound libraries. All your audio clips
		//==============================================================
		SoundLibrary soundLibrary;
		MusicLibrary musicLibrary;

		//==============================================================
		// Awake
		//==============================================================
		protected override void Awake()
		{
			base.Awake();

			//==============================================================
			// Get FX, Music and Ambient sound library
			//==============================================================
			soundLibrary = GetComponent<SoundLibrary>();
			musicLibrary = GetComponent<MusicLibrary>();

			//==============================================================
			// Create audio sources
			//==============================================================
			GameObject newfxSource = new GameObject("2D fx source");
			fxSource = newfxSource.AddComponent<AudioSource>();
			newfxSource.transform.parent = transform;
			fxSource.playOnAwake = false;

			GameObject newMusicSource = new GameObject("Music source");
			musicSource = newMusicSource.AddComponent<AudioSource>();
			newMusicSource.transform.parent = transform;
			musicSource.loop = MusicIsLooping; // Music is looping
			musicSource.playOnAwake = false;

			//==============================================================
			// Set volume on all the channels
			//==============================================================
			SetVolume(masterVolume, AudioChannel.Master);
			SetVolume(fxVolume, AudioChannel.fx);
			SetVolume(musicVolume, AudioChannel.Music);
		}

		//==============================================================
		// Set volume on all the channels
		//==============================================================
		public void SetVolume(float volumePercent, AudioChannel channel)
		{
			switch (channel)
			{
				case AudioChannel.Master:
					masterVolume = volumePercent;
					break;
				case AudioChannel.fx:
					fxVolume = volumePercent;
					break;
				case AudioChannel.Music:
					musicVolume = volumePercent;
					break;
			}

			// Set the audiosource volume
			fxSource.volume = fxVolume * masterVolume;
			musicSource.volume = musicVolume * masterVolume;
		}

		//==============================================================
		// Play music with delay. 0 = No delay
		//==============================================================
		public void PlayMusic(string musicName, float delay)
		{
			musicSource.clip = musicLibrary.GetClipFromName(musicName);
			musicSource.PlayDelayed(delay);
		}

		//==============================================================
		// Play music fade in
		//==============================================================
		public IEnumerator PlayMusicFade(string musicName, float duration)
		{
			CoroutineRun = true; // Used in demo

			float startVolume = 0;
			float targetVolume = musicSource.volume;
			float currentTime = 0;

			musicSource.clip = musicLibrary.GetClipFromName(musicName);
			musicSource.Play();

			while (currentTime < duration)
			{
				currentTime += Time.deltaTime;
				musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
				yield return null;
			}

			CoroutineRun = false; // Used in demo

			yield break;
		}

		//==============================================================
		// Stop music
		//==============================================================
		public void StopMusic()
		{
			musicSource.Stop();
		}

		//==============================================================
		// Stop music fade out
		//==============================================================
		public IEnumerator StopMusicFade(float duration)
		{
			CoroutineRun = true; // Used in demo

			float currentVolume = musicSource.volume;
			float startVolume = musicSource.volume;
			float targetVolume = 0;
			float currentTime = 0;

			while (currentTime < duration)
			{
				currentTime += Time.deltaTime;
				musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
				yield return null;
			}
			musicSource.Stop();
			musicSource.volume = currentVolume;

			CoroutineRun = false; // Used in demo

			yield break;
		}

		//==============================================================
		// FX Audio
		//==============================================================
		public void PlaySound2D(string soundName)
		{
			fxSource.PlayOneShot(soundLibrary.GetClipFromName(soundName), fxVolume * masterVolume);
		}

		public void PlaySound3D(string soundName, Vector3 soundPosition)
		{
			AudioSource.PlayClipAtPoint(soundLibrary.GetClipFromName(soundName), soundPosition, fxVolume * masterVolume);
		}
	}
}