using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    public Vector2 TileSize;
    public float enemyRepositionDistance = 20.0f;

    private Collider2D coll;

    private void Awake()
    {
        TileSize = Vector2.one * 20;
        coll = GetComponent<Collider2D>();
    }
    

    // Update is called once per frame
    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 thisPos = transform.position;
        float diffX = Mathf.Abs(playerPos.x - thisPos.x);
        float diffY = Mathf.Abs(playerPos.y - thisPos.y);

        Vector3 playerDir = GameManager.instance.player.inputVec;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch(transform.tag)
        {
            case "Ground":
                if(diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * TileSize.x * 2);
                    break;
                }
                else if(diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * TileSize.y * 2);
                }
                break;
            case "Enemy":
                if(coll.enabled)
                {
                    transform.Translate(playerDir * enemyRepositionDistance + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                }


                break;
        }
    }
}
