using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform m_target;
    [SerializeField]
    [Range(0f, 20f)]
    float m_distance = 1f;
    [SerializeField]
    [Range(0f, 20f)]
    float m_height = 2f;
    [SerializeField]
    [Range(-90f, 90f)]
    float m_angle = 45f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float m_speed = 0.5f;
    Transform m_prevTransform;
    
    // Start is called before the first frame update
    void Start()
    {        
        m_prevTransform = transform;
       // Screen.SetResolution(Mathf.RoundToInt(Screen.width * 0.4f), Mathf.RoundToInt(Screen.height * 0.4f), true);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Lerp(m_prevTransform.position.x, m_target.transform.position.x, m_speed * Time.deltaTime),
                                         Mathf.Lerp(m_prevTransform.position.y, m_target.transform.position.y + m_height, m_speed * Time.deltaTime),
                                         Mathf.Lerp(m_prevTransform.position.z, m_target.transform.position.z - m_distance, m_speed * Time.deltaTime));
        transform.rotation = Quaternion.Euler(Mathf.Lerp(m_prevTransform.eulerAngles.x, m_angle, m_speed * Time.deltaTime), 0f, 0f);
        m_prevTransform = transform;
    }
}
