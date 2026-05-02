using System;
using BepInEx;
using GlobalSettings;
using UnityEngine.SceneManagement;
using HarmonyLib;
using Needleforge;
using TeamCherry.Localization;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.Reflection;

namespace TripleJumpTool;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.jasonfornian.triplejumptool")]
public partial class TripleJumpToolPlugin : BaseUnityPlugin
{
    public static Config config;
    
    private void Awake()
    {
        // Put your initialization logic here
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
        var harmopny = new Harmony(Id);
        harmopny.PatchAll();
        RegisterTrippleJump();
        Initialize();
    }
    private void RegisterTrippleJump()
    {
        Logger.LogInfo($"TripleJumpToolPlugin: aaaaa");
        string dllDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Logger.LogInfo($"TripleJumpToolPlugin: {dllDir}");
        string jsonPath = Path.Combine(dllDir, "TripleJump.json");
        Logger.LogInfo($"TripleJumpToolPlugin: {jsonPath}");
        if (File.Exists(jsonPath))
        {
            Logger.LogInfo($"TripleJumpToolPlugin: {jsonPath},has file");
            string json = File.ReadAllText(jsonPath);
            config = JsonConvert.DeserializeObject<Config>(json);
            Logger.LogInfo($"TripleJumpToolPlugin: {config.Name} {config.Desc}");
            var data = NeedleforgePlugin.AddTool(
            name: "TripleJump",
            type: ParseType(config.Type),
            displayName: new LocalisedString { Sheet = "", Key = "TripleJump_Name" },
            description: new LocalisedString { Sheet = "", Key = "TripleJump_Desc" },
            InventorySprite: FromFile(config.Icon)
        );
            data.UnlockedAtStart = false;
        }
    }
    private static ToolItemType ParseType(string typeStr)
    {
        if (System.Enum.TryParse<ToolItemType>(typeStr, true, out var result))
            return result;
        return ToolItemType.Yellow;
    }
    private static Sprite FromFile(string fileName)
    {
        string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string fullPath = Path.Combine(dir, fileName);
        if (!File.Exists(fullPath))
        {
            return null;
        }
        byte[] bytes = File.ReadAllBytes(fullPath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
    public static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"TripleJumpToolPlugin: OnSceneLoaded {scene.name},{config.MapID}");
        if (scene.name != config.MapID) return;
        if(PlayerData.instance == null)return;
        var field = typeof(PlayerData).GetField(config.state, BindingFlags.Instance | BindingFlags.Public);
        if(field == null)return;
        bool state = (bool)field.GetValue(PlayerData.instance);
        if (state && !ToolItemManager.GetToolByName("TripleJump").IsUnlocked)
        {
            Debug.Log($"TripleJumpToolPlugin: {config.Name} {config.Desc}");
            Vector3 pos = HeroController.instance.transform.position;
            var spawnPoint = new GameObject("TempSpawnPoint");
            spawnPoint.transform.position = new Vector3(config.Position[0], config.Position[1], 0f);
            var go = GameObject.Instantiate(Gameplay.CollectableItemPickupPrefab);
            var pickitem = go.GetComponent<CollectableItemPickup>();
            pickitem.SetPlayerDataBool("TripleJump");
            pickitem.SetItem(null, false);
            FlingUtils.SpawnAndFling(new FlingUtils.Config
            {
                Prefab = pickitem.gameObject,
                AmountMin = 1,
                AmountMax = 1,
                SpeedMin = 10f,
                SpeedMax = 10f,
                AngleMin = 90f,
                AngleMax = 90f
            }, spawnPoint.transform, Vector3.zero, null, -1f);
        }
    }
}
public class Config
{
    public string Name;
    public string Desc;
    public string Icon;
    public string Type;
    public string MapID;
    public float[] Position;
    public string state;
}

