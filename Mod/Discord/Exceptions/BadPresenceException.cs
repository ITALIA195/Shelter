﻿using System;

namespace Mod.Discord.Exceptions
{
    class BadPresenceException : Exception
    {
        public BadPresenceException(string message) : base(message) { }
    }
}
