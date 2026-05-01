using StardewValley;
using StardewValley.GameData.FarmAnimals;
using StardewValley.TokenizableStrings;
using StardewValley.Events;
using Object = StardewValley.Object;

namespace UnlimitedIncubatorHatches
{
    internal class Patches
    {
        public static void ResetSharedState_Postfix(AnimalHouse __instance)
        {
            int found = 0;
            foreach (Object o in __instance.objects.Values)
            {
                if (!o.bigCraftable) continue;
                var machineData = o.GetMachineData();
                if (machineData == null || !machineData.IsIncubator
                    || o.heldObject.Value == null || o.MinutesUntilReady > 0) continue;
                
                found++;
                if (found == 1) continue;
                ModEntry.PendingHatches.Enqueue(__instance);
            }
        }

        public static void AddNewHatchedAnimal_Postfix()
        {
            if (ModEntry.PendingHatches.Count == 0) return;

            AnimalHouse house = ModEntry.PendingHatches.Dequeue();

            if (house.isFull())
            {
                if (!house.hasShownIncubatorBuildingFullMessage)
                {
                    house.hasShownIncubatorBuildingFullMessage = true;
                    Game1.showGlobalMessage(
                        Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_HouseFull"));
                }
                ModEntry.PendingHatches.Clear();
                return;
            }

            string whatHatched = "??";
            foreach (Object o in house.objects.Values)
            {
                if (!o.bigCraftable) continue;
                var machineData = o.GetMachineData();
                if (machineData == null || !machineData.IsIncubator
                    || o.heldObject.Value == null || o.MinutesUntilReady > 0) continue;
                
                FarmAnimalData hatchedAnimal = FarmAnimal.GetAnimalDataFromEgg(o.heldObject.Value, house);
                if (hatchedAnimal?.BirthText != null)
                    whatHatched = TokenParser.ParseText(hatchedAnimal.BirthText);
                break;
            }

            house.currentEvent = new EventArgs(
                "none/-1000 -1000/farmer 2 9 0/pause 250/message \""
                + whatHatched
                + "\"/pause 500/animalNaming/pause 500/end"
            );
        }
    }
}