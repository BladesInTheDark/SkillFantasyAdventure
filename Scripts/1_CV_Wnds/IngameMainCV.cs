using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Def;

// 게임중에 조작하는 UI들에 반응하기 위해 만들어진 클래스
public class IngameMainCV : MonoBehaviour
{
    #region [인스턴스화]
    static IngameMainCV _uniqueInstance;
    public static IngameMainCV m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI hpText;
    [SerializeField] Image playerIcon;
    [SerializeField] TextMeshProUGUI mvpText;
    [SerializeField] TextMeshProUGUI mvpMaxText;
    [SerializeField] Image mvpMask;
    [SerializeField] public Slider hpBar;
    [SerializeField] Animator resultWndAnim;
    [SerializeField] SkillRewardIcon rewardSkill;
    [SerializeField] TextMeshProUGUI resultTitle;
    [SerializeField] GameObject rewardArea;
    [SerializeField] GameObject deadText;
    [SerializeField] public Button attackButton;
    [SerializeField] Animator playerInfoAnim;
    bool nowInfoFalse;

    public PassiveSkillButton[] pSkill = new PassiveSkillButton[3];
    public ActiveSkillButton[] aSkill = new ActiveSkillButton[3];
    public bool nowProcessing = false;

    private void Awake()
    {
        _uniqueInstance = this;
        stageText.text = string.Format("스테이지 {0} - {1}", FieldManager.m.curStageCode / 100, FieldManager.m.curStageCode % 100);
        nameText.text = UserInfo.m.userName;
        hpText.text = string.Format("{0} / {0}", UserInfo.m.hp);
        hpBar.value = 1;
        mvpMaxText.text = UserInfo.m.mvp_max.ToString();
    }
    public void PlayerMoveUp()
    {
        if (nowProcessing) return;
        nowProcessing = true;
        PlayerCtrlManager.m.PlayerMove(eMoveDir.Up);
    }
    public void PlayerMoveDown()
    {
        if (nowProcessing) return;
        nowProcessing = true;
        PlayerCtrlManager.m.PlayerMove(eMoveDir.Down);
    }
    public void PlayerMoveLeft()
    {
        if (nowProcessing) return;
        nowProcessing = true;
        PlayerCtrlManager.m.PlayerMove(eMoveDir.Left);
    }
    public void PlayerMoveRight()
    {
        if (nowProcessing) return;
        nowProcessing = true;
        PlayerCtrlManager.m.PlayerMove(eMoveDir.Right);
    }
    public void PlayerNormalAttack()
    {
        attackButton.interactable = false;
        PlayerCtrlManager.m.curPlayer.AttackAnimPlay();
    }
    public void FieldStart()
    {
        FieldManager.m.GameStart();
    }
    public void NextStageCall()
    {
        anim.SetTrigger("FadeIn");
        StartCoroutine(NextStageLoad());
        for (int i = 0; i < 3; i++)
            aSkill[i].CoolTimeReset();
        PlayerCtrlManager.m.FieldReset();
        MvpTextSet(PlayerCtrlManager.m.curPlayer.mvp);
    }
    public void GameEndResultSet(UserSkill us)
    {
        if (us != null)
        {
            resultTitle.text = "스테이지 보상";
            rewardArea.SetActive(true);
            deadText.SetActive(false);
            rewardSkill.SetSkill(us);
        }
        else
        {
            resultTitle.text = "스테이지 결과";
            rewardArea.SetActive(false);
            deadText.SetActive(true);
        }
        resultWndAnim.SetTrigger("Open");
        
    }
    public void ReturnToLobby()
    {
        // 확인버튼 누르면 콜됨
        SceneCtrlManager.m.StartLobbyScene();
    }
    IEnumerator NextStageLoad()
    {
        yield return new WaitForSeconds(1f); // 애니메이션 재생시간동안 잠깐 막았다가 로딩 시작함
        FieldManager.m.FieldCodePlus();
        stageText.text = string.Format("스테이지 {0} - {1}", FieldManager.m.curStageCode / 100, FieldManager.m.curStageCode % 100);
        FieldManager.m.InitField();
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        FieldStart();
    }
    public void MvpTextSet(int mvp)
    {
        mvpText.text = mvp.ToString();
    }
    public void MvpMaxTextSet(int mvpmax)
    {
        mvpMaxText.text = mvpmax.ToString();
    }
    public void MvpMaskSet(float p)
    {
        mvpMask.fillAmount = 1 - p;
    }

    public void UISetCall()
    {
        for (int i = 0; i < 3; i++)
        {
            pSkill[i].UISetCall();
            aSkill[i].UISetCall();
        }
    }
}
