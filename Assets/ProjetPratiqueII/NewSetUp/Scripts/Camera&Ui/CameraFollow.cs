using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform m_Player;
    private Transform m_Focus;
    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Speed;

    private Vector3 m_Position;

    void Start()
    {
        LevelManager.instance.LoadLevel();
        m_Player = LevelManager.instance.m_Player.transform;
        m_Focus = m_Player;
        m_Position = m_Focus.position + new Vector3(0, 1, 0) - m_Offset;
        transform.position = m_Position;
        transform.LookAt(m_Focus.position + new Vector3(0, 1, 0));
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        // if (LevelManager.instance.takeInput)
        // {
            m_Position = m_Focus.position + new Vector3(0, 1, 0) - m_Offset;
            m_Position.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, m_Position, Time.deltaTime * m_Speed);
            //m_Position += m_Offset;
            
        // }
    }

    public void SetFocus(string focus)
    {
        m_Focus = GameObject.Find(focus).transform;
    }
    
    public void ResetFocus()
    {
        m_Focus = m_Player;
    }

    public Vector3 GetOffset()
    {
        return m_Offset;
    }
}