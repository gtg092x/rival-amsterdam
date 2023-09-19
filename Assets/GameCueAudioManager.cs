using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCueAudioManager : MonoBehaviour
{
    public AudioClip StepBack;

    public AudioSource Source;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayStepBack()
    {
        Source.PlayOneShot(StepBack);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
