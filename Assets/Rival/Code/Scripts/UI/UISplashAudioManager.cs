using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISplashAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip _menuMusic;
    
    [SerializeField]
    private AudioSource _menuSoure;

    private void OnValidate()
    {
        _menuSoure ??= GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    public void PlayMenuMusic()
    {
        _menuSoure.clip = _menuMusic;
        _menuSoure.loop = true;
        _menuSoure.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
