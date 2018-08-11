using UnityEngine;

[RequireComponent(typeof(UIWidget)), ExecuteInEditMode]
public class AnimatedColor : MonoBehaviour
{
    public Color color = Color.white;
    private UIWidget mWidget;

    private void Awake()
    {
        this.mWidget = GetComponent<UIWidget>();
    }

    private void Update()
    {
        this.mWidget.color = this.color;
    }
}

