using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private bool showWindow;

    private Rect m_WinRect = new Rect(20, 20, 500, 500);

    private const int m_CrystalTypesNum = 4;

    private bool showCrystalState;
    private bool showCheatsState;

    private string[] m_CrystalTags;
    private string[] m_AiTags;

    private bool godMode;
    private bool maxDamage;
    private bool manuelControl;


    private static CheatManager m_UniqueInstance;

    public static CheatManager Instance
    {
        get
        {
            if (m_UniqueInstance == null)
            {
                GameObject go = new GameObject("CheatManager");
                m_UniqueInstance = go.AddComponent<CheatManager>();
            }

            return m_UniqueInstance;
        }
    }

    private void Awake()
    {
        if (m_UniqueInstance != null && m_UniqueInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_UniqueInstance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        godMode = false;
        maxDamage = false;
        manuelControl = false;
        showWindow = false;
        showCrystalState = false;
        showCheatsState = false;
        m_AiTags = new string[m_CrystalTypesNum] { "Green_Ai", "Red_Ai", "Yellow_Ai", "Blue_Ai" };

        m_CrystalTags = new string[m_CrystalTypesNum]
            { "Green_Crystal_Obj", "Red_Crystal_Obj", "Yellow_Crystal_Obj", "Blue_Crystal_Obj" };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3)) ToggleShowWindow();
    }

    private void OnGUI()
    {
        if (showWindow) m_WinRect = GUI.Window(0, m_WinRect, WindowFunction, "GameInfo");
    }

    void WindowFunction(int windowID)
    {
        string crystalWindowTxt =
            showCrystalState ? "\u2191 Show Crystal Infos \u2191" : "\u2193 Show Crystal Infos \u2193";
        if (GUILayout.Button(crystalWindowTxt))
        {
            ToggleshowCrystalState();
        }

        GUILayout.BeginVertical();
        if (showCrystalState)
        {
            for (int i = 0; i < m_CrystalTypesNum; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.TextArea(
                    $"{m_CrystalTags[i]}: {LevelManager.instance.GetActiveInScene(m_CrystalTags[i]).Count}");

                GUILayout.Space(10);

                GUILayout.TextArea($"{m_AiTags[i]} {LevelManager.instance.GetActiveInScene(m_AiTags[i]).Count}");

                GUILayout.Space(20);

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
        }

        GUILayout.EndVertical();

        // Cheats
        string cheatsWindowTxt =
            showCheatsState ? "\u2191 Show Cheat Options \u2191" : "\u2193 Show Cheat Options \u2193";

        if (GUILayout.Button(cheatsWindowTxt))
        {
            ToggleshowCheatsState();
        }

        if (showCheatsState)
        {
            godMode = GUILayout.Toggle(godMode, "Heal");
            if (godMode)
            {
                LevelManager.instance.playerGodmode = true;
            }
            else
            {
                LevelManager.instance.playerGodmode = false;
            }

            if (GUILayout.Button("Heal"))
            {
                Player.GetComponent<PlayerStateMachine>().Heal(100);
            }

            maxDamage = GUILayout.Toggle(maxDamage, "One Shot Enemies");
            if (maxDamage)
            {
                LevelManager.instance.SetPlayerDamage(100);
            }
            else
            {
                LevelManager.instance.SetPlayerDamage(Player.GetComponent<PlayerStateMachine>().m_MaxDamage);
            }

            if (GUILayout.RepeatButton("Give Crystals"))
            {
                LevelManager.instance.CollectAction?.Invoke(100, "Green");
                LevelManager.instance.CollectAction?.Invoke(100, "Red");
                LevelManager.instance.CollectAction?.Invoke(100, "Yellow");
                LevelManager.instance.CollectAction?.Invoke(100, "Blue");
            }

            string manuelString = manuelControl ? "Manuel Crystals: Y" : "Manuel Crystals: N";
            if (GUILayout.Button(manuelString))
            {
                manuelControl = !manuelControl;
                LevelManager.instance.ToggleManuel();
            }

            if (manuelControl)
            {
                if (GUILayout.Button("Duplicate"))
                {
                    LevelManager.instance.DuplicateCrystals();
                }
            }
        }
    }


    void ToggleShowWindow()
    {
        showWindow = !showWindow;
    }

    void ToggleshowCrystalState()
    {
        showCrystalState = !showCrystalState;
    }

    void ToggleshowCheatsState()
    {
        showCheatsState = !showCheatsState;
    }
}