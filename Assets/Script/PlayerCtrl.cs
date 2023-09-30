using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;


public class PlayerCtrl : MonoBehaviour
{
    //컴포넌트를 받을 변수
    Transform transform;
    Rigidbody2D rigidbody2D;
    PlayerInput playerInput;
    Camera camera;

    public float speed = 2.0f;                          //좌우 움직임의 가속 크기
    public float maxSpeed = 10.0f;                      //좌우 움직임의 최고 속도
    public float jumpPower = 100.0f;                    //점프를 할 때 위로 가하는 힘의 크기
    public float dashPower = 1000.0f;                   //대쉬를 할 때 가해지는 힘의 크기
    public float dashTime = 0.1f;                       //대쉬를 하는 시간을 결정하는 변수
    public float dashCoolTimeSetting = 0.5f;                    //대쉬 쿨타임을 조정하는 변수
    private float dashCoolTime = 0f;                  //현재 대쉬 쿨타임
    public float brakingPower = 0.75f;                  //키보드에서 키보드를 뗄 경우 감속되는 배율, 값은 0 ~ 1이다.
    private Vector2 inputVector;                        //상하좌우 버튼이 눌린 것을 백터로 표현한 변수, 오른쪽 방향키를 누르면 (1, 0)값을 가진다.
    private bool isDash = false;                        //대쉬 중인지 아닌지 확인시키는 함수


    // Start is called before the first frame update
    void Awake()
    {
        //컴포넌트 초기화
        transform = GetComponent<Transform>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    private void Update()
    {
        if(dashCoolTime >= 0)
        {
            dashCoolTime -= Time.deltaTime;
        }
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

    void OnJump()   //스페이스바를 누르면 호출되는 함수이다.
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);  //현재 받고 있는 힘의 크기와 상관없이 일정한 점프를 하기 위해 velocity를 0으로 고정 후 점프
        rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
    }

    void OnLeftClick()  //좌클릭을 하면 호출되는 함수이다.
    {
        /*
        Debug.Log("Left Click");
        Debug.Log(Mouse.current.position.ReadValue());
        Debug.Log(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        */
    }

    void OnRightClick() //우클릭을 하면 호출되는 함수이다.
    {
        if(!isDash && dashCoolTime <= 0)
        {
            StartCoroutine(Dash(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue())));    // Dash 코루틴을 호출
        }
    }

    void Move()
    {
        if (isDash) return; //만약 대쉬중인경우에는 이 함수를 중단시킨다.
        //만약 좌우 방향키에서 손을 뗀다면 속도를 급속도로 감소시킨다.
        if(Mathf.Abs(inputVector.x) < 1)
        {
            //Debug.Log("stop");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * brakingPower, rigidbody2D.velocity.y);
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
    //대쉬를 수행하는 함수
    IEnumerator Dash(Vector3 targetPos)
    {
        Vector2 dashDirVec = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y).normalized;    //대쉬방향벡터
        //대쉬중임을 isDash를 통해 다른 함수에도 알린 뒤, 중력 및 가속을 0으로 만들고 대쉬 방향으로 큰 힘을 가한다.
        isDash = true;  
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(dashDirVec * dashPower, ForceMode2D.Force);
        //대쉬 시간만큼 기다린 후 가속을 0으로 만들고 이전에 0으로 만들었던 중력을 원래대로 돌린다. 그 후 대쉬 쿨타임을 지정하고 isDash를 false 함으로써 대쉬를 종료한다. 
        yield return new WaitForSeconds(dashTime);
        dashCoolTime = dashCoolTimeSetting;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.gravityScale = 1f;
        isDash = false;
    }
}


