/*
SoundController.js
This script is used to control all sound in the game to avoid having AudioSorces on induvidual GameObjects.

To play sound: SoundController.instance.Play(clip:AudioClip, volume:float, pitch:float);
To play music: SoundController.instance.PlayMusic(clip:AudioClip, volume:float, pitch:float, fade:boolean);
To stop music: SoundController.instance.StopMusic(fade:boolean);

Music uses to channels so that it is possible to fade between them

*/
#pragma strict
var _audioClips:AudioClip[];		//Place audio clips (Optional: audio clips can also be played directly from gameObjects)
var _audioChannels:int = 10;		//How many audio sources can play at the same time
var _masterVol:float = .5;			//Volume applied to all sound (keep at aprox 0.5 for better individual object volume control - Example: Setting object volume to 2 on events that are important)
var _soundVol:float = 1;			//Volume multiplier for sound effects
var _musicVol:float = 1;			//Volume multiplier for music
var _linearRollOff:boolean;			//Enable to change rollOff
var channels:AudioSource[];			//List of audio sources (channels)
var channel:int;					//Current channel
var _musicChannels:AudioSource[];	//List of music audio sources (channels)
var _musicChannel:int;				//Current channel
private var _currentMusicVol:float;		//Cache for the music clips volume, makes controller able to change volume on runtime. Run UpdateMusicVolume(); after changing _musicVol
private var _fadeTo:float;				//Music will fade to this value in FadeUpMusic()
static var instance : SoundController;	// SoundController is a singleton. SoundController.instance.DoSomeThing();


function OnApplicationQuit() {			// Ensure that the instance is destroyed when the game is stopped in the editor.
    instance = null;
}


function Start () {
	 if (instance){
        Destroy (gameObject);
    }else{
        instance = this;
        DontDestroyOnLoad (gameObject);
    }
	AddChannels();

	DontDestroyOnLoad (transform.gameObject);
}


function StopMusic(fade:boolean) {
	PlayMusic(null, 0, 1, fade);
}


function FadeUpMusic(){
	if(_musicChannels[_musicChannel].volume < _fadeTo){
		_musicChannels[_musicChannel].volume += 0.0025;	
	}else{
		
		CancelInvoke("FadeUpMusic");
	}
}


function FadeDownMusic(){
	var c:int = 0;
	if(_musicChannel == 0)
	c = 1;
	if(_musicChannels[c].volume > 0){
		_musicChannels[c].volume -= 0.0025;
	}else{
		_musicChannels[c].Stop();
		CancelInvoke("FadeDownMusic");
	}
}


function UpdateMusicVolume(){
	for(var j:int; j < 2; j++){	
		_musicChannels[j].volume = _currentMusicVol*_masterVol*_musicVol;
	}
}


function AddChannels () {
	//Add channels to stage (Future Update Note: decrease startup peak if this is done in editor)
	channels = new AudioSource[_audioChannels];
	_musicChannels = new AudioSource[2];
	if(channels.Length <= _audioChannels){		
		for(var i:int; i < _audioChannels; i++){	
			var chan:GameObject = new GameObject();
			chan.AddComponent(AudioSource);
			chan.name = "AudioChannel " + i;
			chan.transform.parent = this.transform;
			channels[i] = chan.GetComponent(AudioSource);
			if(_linearRollOff)	
			channels[i].rolloffMode =  AudioRolloffMode.Linear;
		}
	}
	for(var j:int; j < 2; j++){	
			var mchan:GameObject = new GameObject();		
			mchan.AddComponent(AudioSource);
			mchan.name = "MusicChannel " + j;
			mchan.transform.parent = this.transform;
			_musicChannels[j] = mchan.GetComponent(AudioSource);	
			_musicChannels[j].loop = true;
			_musicChannels[j].volume = 0;
			if(_linearRollOff)
			_musicChannels[j].rolloffMode =  AudioRolloffMode.Linear;
		}
}

//Play music clip
function PlayMusic (clip:AudioClip, volume:float, pitch:float, fade:boolean) {
	if(!fade)_musicChannels[_musicChannel].volume = 0;
	if(_musicChannel == 0) _musicChannel = 1;
	else _musicChannel = 0;
	_currentMusicVol = volume;
	_musicChannels[_musicChannel].clip = clip;
	if(fade){
		this._fadeTo = volume*_masterVol*_musicVol;
		InvokeRepeating("FadeUpMusic", 0.01, 0.01);
		InvokeRepeating("FadeDownMusic", 0.01, 0.01);
	}else{
		_musicChannels[_musicChannel].volume = volume*_masterVol*_musicVol;
	}
	_musicChannels[_musicChannel].audio.pitch = pitch;
	_musicChannels[_musicChannel].audio.Play();
}

//Play from channels list
function Play (audioClipIndex:int, volume:float, pitch:float) {
	if(channel < channels.Length-1) channel++;
	else channel = 0;
	if(audioClipIndex<_audioClips.Length){	
		channels[channel].clip = _audioClips[audioClipIndex];
		channels[channel].audio.volume = volume*_masterVol*_soundVol;
		channels[channel].audio.pitch = pitch;
		channels[channel].audio.Play();
	}
}

//Play clip
function Play (clip:AudioClip, volume:float, pitch:float, position:Vector3) {
	if(channel < channels.Length-1)	channel++;
	else channel = 0;	
	channels[channel].clip = clip;
	channels[channel].audio.volume = volume*_masterVol*_soundVol;
	channels[channel].audio.pitch = pitch;
	channels[channel].transform.position = position;
	channels[channel].audio.Play();
}

//Play clip
function Play (clip:AudioClip, volume:float, pitch:float) {
	if(channel < channels.Length-1)	channel++;
	else channel = 0;	
	channels[channel].clip = clip;
	channels[channel].audio.volume = volume*_masterVol*_soundVol;
	channels[channel].audio.pitch = pitch;
	channels[channel].audio.Play();
}

function StopAll () {	//Stops all sound from channels (Future Update Note: activate delay?)
	for(var i:int; i < channels.Length; i++){
		channels[i].Stop();	
	}
}