using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrationDialogue : MonoBehaviour
{
    private Narratives m_Dialogue;
    [SerializeField] private TextMeshProUGUI m_Text;
    private Coroutine m_TextRoutine;
    private int m_TextIndex;
    private int m_CharIndex;
    private float m_Elapsed;
    [SerializeField] private float m_LetterTimer;

    void Start()
    {
        LevelManager.instance.StartDialogue += StartDialogue;
    }

    private void OnEnable()
    {
        m_Elapsed = 0f;
    }
    void Update()
    {
        if (m_TextRoutine == null) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (m_CharIndex == m_Dialogue.text[m_TextIndex].Length)
            {
                if (m_TextIndex + 1 == m_Dialogue.text.Count)
                {
                    LevelManager.instance.EndDialogue?.Invoke();
                    LevelManager.instance.HideDialogue?.Invoke();
                }
                else
                {
                    m_TextIndex++;
                    m_CharIndex = 0;
                    m_TextRoutine = StartCoroutine(ShowText());
                }
            }
            else
            {
                m_CharIndex = m_Dialogue.text[m_TextIndex].Length;
            }
        }
    }

    private IEnumerator ShowText()
    {
        while (true)
        {
            if (m_CharIndex == m_Dialogue.text[m_TextIndex].Length)
            {
                m_Text.text = m_Dialogue.text[m_TextIndex];
                break;
            }
            m_Elapsed += Time.deltaTime;
            if (m_Elapsed > m_LetterTimer)
            {
                m_Elapsed = 0f;
                m_CharIndex++;
            }

            m_Text.text = m_Dialogue.text[m_TextIndex].Substring(0, m_CharIndex);

            yield return null;
        }
    }

    private void StartDialogue(Narratives narrative)
    {
        m_TextIndex = 0;
        m_CharIndex = 0;
        m_Dialogue = narrative;
        Camera mainCam = Camera.main;
        CameraFollow followCam = mainCam.GetComponent<CameraFollow>();
        followCam.SetFocus(narrative.focus);
        m_TextRoutine = StartCoroutine(ShowText());
    }
}