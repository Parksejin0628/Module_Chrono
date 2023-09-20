using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    Transform[] spawnPoints;
    public SpawnData[] spawnData;
    GameObject temp;

    int spawnLevel;
    private float timer = 0f;
    // Start is called before the first frame update
    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        spawnLevel = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 10f), 1);

        if (timer > spawnData[spawnLevel].spawnTime)
        {
            temp = GameManager.instance.poolManager.Spawn(0);
            temp.transform.position = spawnPoints[Random.Range(1, spawnPoints.Length)].position;
            temp.GetComponent<Enemy>().Init(spawnData[spawnLevel]);
            timer = 0f;
        }
    }


}

[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public float health;
    public float speed;
}