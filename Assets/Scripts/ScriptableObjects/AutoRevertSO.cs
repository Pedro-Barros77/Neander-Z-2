using UnityEditor;
using UnityEngine;

public class AutoRevertSO : ScriptableObject
{
    protected virtual void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    protected virtual void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Resources.UnloadAsset(this);
        }
    }
}
