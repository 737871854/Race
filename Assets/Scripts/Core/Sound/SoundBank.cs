using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SoundBank
{
	public string name;
    public AudioClip[] sounds;
	List<AudioClip> soundsList = new List<AudioClip>();
	
	void FillList()
	{
		foreach( AudioClip clip in sounds )
		{
			soundsList.Add(clip);
		}
	}
	
    public AudioClip GetRandomClip()
    {
		if ( soundsList.Count == 0 )
		{
			FillList();
		}
		
		if (  soundsList.Count > 0 )
		{
			AudioClip clip = soundsList[0];
			soundsList.RemoveAt(0);
			return clip;
		}
		
		/*if ( _sounds.Length == 1 )
		{
			return _sounds[0];
		}
		else if ( _sounds.Length > 0 )
        {
            int ind = Random.Range( 0, _sounds.Length );
            return _sounds[ind];
        }*/

        return null;
    }

    public AudioClip GetClip( int index )
    {
        if ( index < sounds.Length )
        {
            return sounds[index];
        }
        return null;
    }
} 