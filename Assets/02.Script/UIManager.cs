using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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
        UpdateDashUI();
    }

    void UpdateDashUI()
    {
        if(player == null || dashImage == null || dashCountText == null)
        {
            Debug.Log("UIManager_UpdateDashUI_변수 할당이 필요합니다. player, dashImage, dashCountText중 할당되지 않은 변수가 있습니다.");
            return;
        }
        PlayerCtrl playerCtrl = player.GetComponent<PlayerCtrl>();
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
        dashCountText.text = playerCtrl.dashCount.ToString();
    }
}
