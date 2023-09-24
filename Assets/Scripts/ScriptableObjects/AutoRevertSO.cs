using UnityEditor;
using UnityEngine;

public class AutoRevertSO : ScriptableObject
{
    protected virtual void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        MenuController.RestartEvent += UnloadSO;
    }

    protected virtual void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            UnloadSO();
        }
    }

    public virtual void UnloadSO()
    {
        Resources.UnloadAsset(this);
    }
}
