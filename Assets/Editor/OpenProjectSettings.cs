using UnityEngine;
using UnityEditor;
using System;

public class OpenProjectSettings : EditorWindow
{
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("Open Render Settings...", EditorStyles.boldLabel);
            {
                if (GUILayout.Button("Render Settings"))
                    OpenRenderSettings();
            }

            EditorGUILayout.LabelField("Open Project Settings...", EditorStyles.boldLabel);
            {
                if (GUILayout.Button("Player"))
                    OpenPlayer();
                if (GUILayout.Button("Quality"))
                    OpenQuality();
                if (GUILayout.Button("Input"))
                    OpenInput();
                if (GUILayout.Button("Tags and Layers"))
                    OpenTagsAndLayers();
                if (GUILayout.Button("Audio"))
                    OpenAudio();
                if (GUILayout.Button("Time"))
                    OpenTime();
                if (GUILayout.Button("Physics"))
                    OpenPhysics();
                if (GUILayout.Button("Physics 2D"))
                    OpenPhysics2D();
                if (GUILayout.Button("Graphics"))
                    OpenGraphics();
                if (GUILayout.Button("Network"))
                    OpenNetwork();
                if (GUILayout.Button("Editor"))
                    OpenEditor();
                if (GUILayout.Button("Script Execution Order"))
                    OpenScriptExecutionOrder();
            }
        }
        EditorGUILayout.EndVertical();
    }

    void OpenRenderSettings()
    {
        EditorApplication.ExecuteMenuItem("Edit/Render Settings");
    }

    void OpenPlayer()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
    }

    void OpenQuality()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Quality");
    }

    void OpenInput()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Input");
    }

    void OpenTagsAndLayers()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Tags and Layers");
    }

    void OpenAudio()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Audio");
    }

    void OpenTime()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Time");
    }

    void OpenPhysics()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics");
    }

    void OpenPhysics2D()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics 2D");
    }

    void OpenGraphics()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
    }

    void OpenNetwork()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Network");
    }

    void OpenEditor()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
    }

    void OpenScriptExecutionOrder()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Script Execution Order");
    }

    [MenuItem("Tools/Open Project Settings...")]
    static void Init()
    {
        OpenProjectSettings window = EditorWindow.GetWindow<OpenProjectSettings>("Open Proj Settings");
        window.ShowPopup();
    }
}