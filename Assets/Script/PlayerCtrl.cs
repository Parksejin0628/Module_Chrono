using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerCtrl : MonoBehaviour
{
    //������Ʈ�� ���� ����
    Transform transform;
    Rigidbody2D rigidbody2D;
    PlayerInput playerInput;

    public float speed = 2.0f;                          //�¿� �������� ���� ũ��
    public float maxSpeed = 10.0f;                      //�¿� �������� �ְ� �ӵ�
    public float jumpPower = 100.0f;                    //������ �� �� ���� ���ϴ� ���� ũ��
    private Vector2 inputVector;                        //�����¿� ��ư�� ���� ���� ���ͷ� ǥ���� ����, ������ ����Ű�� ������ (1, 0)���� ������.


    // Start is called before the first frame update
    void Awake()
    {
        //������Ʈ �ʱ�ȭ
        transform = GetComponent<Transform>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
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

    void OnJump()
    {
        rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
    }

    void Move()
    {
        //���� �¿� ����Ű���� ���� ���ٸ� �ӵ��� �޼ӵ��� ���ҽ�Ų��.
        if(Mathf.Abs(inputVector.x) < 1)
        {
            Debug.Log("stop");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * 0.75f, rigidbody2D.velocity.y);
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


}


