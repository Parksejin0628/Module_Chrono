using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;


public class PlayerCtrl : MonoBehaviour
{
    //컴포넌트를 받을 변수
    Rigidbody2D rigidbody2D;
    PlayerInput playerInput;
    Camera camera;

    //캐릭터 기본 정보
    public float speed = 2.0f;                          //좌우 움직임의 가속 크기
    public float maxSpeed = 10.0f;                      //좌우 움직임의 최고 속도
    public float brakingPower = 0.75f;                  //키보드에서 키보드를 뗄 경우 감속되는 배율, 값은 0 ~ 1이다.
    private Vector2 inputVector;                        //상하좌우 버튼이 눌린 것을 백터로 표현한 변수, 오른쪽 방향키를 누르면 (1, 0)값을 가진다.
    
    //점프 관련 변수
    public int jumpCount = 1;                           //남은 점프 개수
    public int maxJumpCount = 1;                        //점프 충전시 회복되는 개수
    public float jumpPower = 100.0f;                    //점프를 할 때 위로 가하는 힘의 크기
    //대쉬 관련 변수
    private bool isDash = false;                        //대쉬 중인지 아닌지 확인시키는 변수
    public float dashPower = 1000.0f;                   //대쉬를 할 때 가해지는 힘의 크기
    public float dashTime = 0.1f;                       //대쉬를 하는 시간을 결정하는 변수
    public float dashCoolTimeSetting = 2f;            //대쉬 쿨타임을 조정하는 변수
    public float dashCoolTime = 2f;                   //현재 대쉬 쿨타임
    public float dashCount = 2;                         //남은 대쉬 개수
    public float maxDashCount = 2;                      //대쉬 충전되는 개수
    public float continuousDashDelay = 0.5f;            //연속으로 대쉬를 할 때의 딜레이 남은 시간
    public float continuousDashDelaySetting = 0.5f;     //연속으로 대쉬를 할 때의 딜레이



    // Start is called before the first frame update
    void Awake()
    {
        //컴포넌트 초기화
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //플레이어 데이터 초기화
        
    }
    private void Update()
    {
        SetCoolTime(); //쿨타임 업데이트
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        //땅으로 떨어지는 중이면서 바닥에 닿을 경우 점프를 회복한다.
        if(rigidbody2D.velocity.y < 0 && CheckIsGround())
        {
            jumpCount = maxJumpCount;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("DashBonus"))
        {
            if(dashCount < maxDashCount)
            {
                dashCount++;
                StartCoroutine(GameManager.instance.InteractDashBonus(coll.gameObject));
            }

        }
    }

    void OnMove(InputValue value)   //방향키를 누르거나 땔 때 호출되는 함수이다.
    {
        inputVector = value.Get<Vector2>();     //현재 방향키의 정보를 담은 value를 Vector2로 치환한다. InputValue인 value를 사용하기 편하게 Vector2로 치환한 것이다.

        //점프도 addForce로 처리하면서 maxSpeed를 정해둔다음 점프버튼을 누른 길이에 따라 점프 높이를 바뀌도록 하면 어떨까?
    }

    void OnJump()   //스페이스바를 누르면 호출되는 함수이다.
    {
        if (jumpCount <= 0 || CheckIsGround() == false)
        {
            return;
        }
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);  //현재 받고 있는 힘의 크기와 상관없이 일정한 점프를 하기 위해 velocity를 0으로 고정 후 점프
        rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
        jumpCount--;
    }

    void OnLeftClick()  //좌클릭을 하면 호출되는 함수이다.
    {
        
    }

    void OnRightClick() //우클릭을 하면 호출되는 함수이다.
    {
        if(!isDash && dashCount > 0 && continuousDashDelay <= 0)
        {
            StartCoroutine(Dash(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue())));    // Dash 코루틴을 호출
        }
    }

    void OnChangeTime(InputValue value) //시간을 변경 키를 누르면 작동하는 함수이다.
    {
        Vector2 inputKey = value.Get<Vector2>();
        Debug.Log(value.Get<Vector2>());
        if(inputKey.y > 0)
        {
            StartCoroutine(GameManager.instance.ChangeTime(2));
        }
        else if(inputKey.y < 0)
        {
            StartCoroutine(GameManager.instance.ChangeTime(0));
        }
        else if(inputKey.x > 0)
        {
            StartCoroutine(GameManager.instance.ChangeTime(1));
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

    bool CheckIsGround() //땅인지 체크하는 함수
    {
        float rayDistance = 0.2f;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + Vector3.down * transform.localScale.y / 4, transform.localScale / 3, 0, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));
        if (hit == true)
        {
            return true;
        }
        return false;
    }

    void SetCoolTime()
    {
        //대쉬가 꽉차있으면 대쉬 쿨타임을 초기화한다.
        if (dashCount == maxDashCount && dashCoolTime != dashCoolTimeSetting)
        {
            dashCoolTime = dashCoolTimeSetting;
        }
        //대쉬가 꽉차있지 않으면 대쉬 쿨타임을 돌게한다.
        if (dashCount < maxDashCount && dashCoolTime > 0)
        {
            dashCoolTime -= Time.deltaTime;
            if(dashCoolTime < 0)
            {
                dashCount++;
                dashCoolTime = dashCoolTimeSetting;
            }
        }
        //대쉬 연속 딜레이 쿨타임을 돌게 한다.
        if(continuousDashDelay > 0)
        {
            continuousDashDelay -= Time.deltaTime;
        }
    }

    /*void OnDrawGizmos()
    {
        if (rigidbody2D.velocity.y > 0)
        {
            return;
        }
        Gizmos.color = Color.red;
        float rayDistance = 0.2f;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + Vector3.down * transform.localScale.y / 4, transform.localScale / 3, 0, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));
        if (hit == true)
        {
            Gizmos.DrawRay(transform.position + Vector3.down * transform.localScale.y / 4, Vector3.down * hit.distance);
            Gizmos.DrawWireCube(transform.position + Vector3.down * transform.localScale.y / 4 + Vector3.down * hit.distance, transform.localScale/3);
        }
        else
        {
            Gizmos.DrawRay(transform.position + Vector3.down * transform.localScale.y / 4, Vector3.down * rayDistance);
            Gizmos.DrawWireCube(transform.position + Vector3.down * transform.localScale.y / 4 + Vector3.down * rayDistance, transform.localScale/3);
        }
    }
    */
    //대쉬를 수행하는 함수
    IEnumerator Dash(Vector3 targetPos)
    {
        Vector2 dashDirVec = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y).normalized;    //대쉬방향벡터
        float originGravity = rigidbody2D.gravityScale;
        //대쉬중임을 isDash를 통해 다른 함수에도 알린 뒤, 중력 및 가속을 0으로 만들고 대쉬 방향으로 큰 힘을 가한다.
        isDash = true;
        dashCount--;
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(dashDirVec * dashPower, ForceMode2D.Force);
        //대쉬 시간만큼 기다린 후 가속을 0으로 만들고 이전에 0으로 만들었던 중력을 원래대로 돌린다. 그 후 대쉬 쿨타임을 지정하고 isDash를 false 함으로써 대쉬를 종료한다. 
        yield return new WaitForSeconds(dashTime);
        continuousDashDelay = continuousDashDelaySetting;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.gravityScale = originGravity;
        isDash = false;
    }


}


