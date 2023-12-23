using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //�뽬 UI�� ���õ� ������
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
        UpdateDashUI(); // �뽬 UI ������Ʈ
    }

    void UpdateDashUI() //�뽬 UI�� ������Ʈ �ϴ� �Լ�
    {
        //�ʿ��� ������ null�� ��� ���� �ڵ带 �������� ����, �ڼ��� ���� �޼��� ��¿�
        if(player == null || dashImage == null || dashCountText == null)
        {
            Debug.Log("UIManager_UpdateDashUI_���� �Ҵ��� �ʿ��մϴ�. player, dashImage, dashCountText�� �Ҵ���� ���� ������ �ֽ��ϴ�.");
            return;
        }
        PlayerCtrl playerCtrl = player.GetComponent<PlayerCtrl>();
        // �뽬 ������ ���缭 �뽬 ������ Ȱ��ȭ/��Ȱ��ȭ/�������·� �����Ŵ
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
        //�뽬 ����
        dashCountText.text = playerCtrl.dashCount.ToString();
    }
}
