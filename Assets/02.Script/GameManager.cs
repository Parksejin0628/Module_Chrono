using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //���� �ý��� ���� �⺻ ����
    static public GameManager instance;
    public GameObject player;
    public GameObject camera;
    public CinemachineVirtualCamera virtualCamera;
    //�ð� ����� ���õ� ����
    public Transform[] timePoint;
    private Transform nowPoint;
    //�뽬 ���ʽ��� ���õ� ����
    public float dashBonusReActiveTime = 5.0f;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        nowPoint = timePoint[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ChangeTime(int timeTarget)
    {
        Transform playerTr = player.GetComponent<Transform>();
        Transform cameraTr = camera.GetComponent<Transform>();

        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;
        playerTr.position = playerTr.position - nowPoint.position + timePoint[timeTarget].position;
        cameraTr.position = cameraTr.position - nowPoint.position + timePoint[timeTarget].position;
        nowPoint = timePoint[timeTarget];
        yield return new WaitForFixedUpdate();
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 1;
    }

    public IEnumerator InteractDashBonus(GameObject dashBonusObject)
    {
        dashBonusObject.SetActive(false);
        yield return new WaitForSeconds(dashBonusReActiveTime);
        dashBonusObject.SetActive(true);
    }
}
