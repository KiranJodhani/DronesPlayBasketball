// Unity Ref - https://github.com/Unity-Technologies/UnityCsReference/tree/master/Editor/Mono/GameView
// Resolution Ref - https://answers.unity.com/questions/956123/add-and-select-game-view-resolution.html
// Scale Ref - https://answers.unity.com/questions/1582635/game-view-scale-on-compilationplay.html

using UnityEditor;
using System.Reflection;
using UnityEngine;
using System;

public class GameViewUtils
{
    static object gameViewSizesInstance;
    static MethodInfo getGroup;
    private static int screenIndex;

    private enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }

    static GameViewUtils()
    {
        var gameViewSizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
        var instanceProperty = singleType.GetProperty("instance");
        getGroup = gameViewSizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProperty.GetValue(null, null);
    }



    /// <summary>
    /// Returns Game View Window Type
    /// </summary>
    /// <returns></returns>
    private static Type GetGameViewWindowType()
    {
        //Assembly unityEditorAssembly = typeof(Editor).Assembly;
        //Type gameViewWindowType = unityEditorAssembly.GetType("UnityEditor.GameView");
        //return gameViewWindowType;
        return typeof(Editor).Assembly.GetType("UnityEditor.GameView");
    }



    /// <summary>
    /// Returns Game View Window
    /// </summary>
    /// <param name="gameViewWindowType"></param>
    /// <returns></returns>
    private static EditorWindow GetGameViewWindow(Type gameViewWindowType)
    {
        return EditorWindow.GetWindow(gameViewWindowType);
    }



    /// <summary>
    /// Returns Game View Sizes Length
    /// </summary>
    /// <returns></returns>
    private static int GameViewSizeLength()
    {
        var group = GetGameViewSizeGroup(GetCurrentGameViewSizeGroupType());
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        return (getDisplayTexts.Invoke(group, null) as string[]).Length;
    }



    /// <summary>
    /// Get Game View Size Group
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static object GetGameViewSizeGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }



    /// <summary>
    /// Get Current Game View Size Group Type
    /// </summary>
    /// <returns></returns>
    public static GameViewSizeGroupType GetCurrentGameViewSizeGroupType()
    {
        var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
    }



    /// <summary>
    /// Set size of Game View as per given index from List
    /// </summary>
    /// <param name="index"></param>
    public static void SetSize(int index)
    {
        var gameViewWindowType = GetGameViewWindowType();
        var gameViewWindow = GetGameViewWindow(gameViewWindowType);

        //var selectedSizeIndexProp = gameViewWindowType.GetProperty("selectedSizeIndex",
        //        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //selectedSizeIndexProp.SetValue(gameViewWindow, index, null);

        //var updateZoomAreaAndParentMethod = gameViewWindowType.GetMethod("UpdateZoomAreaAndParent", BindingFlags.Instance | BindingFlags.NonPublic);
        //updateZoomAreaAndParentMethod.Invoke(gameViewWindow, null);

        var SizeSelectionCallback = gameViewWindowType.GetMethod("SizeSelectionCallback",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gameViewWindow, new object[] { index, null });


        //SetGameViewScale();
        //SnapZoomMethod();
    }



    /// <summary>
    /// Set Game View Scale
    /// </summary>
    private static void SetGameViewScale()
    {
        var gameViewWindowType = GetGameViewWindowType();
        var gameViewWindow = GetGameViewWindow(gameViewWindowType);

        if (gameViewWindowType == null)
        {
            Debug.LogError("GameView is null!");
            return;
        }

        var defScaleField = gameViewWindowType.GetField("m_defaultScale", BindingFlags.Instance | BindingFlags.NonPublic);

        float defaultScale = 1f;

        var areaField = gameViewWindowType.GetField("m_ZoomArea", BindingFlags.Instance | BindingFlags.NonPublic);
        var areaObj = areaField.GetValue(gameViewWindow);

        var scaleField = areaObj.GetType().GetField("m_Scale", BindingFlags.Instance | BindingFlags.NonPublic);
        scaleField.SetValue(areaObj, new Vector2(defaultScale, defaultScale));
    }



    /// <summary>
    /// Snap Zoom Game View
    /// </summary>
    private static void SnapZoomMethod()
    {
        var gameViewWindowType = GetGameViewWindowType();
        var gameViewWindow = GetGameViewWindow(gameViewWindowType);

        var snapZoomMethod = gameViewWindowType.GetMethod("SnapZoom", BindingFlags.Instance | BindingFlags.NonPublic);
        var minScaleProperty = gameViewWindowType.GetProperty("minScale", BindingFlags.Instance | BindingFlags.NonPublic);
        if (minScaleProperty != null && snapZoomMethod != null)
        {
            float minScale = (float)minScaleProperty.GetValue(gameViewWindow, null);
            snapZoomMethod.Invoke(gameViewWindow, new object[] { minScale });
        }
    }



    #region Menu Items

    [MenuItem("Tools/GameViewSize/Previous %F1")]
    private static void SetPreviousGameViewSize()
    {
        if (screenIndex - 1 >= 0)
        {
            screenIndex -= 1;
        }
        else
        {
            screenIndex = GameViewSizeLength() - 1;
        }

        SetSize(screenIndex);
    }

    [MenuItem("Tools/GameViewSize/Next  %F2")]
    private static void SetNextGameViewSize()
    {
        if (screenIndex + 1 < GameViewSizeLength())
        {
            screenIndex += 1;
        }
        else
        {
            screenIndex = 0;
        }

        SetSize(screenIndex);
    }

    #endregion
}