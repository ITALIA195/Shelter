using System.Collections.Generic;
using JetBrains.Annotations;
using Mod;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
// ReSharper disable once CheckNamespace
public class PickupItemSyncer : Photon.MonoBehaviour
{
    public bool IsWaitingForPickupInit;
    private const float TimeDeltaToIgnore = 0.2f;

    public void AskForPickupItemSpawnTimes()
    {
        if (this.IsWaitingForPickupInit)
        {
            if (PhotonNetwork.PlayerList.Length < 2)
            {
                Debug.Log("Cant ask anyone else for PickupItem spawn times.");
                this.IsWaitingForPickupInit = false;
            }
            else
            {
                Player next = PhotonNetwork.MasterClient.GetNext();
                if (next == null || next.Equals(Player.Self))
                {
                    next = Player.Self.GetNext();
                }
                if (next != null && !next.Equals(Player.Self))
                {
                    photonView.RPC(Rpc.RequestForPickupTimes, next);
                }
                else
                {
                    Debug.Log("No player left to ask");
                    this.IsWaitingForPickupInit = false;
                }
            }
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log(string.Concat("Joined Room. isMasterClient: ", PhotonNetwork.isMasterClient, " id: ", Player.Self.ID));
        this.IsWaitingForPickupInit = !PhotonNetwork.isMasterClient;
        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            Invoke(nameof(AskForPickupItemSpawnTimes), 2f);
        }
    }

    public void OnPhotonPlayerConnected(Player newPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            this.SendPickedUpItems(newPlayer);
        }
    }

    [RPC]
    [UsedImplicitly]
    public void PickupItemInit(double timeBase, float[] inactivePickupsAndTimes)
    {
        this.IsWaitingForPickupInit = false;
        for (int i = 0; i < inactivePickupsAndTimes.Length / 2; i++)
        {
            int index = i * 2;
            int viewID = (int) inactivePickupsAndTimes[index];
            float num4 = inactivePickupsAndTimes[index + 1];
            PhotonView view = PhotonView.Find(viewID);
            PickupItem component = view.GetComponent<PickupItem>();
            if (num4 <= 0f)
            {
                component.PickedUp(0f);
            }
            else
            {
                double num5 = num4 + timeBase;
                Debug.Log(string.Concat(view.viewID, " respawn: ", num5, " timeUntilRespawnBasedOnTimeBase:", num4, " SecondsBeforeRespawn: ", component.SecondsBeforeRespawn));
                double num6 = num5 - PhotonNetwork.time;
                if (num4 <= 0f)
                {
                    num6 = 0.0;
                }
                component.PickedUp((float) num6);
            }
        }
    }

    [RPC]
    [UsedImplicitly]
    public void RequestForPickupTimes(PhotonMessageInfo msgInfo)
    {
        if (msgInfo.sender == null)
        {
            Debug.LogError("Unknown player asked for PickupItems");
        }
        else
        {
            this.SendPickedUpItems(msgInfo.sender);
        }
    }

    private void SendPickedUpItems(Player targtePlayer)
    {
        if (targtePlayer == null)
        {
            Debug.LogWarning("Cant send PickupItem spawn times to unknown targetPlayer.");
        }
        else
        {
            double time = PhotonNetwork.time;
            double num2 = time + 0.20000000298023224;
            PickupItem[] array = new PickupItem[PickupItem.DisabledPickupItems.Count];
            PickupItem.DisabledPickupItems.CopyTo(array);
            List<float> list = new List<float>(array.Length * 2);
            for (int i = 0; i < array.Length; i++)
            {
                PickupItem item = array[i];
                if (item.SecondsBeforeRespawn <= 0f)
                {
                    list.Add(item.ViewID);
                    list.Add(0f);
                }
                else
                {
                    double num4 = item.TimeOfRespawn - PhotonNetwork.time;
                    if (item.TimeOfRespawn > num2)
                    {
                        Debug.Log(string.Concat(item.ViewID, " respawn: ", item.TimeOfRespawn, " timeUntilRespawn: ", num4, " (now: ", PhotonNetwork.time, ")"));
                        list.Add(item.ViewID);
                        list.Add((float) num4);
                    }
                }
            }
            photonView.RPC(Rpc.PickupItemInit, targtePlayer, PhotonNetwork.time, list.ToArray());
        }
    }
}

