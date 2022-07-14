using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePadController : SingletonMonoBehaviour<MovePadController>
{
    [SerializeField]
    Camera m_uiCamera;
    [SerializeField]
    GameObject m_padBG;
    [SerializeField]
    GameObject m_padBtn;
    Vector3 m_dir;
    bool m_isDrag;
    float m_maxDist = 0.216f;
    int m_fingerId = -1;
    public Vector2 GetAxis { get { return m_dir; } }
    
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }
    public void HideUI()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {           
            Ray ray = m_uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("UI")))
            {
                if (hit.collider.transform == m_padBG.transform)
                {
                    m_isDrag = true;
                    var dir = hit.point - m_padBG.transform.position;
                   
                    if (Mathf.Approximately(dir.sqrMagnitude, Mathf.Pow(m_maxDist, 2f)) || dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                    {
                        m_padBtn.transform.position = m_padBG.transform.position + dir;                        
                    }
                    else
                    {
                        dir = dir.normalized * m_maxDist;
                        m_padBtn.transform.position = m_padBG.transform.position + dir;
                    }
                    m_dir = dir / m_maxDist;
                }
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            m_isDrag = false;
            m_padBtn.transform.localPosition = Vector3.zero;
            m_dir = Vector3.zero;
        }
        if(m_isDrag)
        {
            var worldPos = m_uiCamera.ScreenToWorldPoint(Input.mousePosition);
            var dir = worldPos - m_padBG.transform.position;
            
            if (Mathf.Approximately(dir.sqrMagnitude, Mathf.Pow(m_maxDist, 2f)) || dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
            {
                m_padBtn.transform.position = m_padBG.transform.position + dir;               
            }
            else
            {
                dir = dir.normalized * m_maxDist;
                m_padBtn.transform.position = m_padBG.transform.position + dir;
            }
            m_dir = dir / m_maxDist;
        }
        if(Input.touchSupported)
        {
            for(int i = 0; i < Input.touchCount; i++)
            {
                if(Input.touches[i].phase == TouchPhase.Began)
                {
                    Ray ray = m_uiCamera.ScreenPointToRay(Input.touches[i].position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("UI")))
                    {
                        if (hit.collider.transform == m_padBG.transform)
                        {
                            m_isDrag = true;
                            var dir = hit.point - m_padBG.transform.position;

                            if (Mathf.Approximately(dir.sqrMagnitude, Mathf.Pow(m_maxDist, 2f)) || dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                            {
                                m_padBtn.transform.position = m_padBG.transform.position + dir;
                            }
                            else
                            {
                                dir = dir.normalized * m_maxDist;
                                m_padBtn.transform.position = m_padBG.transform.position + dir;
                            }
                            m_fingerId = Input.touches[i].fingerId;
                            m_dir = dir / m_maxDist;
                        }
                    }
                }
                if((Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled) && Input.touches[i].fingerId == m_fingerId)
                {
                    m_isDrag = false;
                    m_padBtn.transform.localPosition = Vector3.zero;
                    m_dir = Vector3.zero;
                    m_fingerId = -1;
                }
                if(m_isDrag && Input.touches[i].fingerId == m_fingerId)
                {
                    var worldPos = m_uiCamera.ScreenToWorldPoint(Input.touches[i].position);
                    var dir = worldPos - m_padBG.transform.position;

                    if (Mathf.Approximately(dir.sqrMagnitude, Mathf.Pow(m_maxDist, 2f)) || dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                    {
                        m_padBtn.transform.position = m_padBG.transform.position + dir;
                    }
                    else
                    {
                        dir = dir.normalized * m_maxDist;
                        m_padBtn.transform.position = m_padBG.transform.position + dir;
                    }
                    m_dir = dir / m_maxDist;
                }
            }
        }
    }
}
