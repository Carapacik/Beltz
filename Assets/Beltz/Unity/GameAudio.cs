using UnityEngine;

namespace Beltz.Unity
{
    public sealed class GameAudio
    {
        readonly AudioSource music, effects;
        readonly AudioClip click, win;
        public bool MusicEnabled {get;private set;}
        public bool SoundEnabled {get;private set;}
        bool unlocked;
        public float MusicPitch=>music.pitch;
        public GameAudio(GameObject root,AudioClip theme)
        {
            music=root.AddComponent<AudioSource>();music.clip=theme;music.loop=true;music.playOnAwake=false;music.volume=.28f;
            effects=root.AddComponent<AudioSource>();effects.playOnAwake=false;effects.volume=.25f;
            MusicEnabled=PlayerPrefs.GetInt("Beltz2.Music",PlayerPrefs.GetString("Music","ON")=="OFF"?0:1)!=0;
            SoundEnabled=PlayerPrefs.GetInt("Beltz2.Sound",PlayerPrefs.GetString("Sound","ON")=="OFF"?0:1)!=0;
            click=Tone("Soft click",480,.07f);win=Tone("Resolved",660,.42f);
        }
        static AudioClip Tone(string name,float frequency,float duration)
        {
            const int rate=22050;var samples=new float[(int)(rate*duration)];
            for(int i=0;i<samples.Length;i++){float t=i/(float)rate;float envelope=Mathf.Sin(Mathf.PI*i/samples.Length)*Mathf.Exp(-t*8);samples[i]=Mathf.Sin(t*frequency*Mathf.PI*2)*envelope*.35f;}
            var c=AudioClip.Create(name,samples.Length,1,rate,false);c.SetData(samples,0);return c;
        }
        public void Unlock(){unlocked=true;if(MusicEnabled&&!music.isPlaying)music.Play();}
        public void Click(){Unlock();if(SoundEnabled)effects.PlayOneShot(click);}
        public void Win(){if(SoundEnabled)effects.PlayOneShot(win);}
        public void Tick(bool settingsOpen,float dt)
        {
            // Muting remains authoritative: pitch changes never start playback.
            float target=MusicEnabled&&settingsOpen?.78f:1f;
            music.pitch=Mathf.Lerp(music.pitch,target,1-Mathf.Exp(-dt*7));
        }
        public void ToggleMusic(){MusicEnabled=!MusicEnabled;PlayerPrefs.SetInt("Beltz2.Music",MusicEnabled?1:0);if(MusicEnabled&&unlocked)music.UnPause();else music.Pause();if(MusicEnabled&&unlocked&&!music.isPlaying)music.Play();PlayerPrefs.Save();}
        public void ToggleSound(){SoundEnabled=!SoundEnabled;PlayerPrefs.SetInt("Beltz2.Sound",SoundEnabled?1:0);PlayerPrefs.Save();}
        public void Dispose(){Object.Destroy(click);Object.Destroy(win);}
    }
}
