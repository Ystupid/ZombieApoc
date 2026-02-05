using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GameTools
{
    [MenuItem("游戏工具/打开目录/打开存档目录")]
    public static void OpenPerFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("游戏工具/打开场景/打开Main场景")]
    public static void OpenMainScene() => OpenScene("Assets/Scenes/MainGame.unity");

    [MenuItem("游戏工具/打开场景/打开UI场景")]
    public static void OpenUIScene() => OpenScene("Assets/Scenes/UIScene.unity");

    private static void OpenScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
