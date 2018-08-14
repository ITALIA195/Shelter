using System.Collections.Generic;
using ExitGames.Client.Photon;
using Mod;
using Mod.Exceptions;
using UnityEngine;

public partial class FengGameManagerMKII
{
    public Dictionary<int, CannonValues> allowedToCannon;
    public Dictionary<string, Texture2D> assetCacheTextures;
    public InRoomChat chatRoom;
    public GameObject checkpoint;
    public int cyanKills;
    public int difficulty;
    public float distanceSlider;
    public bool gameStart;
    public List<GameObject> groundList;
    public FengCustomInputs inputManager;
    public bool isFirstLoad;
    public bool isPlayer1Winning;
    public bool isPlayer2Winning;
    public bool isRecompiling;
    public bool isRestarting;
    public bool isSpawning;
    public bool isUnloading;
    public bool justSuicide;
    public List<string[]> levelCache;
    public int magentaKills;
    public int maxPlayers;
    public float mouseSlider;
    public float myRespawnTime;
    public bool needChooseSide;
    public float pauseWaitTime;
    public string playerList;
    public List<Vector3> playerSpawnsC;
    public List<Vector3> playerSpawnsM;
    public List<Player> playersRPC;
    public Dictionary<string, int[]> PreservedPlayerKDR;
    public int PVPhumanScore;
    public int PVPtitanScore;
    public float qualitySlider;
    public List<GameObject> racingDoors;
    public Vector3 racingSpawnPoint;
    public bool racingSpawnPointSet;
    public List<float> restartCount;
    public bool restartingBomb;
    public bool restartingEren;
    public bool restartingHorse;
    public bool restartingMC;
    public bool restartingTitan;
    public float retryTime;
    public float roundTime;
    public Vector2 scroll;
    public Vector2 scroll2;
    public GameObject selectedObj;
    public List<GameObject> spectateSprites;
    public Texture2D textureBackgroundBlack;
    public Texture2D textureBackgroundBlue;
    public int time = 600;
    public List<TitanSpawner> titanSpawners;
    public List<Vector3> titanSpawns;
    public float transparencySlider;
    public float updateTime;
    public int wave = 1;

    [RPC]
    private void RequireStatus()
    {
        object[] parameters =
            {humanScore, titanScore, wave, highestwave, roundTime, timeTotalServer, startRacing, endRacing};
        photonView.RPC("refreshStatus", PhotonTargets.Others, parameters);
        object[] objArray2 = {PVPhumanScore, PVPtitanScore};
        photonView.RPC("refreshPVPStatus", PhotonTargets.Others, objArray2);
        object[] objArray3 = {teamScores};
        photonView.RPC("refreshPVPStatus_AHSS", PhotonTargets.Others, objArray3);
    }

