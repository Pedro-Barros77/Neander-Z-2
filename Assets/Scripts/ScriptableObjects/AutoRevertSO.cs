using UnityEditor;
using UnityEngine;

public class AutoRevertSO : ScriptableObject
{
    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
        MenuController.RestartEvent += UnloadSO;
    }

#if UNITY_EDITOR
    protected virtual void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            UnloadSO();
        }
    }

#endif
    public virtual void UnloadSO()
    {
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
        {
            Resources.UnloadAsset(this);
        }
#else
        Resources.UnloadAsset(this);
#endif
    }

}
