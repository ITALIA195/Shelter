﻿using Mod.Exceptions;
using Mod.Interface;
using Photon;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandTeleport : Command
    {
        public override string CommandName => "tp";
        public override string[] Aliases => new[] {"teleport"};

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/tp [id]");
            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);

            if (player.Hero != null) //TODO: Change to player.Hero.IsInstantiated, as it is not granted that if it exist it is already instantiated
            {
                Transform t = player.Hero.transform;
                Player.Self.Hero.transform.position = t.position;
                Player.Self.Hero.transform.rotation = t.rotation;
                Notify.New($"You teleported to {player.Properties.HexName}!", 1.3f, 35F);
            }

            Shelter.Chat.System("Player [{0}] is not alive", player.ToStringHex());
        }
    }
}
