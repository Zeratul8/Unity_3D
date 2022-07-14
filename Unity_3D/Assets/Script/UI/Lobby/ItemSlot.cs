using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    [SerializeField]
    UILabel m_nameLabel;
    [SerializeField]
    UILabel m_tierLabel;
    [SerializeField]
    UI2DSprite m_icon;
    [SerializeField]
    UISprite m_slotBG;
    [SerializeField]
    GameObject[] m_bottomMenuObj;
    Inventory m_inven;
    ItemData m_data;
    static string[] m_tierList = { "B", "B+", "A", "S", "SS" };
    static Color[] m_gradeColorList = {Color.gray, Color.green, Color.yellow, Color.red, new Color32(200, 51, 197, 255) };
    public ItemData ItemData { get { return m_data; } }
    public void ResetBottomMenu()
    {
        m_bottomMenuObj[0].gameObject.SetActive(false);
        m_bottomMenuObj[1].gameObject.SetActive(false);
        m_bottomMenuObj[2].gameObject.SetActive(false);
        if (PlayerDataManager.Instance.IsOwnedItem(m_data.id))
        {
            if (m_data.type == ItemType.Weapon)
            {
                if (PlayerDataManager.Instance.WeaponID == m_data.id)
                {
                    m_bottomMenuObj[2].gameObject.SetActive(true);
                }
                else
                {
                    m_bottomMenuObj[1].gameObject.SetActive(true);
                }
            }
            else if (m_data.type == ItemType.Shield)
            {
                if (PlayerDataManager.Instance.ShieldID == m_data.id)
                {
                    m_bottomMenuObj[2].gameObject.SetActive(true);
                }
                else
                {
                    m_bottomMenuObj[1].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            m_bottomMenuObj[0].gameObject.SetActive(true);
        }
    }
    public void SetSlot(Inventory inven, ItemData data, Sprite icon)
    {
        m_inven = inven;
        m_data = data;
        m_nameLabel.text = data.name;
        m_nameLabel.color = m_gradeColorList[data.tier - 1]; 
        m_tierLabel.text = m_tierList[data.tier - 1];
        //m_slotBG.color = m_gradeColorList[data.tier - 1];
        m_icon.sprite2D = icon;
        m_icon.MakePixelPerfect();
        m_icon.width = (int)(m_icon.width * 0.95f);
        m_icon.height = (int)(m_icon.height * 0.95f);
        ResetBottomMenu();
    }
    public void OnEquipItem()
    {
        m_inven.OnEquipItem(this);
    }
    public void EquipItem()
    {
        if (m_data.type == ItemType.Weapon)
            PlayerDataManager.Instance.WeaponID = m_data.id;
        else if (m_data.type == ItemType.Shield)
            PlayerDataManager.Instance.ShieldID = m_data.id;
        PlayerDataManager.Instance.Save();
        ResetBottomMenu();
    }
    public void OnBuyItem()
    {
        PopupManager.Instance.Open_PopupOkCancel("[FFFF00]아이템 구입[-]", string.Format("[000000][00FF00]{0}[-][FFFF00]골드[-]를 소모하여\r\n아이템을 구매하시겠습니까?[-]", m_data.price), ()=> 
        {
            if(PlayerDataManager.Instance.DecreaseGold((uint)m_data.price))
            {
                PlayerDataManager.Instance.AddItem(m_data);
                PlayerDataManager.Instance.Save();                
                PopupManager.Instance.ClosePopup();
                ResetBottomMenu();
            }
            else
            {
                PopupManager.Instance.Open_PopupOk("[FFFF00]NOTICE[-]", "[000000]소지금액이 부족하여\r\n아이템을 구매할 수 없습니다.[-]", null, "확인");
            }
        }, null, "구입", "취소");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
