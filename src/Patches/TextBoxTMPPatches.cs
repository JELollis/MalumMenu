using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu
{
    // Postfix patch to allow copying chat text via Ctrl+C
    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
    public static class TextBoxTMP_Update
    {
        public static void Postfix(TextBoxTMP __instance)
        {
            // Only execute if chat jailbreak is enabled and the text box has focus
            if (!CheatToggles.chatJailbreak || !__instance.hasFocus) return;

            // Handle Ctrl+C keypress to copy text to clipboard
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
            {
                ClipboardHelper.PutClipboardString(__instance.text);
            }
        }
    }

    // Prefix patch to allow almost all characters during chat input
    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
    public static class TextBoxTMP_IsCharAllowed
    {
        public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
        {
            if (CheatToggles.chatJailbreak)
            {
                // Block only *actual* control characters and obvious UI-breakers
                HashSet<char> blockedSymbols = new() { '\b', '\r' /* no < or > */ };

                if (blockedSymbols.Contains(i))
                {
                    Debug.Log($"[MalumMenu] Blocked control character: {i} (Unicode: {(int)i:X4})");
                    __result = false;
                    return false;
                }

                __result = true;
                return false; // Allow all others, including layout-wonky characters
            }

            return true;
        }
    }


}
