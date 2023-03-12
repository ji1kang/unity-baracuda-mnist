// Import packages
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // List 데이터 관련 소스를 간단하게 만들어줌
using Unity.Barracuda;
using UnityEngine;

public class GetInferenceFromModel : MonoBehaviour
{

    public Texture2D texture; // Texture2D: 텍스쳐 생성, 수정을 위한 속성
    // 아래는 바라쿠다에서 쓰이는 타입들
    public NNModel modelAsset;
    private Model _runtimeModel;
    private IWorker _engine;

    [Serializable]
    public struct Prediction
    {
        // The most likely value for this prediction
        public int predictedValue;
        // The list of likelihoods for all the possible classes
        public float[] predicted;

        public void SetPrediction(Tensor t)
        {
            // 주어진 텐서 t에 대해 inference를 수행하는 과정 
            predicted = t.AsFloats();
            // 가장 확률이 높은 클래스를 저장
            predictedValue = Array.IndexOf(predicted, predicted.Max());
            Debug.Log($"Predicted {predictedValue}");
        }
    }

    public Prediction prediction;
    
    // Start is called before the first frame update
    void Start()
    {
        // 모델 실행을 위한 런타임과 엔진 로드
        _runtimeModel = ModelLoader.Load(modelAsset);
        _engine = WorkerFactory.CreateWorker(_runtimeModel, WorkerFactory.Device.GPU);
        // Instantiate our prediction struct.
        prediction = new Prediction();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 그레이 스케일 텐서 만들기
            var channelCount = 1; //grayscale, 3 = color, 4 = color+alpha
            // Create a tensor for input from the texture.
            var inputX = new Tensor(texture, channelCount);

            // Peek at the output tensor without copying it.
            Tensor outputY = _engine.Execute(inputX).PeekOutput();
            // Set the values of our prediction struct using our output tensor.
            prediction.SetPrediction(outputY);
            
            // Dispose of the input tensor manually (not garbage-collected).
            inputX.Dispose();
        }
    }

    private void OnDestroy()
    {
        // Dispose of the engine manually (not garbage-collected).
        _engine?.Dispose();
    }
}
