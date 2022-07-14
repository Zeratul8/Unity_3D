using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    [SerializeField]
    UISprite m_gaugeAlpha;
    [SerializeField]
    UISprite m_coolGauge;
    UISprite[] m_sprites;
    Collider m_collider;
    bool m_isReady;
    bool m_isPress;
    float m_coolTime;
    float m_time;
    ButtonDelegate m_pressDel;
    ButtonDelegate m_releaseDel;
    public void SetButton(float cooltime, ButtonDelegate pressDel = null, ButtonDelegate releaseDel = null)
    {
        m_coolTime = cooltime;
        m_pressDel = pressDel;
        m_releaseDel = releaseDel;
        if (cooltime == 0)
        {
            m_gaugeAlpha.enabled = false;
            m_coolGauge.enabled = false;
        }
        else
        {
            m_gaugeAlpha.fillAmount = 0f;
            m_coolGauge.fillAmount = 1f;
        }
        m_isReady = true;
    }
    public void OnPressButton()
    {
        if(m_isReady)
        {
            m_isPress = true;
            if(m_pressDel != null)
                m_pressDel();
            if (m_coolTime != 0)
                m_isReady = false;
        }
    }
    public void OnReleaseButton()
    {
        if(m_isPress)
        {
            if(m_releaseDel != null)
                m_releaseDel();
            m_isPress = false;
        }
    }
    public void Show()
    {
        for (int i = 0; i < m_sprites.Length; i++)
            m_sprites[i].enabled = true;
        m_collider.enabled = true;
    }
    public void Hide()
    {
        for (int i = 0; i < m_sprites.Length; i++)
            m_sprites[i].enabled = false;
        m_collider.enabled = false;
    }
    void Start()
    {
        m_sprites = GetComponentsInChildren<UISprite>();
        m_collider = GetComponent<Collider>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!m_isReady)
        {
            m_time += Time.deltaTime;
            var fill = m_time / m_coolTime;
            m_gaugeAlpha.fillAmount = 1f - fill;
            m_coolGauge.fillAmount = fill;
            if(m_time > m_coolTime)
            {
                m_gaugeAlpha.fillAmount = 0f;
                m_coolGauge.fillAmount = 1f;
                m_isReady = true;
                m_time = 0f;
            }
        }
    }
}
