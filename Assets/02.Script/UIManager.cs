using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //대쉬 UI와 관련된 변수들
    public GameObject player;
    public Image[] dashImage;
    public TextMeshProUGUI dashCountText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateDashUI(); // 대쉬 UI 업데이트
    }

    void UpdateDashUI() //대쉬 UI를 업데이트 하는 함수
    {
        //필요한 변수가 null인 경우 다음 코드를 진행하지 않음, 자세한 에러 메세지 출력용
        if(player == null || dashImage == null || dashCountText == null)
        {
            Debug.Log("UIManager_UpdateDashUI_변수 할당이 필요합니다. player, dashImage, dashCountText중 할당되지 않은 변수가 있습니다.");
            return;
        }
        PlayerCtrl playerCtrl = player.GetComponent<PlayerCtrl>();
        // 대쉬 개수에 맞춰서 대쉬 아이콘 활성화/비활성화/충전상태로 변경시킴
        for(int i=0; i<dashImage.Length; i++)
        {
            if(playerCtrl.dashCount > i)
            {
                dashImage[i].fillAmount = 1;
            }
            else if(playerCtrl.dashCount == i)
            {
                dashImage[i].fillAmount = (playerCtrl.dashCoolTimeSetting - playerCtrl.dashCoolTime) / playerCtrl.dashCoolTimeSetting;
            }
            else if(playerCtrl.dashCount < i)
            {
                dashImage[i].fillAmount = 0;
            }
        }
        //대쉬 개수
        dashCountText.text = playerCtrl.dashCount.ToString();
    }
}
