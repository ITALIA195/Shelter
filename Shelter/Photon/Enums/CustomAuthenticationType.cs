﻿namespace Photon.Enums
{
    /// <summary>
    /// Options for optional "Custom Authentication" services used with Photon. Used by OpAuthenticate after connecting to Photon.
    /// </summary>
    public enum CustomAuthenticationType : byte
    {
        /// <summary>Use a custom authentification service. Currently the only implemented option.</summary>
        Custom = 0,

        /// <summary>Authenticates users by their Steam Account. Set auth values accordingly!</summary>
        Steam = 1,

        /// <summary>Authenticates users by their Facebook Account. Set auth values accordingly!</summary>
        Facebook = 2,

        /// <summary>Authenticates users by their Oculus Account and token.</summary>
        Oculus = 3,

        /// <summary>Authenticates users by their PSN Account and token.</summary>
        PlayStation = 4,

        /// <summary>Authenticates users by their Xbox Account and XSTS token.</summary>
        Xbox = 5,

        /// <summary>Disables custom authentification. Same as not providing any AuthenticationValues for connect (more precisely for: OpAuthenticate).</summary>
        None = byte.MaxValue
    }
}