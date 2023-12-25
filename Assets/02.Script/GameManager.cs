using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //게임 시스템 대한 기본 정보
    static public GameManager instance;
    public GameObject player;
    public GameObject camera;
    //시간 변경과 관련된 변수
    public int nowTime = 0;
    private Transform nowPoint;
    //대쉬 보너스와 관련된 변수
    public float dashBonusReActiveTime = 5.0f;
    //세이브 로드와 관련된 변수
    public int stageIndex = 0;
    public StageInformation[] stageInformation;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        nowPoint = stageInformation[0].timePoint[0];
    }

    // Update is called once per frame
    void Update()
    {
        
        if(stageIndex >= 20)
        {
            Debug.Log("Loading");
            SceneManager.LoadScene("Ending");
        }
    }

    public IEnumerator ChangeTime(int timeTarget)
    {
        Transform playerTr = player.GetComponent<Transform>();
        Transform cameraTr = camera.GetComponent<Transform>();
        nowPoint = stageInformation[stageIndex].timePoint[nowTime];
        nowTime = timeTarget;

        playerTr.position = playerTr.position - nowPoint.position + stageInformation[stageIndex].timePoint[timeTarget].position;
        cameraTr.position = cameraTr.position - nowPoint.position + stageInformation[stageIndex].timePoint[timeTarget].position;
        
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator InteractDashBonus(GameObject dashBonusObject)
    {
        dashBonusObject.SetActive(false);
        yield return new WaitForSeconds(dashBonusReActiveTime);
        dashBonusObject.SetActive(true);
    }

    public void MoveNextStage()
    {
        stageIndex++;
        Debug.Log(stageIndex);
        camera.GetComponent<CameraCtrl>().cameraArea = stageInformation[stageIndex].cameraArea;
    }

    public void Revive()
    {
        Transform playerTr = player.GetComponent<Transform>();
        Transform cameraTr = camera.GetComponent<Transform>();
        playerTr.position = stageInformation[stageIndex].startPosition[nowTime].position;
        cameraTr.position = stageInformation[stageIndex].startPosition[nowTime].position + new Vector3(0, 0, -10);
        for (int i=0; i < stageInformation[stageIndex].apples.Length; i++)
        {
            stageInformation[stageIndex].apples[i].SetActive(true);
        }
    }
}

[Serializable]
public class StageInformation
{
    public Transform[] timePoint;
    public Transform[] startPosition;
    public Transform[] cameraArea;
    public GameObject[] apples;
}
