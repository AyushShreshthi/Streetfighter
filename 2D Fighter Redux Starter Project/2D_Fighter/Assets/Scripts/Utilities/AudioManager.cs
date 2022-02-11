using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource ad;
    public List<SoundClass> sc = new List<SoundClass>();

    public static AudioManager instance;
    public static AudioManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //ad = GetComponent<AudioSource>();
    }
    public void PlaySound(string name)
    {
        foreach(var s in sc)
        {
            if (s.name == name)
            {
                ad.clip = s.clip;
                ad.loop = false;
                ad.PlayOneShot(ad.clip);
                break;
            }
        }
        ad.clip = null;
    }
}
[System.Serializable]
public class SoundClass
{
    public AudioClip clip;
    public string name;
}
