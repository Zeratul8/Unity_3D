using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPadController : MonoBehaviour
{
    bool m_isShow = true;
    private void OnGUI()
    {
        m_isShow = GUI.Toggle(new Rect(Screen.width - 100, 10, 100, 20), m_isShow, "Show Pad");
        if(m_isShow)
        {
            MovePadController.Instance.ShowUI();
            ActionButtonManager.Instance.ShowUI();
        }
        else
        {
            MovePadController.Instance.HideUI();
            ActionButtonManager.Instance.HideUI();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
}
