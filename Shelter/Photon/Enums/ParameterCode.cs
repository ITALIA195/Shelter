﻿using System;

namespace Photon.Enums
{
    /// <summary>
    /// Class for constants. Codes for parameters of Operations and Events.
    /// Pun uses these constants internally.
    /// </summary>
    public static class ParameterCode
    {
        /// <summary>(237) A bool parameter for creating games. If set to true, no room events are sent to the clients on join and leave. Default: false (and not sent).</summary>
        public const byte SuppressRoomEvents = 237;

        /// <summary>(236) Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
        public const byte EmptyRoomTTL = 236;

        /// <summary>(235) Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds.</summary>
        public const byte PlayerTTL = 235;

        /// <summary>(234) Optional parameter of OpRaiseEvent and OpSetCustomProperties to forward the event/operation to a web-service.</summary>
        public const byte EventForward = 234;

        /// <summary>(233) Optional parameter of OpLeave in async games. If false, the player does abandons the game (forever). By default players become inactive and can re-join.</summary>
        [Obsolete("Use: IsInactive")]
        public const byte IsComingBack = 233;

        /// <summary>(233) Used in EvLeave to describe if a user is inactive (and might come back) or not. In rooms with PlayerTTL, becoming inactive is the default case.</summary>
        public const byte IsInactive = 233;

        /// <summary>(232) Used when creating rooms to define if any userid can join the room only once.</summary>
        public const byte CheckUserOnJoin = 232;

        /// <summary>(231) Code for "Check And Swap" (CAS) when changing properties.</summary>
        public const byte ExpectedValues = 231;

        /// <summary>(230) Address of a (game) server to use.</summary>
        public const byte Address = 230;

        /// <summary>(229) Count of players in this application in a rooms (used in stats event)</summary>
        public const byte PeerCount = 229;

        /// <summary>(228) Count of games in this application (used in stats event)</summary>
        public const byte GameCount = 228;

        /// <summary>(227) Count of players on the master server (in this app, looking for rooms)</summary>
        public const byte MasterPeerCount = 227;

        /// <summary>(225) User's ID</summary>
        public const byte UserId = 225;

        /// <summary>(224) Your application's ID: a name on your own Photon or a GUID on the Photon Cloud</summary>
        public const byte ApplicationId = 224;

        /// <summary>(223) Not used currently (as "Position"). If you get queued before connect, this is your position</summary>
        public const byte Position = 223;

        /// <summary>(223) Modifies the matchmaking algorithm used for OpJoinRandom. Allowed parameter values are defined in enum MatchmakingMode.</summary>
        public const byte MatchMakingType = 223;

        /// <summary>(222) List of RoomInfos about open / listed rooms</summary>
        public const byte GameList = 222;

        /// <summary>(221) Internally used to establish encryption</summary>
        public const byte Secret = 221;

        /// <summary>(220) Version of your application</summary>
        public const byte AppVersion = 220;

        /// <summary>(210) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureNodeInfo = 210;	// only used within events, so use: EventCode.AzureNodeInfo

        /// <summary>(209) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureLocalNodeId = 209;

        /// <summary>(208) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureMasterNodeId = 208;

        /// <summary>(255) Code for the gameId/roomName (a unique name per room). Used in OpJoin and similar.</summary>
        public const byte RoomName = 255;

        /// <summary>(250) Code for broadcast parameter of OpSetProperties method.</summary>
        public const byte Broadcast = 250;

        /// <summary>(252) Code for list of players in a room. Currently not used.</summary>
        public const byte ActorList = 252;

        /// <summary>(254) Code of the Actor of an operation. Used for property get and set.</summary>
        public const byte ActorNr = 254;

        /// <summary>(249) Code for property set (Hashtable).</summary>
        public const byte PlayerProperties = 249;

        /// <summary>(245) Code of data/custom content of an event. Used in OpRaiseEvent.</summary>
        public const byte CustomEventContent = 245;

        /// <summary>(245) Code of data of an event. Used in OpRaiseEvent.</summary>
        public const byte Data = 245;

        /// <summary>(244) Code used when sending some code-related parameter, like OpRaiseEvent's event-code.</summary>
        /// <remarks>This is not the same as the Operation's code, which is no longer sent as part of the parameter Dictionary in Photon 3.</remarks>
        public const byte Code = 244;

        /// <summary>(248) Code for property set (Hashtable).</summary>
        public const byte GameProperties = 248;

        /// <summary>
        /// (251) Code for property-set (Hashtable). This key is used when sending only one set of properties.
        /// If either ActorProperties or GameProperties are used (or both), check those keys.
        /// </summary>
        public const byte Properties = 251;

        /// <summary>(253) Code of the target Actor of an operation. Used for property set. Is 0 for game</summary>
        public const byte TargetActorNr = 253;

        /// <summary>(246) Code to select the receivers of events (used in Lite, Operation RaiseEvent).</summary>
        public const byte ReceiverGroup = 246;

        /// <summary>(247) Code for caching events while raising them.</summary>
        public const byte Cache = 247;

        /// <summary>(241) Bool parameter of CreateRoom Operation. If true, server cleans up roomcache of leaving players (their cached events get removed).</summary>
        public const byte CleanupCacheOnLeave = 241;

