using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource Sound_effects;
    private Dictionary<string, AudioClip> Sounds;
    private AudioSource WalkingAudio;
    private AudioSource Music;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Initialize your audio sources and load audio clips here
            LoadAudioClips();
            InitializeAudioSources();
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // Initialize AudioSources
        Sound_effects = gameObject.AddComponent<AudioSource>();

        Music = gameObject.AddComponent<AudioSource>();
        Music.clip = Sounds["ambiance"];
        Music.loop = true;
        Music.volume = 0.5f;
        Music.Play();

        WalkingAudio = gameObject.AddComponent<AudioSource>();
        WalkingAudio.clip = Sounds["walk"];
        WalkingAudio.loop = true;
        
    }
    


    private void LoadAudioClips()
    {
        // Load and store audio clips
        Sounds = new Dictionary<string, AudioClip>();
        Sounds["ambiance"] = Resources.Load<AudioClip>("sounds/ambiance");
        Sounds["punch"] = Resources.Load<AudioClip>("sounds/punch");
        //Sounds["bounce"] = Resources.Load<AudioClip>("sounds/hit-sound-effect-12445");
        Sounds["bounce"] = Resources.Load<AudioClip>("sounds/bounce");
        Sounds["walk"] = Resources.Load<AudioClip>("sounds/walk");
        Sounds["blaster"] = Resources.Load<AudioClip>("sounds/blaster");
        /* Sounds["explosion"] = Resources.Load<AudioClip>("sounds/explosion");*/
        Sounds["gunshot"] = Resources.Load<AudioClip>("sounds/shotgun");
        Sounds["scream"] = Resources.Load<AudioClip>("sounds/scream");
        Sounds["swing"] = Resources.Load<AudioClip>("sounds/swing");
        Sounds["jump"] = Resources.Load<AudioClip>("sounds/jump");
        Sounds["land"] = Resources.Load<AudioClip>("sounds/land");
        Sounds["soulExplosion"] = Resources.Load<AudioClip>("sounds/electronic-impact-hard-10018");
        Sounds["swap"] = Resources.Load<AudioClip>("sounds/swap");
        Sounds["type"] = Resources.Load<AudioClip>("sounds/type");
        Sounds["ding"] = Resources.Load<AudioClip>("sounds/ding");
        Sounds["bulletImpact"] = Resources.Load<AudioClip>("sounds/bulletImpact");
        Sounds["stab"] = Resources.Load<AudioClip>("sounds/stab");
        Sounds["dash"] = Resources.Load<AudioClip>("sounds/dash");
        Sounds["broken_wood"] = Resources.Load<AudioClip>("sounds/broken_wood");
        Sounds["potion"] = Resources.Load<AudioClip>("sounds/potion");
        Sounds["unlocked"] = Resources.Load<AudioClip>("sounds/unlocked");
        Sounds["shield"] = Resources.Load<AudioClip>("sounds/shield");
        Sounds["clock"] = Resources.Load<AudioClip>("sounds/clock");
        Sounds["tic"] = Resources.Load<AudioClip>("sounds/tic");
        Sounds["tac"] = Resources.Load<AudioClip>("sounds/tac");
        Sounds["select"] = Resources.Load<AudioClip>("sounds/select");
        Sounds["win"] = Resources.Load<AudioClip>("sounds/win");
        Sounds["lose"] = Resources.Load<AudioClip>("sounds/lose");
        Sounds["big_win"] = Resources.Load<AudioClip>("sounds/big_win");
        Sounds["pause"] = Resources.Load<AudioClip>("sounds/pause");
    }
    public void PlayWalk(float volume = 1f)
    {
        if (!WalkingAudio.isPlaying)
        {
            // set volume
            WalkingAudio.volume = volume;
            WalkingAudio.Play();

        }
    }

    public void StopWalk()
    {
        WalkingAudio.Stop();
    }
    public void Pause()
    {
        Music.Pause();
    }

    public void UnPause()
    {
        Music.UnPause();
    }
    public void PlaySound(string clipName, float volume = 1f)
    {
        if (Sounds.ContainsKey(clipName))
        {
            Sound_effects.PlayOneShot(Sounds[clipName], volume);
        }
    }

    public void SetTime(float time)
    {
        Sound_effects.time = time;
    }
}
