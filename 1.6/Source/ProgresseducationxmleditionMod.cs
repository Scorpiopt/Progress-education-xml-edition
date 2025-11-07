using HarmonyLib;
using Verse;

namespace Progresseducationxmledition
{
    public class ProgresseducationxmleditionMod : Mod
    {
        public ProgresseducationxmleditionMod(ModContentPack pack) : base(pack)
        {
            new Harmony("ProgresseducationxmleditionMod").PatchAll();
        }
    }
}