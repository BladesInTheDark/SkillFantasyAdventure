using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

// 게임 종료창
public class CloseCheckWindow : MonoBehaviour
{
    [SerializeField] Animator ownAnim;
    public void CloseGameApp()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OpenWindow()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        ownAnim.SetTrigger("Open");
    }
    public void CloseWindow()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        ownAnim.SetTrigger("Close");
    }
}
