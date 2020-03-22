using UnityEngine;
using UnityEngine.Networking;

public class CS_BotSpawn : NetworkBehaviour {

    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float startSpawn, repeatSpawn;

    public override void OnStartServer()
    {
        InvokeRepeating("Spawn", startSpawn, repeatSpawn);
    }

    void Spawn ()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        NetworkServer.Spawn(enemy);
    }
}