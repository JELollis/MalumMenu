using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sentry.Internal.Extensions;

namespace MalumMenu;

[HarmonyPatch(typeof(EngineerRole), nameof(EngineerRole.FixedUpdate))]
public static class EngineerRole_FixedUpdate
{
    public static void Postfix(EngineerRole __instance){

        if(__instance.Player.AmOwner){

            MalumCheats.engineerCheats(__instance);
        }
    }
}

[HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.FixedUpdate))]
public static class ShapeshifterRole_FixedUpdate
{
    public static void Postfix(ShapeshifterRole __instance){

        try{
            if(__instance.Player.AmOwner){

                MalumCheats.shapeshifterCheats(__instance);
            }
        }catch{}
    }
}

[HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.Update))]
public static class ScientistRole_Update
{

    public static void Postfix(ScientistRole __instance){

        if(__instance.Player.AmOwner){

            MalumCheats.scientistCheats(__instance);
        }
    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorRole_IsValidTarget
{
    // Prefix patch of ImpostorRole.IsValidTarget to allow forbidden kill targets for killAnyone cheat
    // Allows killing ghosts (with seeGhosts), impostors, players in vents, etc...
    public static bool Prefix(ImpostorRole __instance, GameData.PlayerInfo target, ref bool __result){

        if (CheatToggles.killAnyone){
           __result = target != null && !target.Disconnected && (!target.IsDead || CheatToggles.seeGhosts) && target.PlayerId != __instance.Player.PlayerId && !(target.Object == null);
        
            return false;
        }

        return true;

    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.FindClosestTarget))]
public static class ImpostorRole_FindClosestTarget
{
    // Prefix patch of ImpostorRole.FindClosestTarget to allow for infinite kill reach
    public static bool Prefix(ImpostorRole __instance, ref PlayerControl __result){

        if (CheatToggles.killReach){

            List<PlayerControl> playerList = Utils.getPlayersSortedByDistance().Where(player => !player.IsNull() && __instance.IsValidTarget(player.Data) && player.Collider.enabled).ToList();

            __result = playerList[0];

            return false;

        }

        return true;
    }
}