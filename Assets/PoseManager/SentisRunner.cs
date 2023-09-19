using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.MediaPipeParser;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SentisRunner : MonoBehaviour
{

    
    [Serializable]
    public class MLVisReadEvent : UnityEvent<ParseMediaPipe.MediaPipePoseSet>
    {
    }
    
    public MLVisReadEvent onPredictionUpdate;

    public ModelAsset modelAsset;

    public Texture inputTexture;
    // Start is called before the first frame update
    private Model runtimeModel;
    private IWorker worker;
    private Vector2Int modelDimensions;
    private HashSet<string> jointsWl;
    void Start()
    {
        jointsWl = ShowJoints.ToHashSet();
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
        var tensorShape = runtimeModel.inputs[0].shape.ToTensorShape();
        modelDimensions = new Vector2Int(tensorShape[1], tensorShape[2]);
    }

    private Coroutine playRoutine;
    public void Play()
    {
        playRoutine = StartCoroutine(ExecutePoseRoutine());
    }

    public void Stop()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }
    }
    
    void OnDisable()
    {
        // Tell the GPU we're finished with the memory the engine used 
        worker.Dispose();
    }

    public void SetTexture(Texture tex)
    {
        inputTexture = tex;
    }

    private float lastRun = 0f;
    private float lastCheck = 0f;
    public bool LogEnabled = false;
    void LogTime(string str = "Delta")
    {
        if (!LogEnabled) return;
        var delta = Time.time - lastRun;
        var cdelta = Time.time - lastCheck;
        Debug.Log($"{str}: {delta} - {cdelta}");
        lastCheck = Time.time;
    }

    public string[] ShowJoints;
    IEnumerator ExecutePoseRoutine()
    {
        var delta = Time.time - lastRun;
        LogTime("Start");

        var tensor = TextureConverter.ToTensor(inputTexture, new TextureTransform().SetDimensions(256,256,3).SetTensorLayout(TensorLayout.NHWC));
       
        LogTime("PostConvert");
        
        LogTime("PostSchedule");
        TensorFloat outputTensor = worker.Execute(tensor).PeekOutput() as TensorFloat;
        LogTime("PostPeek");
        outputTensor.AsyncReadbackRequest();
        while (! outputTensor.IsAsyncReadbackRequestDone())
            yield return null;
        LogTime("PostAsync");
        //var arr = outputTensor.ToReadOnlyArray();
        outputTensor.MakeReadable();
        LogTime("PostRead");
        ParseMediaPipe.MediaPipePoseSet results = ParseMediaPipe.ParseOutput(
            outputTensor,
            modelDimensions
        );
        if (jointsWl.Count > 0)
        {
            results.Keypoints = results.Keypoints.Where(x => jointsWl.Contains(x.Label)).ToArray();
        }
        try
        {
            onPredictionUpdate?.Invoke(results);    
        } catch(Exception e){}
        
        outputTensor.Dispose();
        tensor.Dispose();   
        LogTime("PostDispose");
        
        yield return new WaitForEndOfFrame();
        
        playRoutine = StartCoroutine(ExecutePoseRoutine());
        lastRun = Time.time;
        yield break;
    }

    private void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // <class_index> <box x> <box y> <box width> <box height> <x, y, z>[17]
}
