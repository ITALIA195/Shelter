﻿using JetBrains.Annotations;

namespace Mod
{
    [UsedImplicitly]
    public abstract class Command
    {
        public abstract string CommandName { get; }
        public virtual string[] Aliases { get; } = null;
        public abstract void Execute(string[] args);
    }
}
