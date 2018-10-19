using UnityEngine;

[AddComponentMenu("NGUI/Examples/Load Level On Click")]
// ReSharper disable once CheckNamespace
public class LoadLevelOnClick : MonoBehaviour
{
    public string levelName;

    private void OnClick()
    {
        if (!string.IsNullOrEmpty(this.levelName))
        {
            Application.LoadLevel(this.levelName);
        }
    }
}
