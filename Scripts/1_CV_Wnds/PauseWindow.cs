using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일시정지 창에 반응하는 클래스
public class PauseWindow : MonoBehaviour
{
    [SerializeField] Animator ownAnim;
    [SerializeField] OptionWnd optionWnd;
    [SerializeField] CloseCheckWindow closeWnd;
    public void OpenWindow()
    {
        FieldManager.m.PauseGame();
        ownAnim.SetTrigger("Open");
        
    }
    public void CloseWindow()
    {
        ownAnim.SetTrigger("Close");
        FieldManager.m.GameStart();
    }
    public void OpenOptionWindow()
    {
        optionWnd.OpenWindow();
    }
    public void OpenCloseWindow()
    {
        closeWnd.OpenWindow();
    }
    public void GoToLobbyClicked()
    {
        SceneCtrlManager.m.StartLobbyScene();
    }
}
