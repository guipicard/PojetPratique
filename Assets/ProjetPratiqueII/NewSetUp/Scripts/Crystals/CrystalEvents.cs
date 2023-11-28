using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalEvents : MonoBehaviour
{
    [SerializeField] private string m_CrystalTag;
    [SerializeField] private string m_PartsTag;
    public CrystalsBehaviour m_Interface;
    private Outline m_OutlineScript;
    public string m_Biome;
    public int m_Id;

    void Start()
    {
        m_Biome = LevelManager.instance.currentWorld;
    }

    private void OnEnable()
    {
        m_OutlineScript = GetComponent<Outline>();
        m_OutlineScript.enabled = false;
    }

    private void OnDisable()
    {
    }

    public void GetMined()
    {
        var position = transform.position;
        LevelManager.instance.SpawnObj(m_PartsTag, position, Quaternion.identity);
        LevelManager.instance.ToggleInactive(gameObject);
        LevelManager.instance.UpdateCrystalNums(m_CrystalTag);
        AudioManager.instance.PlaySound(SoundClip.CrystalExplosion, 1f, position);
        VfxManager.instance.PlayVfx(VfxClip.CrystalMine, position);
        if (m_Id != -1)
        {
            m_Id = -1;
            m_Interface.m_CrystalActive--;
            m_Interface = null;
        }
    }

    public Outline GetOutlineComponent()
    {
        return m_OutlineScript;
    }
}
