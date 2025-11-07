using Verse;

namespace Progresseducationxmledition
{
    public class CompProperties_Bell : CompProperties
    {
        public SoundDef soundDef;
        public int ticksToRing = 60;

        public CompProperties_Bell()
        {
            compClass = typeof(CompBell);
        }
    }

    public class CompBell : ThingComp
    {
        public CompProperties_Bell Props => (CompProperties_Bell)props;
    }
}