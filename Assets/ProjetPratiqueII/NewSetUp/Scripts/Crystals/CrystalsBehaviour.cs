using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CrystalsBehaviour : MonoBehaviour
{
    [SerializeField][Range(5.0f, 30.0f)] public float m_CrystalSpawnTimer;
    public int m_AiByCrystals;
    [SerializeField] private float m_CrystalDisableTimer;

    [SerializeField] private string m_CrystalName;
    [SerializeField] private string m_CrystalTag;
    [SerializeField] private string m_AiTag;
    [SerializeField] private int m_SpawnChances;
    [SerializeField] private string m_Biome;

    public HashSet<Vector2> m_CrystalsPosition;
    public HashSet<Vector2> m_PotentialPosition;
    public List<Vector2> m_LastCrystalWave;

    private Vector2 m_Pos2D;

    private float m_Elapsed;
    private Ray m_Ray;
    private RaycastHit m_HitInfo;

    public int m_AiActive;
    public int m_CrystalActive;
    private float CrystalSpacing;
    private Vector2[] m_SurroundOffsets;

    private bool m_ManuelControl;
    public int m_Id;

    private bool m_IsDisabled;
    private bool m_IsUnlocked;

    [SerializeField] private GameObject m_Ground;
    [SerializeField] private Material m_MaterialEnabled;
    [SerializeField] private Material m_MaterialDisabled;

    void Start()
    {
        m_IsUnlocked = false;
        m_IsDisabled = false;
        m_ManuelControl = false;
        m_Pos2D = new Vector2(transform.position.x, transform.position.z);
        CrystalSpacing = LevelManager.instance.m_CrystalSpaceBetween;
        m_SurroundOffsets = new Vector2[4]
        {
            new Vector2(CrystalSpacing, CrystalSpacing),
            new Vector2(-CrystalSpacing, CrystalSpacing),
            new Vector2(CrystalSpacing, -CrystalSpacing),
            new Vector2(-CrystalSpacing, -CrystalSpacing)
        };

        m_Ray = new Ray();
        m_Ray.direction = Vector3.down;

        m_AiActive = 0;
        m_CrystalActive = 1;
        m_Elapsed = 0;
        m_AiActive = 0;
        m_CrystalActive = 0;

        m_PotentialPosition = new HashSet<Vector2>();
        m_CrystalsPosition = new HashSet<Vector2>();
        m_LastCrystalWave = new List<Vector2>();
        // Add All Present Crystals Positions in List "CrystalsPosition"
        FillCrystalList();
        foreach (var pos in m_CrystalsPosition)
        {
            m_LastCrystalWave.Add(pos);
        }
    }

    void Update()
    {
        if (m_IsDisabled)
        {
            m_Elapsed += Time.deltaTime;
            if (m_Elapsed > m_CrystalDisableTimer)
            {
                m_IsDisabled = false;
                m_Ground.GetComponent<Renderer>().material = m_MaterialEnabled;
            }
        }
        else
        {
            if (!m_ManuelControl) m_Elapsed += Time.deltaTime;
            if (m_Elapsed > m_CrystalSpawnTimer)
            {
                CrystalDuplicationLoop();
            }
        }
    }

    public void CrystalDuplicationLoop()
    {
        if (m_Biome == LevelManager.instance.currentWorld)
        {
            FillCrystalList();
            GetNewPositions();
            Multiply();
            SpawnAi();


            // Reset Lists
            m_PotentialPosition.Clear();
            m_CrystalsPosition.Clear();
            m_LastCrystalWave.Clear();

            m_Elapsed = 0;
        }
    }

    private void FillCrystalList()
    {
        m_CrystalsPosition.Add(m_Pos2D);

        foreach (GameObject crystal in LevelManager.instance.GetActiveInScene(m_CrystalTag))
        {
            if (crystal.GetComponent<CrystalEvents>().m_Biome != LevelManager.instance.currentWorld || crystal.GetComponent<CrystalEvents>().m_Id != m_Id) continue;
            m_CrystalsPosition.Add(new Vector2(crystal.transform.position.x, crystal.transform.position.z));
        }
    }

    private void GetNewPositions()
    {
        foreach (var pos in m_CrystalsPosition)
        {
            for (int j = m_SurroundOffsets.Length - 1; j >= 0; j--)
            {
                Vector2 m_currentPosition = m_SurroundOffsets[j] + pos;

                m_Ray.origin = new Vector3(m_currentPosition.x, 5.0f, m_currentPosition.y);
                if (Physics.Raycast(m_Ray, out m_HitInfo, Mathf.Infinity, 1 << 8))
                {
                    if (Vector2.Distance(m_currentPosition, m_Pos2D) > 1.0f)
                    {
                        Debug.Log("distance");
                        m_PotentialPosition.Add(m_currentPosition);
                    }
                }
            }
        }
    }

    private void Multiply()
    {
        // Create New Crystals
        HashSet<Vector2> newWave = new HashSet<Vector2>();
        foreach (Vector2 pos in m_PotentialPosition)
        {
            var chances = Random.Range(0, m_SpawnChances);

            m_Ray = new Ray(new Vector3(pos.x, transform.position.y + 5.0f, pos.y), Vector3.down);
            var myRaycast = Physics.Raycast(m_Ray, out m_HitInfo, Mathf.Infinity, 1 << 8 | 1 << 6| 1 << 18);
            if (chances == 0) continue;
            if (myRaycast)
            {
                if (m_HitInfo.collider.gameObject.layer == 10) continue;
                CrystalEvents eventScript = m_HitInfo.collider.GetComponent<CrystalEvents>();
                if (m_HitInfo.collider.gameObject.layer == 6)
                {
                    int hitId = eventScript.m_Id;
                    int hitActiveInSceneCount = LevelManager.instance.GetSpawner(hitId).m_CrystalActive;
                    bool hasMoreCrystals = m_CrystalActive > hitActiveInSceneCount;
                    if (hasMoreCrystals)
                    {
                        eventScript.m_Id = -1;
                        LevelManager.instance.ToggleInactive(m_HitInfo.collider.gameObject);
                        m_HitInfo.collider.gameObject.GetComponent<CrystalsBehaviour>().m_CrystalActive--;
                        newWave.Add(pos);
                        Vector3 newPos = new Vector3(pos.x, m_HitInfo.point.y, pos.y);
                        var obj = LevelManager.instance.SpawnObj(m_CrystalTag, newPos, Quaternion.identity);
                        m_CrystalActive++;
                        obj.GetComponent<CrystalEvents>().m_Interface = this;
                        obj.GetComponent<CrystalEvents>().m_Id = m_Id;
                    }
                }
                else if (m_HitInfo.collider.gameObject.layer == 8)
                {
                    newWave.Add(pos);
                    var newCrystalPosition = new Vector3(pos.x, m_HitInfo.point.y, pos.y);
                    var obj = LevelManager.instance.SpawnObj(m_CrystalTag, newCrystalPosition, Quaternion.identity);
                    m_CrystalActive++;
                    obj.GetComponent<CrystalEvents>().m_Interface = this;
                    obj.GetComponent<CrystalEvents>().m_Id = m_Id;
                }
            }
        }

        if (newWave.Count > 0)
        {
            m_LastCrystalWave = newWave.ToList();
        }
    }

    private void SpawnAi()
    {
        List<Vector2> crystalList = new List<Vector2>();
        if (m_LastCrystalWave.Count == 0)
        {
            crystalList = m_CrystalsPosition.ToList();
        }
        else
        {
            crystalList = m_LastCrystalWave;
        }

        bool aiCap = m_AiActive >= Mathf.Ceil(m_CrystalActive / m_AiByCrystals);
        if (aiCap) return;

        int spawnPointCrystalIndex = Random.Range(0, crystalList.Count == 0 ? 0 : crystalList.Count);
        Vector2 spawnPointCrystal = crystalList[spawnPointCrystalIndex];
        Vector2 spawnPointOffset = m_SurroundOffsets[0] / 2;
        Vector2 spawnPointAi = spawnPointCrystal - spawnPointOffset;
        Vector3 newAiPosition = new Vector3(spawnPointAi.x, transform.position.y, spawnPointAi.y);
        var obj = LevelManager.instance.SpawnObj(m_AiTag, transform.position, Quaternion.identity);
        obj.GetComponent<AIStateMachine>().m_Id = m_Id;
        obj.GetComponent<AIStateMachine>().m_Interface = this;
        obj.GetComponent<NavMeshAgent>().SetDestination(newAiPosition);
        m_AiActive++;
    }

    // private int GetAiCount()
    // {
    //     int result = 0;
    //     foreach (GameObject ai in LevelManager.instance.GetActiveInScene(m_AiTag))
    //     {
    //         if (ai.GetComponent<AIStateMachine>().m_Biome == LevelManager.instance.currentWorld && ai.GetComponent<AIStateMachine>().m_Id == m_Id)
    //         {
    //             result++;
    //         }
    //     }
    //
    //     return result;
    // }

    // private int GetCrystalCount()
    // {
    //     int result = 0;
    //     foreach (GameObject Cry in LevelManager.instance.GetActiveInScene(m_CrystalTag))
    //     {
    //         if (Cry.GetComponent<CrystalEvents>().m_Biome == LevelManager.instance.currentWorld && Cry.GetComponent<CrystalEvents>().m_Id == m_Id)
    //         {
    //             result++;
    //         }
    //     }
    //
    //     return result;
    // }

    public void ToggleManuel()
    {
        m_ManuelControl = !m_ManuelControl;
    }

    public void DisableCamp()
    {
        if (m_AiActive == 0 && m_CrystalActive == 0)
        {
            m_IsDisabled = true;
            m_Elapsed = 0.0f;
            m_Ground.GetComponent<Renderer>().material = m_MaterialDisabled;
            if (!GetIsUnlocked())
            {
                SetUnlocked();
            }
        }
        else
        {
            LevelManager.instance.ErrorAction?.Invoke("You must first clear the camp before disabling it.");
        }
    }

    public bool GetIsUnlocked()
    {
        return m_IsUnlocked;    
    }

    public void SetUnlocked()
    {
        m_IsUnlocked = true;
        LevelManager.instance.UnlockBiome(m_Id);
    }
}