using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;

public class AudioChannel
{
    public AudioSource source;
    public float volume;
    public bool paused;
    public bool isMusic = false;
}


public class SoundManager : SingletonBehaviour<SoundManager>
{
    class FadeInfo
    {
        public AudioSource source;
        public AudioChannel channel;
        public float targetVolume;
        public float fadeSpeed;
        public bool freeOnCompletion;
        public bool isMusic = false;
    }

    public int numberOfChannels = 32;
    public GameObject channelTemplate;
    public AudioClip buttonClickSound;

    List<AudioChannel> freeChannels = new List<AudioChannel>();
    List<AudioChannel> usedChannels = new List<AudioChannel>();
    List<AudioChannel> channelsToFree = new List<AudioChannel>();
    List<AudioChannel> musicChannels = new List<AudioChannel>();
    List<AudioChannel> externalSources = new List<AudioChannel>();
    List<FadeInfo> fadingChannels = new List<FadeInfo>();
    List<FadeInfo> fadingChannelsToFree = new List<FadeInfo>();


    float baseMusicLevel = 1.0f;
    float baseSoundLevel = 1.0f;

    public System.Action<float> onSoundLevelChanged;
    public System.Action<float> onMusicLevelChanged;

    public float MusicLevel
    {
        get { return baseMusicLevel; }
        set
        {
            baseMusicLevel = value;
            SetNewMusicLevel(value);
        }
    }

    public float SoundLevel
    {
        get { return baseSoundLevel; }
        set
        {
            baseSoundLevel = value;
            SetNewSoundLevel(value);
        }
    }

    bool IsSourceFading(AudioSource source)
    {
        foreach (FadeInfo info in fadingChannels)
        {
            if (info.source == source)
            {
                return true;
            }
        }

        return false;
    }

    void SetNewMusicLevel(float level)
    {
        foreach (AudioChannel channel in musicChannels)
        {
            if (!IsSourceFading(channel.source))
            {
                channel.source.volume = channel.volume * baseMusicLevel;
            }
        }

        if (onMusicLevelChanged != null)
        {
            onMusicLevelChanged(level);
        }
    }

    void SetNewSoundLevel(float level)
    {
        foreach (AudioChannel channel in usedChannels)
        {
            channel.source.volume = channel.volume * baseSoundLevel;
        }

        foreach (AudioChannel channel in externalSources)
        {
            channel.source.volume = channel.volume * baseSoundLevel;
        }

        if (onSoundLevelChanged != null)
        {
            onSoundLevelChanged(level);
        }
    }

    public void Init()
    {
        GameObject channelsParent = new GameObject();
        channelsParent.name = "Channels";
        channelsParent.transform.parent = transform;
        channelsParent.transform.position = Vector3.zero;

        for (int i = 0; i < numberOfChannels; i++)
        {
            GameObject newChannel = GameObject.Instantiate(channelTemplate) as GameObject;
            newChannel.transform.parent = channelsParent.transform;
            newChannel.transform.position = Vector3.zero;
            newChannel.name = "AudioChannel " + (i + 1);

            AudioChannel chan = new AudioChannel();
            chan.source = newChannel.GetComponent<AudioSource>();
            chan.paused = false;

            freeChannels.Add(chan);

            newChannel.SetActive(false);
        }
    }

    public void RegisterExternalSource(AudioSource source)
    {
        AudioChannel channel = new AudioChannel();
        channel.source = source;
        channel.volume = source.volume;
        channel.source.volume = channel.volume * baseSoundLevel;
        channel.paused = false;
        externalSources.Add(channel);
    }

    public void DeregisterExternalSource(AudioSource source)
    {
        AudioChannel channelToRemove = null;
        foreach (AudioChannel channel in externalSources)
        {
            if (channel.source == source)
            {
                channelToRemove = channel;
            }
        }

        if (channelToRemove != null)
        {
            externalSources.Remove(channelToRemove);
        }

        FadeInfo infoToRemove = null;
        foreach (FadeInfo info in fadingChannels)
        {
            if (info.source == source)
            {
                infoToRemove = info;
            }
        }

        if (infoToRemove != null)
        {
            fadingChannels.Remove(infoToRemove);
        }
    }

