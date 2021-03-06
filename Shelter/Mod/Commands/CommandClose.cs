﻿using Mod.Exceptions;
using Mod.Interface;
using Photon;

namespace Mod.Commands
{
    public class CommandClose : Command
    {
        public override string CommandName => "close";

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/close [id]");
            if (!Player.TryParse(args[0], out var player))
                throw new PlayerNotFoundException(args[0]);

            PhotonNetwork.RaiseEvent(203, null, true, new RaiseEventOptions { TargetActors = new[] { player.ID } });
            PhotonNetwork.DestroyPlayerObjects(player);
            GameManager.instance.photonView.RPC(Rpc.ShowResult, player, "", "", "", "", "", "Kicked by ShelterMod");
            Notify.New("Player has been kicked!", 3f);
        }
    }
}
