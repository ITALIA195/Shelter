using NGUI;
using NGUI.Internal;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Surface")]
// ReSharper disable once CheckNamespace
public class DragDropSurface : MonoBehaviour
{
    public bool rotatePlacedObject;

    private void OnDrop(GameObject go)
    {
        DragDropItem component = go.GetComponent<DragDropItem>();
        if (component != null)
        {
            Transform transform = NGUITools.AddChild(gameObject, component.prefab).transform;
            transform.position = UICamera.lastHit.point;
            if (this.rotatePlacedObject)
            {
                transform.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
            }
            Destroy(go);
        }
    }
}