[HarmonyPatch(typeof(HeroController))]
public class TripleJump
{
    private static bool TripleJumping = false;
    [HarmonyPatch(nameof(HeroController.StartFloat))]
    [HarmonyPrefix]
    public static bool StartFloat_Prefix(HeroController __instance)
    {
        bool hasTripleJump = ToolItemManager.IsToolEquipped("TripleJump");
        if (!hasTripleJump) return true;
        if (__instance.doubleJumped && !TripleJumping)
        {
            TripleJumping = true;
            DoTripleJump(__instance);
            return false;
        }
        else if (TripleJumping)
        {
            __instance.CancelQueuedBounces();
            __instance.umbrellaFSM.SendEvent("FLOAT");
            return false;
        }
        else
        {
            return true;
        }

    }
    [HarmonyPatch(nameof(HeroController.DoDoubleJump))]
    [HarmonyPrefix]
    public static bool DoDoubleJump_Prefix(HeroController __instance)
    {
        TripleJumping = false;
        return true;
    }
    public static void DoTripleJump(HeroController __instance)
    {
        if (SlideSurface.IsInJumpGracePeriod)
        {
            return;
        }
        if (__instance.cState.inUpdraft && __instance.CanFloat(true))
        {
            __instance.fsm_brollyControl.SendEvent("FORCE UPDRAFT ENTER");
            return;
        }
        if (__instance.shuttleCockJumpSteps > 0)
        {
            return;
        }
        if (__instance.cState.dashing && __instance.dashingDown)
        {
            __instance.FinishedDashing(true);
        }
        if (__instance.cState.jumping)
        {
            __instance.Jump();
        }
        __instance.doubleJumpEffectPrefab.Spawn(__instance.transform, Vector3.zero);
        if (Gameplay.BrollySpikeTool.IsEquipped)
        {
            GameObject gameObject = Gameplay.BrollySpikeObject_dj.Spawn(__instance.transform);
            gameObject.transform.Translate(0f, 0f, -0.001f);
            gameObject.transform.Rotate(0f, 0f, -10f);
        }
        __instance.vibrationCtrl.PlayDoubleJump();
        if (__instance.audioSource && __instance.doubleJumpClip)
        {
            __instance.audioSource.PlayOneShot(__instance.doubleJumpClip, 1f);
        }
        Vector2 linearVelocity = __instance.rb2d.linearVelocity;
        if (linearVelocity.y < -__instance.MAX_FALL_VELOCITY_DJUMP)
        {
            __instance.rb2d.linearVelocity = new Vector2(linearVelocity.x, -__instance.MAX_FALL_VELOCITY_DJUMP);
        }
        __instance.ShuttleCockCancel();
        __instance.ResetLook();
        __instance.startWithDownSpikeBounceShort = false;
        __instance.cState.downSpikeBouncingShort = false;
        __instance.startWithDownSpikeBounce = false;
        __instance.cState.downSpikeBouncing = false;
        __instance.cState.jumping = false;
        __instance.cState.doubleJumping = true;
        __instance.cState.downSpikeRecovery = false;
        __instance.animCtrl.AllowDoubleJumpReEntry();
        if (__instance.jumped_steps < __instance.JUMP_STEPS_MIN)
        {
            __instance.jumped_steps = __instance.JUMP_STEPS_MIN;
        }
        __instance.doubleJump_steps = 0;
        __instance.doubleJumped = true;
        __instance.ResetHardLandingTimer();
        var onDoubleJumped = Traverse.Create(__instance)
                          .Field("OnDoubleJumped")
                          .GetValue<Action>();
        if (onDoubleJumped != null)
        {
            onDoubleJumped();
        }
    }
}
[HarmonyPatch(typeof(CollectableItemPickup))]
public class Pickup
{
    [HarmonyPatch(nameof(CollectableItemPickup.DoPickupAction))]
    [HarmonyPostfix]
    public static void DoPickupPostfix(CollectableItemPickup __instance, bool __result)
    {
        if (!__result) return;
        string playerDataBool = Traverse.Create(__instance).Field("playerDataBool").GetValue<string>();
        if (playerDataBool == "TripleJump")
        {
            ToolItemManager.GetToolByName(playerDataBool).Unlock();
        }

    }

}
[HarmonyPatch(typeof(LocalisedString))]
[HarmonyPatch(nameof(LocalisedString.ToString), typeof(bool))]

public static class LocalisedStringOverride
{
    [HarmonyPostfix]
    static void Postfix(LocalisedString __instance, ref string __result)
    {
        string current = __result;
        if (string.IsNullOrEmpty(current))
            return;

        const string prefix = "!!/";
        if (!current.StartsWith(prefix))
            return;

        string inner = current.Substring(prefix.Length, current.Length - prefix.Length - 2);
        int lastUnderscore = inner.LastIndexOf('_');
        if (lastUnderscore < 0) return;

        string toolName = inner.Substring(0, lastUnderscore);
        string suffix   = inner.Substring(lastUnderscore + 1);
            
            if(suffix == "Name")
               __result = TripleJumpToolPlugin.config.Name;
            else if(suffix == "Desc")
               __result = TripleJumpToolPlugin.config.Desc;

        else
        {
            Debug.LogWarning($"Unknown suffix {suffix} in localised string {current}");
        }
    }
}
