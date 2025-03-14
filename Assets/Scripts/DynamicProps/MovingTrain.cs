using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MovingTrain : MonoBehaviour
{
    [SerializeField]private Transform trainObject;
    private Vector3 initialTrainPosition;
    private Quaternion initialTrainRotation;

    [SerializeField]private float trainRunDuration;
    private float resetTrainDuration;
    [SerializeField]private int trainSpeed;
    private bool trainRun;

    private void Start(){
        resetTrainDuration = trainRunDuration;
        initialTrainPosition = trainObject.position;
        initialTrainRotation = trainObject.rotation;
        trainObject.gameObject.SetActive(false);
        StartCoroutine(TrainStopInterval());
    }
    private void Update()
    {
        if(trainRun){
            trainRunDuration -= Time.deltaTime;
            if(trainRunDuration < 0){
                trainRun = false;
                trainRunDuration = resetTrainDuration;
                StartCoroutine(TrainStopInterval());
            }else{
                trainObject.position += Time.deltaTime * trainSpeed * trainObject.forward;
            }
        }
    }
    IEnumerator TrainStopInterval(){
        trainRun = false;
        trainObject.gameObject.SetActive(false);
        yield return new WaitForSeconds(40);
        trainObject.SetPositionAndRotation(initialTrainPosition , initialTrainRotation);
        trainObject.gameObject.SetActive(true);
        trainRun = true;
        yield break;
    }
}
