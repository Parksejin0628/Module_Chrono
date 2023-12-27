using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public enum PlayerTime { CURRENT, PAST };

public class PlayerCtrl : MonoBehaviour
{
    //������Ʈ�� ���� ����
    Rigidbody2D rigidbody2D;
    PlayerInput playerInput;
    GameObject virtualCamera;
    SpriteRenderer spriteRenderer;
    Animator anim;
    Camera camera;

    //ĳ���� �⺻ ����
    public float speed = 2.0f;                          //�¿� �������� ���� ũ��
    public float maxSpeed = 10.0f;                      //�¿� �������� �ְ� �ӵ�
    public float brakingPower = 0.75f;                  //Ű���忡�� Ű���带 �� ��� ���ӵǴ� ����, ���� 0 ~ 1�̴�.
    private Vector2 inputVector;                        //�����¿� ��ư�� ���� ���� ���ͷ� ǥ���� ����, ������ ����Ű�� ������ (1, 0)���� ������.
    public int hp = 5;                                      //ü��
    public int maxHp = 5;                                   //�ִ�ü��

    //���� ���� ����
    public int jumpCount = 1;                           //���� ���� ����
    public int maxJumpCount = 1;                        //���� ������ ȸ���Ǵ� ����
    public float jumpPower = 100.0f;                    //������ �� �� ���� ���ϴ� ���� ũ��
    //�뽬 ���� ����
    private bool isDash = false;                        //�뽬 ������ �ƴ��� Ȯ�ν�Ű�� ����
    public float dashPower = 1000.0f;                   //�뽬�� �� �� �������� ���� ũ��
    public float dashTime = 0.1f;                       //�뽬�� �ϴ� �ð��� �����ϴ� ����
    public float dashCoolTimeSetting = 2f;              //�뽬 ��Ÿ���� �����ϴ� ����
    public float dashCoolTime = 2f;                     //���� �뽬 ��Ÿ��
    public float dashCount = 2;                         //���� �뽬 ����
    public float maxDashCount = 2;                      //�뽬 �����Ǵ� ����
    public float continuousDashDelay = 0.5f;            //�������� �뽬�� �� ���� ������ ���� �ð�
    public float continuousDashDelaySetting = 0.5f;     //�������� �뽬�� �� ���� ������
    //�� ���� ���� ����
    public PlayerTime nowTime = PlayerTime.CURRENT;