    // Update is called once per frame 	
    void Update()
    {
        channelsToFree.Clear();

        for (int i = 0; i < usedChannels.Count; i++)
        {
            AudioChannel channel = usedChannels[i];
            if (!channel.source.isPlaying && !channel.paused)
            {
                channelsToFree.Add(channel);
            }
        }

        for (int i = 0; i < channelsToFree.Count; i++)
        {
            AudioChannel channel = channelsToFree[i];
            usedChannels.Remove(channel);
            freeChannels.Add(channel);
            channel.source.gameObject.SetActive(false);
        }

        // Update any fading channels
        fadingChannelsToFree.Clear();
        for (int i = 0; i < fadingChannels.Count; i++)
        {
            FadeInfo info = fadingChannels[i];

            float targetLevel = SoundLevel;
            if (info.isMusic)
            {
                targetLevel = MusicLevel;
            }

            if (info.source.volume != info.targetVolume * targetLevel)
            {
                float delta = 1;
                if (info.fadeSpeed != 0)
                {
                    delta = NonStopTime.Instance.deltaTime * info.fadeSpeed; //Main.NonStopTime.deltaTime * info.fadeSpeed;
                }
                if ((info.targetVolume * targetLevel) - info.source.volume < 0)
                {
                    delta *= -1;
                }

                //DebugUtils.Log(info.source.clip);
                info.source.volume += delta;// * NonStopTime.deltaTime;
                info.channel.volume = info.source.volume;


                if ((delta > 0 && info.source.volume >= (info.targetVolume * targetLevel))
                    || (delta < 0 && info.source.volume <= (info.targetVolume * targetLevel)))
                {
                    info.source.volume = info.targetVolume * targetLevel;
                    fadingChannelsToFree.Add(info);
                }
            }
            else
            {
                fadingChannelsToFree.Add(info);
            }
        }
        // Remove fading channels that are finished from queue
        for (int i = 0; i < fadingChannelsToFree.Count; i++)
        {
            FadeInfo info = fadingChannelsToFree[i];

            if (info.freeOnCompletion)
            {
                info.source.volume = 0;
                AudioChannel channel = FindChannelFromSource(usedChannels, info.source);
                if (channel != null)
                {
                    usedChannels.Remove(channel);
                    channel.volume = 0;
                    channel.source.volume = 0;
                    channel.source.gameObject.SetActive(false);
                    freeChannels.Add(channel);
                }

                channel = FindChannelFromSource(musicChannels, info.source);
                if (channel != null)
                {
                    musicChannels.Remove(channel);
                    channel.volume = 0;
                    channel.source.volume = 0;
                    channel.source.gameObject.SetActive(false);
                    freeChannels.Add(channel);
                }
            }

            fadingChannels.Remove(info);

        }

    }

    AudioChannel GetFreeChannel()
    {
        AudioChannel channel = null;
        if (freeChannels.Count > 0)
        {
            channel = freeChannels[0];
            channel.paused = false;

            //usedChannels.Add( channel );
            freeChannels.Remove(channel);
        }

        return channel;
    }

    public void StopAllSounds(bool stopExternal)
    {
        foreach (AudioChannel channel in usedChannels)
        {
            channel.source.Stop();
        }

        if (stopExternal)
        {
            foreach (AudioChannel channel in externalSources)
            {
                channel.source.Stop();
            }
        }
    }

    public void StopChannel(AudioChannel channel)
    {
        if (channel.source != null)
        {
            channel.source.Stop();
        }
        channel.paused = false;
    }

    public void MuteAllSounds()
    {
        foreach (AudioChannel channel in usedChannels)
        {
            FadeChannel(channel, 0, 4);
        }

        for (int i = 0; i < externalSources.Count; i++)
        {
            externalSources[i].volume = externalSources[i].source.volume;
            FadeChannel(externalSources[i], 0, 4);
        }
    }

    public void UnmuteAllSounds()
    {
        foreach (AudioChannel channel in externalSources)
        {
            FadeChannel(channel, channel.volume, 4);
        }
    }

    AudioChannel FindChannelFromSource(List<AudioChannel> list, AudioSource source)
    {
        AudioChannel channelFound = null;
        foreach (AudioChannel channel in list)
        {
            if (channel.source == source)
            {
                channelFound = channel;
            }
        }

        return channelFound;
    }

    public void PlayRandomAt(SoundBank bank, Vector3 pos, float volume)
    {
        AudioClip clip = bank.GetRandomClip();
        PlaySoundAt(clip, pos, volume);
    }

    public void PlayAt(SoundBank bank, int index, Vector3 pos, float volume)
    {
        AudioClip clip = bank.GetClip(index);
        PlaySoundAt(clip, pos, volume);
    }

    public AudioChannel PlaySound(AudioClip clip)
    {
        return PlaySoundAt(clip, Vector3.zero, 1);
    }


    #region PlaySoundAt functions
    // NB: All of these PlaySoundAt function variations are just to get around Unity not setting the AudioSource.isPlaying property correctly 
    // 		when using a delay to play a sound. Now we have to use coroutines to accomplish the same functionality.
    public AudioChannel PlaySoundAt(AudioClip clip, Vector3 pos, float volume, float pitch, bool loop, float delay)
    {
        AudioChannel channel = GetFreeChannel();
        if (channel != null)
        {
            if (delay > 0)
            {
                StartCoroutine(AfterSecondsPlay(channel, clip, pos, volume, pitch, loop, delay));
            }
            else
            {
                PlaySoundAt(channel, clip, pos, volume, pitch, loop);
            }
        }
        return channel;
    }

    IEnumerator AfterSecondsPlay(AudioChannel channel, AudioClip clip, Vector3 pos, float volume, float pitch, bool loop, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySoundAt(channel, clip, pos, volume, pitch, loop);
    }

