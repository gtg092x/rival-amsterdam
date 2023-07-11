using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip _songAudio;
    
    [SerializeField] private AudioSource _audioEmitter;
    
    [SerializeField] private GameManager _gameManager;

    private void OnValidate()
    {
        _audioEmitter ??= GetComponent<AudioSource>();
        _gameManager ??= FindAnyObjectByType<GameManager>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