    [RPC]
    private void RefreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2,
        bool startRacin, bool endRacin)
    {
        humanScore = score1;
        titanScore = score2;
        wave = wav;
        highestwave = highestWav;
        roundTime = time1;
        timeTotalServer = time2;
        startRacing = startRacin;
        endRacing = endRacin;
        if (startRacing && GameObject.Find("door") != null)
        {
            GameObject.Find("door").SetActive(false);
        }
    }

    [RPC]
    private void RefreshPVPStatus(int score1, int score2)
    {
        PVPhumanScore = score1;
        PVPtitanScore = score2;
    }

    [RPC]
    private void RefreshPVPStatus_AHSS(int[] score1)
    {
        print(score1);
        teamScores = score1;
    }

    [RPC]
    public void PauseRPC(bool pause, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            if (pause)
            {
                pauseWaitTime = 100000f;
                Time.timeScale = 1E-06f;
            }
            else
            {
                pauseWaitTime = 3f;
            }
        }
    }

    [RPC]
    private void LabelRPC(int viewId, PhotonMessageInfo info)
    {
        if (PhotonView.TryParse(viewId, out PhotonView view))
        {
            if (info.sender != view.owner)
                throw new NotAllowedException(nameof(LabelRPC), info);

            HERO hero = PhotonView.Find(viewId).gameObject.GetComponent<HERO>();
            if (hero != null)
            {
                if (info.sender.Properties.Guild != string.Empty)
                    hero.myNetWorkName.GetComponent<UILabel>().text =
                        "[FFFF00]" + info.sender.Properties.Guild + "\n[FFFFFF]" + info.sender.Properties.Name;
                else
                    hero.myNetWorkName.GetComponent<UILabel>().text = info.sender.Properties.Name;
            }
        }
    }

    [RPC]
    public void OneTitanDown(string name1, bool onPlayerLeave)
    {
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || PhotonNetwork.isMasterClient)
        {
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                if (name1 != string.Empty)
                {
                    if (name1 == "Titan")
                    {
                        PVPhumanScore++;
                    }
                    else if (name1 == "Aberrant")
                    {
                        PVPhumanScore += 2;
                    }
                    else if (name1 == "Jumper")
                    {
                        PVPhumanScore += 3;
                    }
                    else if (name1 == "Crawler")
                    {
                        PVPhumanScore += 4;
                    }
                    else if (name1 == "Female Titan")
                    {
                        PVPhumanScore += 10;
                    }
                    else
                    {
                        PVPhumanScore += 3;
                    }
                }

                CheckPVPPoints();
                object[] parameters = {PVPhumanScore, PVPtitanScore};
                photonView.RPC("refreshPVPStatus", PhotonTargets.Others, parameters);
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.CAGE_FIGHT)
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
                {
                    if (IsAnyTitanAlive())
                    {
                        GameWin();
                        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    if (IsAnyTitanAlive())
                    {
                        wave++;
                        if (!(LevelInfoManager.GetInfo(Level).RespawnMode != RespawnMode.NEWROUND &&
                              (!Level.StartsWith("Custom") || RCSettings.gameType != 1) ||
                              IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER))
                        {
                            foreach (Player player in PhotonNetwork.playerList)
                            {
                                if (player.Properties.PlayerType != PlayerType.Titan)
                                {
                                    photonView.RPC("respawnHeroInNewRound", player);
                                }
                            }
                        }

                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
                        {
                            SendChatContentInfo("<color=#A8FF24>Wave : " + wave + "</color>");
                        }

                        if (wave > highestwave)
                        {
                            highestwave = wave;
                        }

                        if (PhotonNetwork.isMasterClient)
                        {
                            RequireStatus();
                        }

                        if (!((RCSettings.maxWave != 0 || wave <= 20) &&
                              (RCSettings.maxWave <= 0 || wave <= RCSettings.maxWave)))
                        {
                            GameWin();
                        }
                        else
                        {
                            int abnormal = 90;
                            if (difficulty == 1)
                            {
                                abnormal = 70;
                            }

                            if (!LevelInfoManager.GetInfo(Level).HasPunk)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, wave + 2, false);
                            }
                            else if (wave == 5)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 1, true);
                            }
                            else if (wave == 10)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 2, true);
                            }
                            else if (wave == 15)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 3, true);
                            }
                            else if (wave == 20)
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, 4, true);
                            }
                            else
                            {
                                SpawnTitanCustom("titanRespawn", abnormal, wave + 2, false);
                            }
                        }
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
                {
                    if (!onPlayerLeave)
                    {
                        humanScore++;
                        int num2 = 90;
                        if (difficulty == 1)
                        {
                            num2 = 70;
                        }

                        SpawnTitanCustom("titanRespawn", num2, 1, false);
                    }
                }
                else if (LevelInfoManager.GetInfo(Level).EnemyNumber != -1)
                {
                }
            }
        }
    }

    [RPC]
    private void SetMasterRC(PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            masterRC = true;
        }
    }

    [RPC]
    public void RespawnHeroInNewRound()
    {
        if (!needChooseSide && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            SpawnPlayer(myLastHero);
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    private void NetGameLose(int score, PhotonMessageInfo info)
    {
        isLosing = true;
        titanScore = score;
        gameEndCD = gameEndTotalCDtime;
        if ((int) settings[244] == 1)
        {
            Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") +
                                      ")</color> Round ended (game lose).");
        }

        if (!(info.sender == PhotonNetwork.masterClient || info.sender.isLocal) && PhotonNetwork.isMasterClient)
        {
            Mod.Interface.Chat.System("Round end sent from Player " + info.sender.ID);
        }
    }

    [RPC]
    private void RestartGameByClient(PhotonMessageInfo info)
    {
        throw new NotAllowedException(nameof(RestartGameByClient), info);
    }

    [RPC]
    private void NetGameWin(int score, PhotonMessageInfo info)
    {
        humanScore = score;
        isWinning = true;
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
        {
            teamWinner = score;
            teamScores[teamWinner - 1]++;
            gameEndCD = gameEndTotalCDtime;
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
        {
            if (RCSettings.racingStatic == 1)
            {
                gameEndCD = 1000f;
            }
            else
            {
                gameEndCD = 20f;
            }
        }
        else
        {
            gameEndCD = gameEndTotalCDtime;
        }

        if ((int) settings[244] == 1)
        {
            Mod.Interface.Chat.System("<color=#FFC000>(" + roundTime.ToString("F2") +
                                      ")</color> Round ended (game win).");
        }

        if (!(Equals(info.sender, PhotonNetwork.masterClient) || info.sender.isLocal))
        {
            Mod.Interface.Chat.System("Round end sent from Player " + info.sender.ID);
        }
    }

    [RPC]
    public void SomeOneIsDead(int id = -1)
    {
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
        {
            if (id != 0)
            {
                PVPtitanScore += 2;
            }

            CheckPVPPoints();
            object[] parameters = {PVPhumanScore, PVPtitanScore};
            photonView.RPC("refreshPVPStatus", PhotonTargets.Others, parameters);
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
        {
            titanScore++;
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN ||
                 IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE ||
                 IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT ||
                 IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.TROST)
        {
            if (IsAnyPlayerAlive())
            {
                GameLose();
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS && RCSettings.pvpMode == 0 &&
                 RCSettings.bombMode == 0)
        {
            if (IsAnyPlayerAlive())
            {
                GameLose();
                teamWinner = 0;
            }

            if (IsAnyTeamMemberAlive(1))
            {
                teamWinner = 2;
                GameWin();
            }

            if (IsAnyTeamMemberAlive(2))
            {
                teamWinner = 1;
                GameWin();
            }
        }
    }

    [RPC]
    private void NetRefreshRacingResult(string tmp)
    {
        localRacingResult = tmp;
    }

    [RPC]
    public void NetShowDamage(int speed)
    {
        GameObject t;
        if ((t = GameObject.Find("Stylish")) != null)
            t.GetComponent<StylishComponent>().Style(speed);
        GameObject target = GameObject.Find("LabelScore");
        Debug.Log("Target null?:" + target == null);
        if (target != null)
        {
            target.GetComponent<UILabel>().text = speed.ToString();
            target.transform.localScale = Vector3.zero;
            speed = (int) (speed * 0.1f);
            speed = Mathf.Max(40, speed);
            speed = Mathf.Min(150, speed);
            iTween.Stop(target);
            object[] args =
                {"x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f};
            iTween.ScaleTo(target, iTween.Hash(args));
            object[] objArray2 =
                {"x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f};
            iTween.ScaleTo(target, iTween.Hash(objArray2));
        }
    }

    [RPC]
    private void LoadskinRPC(string n, string url, string url2, string[] skybox, PhotonMessageInfo info)
    {
        if ((int) settings[2] == 1 && info.sender.IsMasterClient)
        {
            StartCoroutine(LoadSkinEnumerator(n, url, url2, skybox));
        }
    }

    [RPC]
    private void RPCLoadLevel(PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            DestroyAllExistingCloths();
            PhotonNetwork.LoadLevel(LevelInfoManager.GetInfo(Level).LevelName);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            KickPlayerRC(info.sender, true, "false restart.");
        }
        else if (!masterRC)
        {
            restartCount.Add(Time.time);
            foreach (float num in restartCount)
            {
                if (Time.time - num > 60f)
                {
                    restartCount.Remove(num);
                }
            }

            if (restartCount.Count < 6)
            {
                DestroyAllExistingCloths();
                PhotonNetwork.LoadLevel(LevelInfoManager.GetInfo(Level).LevelName);
            }
        }
    }

    [RPC]
    private void GetRacingResult(string player, float time1)
    {
        RacingResult result = new RacingResult
        {
            name = player,
            time = time1
        };
        racingResult.Add(result);
        RefreshRacingResult();
    }

    [RPC]
    private void IgnorePlayer(int id, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(IgnorePlayer), info);

        if (Player.TryParse(id, out Player player) && !ignoreList.Contains(id))
        {
            ignoreList.Add(player.ID);
            PhotonNetwork.RaiseEvent(254, null, true, new RaiseEventOptions
            {
                TargetActors = new[] {id}
            });
        }
    }

    [RPC]
    private void IgnorePlayerArray(IEnumerable<int> ids, PhotonMessageInfo info)
    {
        if (!info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(IgnorePlayerArray), info);

        foreach (int id in ids)
        {
            if (!Player.TryParse(id, out Player _))
                continue;
            if (!ignoreList.Contains(id))
            {
                ignoreList.Add(id);
                PhotonNetwork.RaiseEvent(254, null, true, new RaiseEventOptions
                {
                    TargetActors = new[] {id}
                });
            }
        }
    }

    [RPC]
    private void CustomlevelRPC(string[] content, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            if (content.Length == 1 && content[0] == "loadcached")
            {
                StartCoroutine(CustomLevelCache());
            }
            else if (content.Length == 1 && content[0] == "loadempty")
            {
                currentLevel = string.Empty;
                levelCache.Clear();
                titanSpawns.Clear();
                playerSpawnsC.Clear();
                playerSpawnsM.Clear();
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
                {
                    {PlayerProperty.CurrentLevel, currentLevel}
                };
                Player.Self.SetCustomProperties(propertiesToSet);
                customLevelLoaded = true;
                SpawnPlayerCustomMap();
            }
            else
            {
                CustomLevelClient(content, true);
            }
        }
    }

    [RPC]
    private void Clearlevel(string[] link, int gametype, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            if (gametype == 0)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.KILL_TITAN;
            }
            else if (gametype == 1)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.SURVIVE_MODE;
            }
            else if (gametype == 2)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.PVP_AHSS;
            }
            else if (gametype == 3)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.RACING;
            }
            else if (gametype == 4)
            {
                IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.None;
            }

            if (info.sender.IsMasterClient && link.Length > 6)
            {
                StartCoroutine(ClearLevelEnumerator(link));
            }
        }
    }

    [RPC]
    private void ShowResult(string text0, string text1, string text2, string text3, string text4, string text6,
        PhotonMessageInfo t)
    {
        if (!(gameTimesUp || !t.sender.IsMasterClient))
        {
            gameTimesUp = true;
            GameObject obj2 = GameObject.Find("UI_IN_GAME");
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[0], false);
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[2], true);
            NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[3], false);
            GameObject.Find("LabelName").GetComponent<UILabel>().text = text0;
            GameObject.Find("LabelKill").GetComponent<UILabel>().text = text1;
            GameObject.Find("LabelDead").GetComponent<UILabel>().text = text2;
            GameObject.Find("LabelMaxDmg").GetComponent<UILabel>().text = text3;
            GameObject.Find("LabelTotalDmg").GetComponent<UILabel>().text = text4;
            GameObject.Find("LabelResultTitle").GetComponent<UILabel>().text = text6;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
            gameStart = false;
        }
        else if (!(t.sender.IsMasterClient || !Player.Self.IsMasterClient))
        {
            KickPlayerRC(t.sender, true, "false game end.");
        }
    }

    [RPC]
    private void SpawnTitanRPC(PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            foreach (TITAN titan in titans)
            {
                if (titan.photonView.isMine && !(PhotonNetwork.isMasterClient && !titan.nonAI))
                {
                    PhotonNetwork.Destroy(titan.gameObject);
                }
            }

            SpawnPlayerTitan(myLastHero);
        }
    }

    [RPC]
    private void Chat(string content, string sender, PhotonMessageInfo info)
    {
        if (sender == string.Empty)
            Mod.Interface.Chat.ReceiveMessage(info.sender, content);
        else
            Mod.Interface.Chat.ReceiveMessageFromPlayer(info.sender, content);
    }

    [RPC]
    private void SetTeamRPC(int setting, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient || info.sender.isLocal)
        {
            SetTeam(setting);
        }
    }

    [RPC]
    private void SettingRPC(ExitGames.Client.Photon.Hashtable hash, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient)
        {
            SetGameSettings(hash);
        }
    }

    [RPC]
    private void ShowChatContent(string content) // I don't know what this is
    {
        chatContent.Add(content);
        if (chatContent.Count > 10)
        {
            chatContent.RemoveAt(0);
        }

        GameObject.Find("LabelChatContent").GetComponent<UILabel>().text = string.Empty;
        foreach (object chat in chatContent)
        {
            UILabel component = GameObject.Find("LabelChatContent").GetComponent<UILabel>();
            component.text = component.text + chat;
        }
    }

    [RPC]
    private void UpdateKillInfo(bool t1, string killer, bool t2, string victim, int dmg)
    {
        GameObject obj2 = GameObject.Find("UI_IN_GAME");
        GameObject obj3 = (GameObject) Instantiate(Resources.Load("UI/KillInfo"));
        foreach (GameObject t in killInfoGO)
        {
            if (t != null)
                t.GetComponent<KillInfoComponent>().MoveOn();
        }

        if (killInfoGO.Count > 4)
        {
            GameObject obj4 = (GameObject) killInfoGO[0];
            if (obj4 != null)
            {
                obj4.GetComponent<KillInfoComponent>().Destroy();
            }

            killInfoGO.RemoveAt(0);
        }

        obj3.transform.parent = obj2.GetComponent<UIReferArray>().panels[0].transform;
        obj3.GetComponent<KillInfoComponent>().Show(t1, killer, t2, victim, dmg);
        killInfoGO.Add(obj3);
        if ((int) settings[244] == 1)
        {
            string str2 = "<color=#FFC000>(" + roundTime.ToString("F2") + ")</color> " + killer.hexColor() + " killed ";
            string newLine = str2 + victim.hexColor() + " for " + dmg + " damage.";
            Mod.Interface.Chat.System(newLine);
        }
    }

    [RPC]
    public void VerifyPlayerHasLeft(int ID, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient && Player.Find(ID) != null)
        {
            Player player = Player.Find(ID);
            banHash.Add(ID, player.Properties.Name);
        }
    }

    [RPC]
    public void SpawnPlayerAtRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
    {
        if (info.sender.IsMasterClient && logicLoaded && customLevelLoaded && !needChooseSide &&
            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            Vector3 position = new Vector3(posX, posY, posZ);
            IN_GAME_MAIN_CAMERA component = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>();
            component.setMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", position, new Quaternion(0f, 0f, 0f, 1f),
                0));
            string slot = myLastHero.ToUpper();
            switch (slot)
            {
                case "SET 1": //TODO: Remove and use ProfileSystem
                case "SET 2":
                case "SET 3":
                {
                    HeroCostume costume = CostumeConverter.LocalDataToHeroCostume(slot);
                    costume?.checkstat();
                    CostumeConverter.HeroCostumeToLocalData(costume, slot);
                    component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                    if (costume != null)
                    {
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat =
                            costume.stat;
                    }
                    else
                    {
                        costume = HeroCostume.costumeOption[3];
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat =
                            HeroStat.GetInfo(costume.name);
                    }

                    component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                    component.main_object.GetComponent<HERO>().setStat2();
                    component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
                    break;
                }
                default:
                    foreach (HeroCostume hero in HeroCostume.costume)
                    {
                        if (hero.name.EqualsIgnoreCase(slot))
                        {
                            int id = hero.id;
                            if (slot.ToUpper() != "AHSS")
                                id += CheckBoxCostume.costumeSet - 1;
                            if (HeroCostume.costume[id].name != hero.name)
                                id = hero.id + 1;

                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume =
                                HeroCostume.costume[id];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat =
                                HeroStat.GetInfo(HeroCostume.costume[id].name);
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>()
                                .setCharacterComponent();
                            component.main_object.GetComponent<HERO>().setStat2();
                            component.main_object.GetComponent<HERO>().setSkillHUDPosition2();
                            break;
                        }
                    }

                    break;
            }

            CostumeConverter.HeroCostumeToPhotonData(
                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, Player.Self);
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                Transform transform1 = component.main_object.transform;
                transform1.position +=
                    new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20));
            }

            Player.Self.SetCustomProperties(new Hashtable
            {
                {"dead", false},
                {PlayerProperty.IsTitan, 1}
            });
            component.enabled = true;
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().SetInterfacePosition();
            GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
            GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
            component.gameOver = false;
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }

            Screen.showCursor = false;
            isLosing = false;
            ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    public void TitanGetKill(Player player, int Damage, string name1, PhotonMessageInfo info)
    {
        if (info != null && !info.sender.IsMasterClient)
            throw new NotAllowedException(nameof(TitanGetKill), info);

        Damage = Mathf.Max(10, Damage);
        object[] parameters = {Damage};
        photonView.RPC("netShowDamage", player, parameters);
        object[] objArray2 = {name1, false};
        photonView.RPC("oneTitanDown", PhotonTargets.MasterClient, objArray2);
        SendKillInfo(false, player.Properties.Name, true, name1, Damage);
        PlayerKillInfoUpdate(player, Damage);
    }

    [RPC]
    private void ChatPM(string sender, string content, PhotonMessageInfo info) //TODO: Customize PMs message
    {
        Mod.Interface.Chat.ReceivePrivateMessage(info.sender,
            $"<color=#1068D4>PM</color><color=#108CD4>></color> <color=#{Mod.Interface.Chat.SystemColor}>{info.sender.HexName}: {content}</color>");
    }
}