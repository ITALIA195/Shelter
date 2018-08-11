using ExitGames.Client.Photon;
using UnityEngine;

public class BTN_choose_human : MonoBehaviour
{
    public bool isPlayerAllDead()
    {
        int num = 0;
        int num2 = 0;
        foreach (Player player in PhotonNetwork.playerList)
        {
            if ((int) player.Properties[PhotonPlayerProperty.isTitan] == 1)
            {
                num++;
                if ((bool) player.Properties[PhotonPlayerProperty.dead])
                {
                    num2++;
                }
            }
        }
        return num == num2;
    }

    public bool isPlayerAllDead2()
    {
        int num = 0;
        int num2 = 0;
        foreach (Player player in PhotonNetwork.playerList)
        {
            if (RCextensions.returnIntFromObject(player.Properties[PhotonPlayerProperty.isTitan]) == 1)
            {
                num++;
                if (RCextensions.returnBoolFromObject(player.Properties[PhotonPlayerProperty.dead]))
                {
                    num2++;
                }
            }
        }
        return num == num2;
    }

    private void OnClick()
    {
        string selection = GameObject.Find("PopupListCharacterHUMAN").GetComponent<UIPopupList>().selection;
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], true);
        FengGameManagerMKII.instance.needChooseSide = false;
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
        {
            FengGameManagerMKII.instance.checkpoint = GameObject.Find("PVPchkPtH");
        }
        if (!PhotonNetwork.isMasterClient && FengGameManagerMKII.instance.roundTime > 60f)
        {
            if (!this.isPlayerAllDead2())
            {
                FengGameManagerMKII.instance.SpawnPlayerAfterGameEnd(selection);
            }
            else
            {
                FengGameManagerMKII.instance.SpawnPlayerAfterGameEnd(selection);
                FengGameManagerMKII.instance.photonView.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.TROST || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
        {
            if (this.isPlayerAllDead2())
            {
                FengGameManagerMKII.instance.SpawnPlayerAfterGameEnd(selection);
                FengGameManagerMKII.instance.photonView.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
            }
            else
            {
                FengGameManagerMKII.instance.SpawnPlayer(selection);
            }
        }
        else
        {
            FengGameManagerMKII.instance.SpawnPlayer(selection);
        }
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
        NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
        IN_GAME_MAIN_CAMERA.usingTitan = false;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
        Hashtable hashtable = new Hashtable
        {
            { PhotonPlayerProperty.character, selection }
        };
        Hashtable propertiesToSet = hashtable;
        Player.Self.SetCustomProperties(propertiesToSet);
    }
}

