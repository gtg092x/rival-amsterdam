using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.MediaPipeParser;
using UnityEngine;

public class PoseAnalyzer : MonoBehaviour
{
    // Start is called before the first frame update
    public void HandleMPPose(ParseMediaPipe.MediaPipePoseSet poseSet)
    {
        CheckPowerPose(poseSet);
        CheckLeftKneePose(poseSet);
        CheckRightKneePose(poseSet);
    }

    private void CheckPowerPose(ParseMediaPipe.MediaPipePoseSet poseSet)
    {
        ParseMediaPipe.MediaPipeKeypoint leftWrist = poseSet.GetLeftWrist();
        ParseMediaPipe.MediaPipeKeypoint rightWrist = poseSet.GetRightWrist();
        
        ParseMediaPipe.MediaPipeKeypoint rightShoulder = poseSet.GetRightShoulder();
        ParseMediaPipe.MediaPipeKeypoint leftShoulder = poseSet.GetLeftShoulder();
        
        if (leftWrist.Position.y - rightWrist.Position.y < PowerPoseVariance && leftWrist.Position.y > leftShoulder.Position.y &&
            rightWrist.Position.y > rightShoulder.Position.y && leftWrist.Visibility > 0f && rightWrist.Visibility > 0f)
        {
            PoseController.DoPowerPose();
        }
    }

    public float PowerPoseVariance;

    private void CheckRightKneePose(ParseMediaPipe.MediaPipePoseSet poseSet)
    {
        var rightKnee = poseSet.GetRightKnee();
        var leftKnee = poseSet.GetLeftKnee();
        if (rightKnee.Position.y - leftKnee.Position.y > KneeThreshold && rightKnee.Visibility > 0f)
        {
            PoseController.DoRightThighPose();
        }
    }

    public float KneeThreshold = 0.1f;

    private void CheckLeftKneePose(ParseMediaPipe.MediaPipePoseSet poseSet)
    {
        var rightKnee = poseSet.GetRightKnee();
        var leftKnee = poseSet.GetLeftKnee();
        if (leftKnee.Position.y - rightKnee.Position.y > KneeThreshold && leftKnee.Visibility > 0f)
        {
            PoseController.DoLeftThighPose();
        }
    }


    public PoseReaderController PoseController;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
