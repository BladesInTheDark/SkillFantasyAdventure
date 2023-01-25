using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Def;

// 로비 관리 화면에서 UI들에 반응하기 위해 만들어진 클래스
public class LobbyMainCV : MonoBehaviour
{
    #region [인스턴스화]
    static LobbyMainCV _uniqueInstance;
    public static LobbyMainCV m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    [SerializeField] TextMeshProUGUI stageRecord;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI diamondText;
    [SerializeField] TextMeshProUGUI stageTicket;
    [SerializeField] TextMeshProUGUI playerInfoText;

    [SerializeField] Animator madadayo;

    [SerializeField] OptionWnd optionWnd;
    [SerializeField] SkillMngWindow skillMngWnd;
    [SerializeField] CloseCheckWindow closeWnd;
    bool isStart = false;

    private void Awake()
    {
        _uniqueInstance = this;
    }

    public void InitSet()
    {
        stageRecord.text = string.Format("Best Record\nStage {0} - {1}", (UserInfo.m.bestStage / 100), UserInfo.m.bestStage % 100);
        goldText.text = string.Format("{0:#,###}", UserInfo.m.gold);
        diamondText.text = string.Format("{0:#,###}", UserInfo.m.diamond);
        stageTicket.text = string.Format("{0} / {1}", UserInfo.m.gameTicket, UserInfo.gameTicketMax);
        playerInfoText.text = string.Format("Lv{0}. {1}", UserInfo.m.userLevel, UserInfo.m.userName);

        if (goldText.text == string.Empty) goldText.text = "0";
        if (diamondText.text == string.Empty) diamondText.text = "0";
        if (UserInfo.m.gameTicket == 0) stageTicket.text = string.Format("0 / {0}", UserInfo.gameTicketMax);
    }

    public void KaraWndOpen()
    {
        // 구현되지 않은 기능을 클릭했을 경우 해당 UI가 나와 아직이라고 알림
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        madadayo.SetTrigger("Open");
    }
    public void KaraWndClose()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        madadayo.SetTrigger("Close");
    }

    public void StageStartClicked()
    {
        if (isStart) return;
        if (UserInfo.m.gameTicket <= 0) return;
        //UserInfo.m.userData.gameTicket--;
        // 이거 게임티켓 없는것도 괜찮긴 한데 지금은 시간에 따라 차오르게 할 방법이 없어서... 일단 보류
        UserInfo.m.userData.tryCnt++;
        isStart = true;
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        SceneCtrlManager.m.StartIngameScene();
    }
    public void OptionButtonClicked()
    {
        optionWnd.OpenWindow();
    }
    public void SkillMngClicked()
    {
        skillMngWnd.OpenWindow();
    }
    public void CloseButtonClicked()
    {
        closeWnd.OpenWindow();
    }
}