        /// <summary>(240) Code for "group" operation-parameter (as used in Op RaiseEvent).</summary>
        public const byte Group = 240;

        /// <summary>(239) The "Remove" operation-parameter can be used to remove something from a list. E.g. remove groups from player's interest groups.</summary>
        public const byte Remove = 239;

        /// <summary>(239) Used in Op Join to define if UserIds of the players are broadcast in the room. Useful for FindFriends and reserving slots for expected users.</summary>
        public const byte PublishUserId = 239;

        /// <summary>(238) The "Add" operation-parameter can be used to add something to some list or set. E.g. add groups to player's interest groups.</summary>
        public const byte Add = 238;

        /// <summary>(218) Content for EventCode.ErrorInfo and internal debug operations.</summary>
        public const byte Info = 218;

        /// <summary>(217) This key's (byte) value defines the target custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationType = 217;

        /// <summary>(216) This key's (string) value provides parameters sent to the custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationParams = 216;

        /// <summary>(215) Makes the server create a room if it doesn't exist. OpJoin uses this to always enter a room, unless it exists and is full/closed.</summary>
        public const byte CreateIfNotExists = 215;

        /// <summary>(215) The JoinMode enum defines which variant of joining a room will be executed: Join only if available, create if not exists or re-join.</summary>
        /// <remarks>Replaces CreateIfNotExists which was only a bool-value.</remarks>
        public const byte JoinMode = 215;

        /// <summary>(214) This key's (string or byte[]) value provides parameters sent to the custom authentication service setup in Photon Dashboard. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationData = 214;

        /// <summary>(203) Code for MasterClientId, which is synced by server. When sent as op-parameter this is code 203.</summary>
        /// <remarks>Tightly related to GamePropertyKey.MasterClientId.</remarks>
        public const byte MasterClientId = 203;

        /// <summary>(1) Used in Op FindFriends request. Value must be string[] of friends to look up.</summary>
        public const byte FindFriendsRequestList = 1;

        /// <summary>(1) Used in Op FindFriends response. Contains bool[] list of online states (false if not online).</summary>
        public const byte FindFriendsResponseOnlineList = 1;

        /// <summary>(2) Used in Op FindFriends response. Contains string[] of room names ("" where not known or no room joined).</summary>
        public const byte FindFriendsResponseRoomIdList = 2;

        /// <summary>(213) Used in matchmaking-related methods and when creating a room to name a lobby (to join or to attach a room to).</summary>
        public const byte LobbyName = 213;

        /// <summary>(212) Used in matchmaking-related methods and when creating a room to define the type of a lobby. Combined with the lobby name this identifies the lobby.</summary>
        public const byte LobbyType = 212;

        /// <summary>(211) This (optional) parameter can be sent in Op Authenticate to turn on Lobby Stats (info about lobby names and their user- and game-counts). See: PhotonNetwork.Lobbies</summary>
        public const byte LobbyStats = 211;

        /// <summary>(210) Used for region values in OpAuth and OpGetRegions.</summary>
        public const byte Region = 210;

        /// <summary>(209) Path of the WebRPC that got called. Also known as "WebRpc Name". Type: string.</summary>
        public const byte UriPath = 209;

        /// <summary>(208) Parameters for a WebRPC as: Dictionary&lt;string, object&gt;. This will get serialized to JSon.</summary>
        public const byte WebRpcParameters = 208;

        /// <summary>(207) ReturnCode for the WebRPC, as sent by the web service (not by Photon, which uses ErrorCode). Type: byte.</summary>
        public const byte WebRpcReturnCode = 207;

        /// <summary>(206) Message returned by WebRPC server. Analog to Photon's debug message. Type: string.</summary>
        public const byte WebRpcReturnMessage = 206;

        /// <summary>(205) Used to define a "slice" for cached events. Slices can easily be removed from cache. Type: int.</summary>
        public const byte CacheSliceIndex = 205;

        /// <summary>(204) Informs the server of the expected plugin setup.</summary>
        /// <remarks>
        /// The operation will fail in case of a plugin mismatch returning error code PluginMismatch 32751(0x7FFF - 16).
        /// Setting string[]{} means the client expects no plugin to be setup.
        /// Note: for backwards compatibility null omits any check.
        /// </remarks>
        public const byte Plugins = 204;

        /// <summary>(202) Used by the server in Operation Responses, when it sends the nickname of the client (the user's nickname).</summary>
        public const byte NickName = 202;

        /// <summary>(201) Informs user about name of plugin load to game</summary>
        public const byte PluginName = 201;

        /// <summary>(200) Informs user about version of plugin load to game</summary>
        public const byte PluginVersion = 200;

        /// <summary>(195) Protocol which will be used by client to connect master/game servers. Used for nameserver.</summary>
        public const byte ExpectedProtocol = 195;

        /// <summary>(194) Set of custom parameters which are sent in auth request.</summary>
        public const byte CustomInitData = 194;

        /// <summary>(193) How are we going to encrypt data.</summary>
        public const byte EncryptionMode = 193;

        /// <summary>(192) Parameter of Authentication, which contains encryption keys (depends on AuthMode and EncryptionMode).</summary>
        public const byte EncryptionData = 192;
    }
}