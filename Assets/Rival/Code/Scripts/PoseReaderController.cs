using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class PoseReaderController : MonoBehaviour
{
    [Serializable]
    public enum Poses
    {
        LeftThigh,
        RightThigh,
    }
    public UnityEvent OnPowerPose;
    
    [Serializable]
    public class PoseEvent : UnityEvent<Poses> {
    }

    public PoseEvent OnPose;

    public void DoRightThighPose()
    {
        OnPose?.Invoke(Poses.RightThigh);
    }
    
    public void DoLeftThighPose()
    {
        OnPose?.Invoke(Poses.LeftThigh);
    }

    public void DoPowerPose ()
    {
        OnPowerPose?.Invoke();
    }


    [SerializeField]
    private PlayableDirector _director;

    [SerializeField]
    private UnityEvent OnRead;
    
    [SerializeField]
    private UnityEvent OnEndRead;
    
    public void Read()
    {
        //_director.time = 0f;
        //_director.Play();
        OnRead?.Invoke();
    }
    
    private IEnumerator DoPowerPoseInOne()
    {
        yield return new WaitForSeconds(1f);
        DoPowerPose();
    }

    public void EndRead()
    {
        //_director.Stop();
        OnEndRead?.Invoke();
    }
}
