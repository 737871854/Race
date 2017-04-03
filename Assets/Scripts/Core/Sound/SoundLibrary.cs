using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundLibrary : MonoBehaviour
{
    [SerializeField]
    SoundBank[] soundLibrary;

    Dictionary<string, SoundBank> soundLibraryDict = new Dictionary<string, SoundBank>();

    public SoundBank[] banks
    {
        get { return soundLibrary; }
    }

    // Use this for initialization
    public void Init()
    {
        GenerateDictionary();
    }

    void GenerateDictionary()
    {
        soundLibraryDict = new Dictionary<string, SoundBank>();

        if (soundLibrary != null)
        {
            foreach (SoundBank s in soundLibrary)
            {
                soundLibraryDict.Add(s.name, s);
            }
        }
    }

    public AudioClip GetSoundFromLibrary(string soundName)
    {
        if (soundLibraryDict.ContainsKey(soundName))
        {
            return soundLibraryDict[soundName].GetRandomClip();
        }

        return null;
    }

    public void MergeLibraries(SoundLibrary[] libraries)
    {
        List<SoundBank> newBanks = new List<SoundBank>();

        if (soundLibrary != null)
        {
            newBanks.AddRange(soundLibrary);
        }

        foreach (SoundLibrary sl in libraries)
        {
            newBanks.AddRange(sl.soundLibrary);
        }

        soundLibrary = newBanks.ToArray();

        GenerateDictionary();
    }

    public void RemoveLibrary(SoundLibrary library)
    {
        List<SoundBank> oldBanks = new List<SoundBank>();

        if (soundLibrary != null)
        {
            oldBanks.AddRange(soundLibrary);
        }

        foreach (SoundBank bank in library.soundLibrary)
        {
            foreach (SoundBank oldBank in oldBanks)
            {
                if (oldBank.name == bank.name)
                {
                    oldBanks.Remove(oldBank);
                    break;
                }
            }
        }

        soundLibrary = oldBanks.ToArray();

        GenerateDictionary();
    }
}
