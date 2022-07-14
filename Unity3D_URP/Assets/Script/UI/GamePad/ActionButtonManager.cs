using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonManager : SingletonMonoBehaviour<ActionButtonManager>
{
    public enum ButtonType
    {
        Main,
        Skill1, 
        Skill2,
        Skill3,
        Max
    }
    [SerializeField]
    ActionButton[] m_buttons;
    public void SetButton(ButtonType type, float coolTime, ButtonDelegate pressDel = null, ButtonDelegate releaseDel = null)
    {
        m_buttons[(int)type].SetButton(coolTime, pressDel, releaseDel);
    }
    public void OnPressButton(ButtonType type)
    {
        m_buttons[(int)type].OnPressButton();
    }
    public void OnReleaseButton(ButtonType type)
    {
        m_buttons[(int)type].OnReleaseButton();
    }
    public void ShowUI()
    {
        for (int i = 0; i < m_buttons.Length; i++)
        {
            m_buttons[i].Show();
        }
    }
    public void HideUI()
    {
        for(int i = 0; i < m_buttons.Length; i++)
        {
            m_buttons[i].Hide();
        }
    }
    // Start is called before the first frame update
    protected override void OnStart()
    {
        m_buttons = GetComponentsInChildren<ActionButton>();
    }
    
}
