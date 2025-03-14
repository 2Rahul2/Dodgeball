using System.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BallSpawnManager : NetworkBehaviour
{
    public static BallSpawnManager Instance;
    // [SerializeField]private GameObject throwBallPrefab;
    [SerializeField]private Transform[] spawnPosition;
    [SerializeField]private float throwBallInterval;
    [SerializeField]private int maxSpawnBalls;
    public int currentSpawnBalls;


    [SerializeField]private GameObject[] allBallObject;
    private void Awake(){
        if(Instance == null){
            Instance = this;
        }
    }
    public override void OnNetworkSpawn(){
        if(IsServer){
            // StartCoroutine(StartSpawning());
            // StartSpawningRoutineServerRpc();
        }
    }
    public IEnumerator StartSpawning(){
        while(true){
            // StartSpawningRoutineServerRpc();
            if(currentSpawnBalls < maxSpawnBalls){
                StartSpawningRandomBallsServerRpc(0);
                StartSpawningRandomBallsServerRpc(1);
            }
            yield return new WaitForSeconds(throwBallInterval);
        }
    }
    [ServerRpc]
    private void StartSpawningRandomBallsServerRpc(int index){
        Vector3 offset = new Vector3(UnityEngine.Random.Range(0 ,2) ,0 ,UnityEngine.Random.Range(0 ,2));
        GameObject ballPrefab = Instantiate(allBallObject[UnityEngine.Random.Range(0 , allBallObject.Length)] , spawnPosition[index].position + offset , quaternion.identity);
        NetworkObject ballNetworkObject = ballPrefab.GetComponent<NetworkObject>();
        ballNetworkObject.Spawn(true);
        currentSpawnBalls += 1;
    }
    // [ServerRpc]
    // public void StartSpawningRoutineServerRpc(){
    //     if(currentSpawnBalls < maxSpawnBalls){
    //         // StartCoroutine(StartSpawning());
    //         GameObject ballPrefab = Instantiate(throwBallPrefab , spawnPosition.position , Quaternion.identity);
    //         NetworkObject throwBallNetworkObject = ballPrefab.GetComponent<NetworkObject>();
    //         throwBallNetworkObject.Spawn(true);
    //         currentSpawnBalls += 1;
    //     }
    // }
    public void RemoveCurrentSpawnBalls(){
        currentSpawnBalls -= 1;
    }
}
