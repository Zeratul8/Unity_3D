using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameObjectPool<T> where T : class
{
    public delegate T Func();
    Queue<T> m_objPool = new Queue<T>();
    int m_count = 0;
    Func m_createFunc;
    public int Count { get { return m_objPool.Count; } }
    public GameObjectPool() { }
    public GameObjectPool(int count, Func createFunc)
    {
        m_count = count;
        m_createFunc = createFunc;
        Allocate();
    }
    public T New()
    {
        return m_createFunc();
    }
    public void Allocate(int count, Func createFunc)
    {
        m_count = count;
        m_createFunc = createFunc;
        Allocate();
    }
    void Allocate()
    {
        for(int i = 0; i < m_count; i++)
        {
            m_objPool.Enqueue(m_createFunc());
        }
    }
    public T Get()
    {
        if (m_objPool.Count > 0)
            return m_objPool.Dequeue();
        else
        {            
            return m_createFunc();
        }
    }
    public void Set(T obj)
    {
        m_objPool.Enqueue(obj);
    }
}
