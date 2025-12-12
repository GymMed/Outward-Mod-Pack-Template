using OutwardModsCommunicator.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModPackTemplate.Events
{
    public static class EventBusSubscriber
    {
        public const string Event_SerializeObject = "SerializeObject";

        public static void AddSubscribers()
        {
            // Listen to other mod event and execute function.
            // If you don't want to create function and listen to other mod you can delete this and OnTryEnchant method.
            EventBus.Subscribe("gymmed.outward_game_settings", "EnchantmentMenu@TryEnchant", OnTryEnchant);

            // You can allow multiple mods to publish events and listen for all of them if they do
            // People will be able to view it through EventBusDataPresenter
            // I would recommend to name it GUID + "_*" instead of "*" so they know they publishing just for you
            EventBus.Subscribe(OutwardModPackTemplate.EVENTS_LISTENER_GUID, "ExecuteMyCode", MyExecutingFunction);
        }

        private static void MyExecutingFunction(EventPayload payload)
        {
            if (payload == null) return;

            // try to retrieve passed event data, don't forget to check if retrieve didn't fail
            string modId = payload.Get<string>("callerGUID", null);

            // check if passed variable is not null or empty string ""
            if(!string.IsNullOrEmpty(modId))
                OutwardModPackTemplate.LogSL($"{modId} successfully passed callerGUID!");

            // log passed payload
            EventBusDataPresenter.LogPayload(payload);
            OutwardModPackTemplate.LogSL($"{OutwardModPackTemplate.GUID} caught and executed published event!");
        }

        private static void OnTryEnchant(EventPayload payload)
        {
            if (payload == null) return;

            // try to retrieve passed event data
            EnchantmentMenu menu = payload.Get<EnchantmentMenu>("menu", null);

            // if event data is null log and stop execution
            if (menu == null)
            {
                OutwardModPackTemplate.LogSL("Mod gymmed.outward_game_settings event EnchantmentMenu@TryEnchant returned null for EnchantmentMenu");
                // log received payload for errors inspection
                EventBusDataPresenter.LogPayload(payload);
                return;
            }

            // Lets log success
            OutwardModPackTemplate.LogSL($"{OutwardModPackTemplate.GUID} successfully communicated with gymmed.outward_game_settings mod and passed menu!");

            // Get currently possible enchantmentID from EnchantmentMenu
            int enchantmentID = menu.GetEnchantmentID();

            // if enchatment couldn't be determined stop execution
            if (enchantmentID == -1)
                return;

            // Retrieve enchantment
            Enchantment enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(enchantmentID);

            // Assume that there is possibility that retrieve failed. Stop Code Execution
            if (enchantment == null)
                return;

            // Log results and do what you want with it...
            OutwardModPackTemplate.LogSL($"{enchantment.Name} tried to be applied!");
        }
    }
}
