using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestory : MonoBehaviour
{
    [SerializeField]
    float m_lifeTime = 0f;
    float m_time;
    ParticleSystem[] m_particles;
    void RemoveParticle()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        m_time = Time.time;    
    }
    // Start is called before the first frame update
    void Start()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_lifeTime > 0.0f)
        {
            if(Time.time > m_time + m_lifeTime)
            {
                RemoveParticle();
            }
        }
        else
        {
            bool isPlaying = false;
            for (int i = 0; i < m_particles.Length; i++)
            {
                if (m_particles[i].isPlaying)
                {
                    isPlaying = true;
                    break;
                }
            }
            if (!isPlaying)
            {
                RemoveParticle();
            }
        }
    }
}
