using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Sentis;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace DefaultNamespace.MediaPipeParser
{
    public static class ParseMediaPipe
    {
        private static readonly Dictionary<string, int> MediaPipeIndexes = new Dictionary<string, int>()
        {
            { "NOSE", 0 },
            { "LEFT_EYE_INNER", 1 },
            { "LEFT_EYE", 2 },
            { "LEFT_EYE_OUTER", 3 },
            { "RIGHT_EYE_INNER", 4 },
            { "RIGHT_EYE", 5 },
            { "RIGHT_EYE_OUTER", 6 },
            { "LEFT_EAR", 7 },
            { "RIGHT_EAR", 8 },
            { "MOUTH_LEFT", 9 },
            { "MOUTH_RIGHT", 10 },
            { "LEFT_SHOULDER", 11 },
            { "RIGHT_SHOULDER", 12 },
            { "LEFT_ELBOW", 13 },
            { "RIGHT_ELBOW", 14 },
            { "LEFT_WRIST", 15 },
            { "RIGHT_WRIST", 16 },
            { "LEFT_PINKY", 17 },
            { "RIGHT_PINKY", 18 },
            { "LEFT_INDEX", 19 },
            { "RIGHT_INDEX", 20 },
            { "LEFT_THUMB", 21 },
            { "RIGHT_THUMB", 22 },
            { "LEFT_HIP", 23 },
            { "RIGHT_HIP", 24 },
            { "LEFT_KNEE", 25 },
            { "RIGHT_KNEE", 26 },
            { "LEFT_ANKLE", 27 },
            { "RIGHT_ANKLE", 28 },
            { "LEFT_HEEL", 29 },
            { "RIGHT_HEEL", 30 },
            { "LEFT_FOOT_INDEX", 31 },
            { "RIGHT_FOOT_INDEX", 32 }
        };

        private static (string, string)[] Joints = new (string, string)[]
        {
            ("RIGHT_HIP", "LEFT_HIP"),
            ("RIGHT_SHOULDER", "LEFT_SHOULDER"),
            
            ("RIGHT_HIP", "RIGHT_SHOULDER"),
            ("RIGHT_HIP", "RIGHT_KNEE"),
            ("RIGHT_SHOULDER", "RIGHT_ELBOW"),
            ("RIGHT_WRIST", "RIGHT_ELBOW"),
            ("RIGHT_KNEE", "RIGHT_ANKLE"),
            
            ("LEFT_HIP", "LEFT_SHOULDER"),
            ("LEFT_HIP", "LEFT_KNEE"),
            ("LEFT_SHOULDER", "LEFT_ELBOW"),
            ("LEFT_WRIST", "LEFT_ELBOW"),
            ("LEFT_KNEE", "LEFT_ANKLE"),
        };

         [Serializable]
        public struct MediaPipeKeypoint
        {
            public Vector3 Position;
            public float Presence;
            public float Visibility;
            public string Label;
        }

        [Serializable]
        public struct MediaPipePoseSet
        {
            public MediaPipeKeypoint[] Keypoints;

            public MediaPipeKeypoint GetRightWrist()
            {
                return Keypoints.First(x => x.Label == "RIGHT_WRIST");
            }
            
            public MediaPipeKeypoint GetLeftWrist()
            {
                return Keypoints.First(x => x.Label == "LEFT_WRIST");
            }
            
            public MediaPipeKeypoint GetLeftKnee()
            {
                return Keypoints.First(x => x.Label == "LEFT_KNEE");
            }
            
            public MediaPipeKeypoint GetRightKnee()
            {
                return Keypoints.First(x => x.Label == "RIGHT_KNEE");
            }
            
            public MediaPipeKeypoint GetRightHip()
            {
                return Keypoints.First(x => x.Label == "RIGHT_HIP");
            }
            
            public MediaPipeKeypoint GetLeftHip()
            {
                return Keypoints.First(x => x.Label == "LEFT_HIP");
            }

            public MediaPipeKeypoint GetRightShoulder()
            {
                return Keypoints.First(x => x.Label == "RIGHT_SHOULDER");
            }
            
            public MediaPipeKeypoint GetLeftShoulder()
            {
                return Keypoints.First(x => x.Label == "LEFT_SHOULDER");
            }

            public int GetIndex(string argItem1)
            {
                for (int i = 0; i < Keypoints.Length; i ++)
                {
                    if (Keypoints[i].Label == argItem1)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        private static int _entriesPerPoint = 5;
        
        private static MediaPipePoseSet poseSet = new MediaPipePoseSet()
        {
            Keypoints = new MediaPipeKeypoint[33], 
        };
        public static MediaPipePoseSet ParseOutput(
            TensorFloat outputTensor, 
            Vector2Int modelDimensions)
        {
            foreach (KeyValuePair<string, int> kvp in MediaPipeIndexes)
            {
                var index = kvp.Value;
                var x = outputTensor[index * _entriesPerPoint];
                var y = outputTensor[index * _entriesPerPoint + 1];
                var z = outputTensor[index * _entriesPerPoint + 2];
                var vis = outputTensor[index * _entriesPerPoint + 3];
                var pres = outputTensor[index * _entriesPerPoint + 4];
                poseSet.Keypoints[kvp.Value] = new MediaPipeKeypoint
                {
                    Position = new Vector3(x / modelDimensions.x, 1.0f - y / modelDimensions.y, z),
                    Presence = pres,
                    Visibility = vis,
                    Label = kvp.Key
                };
            }

            return poseSet;
        }

        public static (int, int)[] GetJoints(MediaPipePoseSet results)
        {
            return Joints.Select(x =>
            {
                int leftIndex = results.GetIndex(x.Item1);
                int rightIndex = results.GetIndex(x.Item2);
                return (leftIndex, rightIndex);
            }).ToArray();
        }
    }
}