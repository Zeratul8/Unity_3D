using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    UIFollowTarget m_followTarget;
    [SerializeField]
    UILabel m_name;
    [SerializeField]
    HUDText[] m_hudText;
    [SerializeField]
    UIProgressBar m_hpBar;

    public void InitHUD(Transform pool)
    {
        m_followTarget.target = Util.FindChildObject(transform.parent.gameObject, "Dummy_HUD").transform;
        m_followTarget.gameCamera = Camera.main;
        m_followTarget.uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        transform.SetParent(pool);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
    public void SetName(string name)
    {
        m_name.text = name;
    }
    public void SetHUD()
    {
        gameObject.SetActive(false);
        m_hpBar.value = 1f;
    }
    public void ShowDamage(AttackType type, float normalizedHp, int damage)
    {
        if(type != AttackType.Dodge)
        {
            Show();
            if(IsInvoking("Hide"))
            {
                CancelInvoke("Hide");
            }
            Invoke("Hide", 3f);
        }
        if (type == AttackType.Normal)
        {
            m_hudText[0].Add(-damage, Color.white, 0f);
        }
        else if (type == AttackType.Critical)
        {
            m_hudText[1].Add(damage.ToString(), Color.red, 1f);
        }
        else if(type == AttackType.Dodge)
        {
            m_hudText[0].Add("Miss", Color.cyan, 0.2f);
        }
        m_hpBar.value = normalizedHp;
    }
    void Show()
    {
        gameObject.SetActive(true);
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
}
