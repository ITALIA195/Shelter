using UnityEngine;

public class SettingReciveInput : MonoBehaviour
{
    public int id;

    private void OnClick()
    {
        transform.Find("Label").gameObject.GetComponent<UILabel>().text = "*wait for input";
    }

    private void Start()
    {
    }

    private void Update()
    {
    }
}
