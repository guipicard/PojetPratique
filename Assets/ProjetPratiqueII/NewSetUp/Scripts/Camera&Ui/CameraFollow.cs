using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform m_Player;
    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Speed;

    private Vector3 m_Position;

    void Start()
    {
        LevelManager.instance.LoadLevel();
        m_Player = LevelManager.instance.m_Player.transform;
        m_Position = m_Player.position + new Vector3(0, 1, 0) - m_Offset;
        transform.position = m_Position;
        transform.LookAt(m_Player.position + new Vector3(0, 1, 0));
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (LevelManager.instance.takeInput)
        {
            m_Position = m_Player.position + new Vector3(0, 1, 0) - m_Offset;
            m_Position.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, m_Position, Time.deltaTime * m_Speed);
            //m_Position += m_Offset;
            
        }
    }

    public Vector3 GetOffset()
    {
        return m_Offset;
    }
}