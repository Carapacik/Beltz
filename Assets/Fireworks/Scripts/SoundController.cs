/*
SoundController.js
This script is used to control all sound in the game to avoid having AudioSorces on induvidual GameObjects.

To play sound: SoundController.instance.Play(clip:AudioClip, volume:float, pitch:float);
To play music: SoundController.instance.PlayMusic(clip:AudioClip, volume:float, pitch:float, fade:boolean);
To stop music: SoundController.instance.StopMusic(fade:boolean);

Music uses to channels so that it is possible to fade between them

*/


using UnityEngine;
using System;



public class SoundController:MonoBehaviour{
	public AudioClip[] _audioClips;		//Place audio clips (Optional: audio clips can also be played directly from gameObjects)
	public int _audioChannels = 10;		//How many audio sources can play at the same time
	public float _masterVol = .5f;			//Volume applied to all sound (keep at aprox 0.5 for better individual object volume control - Example: Setting object volume to 2 on events that are important)
	public float _soundVol = 1.0f;			//Volume multiplier for sound effects
	public float _musicVol = 1.0f;			//Volume multiplier for music
	public bool _linearRollOff;			//Enable to change rollOff
	public AudioSource[] channels;			//List of audio sources (channels)
	public int channel;					//Current channel
	public AudioSource[] _musicChannels;	//List of music audio sources (channels)
	public int _musicChannel;				//Current channel
	float _currentMusicVol;		//Cache for the music clips volume, makes controller able to change volume on runtime. Run UpdateMusicVolume(); after changing _musicVol
	float _fadeTo;				//Music will fade to this value in FadeUpMusic()
	public static SoundController instance;	// SoundController is a singleton. SoundController.instance.DoSomeThing();
	
	
	public void OnApplicationQuit() {			// Ensure that the instance is destroyed when the game is stopped in the editor.
	    instance = null;
	}
	
	
	public void Start() {
		 if (instance != null){
	        Destroy (gameObject);
	    }else{
	        instance = this;
	        DontDestroyOnLoad (gameObject);
	    }
		AddChannels();
	
		DontDestroyOnLoad (transform.gameObject);
	}
	
	
	public void StopMusic(bool fade) {
		PlayMusic(null, 0.0f, 1.0f, fade);
	}
	
	
	public void FadeUpMusic(){
		if(_musicChannels[_musicChannel].volume < _fadeTo){
			_musicChannels[_musicChannel].volume += 0.0025f;	
		}else{
			
			CancelInvoke("FadeUpMusic");
		}
	}
	
	
	public void FadeDownMusic(){
		int c = 0;
		if(_musicChannel == 0)
		c = 1;
		if(_musicChannels[c].volume > 0){
			_musicChannels[c].volume -= 0.0025f;
		}else{
			_musicChannels[c].Stop();
			CancelInvoke("FadeDownMusic");
		}
	}
	
	
	public void UpdateMusicVolume(){
		for(int j = 0; j < 2; j++){	
			_musicChannels[j].volume = _currentMusicVol*_masterVol*_musicVol;
		}
	}
	
	
	public void AddChannels() {
		//Add channels to stage (Future Update Note: decrease startup peak if this is done in editor)
		channels = new AudioSource[_audioChannels];
		_musicChannels = new AudioSource[2];
		if(channels.Length <= _audioChannels){		
			for(int i = 0; i < _audioChannels; i++){	
				GameObject chan = new GameObject();
				chan.AddComponent<AudioSource>();
				chan.name = "AudioChannel " + i;
				chan.transform.parent = this.transform;
				channels[i] = chan.GetComponent<AudioSource>();
				if(_linearRollOff)	
				channels[i].rolloffMode =  AudioRolloffMode.Linear;
			}
		}
		for(int j = 0; j < 2; j++){	
				GameObject mchan = new GameObject();		
				mchan.AddComponent<AudioSource>();
				mchan.name = "MusicChannel " + j;
				mchan.transform.parent = this.transform;
				_musicChannels[j] = mchan.GetComponent<AudioSource>();	
				_musicChannels[j].loop = true;
				_musicChannels[j].volume = 0.0f;
				if(_linearRollOff)
				_musicChannels[j].rolloffMode =  AudioRolloffMode.Linear;
			}
	}
	
	//Play music clip
	public void PlayMusic(AudioClip clip,float volume,float pitch,bool fade) {
		if(!fade)_musicChannels[_musicChannel].volume = 0.0f;
		if(_musicChannel == 0) _musicChannel = 1;
		else _musicChannel = 0;
		_currentMusicVol = volume;
		_musicChannels[_musicChannel].clip = clip;
		if(fade){
			this._fadeTo = volume*_masterVol*_musicVol;
			InvokeRepeating("FadeUpMusic", 0.01f, 0.01f);
			InvokeRepeating("FadeDownMusic", 0.01f, 0.01f);
		}else{
			_musicChannels[_musicChannel].volume = volume*_masterVol*_musicVol;
		}
		_musicChannels[_musicChannel].GetComponent<AudioSource>().pitch = pitch;
		_musicChannels[_musicChannel].GetComponent<AudioSource>().Play();
	}
	
	//Play from channels list
	public void Play(int audioClipIndex,float volume,float pitch) {
		if(channel < channels.Length-1) channel++;
		else channel = 0;
		if(audioClipIndex<_audioClips.Length){	
			channels[channel].clip = _audioClips[audioClipIndex];
			channels[channel].GetComponent<AudioSource>().volume = volume*_masterVol*_soundVol;
			channels[channel].GetComponent<AudioSource>().pitch = pitch;
			channels[channel].GetComponent<AudioSource>().Play();
		}
	}
	
	//Play clip
	public void Play(AudioClip clip,float volume,float pitch,Vector3 position) {
		if(channel < channels.Length-1)	channel++;
		else channel = 0;	
		channels[channel].clip = clip;
		channels[channel].GetComponent<AudioSource>().volume = volume*_masterVol*_soundVol;
		channels[channel].GetComponent<AudioSource>().pitch = pitch;
		channels[channel].transform.position = position;
		channels[channel].GetComponent<AudioSource>().Play();
	}
	
	//Play clip
	public void Play(AudioClip clip,float volume,float pitch) {
		if(channel < channels.Length-1)	channel++;
		else channel = 0;	
		channels[channel].clip = clip;
		channels[channel].GetComponent<AudioSource>().volume = volume*_masterVol*_soundVol;
		channels[channel].GetComponent<AudioSource>().pitch = pitch;
		channels[channel].GetComponent<AudioSource>().Play();
	}
	
	public void StopAll() {	//Stops all sound from channels (Future Update Note: activate delay?)
		for(int i = 0; i < channels.Length; i++){
			channels[i].Stop();	
		}
	}
}