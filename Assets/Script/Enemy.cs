using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D target;
  
    public float health = 0f;
    public float maxHealth = 0f;
    public float speed = 2.0f;

    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;
    public RuntimeAnimatorController[] animCon;
    private bool isLive;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        if (!isLive) return;

        Vector2 dirVec = target.position - rigid.position;
        dirVec = dirVec.normalized;
        Vector2 nextVec = dirVec * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!isLive) return;

        if (target.position.x - rigid.position.x < 0)
        {
            spriter.flipX = true;
        }
        else if(target.position.x - rigid.position.x > 0)
        {
            spriter.flipX = false;
        }
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }

    public void Init(SpawnData spawnData)
    {
        health = spawnData.health;
        maxHealth = spawnData.health;
        speed = spawnData.speed;
        anim.runtimeAnimatorController = animCon[spawnData.spriteType];
    }

}
