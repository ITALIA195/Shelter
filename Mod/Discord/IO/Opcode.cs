﻿namespace Mod.Discord.IO
{
    public enum Opcode : uint
    {
        Handshake = 0,
        Frame = 1,
        Close = 2,
        Ping = 3,
        Pong = 4
    }
}
