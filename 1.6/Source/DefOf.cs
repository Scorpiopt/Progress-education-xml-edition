using RimWorld;
using Verse;

namespace Progresseducationxmledition
{
    [DefOf]
    public static class PEDefOf
    {
        static PEDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PEDefOf));
        }

        public static ThingDef SE_RoughSchoolDesk;
        
        public static ThingDef SE_SpacerSchoolDesk;

        public static ThingDef PE_TribalBell;

        public static ThingDef PE_SchoolBell;

        public static ThingDef PE_ElectricSchoolBell;
    }
}