    // Start is called before the first frame update
    void Awake()
    {
        //������Ʈ �ʱ�ȭ
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        virtualCamera = GameObject.Find("Virtual Camera");
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        //�÷��̾� ������ �ʱ�ȭ
        
    }
    private void Update()
    {
        SetCoolTime(); //��Ÿ�� ������Ʈ
        if(hp<=0)
        {
            hp = 5;
            GameManager.instance.Revive();
        }

        if(rigidbody2D.velocity.y >= 40)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 40.0f);
        }

        if(rigidbody2D.velocity.y <= -40)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -40.0f);
        }
        
        if(rigidbody2D.velocity.x >= 40)
        {
            rigidbody2D.velocity = new Vector2(40.0f, rigidbody2D.velocity.y);
        }

        if(rigidbody2D.velocity.x <= -40)
        {
            rigidbody2D.velocity = new Vector2(-40.0f, rigidbody2D.velocity.y);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit2D hit;
        Move();
        //������ �������� ���̸鼭 �ٴڿ� ���� ��� ������ ȸ���Ѵ�.
        CheckIsGround(LayerMask.GetMask("Ground"), out hit);
        if (rigidbody2D.velocity.y < 0 && hit == true)
        {
            Debug.Log(hit.collider.gameObject);
            jumpCount = maxJumpCount;
        }
        anim.SetFloat("velocityY", rigidbody2D.velocity.y);
        if(hit == true)
        {
            anim.SetBool("isFloat", false);
        }
        else if(hit == false)
        {
            anim.SetBool("isFloat", true);
        }
       
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        //�뽬 ���ʽ��� ���� �ܿ� �뽬�� �߰��� �� ���� �Ŵ����� �뽬 ���ʽ� ��ȣ�ۿ� �Լ��� ������.
        if(coll.CompareTag("DashBonus"))
        {
            if(dashCount < maxDashCount)
            {
                dashCount++;
                StartCoroutine(GameManager.instance.InteractDashBonus(coll.gameObject));
            }
        }
        //��Ż�� ���� ��� ��Ż�� �ڽ� ��ǥ�� �̵��Ѵ�.
        if(coll.CompareTag("Portal"))
        {
            transform.position = coll.transform.GetChild(0).transform.position;
            GameManager.instance.MoveNextStage();
        }
        //ü�� ���ʽ��� ���� �� ü���� ȸ���Ѵ�.
        if (coll.CompareTag("HpBonus"))
        {
            if(hp < maxHp)
            {
                hp++;
                coll.gameObject.SetActive(false);
            }
        }
        if(coll.CompareTag("Obstacle"))
        {
            hp--;
        }
    }

    void OnMove(InputValue value)   //����Ű�� �����ų� �� �� ȣ��Ǵ� �Լ��̴�.
    {
        inputVector = value.Get<Vector2>();     //���� ����Ű�� ������ ���� value�� Vector2�� ġȯ�Ѵ�. InputValue�� value�� ����ϱ� ���ϰ� Vector2�� ġȯ�� ���̴�.

        //������ addForce�� ó���ϸ鼭 maxSpeed�� ���صд��� ������ư�� ���� ���̿� ���� ���� ���̸� �ٲ�� �ϸ� ���?
    }

    void OnJump()   //�����̽��ٸ� ������ ȣ��Ǵ� �Լ��̴�.
    {
        RaycastHit2D hit;
        CheckIsGround(LayerMask.GetMask("Ground"), out hit);
        if (hit == true&& inputVector.y < 0 && hit.collider.CompareTag("1-wayFlatform"))
        {
            //Debug.Log("�ϴ�����");
            StartCoroutine(UnderJump(hit.collider));
        }
        else if (jumpCount > 0 && hit == true)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);  //���� �ް� �ִ� ���� ũ��� ������� ������ ������ �ϱ� ���� velocity�� 0���� ���� �� ����
            rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
            jumpCount--;
        }
    }

    void OnLeftClick()  //��Ŭ���� �ϸ� ȣ��Ǵ� �Լ��̴�.
    {
        
    }

    void OnRightClick() //��Ŭ���� �ϸ� ȣ��Ǵ� �Լ��̴�.
    {
        if(!isDash && dashCount > 0 && continuousDashDelay <= 0)
        {
            StartCoroutine(Dash(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue())));    // Dash �ڷ�ƾ�� ȣ��
        }
    }

    void OnChangeTime() //�ð��� ���� Ű�� ������ �۵��ϴ� �Լ��̴�.
    {
        if(nowTime == PlayerTime.CURRENT)
        {
            StartCoroutine(GameManager.instance.ChangeTime(0));
            nowTime = PlayerTime.PAST;
        }
        else if(nowTime == PlayerTime.PAST)
        {
            StartCoroutine(GameManager.instance.ChangeTime(1));
            nowTime = PlayerTime.CURRENT;
        }
        
    }
    
    void Move()
    {
        if (isDash) return; //���� �뽬���ΰ�쿡�� �� �Լ��� �ߴܽ�Ų��.
        //���� �¿� ����Ű���� ���� ���ٸ� �ӵ��� �޼ӵ��� ���ҽ�Ų��.
        if(Mathf.Abs(inputVector.x) < 1)
        {
            //Debug.Log("stop");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * brakingPower, rigidbody2D.velocity.y);
            anim.SetBool("isWalk", false);
            return;
        }
        //����Ű�� ��ȯ�� �� ������ ��ȯ�� �� �ֵ��� ���� �ʱ�ȭ�Ѵ�.
        if(Mathf.Sign(inputVector.x) != Mathf.Sign(rigidbody2D.velocity.x))
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }
        anim.SetBool("isWalk", true);
        rigidbody2D.AddForce(Vector2.right * inputVector.x * speed, ForceMode2D.Force); //�¿�� �����δ�.
        //���� �ʹ� �޾� maxSpeed �̻��� �Ѿ ��� ���� ������Ų��.
        if(rigidbody2D.velocity.x > maxSpeed)
        {
            rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
        }
        else if (rigidbody2D.velocity.x < maxSpeed * -1)
        {
            rigidbody2D.velocity = new Vector2(maxSpeed * -1, rigidbody2D.velocity.y);
        }
        if(inputVector.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if(inputVector.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    bool CheckIsGround(LayerMask layerMask, out RaycastHit2D hit) //layerMask ��ũ�� ���� ������ üũ�ϴ� �Լ�
    {
        float rayDistance = 0.2f;
        hit = Physics2D.BoxCast(transform.position + Vector3.down * transform.localScale.y / 4, transform.localScale / 3, 0, Vector2.down, rayDistance, layerMask);
        if (hit == true)
        {
            return true;
        }
        return false;
    }

    void SetCoolTime()
    {
        //�뽬�� ���������� �뽬 ��Ÿ���� �ʱ�ȭ�Ѵ�.
        if (dashCount == maxDashCount && dashCoolTime != dashCoolTimeSetting)
        {
            dashCoolTime = dashCoolTimeSetting;
        }
        //�뽬�� �������� ������ �뽬 ��Ÿ���� �����Ѵ�.
        if (dashCount < maxDashCount && dashCoolTime > 0)
        {
            dashCoolTime -= Time.deltaTime;
            if(dashCoolTime < 0)
            {
                dashCount++;
                dashCoolTime = dashCoolTimeSetting;
            }
        }
        //�뽬 ���� ������ ��Ÿ���� ���� �Ѵ�.
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
    //�뽬�� �����ϴ� �Լ�
    IEnumerator Dash(Vector3 targetPos)
    {
        Vector2 dashDirVec = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y).normalized;    //�뽬���⺤��
        float originGravity = rigidbody2D.gravityScale;
        //�뽬������ isDash�� ���� �ٸ� �Լ����� �˸� ��, �߷� �� ������ 0���� ����� �뽬 �������� ū ���� ���Ѵ�.
        isDash = true;
        dashCount--;
        if (dashDirVec.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if(dashDirVec.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(dashDirVec * dashPower, ForceMode2D.Force);
        //�뽬 �ð���ŭ ��ٸ� �� ������ 0���� ����� ������ 0���� ������� �߷��� ������� ������. �� �� �뽬 ��Ÿ���� �����ϰ� isDash�� false �����ν� �뽬�� �����Ѵ�. 
        yield return new WaitForSeconds(dashTime);
        continuousDashDelay = continuousDashDelaySetting;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.gravityScale = originGravity;
        isDash = false;
    }

    IEnumerator UnderJump(Collider2D underCollider)
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), underCollider, true);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), underCollider, false);
    }


}


