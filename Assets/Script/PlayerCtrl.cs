using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerCtrl : MonoBehaviour
{
    //컴포넌트를 받을 변수
    Transform transform;
    Rigidbody2D rigidbody2D;
    PlayerInput playerInput;

    public float speed = 2.0f;                          //좌우 움직임의 가속 크기
    public float maxSpeed = 10.0f;                      //좌우 움직임의 최고 속도
    public float jumpPower = 100.0f;                    //점프를 할 때 위로 가하는 힘의 크기
    private Vector2 inputVector;                        //상하좌우 버튼이 눌린 것을 백터로 표현한 변수, 오른쪽 방향키를 누르면 (1, 0)값을 가진다.


    // Start is called before the first frame update
    void Awake()
    {
        //컴포넌트 초기화
        transform = GetComponent<Transform>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void OnMove(InputValue value)   //방향키를 누르거나 땔 때 호출되는 함수이다.
    {
        inputVector = value.Get<Vector2>();     //현재 방향키의 정보를 담은 value를 Vector2로 치환한다. InputValue인 value를 사용하기 편하게 Vector2로 치환한 것이다.

        //점프도 addForce로 처리하면서 maxSpeed를 정해둔다음 점프버튼을 누른 길이에 따라 점프 높이를 바뀌도록 하면 어떨까?
    }

    void OnJump()
    {
        rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
    }

    void Move()
    {
        //만약 좌우 방향키에서 손을 뗀다면 속도를 급속도로 감소시킨다.
        if(Mathf.Abs(inputVector.x) < 1)
        {
            Debug.Log("stop");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * 0.75f, rigidbody2D.velocity.y);
            return;
        }
        //방향키를 전환할 때 빠르게 전환할 수 있도록 힘을 초기화한다.
        if(Mathf.Sign(inputVector.x) != Mathf.Sign(rigidbody2D.velocity.x))
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }
        rigidbody2D.AddForce(Vector2.right * inputVector.x * speed, ForceMode2D.Force); //좌우로 움직인다.
        //힘을 너무 받아 maxSpeed 이상을 넘어갈 경우 힘을 고정시킨다.
        if(rigidbody2D.velocity.x > maxSpeed)
        {
            rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
        }
        else if (rigidbody2D.velocity.x < maxSpeed * -1)
        {
            rigidbody2D.velocity = new Vector2(maxSpeed * -1, rigidbody2D.velocity.y);
        }
        
    }


}


