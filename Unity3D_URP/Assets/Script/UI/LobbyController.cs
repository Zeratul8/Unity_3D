using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField]
    Camera m_uiCamera;
    [SerializeField]
    GameObject m_playerObj;    
    bool isSelect;
    Vector3 m_startPos;
    bool IsSelectCharacter()
    {
        Ray ray = m_uiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("UI_Character") | 1 << LayerMask.NameToLayer("UI")))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
                return false;
            return true;
        }
        return false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        EquipmentManager.Instance.EquipItem(m_playerObj, PlayerDataManager.Instance.WeaponID, PlayerDataManager.Instance.ShieldID);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isSelect = IsSelectCharacter();
            m_startPos = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            isSelect = false;
        }
        if(Input.GetMouseButton(0))
        {
            if (!isSelect) return;

            var endPos = Input.mousePosition;
            var dir = endPos - m_startPos;
            m_startPos = endPos;
            dir.y = 0f;
            m_playerObj.transform.Rotate(Vector3.up, dir.x * -1f, Space.Self);
        }
    }
}
