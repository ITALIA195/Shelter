using System.Collections;
using Mod;
using Mod.Managers;
using Mod.Modules;
using Photon;
using UnityEngine;

public class TITAN_SETUP : Photon.MonoBehaviour
{
    public GameObject eye;
    private CostumeHair hair;
    private GameObject hair_go_ref;
    private int hairType;
    public bool haseye;
    private GameObject part_hair;
    public int skin;

    private void Awake()
    {
        this.hair_go_ref = new GameObject();
        this.eye.transform.parent = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
        this.hair_go_ref.transform.position = this.eye.transform.position + Vector3.up * 3.5f + transform.forward * 5.2f;
        this.hair_go_ref.transform.rotation = this.eye.transform.rotation;
        this.hair_go_ref.transform.RotateAround(this.eye.transform.position, transform.right, -20f);
        this.hair_go_ref.transform.localScale = new Vector3(210f, 210f, 210f);
        this.hair_go_ref.transform.parent = transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
    }

    [RPC]
    private IEnumerator LoadskinE(int hair, int eyeId, string hairlink)
    {
        bool unloadAssets = false;
        Destroy(this.part_hair);
        this.hair = CostumeHair.MaleHairs[hair];
        this.hairType = hair;
        if (this.hair.Texture != string.Empty)
        {
            GameObject hairs = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.Texture));
            hairs.transform.parent = this.hair_go_ref.transform.parent;
            hairs.transform.position = this.hair_go_ref.transform.position;
            hairs.transform.rotation = this.hair_go_ref.transform.rotation;
            hairs.transform.localScale = this.hair_go_ref.transform.localScale;
            hairs.renderer.material = CharacterMaterials.materials[this.hair.Texture];
            bool mipmap = FengGameManagerMKII.settings.UseMipmap;
            if (hairlink.EqualsIgnoreCase("transparent"))
                hairs.renderer.enabled = false;
            else if (Utility.IsValidImageUrl(hairlink))
            {
                if (!FengGameManagerMKII.linkHash[0].ContainsKey(hairlink))
                {
                    using (WWW www = new WWW(hairlink))
                    {
                        yield return www;
                        if (www.error != null)
                            yield break;
                        hairs.renderer.material.mainTexture = RCextensions.LoadImageRC(www, mipmap, 200000);
                    }
    
                    unloadAssets = true;
                    FengGameManagerMKII.linkHash[0].Add(hairlink, hairs.renderer.material);
                    hairs.renderer.material = (Material) FengGameManagerMKII.linkHash[0][hairlink];
                }
                else
                {
                    hairs.renderer.material = (Material) FengGameManagerMKII.linkHash[0][hairlink];
                }
            }
            this.part_hair = hairs;
        }
        
        if (eyeId >= 0)
            SetFacialTexture(eye, eyeId);
        if (unloadAssets)
            FengGameManagerMKII.instance.UnloadAssets();
    }

    private static void SetFacialTexture(GameObject go, int id)
    {
        if (id < 0) 
            return;
        
        float x = 0.125f * (int)(id / 8f);
        float y = -0.25f * (id % 4);
        go.renderer.material.mainTextureOffset = new Vector2(x, y);
    }

    public void SetHair()
    {
        int num;
        if (ModuleManager.Enabled(nameof(ModuleEnableSkins)) && (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer || photonView.isMine))
        {
            var titanSkin = FengGameManagerMKII.settings.TitanSkin;
            
            Color color;
            num = Random.Range(0, 9);
            if (num == 3)
                num = 9;

            int index;
            if (FengGameManagerMKII.settings.Randomize)
                index = Random.Range(0, 5);
            else
                index = 0;
            
            if (titanSkin.Type[index] >= 0)
                num = titanSkin.Type[index];

            string hairlink = titanSkin.Hairs[index];
            int eye = Random.Range(1, 8);
            if (this.haseye)
            {
                eye = 0;
            }
            bool valid = Utility.IsValidImageUrl(hairlink);
            switch (IN_GAME_MAIN_CAMERA.GameType)
            {
                case GameType.Multiplayer when photonView.isMine && valid:
                    photonView.RPC(Rpc.SetHairSkin, PhotonTargets.AllBuffered, num, eye, hairlink);
                    break;
                
                case GameType.Multiplayer when photonView.isMine:
                    color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
                    photonView.RPC(Rpc.SetHairColor, PhotonTargets.AllBuffered, num, eye, color.r, color.g, color.b);
                    break;
                
                case GameType.Singleplayer when valid:
                    StartCoroutine(this.LoadskinE(num, eye, hairlink));
                    break;
                
                case GameType.Singleplayer:
                    color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
                    this.SetHairPRC(num, eye, color.r, color.g, color.b);
                    break;
            }
        }
        else
        {
            num = Random.Range(0, CostumeHair.MaleHairs.Length);
            if (num == 3)
            {
                num = 9;
            }
            Destroy(this.part_hair);
            this.hairType = num;
            this.hair = CostumeHair.MaleHairs[num];
            if (this.hair.Texture == string.Empty)
            {
                this.hair = CostumeHair.MaleHairs[9];
                this.hairType = 9;
            }
            this.part_hair = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.Texture));
            this.part_hair.transform.parent = this.hair_go_ref.transform.parent;
            this.part_hair.transform.position = this.hair_go_ref.transform.position;
            this.part_hair.transform.rotation = this.hair_go_ref.transform.rotation;
            this.part_hair.transform.localScale = this.hair_go_ref.transform.localScale;
            this.part_hair.renderer.material = CharacterMaterials.materials[this.hair.Texture];
            this.part_hair.renderer.material.color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
            int id = Random.Range(1, 8);
            SetFacialTexture(this.eye, id);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
            {
                var color = part_hair.renderer.material.color;
                photonView.RPC(Rpc.SetHairColor, PhotonTargets.OthersBuffered, 
                    hairType, 
                    id, 
                    color.r, 
                    color.g, 
                    color.b);
            }
        }
    }

    [RPC]
    private void SetHairRPC(int type, int eye_type, float r, float g, float b) => SetHairPRC(type, eye_type, r, g, b);
    
    [RPC]
    private void SetHairPRC(int type, int eye_type, float r, float g, float b)
    {
        Destroy(this.part_hair);
        this.hair = CostumeHair.MaleHairs[type];
        this.hairType = type;
        if (this.hair.Texture != string.Empty)
        {
            GameObject obj2 = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.Texture));
            obj2.transform.parent = this.hair_go_ref.transform.parent;
            obj2.transform.position = this.hair_go_ref.transform.position;
            obj2.transform.rotation = this.hair_go_ref.transform.rotation;
            obj2.transform.localScale = this.hair_go_ref.transform.localScale;
            obj2.renderer.material = CharacterMaterials.materials[this.hair.Texture];
            obj2.renderer.material.color = new Color(r, g, b);
            this.part_hair = obj2;
        }
        SetFacialTexture(this.eye, eye_type);
    }

    [RPC]
    public void SetHairRPC2(int hair, int eye, string hairlink)
    {
        if (ModuleManager.Enabled(nameof(ModuleEnableSkins)))
        {
            StartCoroutine(this.LoadskinE(hair, eye, hairlink));
        }
    }

    public void setPunkHair()
    {
        Destroy(this.part_hair);
        this.hair = CostumeHair.MaleHairs[3];
        this.hairType = 3;
        GameObject obj2 = (GameObject) Instantiate(Resources.Load("Character/" + this.hair.Texture));
        obj2.transform.parent = this.hair_go_ref.transform.parent;
        obj2.transform.position = this.hair_go_ref.transform.position;
        obj2.transform.rotation = this.hair_go_ref.transform.rotation;
        obj2.transform.localScale = this.hair_go_ref.transform.localScale;
        obj2.renderer.material = CharacterMaterials.materials[this.hair.Texture];
        switch (Random.Range(1, 4))
        {
            case 1:
                obj2.renderer.material.color = FengColor.hairPunk1;
                break;

            case 2:
                obj2.renderer.material.color = FengColor.hairPunk2;
                break;

            case 3:
                obj2.renderer.material.color = FengColor.hairPunk3;
                break;
        }
        this.part_hair = obj2;
        SetFacialTexture(this.eye, 0);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && photonView.isMine)
        {
            photonView.RPC(Rpc.SetHairColor, PhotonTargets.OthersBuffered, this.hairType, 0, this.part_hair.renderer.material.color.r, this.part_hair.renderer.material.color.g, this.part_hair.renderer.material.color.b);
        }
    }

    public void setVar(int skin, bool haseye)
    {
        this.skin = skin;
        this.haseye = haseye;
    }
}