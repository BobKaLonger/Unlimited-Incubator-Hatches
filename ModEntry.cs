namespace UnlimitedIncubatorHatches
{
    public class ModEntry : Mod
    {
        internal static Queue<AnimalHouse> PendingHatches = new();

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += (s, e) => PendingHatches.Clear();

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalHouse), "resetSharedState"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.ResetSharedState_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.addNewHatchedAnimal)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.AddNewHatchedAnimal_Postfix))
            );
        }
    }
}