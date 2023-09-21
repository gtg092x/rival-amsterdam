using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.MediaPipeParser;
using UnityEngine;

public class PoseAnalyzer : MonoBehaviour
{
    // Start is called before the first frame update
    public void HandleMPPose(ParseMediaPipe.MediaPipePoseSet poseSet)
    {
        var visRate = (float)poseSet.Keypoints.Where(x => x.Visibility > 0f).Count() / (float)poseSet.Keypoints.Length;
        CheckPowerPose(poseSet, visRate);
        CheckLeftKneePose(poseSet, visRate);
        CheckRightKneePose(poseSet, visRate);
    }

    private void CheckPowerPose(ParseMediaPipe.MediaPipePoseSet poseSet, float visRate)
    {
        ParseMediaPipe.MediaPipeKeypoint leftWrist = poseSet.GetLeftWrist();
        ParseMediaPipe.MediaPipeKeypoint rightWrist = poseSet.GetRightWrist();
        
        ParseMediaPipe.MediaPipeKeypoint rightShoulder = poseSet.GetRightShoulder();
        ParseMediaPipe.MediaPipeKeypoint leftShoulder = poseSet.GetLeftShoulder();
        
        if (leftWrist.Position.y - rightWrist.Position.y < PowerPoseVariance && leftWrist.Position.y > leftShoulder.Position.y &&
            rightWrist.Position.y > rightShoulder.Position.y && leftWrist.Visibility > 0f && rightWrist.Visibility > 0f && visRate > 0.75f)
        {
            PoseController.DoPowerPose();
        }
    }

    public float PowerPoseVariance;

    private void CheckRightKneePose(ParseMediaPipe.MediaPipePoseSet poseSet, float visRate)
    {
        var rightKnee = poseSet.GetRightKnee();
        var leftKnee = poseSet.GetLeftKnee();
        if (rightKnee.Position.y - leftKnee.Position.y > KneeThreshold && rightKnee.Visibility > 0f && leftKnee.Visibility > 0f && visRate > 0.75f)
        {
            PoseController.DoRightThighPose();
        }
    }

    public float KneeThreshold = 0.1f;

    private void CheckLeftKneePose(ParseMediaPipe.MediaPipePoseSet poseSet, float visRate)
    {
        var rightKnee = poseSet.GetRightKnee();
        var leftKnee = poseSet.GetLeftKnee();
        if (leftKnee.Position.y - rightKnee.Position.y > KneeThreshold && leftKnee.Visibility > 0f  && rightKnee.Visibility > 0f && visRate > 0.75f)
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
