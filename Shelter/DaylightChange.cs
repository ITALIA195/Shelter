using Mod;
using UnityEngine;

public class DaylightChange : MonoBehaviour
{
    private void OnSelectionChange()
    {
        IN_GAME_MAIN_CAMERA.DayLight = Room.DayLightToEnum(GetComponent<UIPopupList>().selection);
    }
}
