using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using Def;

// 로그인 화면에서 UI들에 반응하기 위해 만들어진 클래스
public class LoginMainCV : MonoBehaviour
{
    [SerializeField] Animator nameAnim;
    [SerializeField] TMP_InputField nameText;
    [SerializeField] OptionWnd optionWnd;

    public void ClickToStart()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        if (UserInfo.m.firstPlay)
            CreateUserName();
        else
            SceneCtrlManager.m.StartLobbyScene();
    }

    void CreateUserName()
    {
        nameAnim.SetTrigger("Open");
    }

    public void UserNameSet()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        if (UserInfo.m.SetCreatedUserName(nameText.text))
        {
            nameAnim.SetTrigger("Close");
            SceneCtrlManager.m.StartLobbyScene();
        }
        else
        {
            // 오류. 닉네임 저장되지 못함. 이유는 모름. 
            nameAnim.SetTrigger("Close");
        }
    }

    public void OptionButtonClicked()
    {
        optionWnd.OpenWindow();
    }
}