    public AudioChannel PlaySoundAt(AudioClip clip, Vector3 pos, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioChannel channel = GetFreeChannel();
        if (channel != null)
        {
            PlaySoundAt(channel, clip, pos, volume, pitch, loop);
        }
        return channel;
    }

    void PlaySoundAt(AudioChannel channel, AudioClip clip, Vector3 pos, float volume = 1, float pitch = 1, bool loop = false)
    {
        if (channel != null)
        {
            channel.source.gameObject.SetActive(true);

            channel.source.transform.position = pos;
            channel.source.loop = loop;
            //channel.PlayOneShot( clip );

            channel.source.volume = volume * SoundLevel;
            channel.source.pitch = pitch;
            channel.source.clip = clip;
            channel.source.Play();
            channel.volume = volume;

            usedChannels.Add(channel);
        }
    }
    #endregion

    public AudioChannel PlayMusic(AudioClip clip, float startVolume, float endVolume, float fadeSpeed, bool loop = true,  Transform parent = null)
    {
        if (fadeSpeed != 0)
        {
            fadeSpeed = 1.0f / fadeSpeed;
        }
        else
        {
            fadeSpeed = 1;
        }

        AudioChannel channel = GetFreeChannel();
        if (channel != null)
        {
            usedChannels.Remove(channel);

            musicChannels.Add(channel);

            channel.source.gameObject.SetActive(true);

            if (null != parent)
            {
                channel.source.gameObject.transform.SetParent(parent);
            }

            channel.source.loop = loop;
            channel.source.volume = startVolume * MusicLevel;
            channel.source.clip = clip;
            channel.source.pitch = 1.0f;

            channel.source.Play();
            channel.volume = endVolume;
            channel.isMusic = true;

            FadeInfo info = new FadeInfo();
            info.source = channel.source;
            info.targetVolume = endVolume;
            info.channel = channel;
            info.fadeSpeed = fadeSpeed;

            info.isMusic = true;

            fadingChannels.Add(info);
        }
        return channel;
    }

    void FadeSource(AudioSource source, float endVolume, float fadeSpeed)
    {
        if (fadeSpeed != 0)
        {
            fadeSpeed = 1.0f / fadeSpeed;
        }
        else
        {
            fadeSpeed = 1;
        }

        foreach (FadeInfo info in fadingChannels)
        {
            if (info.source == source)
            {
                info.targetVolume = endVolume;
                info.fadeSpeed = fadeSpeed;
                return;
            }
        }

        FadeInfo finfo = new FadeInfo();
        finfo.source = source;
        finfo.targetVolume = endVolume;
        finfo.fadeSpeed = fadeSpeed;
        finfo.channel = FindChannelFromSource(usedChannels, source);

        fadingChannels.Add(finfo);
    }

    FadeInfo FadeChannel(AudioChannel source, float endVolume, float fadeSpeed)
    {
        if (fadeSpeed != 0)
        {
            fadeSpeed = 1.0f / fadeSpeed;
        }
        else
        {
            fadeSpeed = 1;
        }

        foreach (FadeInfo info in fadingChannels)
        {
            if (info.source == source.source)
            {
                info.targetVolume = endVolume;
                info.fadeSpeed = fadeSpeed;
                return info;
            }
        }

        FadeInfo finfo = new FadeInfo();
        finfo.source = source.source;
        finfo.targetVolume = endVolume;
        finfo.fadeSpeed = fadeSpeed;
        finfo.freeOnCompletion = false;
        finfo.channel = source;

        fadingChannels.Add(finfo);

        return finfo;
    }

    public void FadeMusic(AudioChannel source, float endVolume, float fadeSpeed, bool keepChanelAlive = false)
    {
        if (source == null)
        {
            return;
        }

        FadeInfo finfo = FadeChannel(source, endVolume, fadeSpeed);

        if (!keepChanelAlive)
        {
            finfo.freeOnCompletion = (endVolume <= Mathf.Epsilon);
        }
        else
        {
            finfo.freeOnCompletion = false;
        }
        finfo.isMusic = true;
    }

    public void PlayButtonClickSound()
    {
        PlaySoundAt(buttonClickSound, Vector3.zero, 1);
    }

    public void PauseAllEffects()
    {
        foreach (AudioChannel channel in usedChannels)
        {
            if (channel.source != null)
            {
                channel.source.Pause();
                channel.paused = true;
            }
        }
    }

    public void ResumeAllEffects()
    {
        foreach (AudioChannel channel in usedChannels)
        {
            if (channel.paused && channel.source != null)
            {
                channel.source.Play();
                channel.paused = false;
            }
        }
    }

    public void CancelPausedEffects()
    {
        foreach (AudioChannel channel in usedChannels)
        {
            if (channel.paused && channel.source != null)
            {
                channel.source.Stop();
                channel.paused = false;
            }
        }
    }
} 