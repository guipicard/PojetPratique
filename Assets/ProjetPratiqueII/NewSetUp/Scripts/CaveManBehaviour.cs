using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CaveManBehaviour : MonoBehaviour
{
    private int m_Step;
    [SerializeField] private List<Narratives> m_Dialogues;
    [SerializeField] private List<Transform> m_Positions;
    [SerializeField] private Transform m_TutoCrystals;
    private static readonly int move = Animator.StringToHash("Move");
    private bool m_FightReady;

    void Start()
    {
        m_Step = 0;
        LevelManager.instance.EndDialogue += ExitDialogue;
        LevelManager.instance.TriggerDialogue += TriggetDialogue;
        m_FightReady = false;
    }

    void Update()
    {
        Debug.Log(m_Step);
        if (m_Step == 1)
        {
            if (GetComponent<NavMeshAgent>().velocity == Vector3.zero &&
                Vector3.Distance(transform.position, m_Positions[1].position) < 1.5f)
            {
                Vector3 euler = transform.eulerAngles;
                euler.y = 230f;
                transform.eulerAngles = euler;
                GetComponent<Animator>().SetBool(move, false);
            }
            else
            {
                GetComponent<Animator>().SetBool(move, true);
            }

            if (m_TutoCrystals.childCount == 0)
            {
                m_Step++;
                EnterDialogue();
            }
        }
        else if (m_Step == 5 || m_Step == 7)
        {
            if (GetComponent<NavMeshAgent>().velocity.x < 0.3f &&
                Vector3.Distance(transform.position, m_Positions[m_Step - 1].position) < 3.5f && !m_FightReady)
            {
                Vector3 euler = transform.eulerAngles;
                euler.y = 230f;
                transform.eulerAngles = euler;
                GetComponent<Animator>().SetBool(move, false);
                m_FightReady = true;
                EnterDialogue();
            }
            else if (!m_FightReady)
            {
                GetComponent<Animator>().SetBool(move, true);
            }
        }
        else if (m_Step == 2 || m_Step == 4 || m_Step == 6)
        {
            Debug.Log(m_FightReady);
            Debug.Log("un");
            Debug.Log(GetComponent<NavMeshAgent>().velocity.x);
            Debug.Log(Vector3.Distance(transform.position, m_Positions[m_Step].position));
            if (GetComponent<NavMeshAgent>().velocity.x < 1f &&
                Vector3.Distance(transform.position, m_Positions[m_Step - 1].position) < 3.5f && !m_FightReady)
            {
                Debug.Log("deux");
                Vector3 euler = transform.eulerAngles;
                euler.y = 230f;
                transform.eulerAngles = euler;
                GetComponent<Animator>().SetBool(move, false);
                if (Vector3.Distance(transform.position, LevelManager.instance.m_Player.transform.position) < 5f)
                {
                    Debug.Log("trois");
                    m_FightReady = true;
                    m_Step++;
                    EnterDialogue();
                }
            }
            else if (!m_FightReady)
            {
                GetComponent<Animator>().SetBool(move, true);
            }
        }
        // else if (m_Step == 4)
        // {
        //     if (GetComponent<NavMeshAgent>().velocity.x < 0.1f  && Vector3.Distance(transform.position, m_Positions[3].position) < 1.5f)
        //     {
        //         Vector3 euler = transform.eulerAngles;
        //         euler.y = 230f;
        //         transform.eulerAngles = euler;
        //         GetComponent<Animator>().SetBool(move, false);
        //     }
        //     else
        //     {
        //         GetComponent<Animator>().SetBool(move, true);
        //     }
        //     
        //     if (Vector3.Distance(transform.position, LevelManager.instance.m_Player.transform.position) < 5f && !m_FightReady)
        //     {
        //         m_FightReady = true;
        //         EnterDialogue();
        //     }
        // }
    }

    public void TriggetDialogue()
    {
        m_Step++;
        EnterDialogue();
    }

    public void EnterDialogue()
    {
        if (m_Step == 0)
        {
        }
        else if (m_Step == 1)
        {
        }
        else if (m_Step == 2)
        {
        }
        else if (m_Step == 3)
        {
        }
        else if (m_Step == 4)
        {
            GetComponent<NavMeshAgent>().destination = m_Positions[3].position;
            GetComponent<Animator>().SetBool(move, true);
        }
        else if (m_Step == 5)
        {
        }
        else if (m_Step == 6)
        {
        }
        else if (m_Step == 7)
        {
        }
        else if (m_Step == 8)
        {
        }

        LevelManager.instance.ShowDialogue?.Invoke();
        LevelManager.instance.StartDialogue?.Invoke(m_Dialogues[m_Step]);
    }

    public void ExitDialogue()
    {
        Camera mainCam = Camera.main;
        CameraFollow followCam = mainCam.GetComponent<CameraFollow>();
        followCam.ResetFocus();

        if (m_Step == 0)
        {
            m_Step++;
            GetComponent<NavMeshAgent>().destination = m_Positions[1].position;
            GetComponent<Animator>().SetBool(move, true);
        }
        else if (m_Step == 1)
        {
        }
        else if (m_Step == 2)
        {
            GetComponent<NavMeshAgent>().destination = m_Positions[2].position;
            LevelManager.instance.LevelUp();
            GetComponent<Animator>().SetBool(move, true);
            m_FightReady = false;
        }
        else if (m_Step == 3)
        {
        }
        else if (m_Step == 4)
        {
            GetComponent<NavMeshAgent>().destination = m_Positions[4].position;
            GetComponent<Animator>().SetBool(move, true);
        }
        else if (m_Step == 5)
        {
        }
        else if (m_Step == 6)
        {
            GetComponent<NavMeshAgent>().destination = m_Positions[5].position;
            GetComponent<Animator>().SetBool(move, true);
        }
        else if (m_Step == 8)
        {
            GetComponent<NavMeshAgent>().destination = m_Positions[6].position;
            GetComponent<Animator>().SetBool(move, true);
        }

        m_FightReady = false;
    }
}