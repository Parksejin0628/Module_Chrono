using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;


public class PlayerCtrl : MonoBehaviour
{
    //������Ʈ�� ���� ����
    Transform transform;
    Rigidbody2D rigidbody2D;
    PlayerInput playerInput;
    Camera camera;

    public float speed = 2.0f;                          //�¿� �������� ���� ũ��
    public float maxSpeed = 10.0f;                      //�¿� �������� �ְ� �ӵ�
    public float jumpPower = 100.0f;                    //������ �� �� ���� ���ϴ� ���� ũ��
    public float dashPower = 1000.0f;                   //�뽬�� �� �� �������� ���� ũ��
    public float dashTime = 0.1f;                       //�뽬�� �ϴ� �ð��� �����ϴ� ����
    public float dashCoolTimeSetting = 0.5f;                    //�뽬 ��Ÿ���� �����ϴ� ����
    private float dashCoolTime = 0f;                  //���� �뽬 ��Ÿ��
    public float brakingPower = 0.75f;                  //Ű���忡�� Ű���带 �� ��� ���ӵǴ� ����, ���� 0 ~ 1�̴�.
    private Vector2 inputVector;                        //�����¿� ��ư�� ���� ���� ���ͷ� ǥ���� ����, ������ ����Ű�� ������ (1, 0)���� ������.
    private bool isDash = false;                        //�뽬 ������ �ƴ��� Ȯ�ν�Ű�� �Լ�


    // Start is called before the first frame update
    void Awake()
    {
        //������Ʈ �ʱ�ȭ
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

    void OnMove(InputValue value)   //����Ű�� �����ų� �� �� ȣ��Ǵ� �Լ��̴�.
    {
        inputVector = value.Get<Vector2>();     //���� ����Ű�� ������ ���� value�� Vector2�� ġȯ�Ѵ�. InputValue�� value�� ����ϱ� ���ϰ� Vector2�� ġȯ�� ���̴�.

        //������ addForce�� ó���ϸ鼭 maxSpeed�� ���صд��� ������ư�� ���� ���̿� ���� ���� ���̸� �ٲ�� �ϸ� ���?
    }

    void OnJump()   //�����̽��ٸ� ������ ȣ��Ǵ� �Լ��̴�.
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);  //���� �ް� �ִ� ���� ũ��� ������� ������ ������ �ϱ� ���� velocity�� 0���� ���� �� ����
        rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
    }

    void OnLeftClick()  //��Ŭ���� �ϸ� ȣ��Ǵ� �Լ��̴�.
    {
        /*
        Debug.Log("Left Click");
        Debug.Log(Mouse.current.position.ReadValue());
        Debug.Log(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        */
    }

    void OnRightClick() //��Ŭ���� �ϸ� ȣ��Ǵ� �Լ��̴�.
    {
        if(!isDash && dashCoolTime <= 0)
        {
            StartCoroutine(Dash(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue())));    // Dash �ڷ�ƾ�� ȣ��
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
            return;
        }
        //����Ű�� ��ȯ�� �� ������ ��ȯ�� �� �ֵ��� ���� �ʱ�ȭ�Ѵ�.
        if(Mathf.Sign(inputVector.x) != Mathf.Sign(rigidbody2D.velocity.x))
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }
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
        
    }
    //�뽬�� �����ϴ� �Լ�
    IEnumerator Dash(Vector3 targetPos)
    {
        Vector2 dashDirVec = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y).normalized;    //�뽬���⺤��
        //�뽬������ isDash�� ���� �ٸ� �Լ����� �˸� ��, �߷� �� ������ 0���� ����� �뽬 �������� ū ���� ���Ѵ�.
        isDash = true;  
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(dashDirVec * dashPower, ForceMode2D.Force);
        //�뽬 �ð���ŭ ��ٸ� �� ������ 0���� ����� ������ 0���� ������� �߷��� ������� ������. �� �� �뽬 ��Ÿ���� �����ϰ� isDash�� false �����ν� �뽬�� �����Ѵ�. 
        yield return new WaitForSeconds(dashTime);
        dashCoolTime = dashCoolTimeSetting;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.gravityScale = 1f;
        isDash = false;
    }
}


