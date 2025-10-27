using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SideLoader;
using OutwardModsCommunicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using OutwardModsCommunicator.EventBus;

// RENAME 'OutwardModPackTemplate' TO SOMETHING ELSE
namespace OutwardModPackTemplate
{
    [BepInPlugin(GUID, NAME, VERSION)]
    // if other mod pack is overwritting configs values you would add dependency to load your mod after it, to overwrite.
    // soft dependency means that your mod can work without it and should be loaded even if dependency is missing
    // you can do the same through manifest.json(look into thunderstore documentation) and just delete this line if you like
    [BepInDependency(OutwardModsCommunicator.OMC.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class OutwardModPackTemplate : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymodpack".
        public const string GUID = "myname.mymodpack";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "My Mod Pack";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "0.0.1";

        // Choose prefix for log messages for quicker search and readablity
        public static string prefix = "[MyModPack]";

        // Will be used as id for accepting events from other mods 
        public const string EVENTS_LISTENER_GUID = GUID + "_*";

        internal static ManualLogSource Log;

        // If you need settings, define them like so:
        //public static ConfigEntry<bool> ExampleConfig;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            // You can find BepInEx logs in directory "BepInEx\LogOutput.log"
            Log = this.Logger;
            LogMessage($"Hello world from {NAME} {VERSION}!");

            // Any config settings you define should be set up like this:
            //ExampleConfig = Config.Bind("ExampleCategory", "ExampleSetting", false, "This is an example setting.");

            // This is why dependency would be needed
            // Overwrite runs post/after 'ResourcesPrefabManager' class 'Load' method
            // You can delete this if you only want to use mods communication and don't need config overwrites
            OutwardModsCommunicator.OMC.xmlFilePath = Path.Combine(GetProjectLocation(), "MyModsOverrides.xml");

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();

            // Listen to other mod event and execute function.
            // If you don't want to create function and listen to other mod you can delete this and OnTryEnchant method.
            EventBus.Subscribe("gymmed.outward_game_settings", "EnchantmentMenu@TryEnchant", OnTryEnchant);

            // Let's register our listener event to provide what kind of parameters we expect for others to see
            // This is not required. Just helper for others and good practice.
            EventBus.RegisterEvent(EVENTS_LISTENER_GUID, "ExecuteMyCode", ("callerGUID", typeof(string), "Optional variable for printing caller id."));

            // You can allow multiple mods to publish events and listen for all of them if they do
            // People will be able to view it through EventBusDataPresenter
            // I would recommend to name it GUID + "_*" instead of "*" so they know they publishing just for you
            EventBus.Subscribe(EVENTS_LISTENER_GUID, "ExecuteMyCode", MyExecutingFunction);
        }

        private void MyExecutingFunction(EventPayload payload)
        {
            if (payload == null) return;

            // try to retrieve passed event data, don't forget to check if retrieve didn't fail
            string modId = payload.Get<string>("callerGUID", null);

            // check if passed variable is not null or empty string ""
            if(!string.IsNullOrEmpty(modId))
                LogSL($"{modId} successfully passed callerGUID!");

            // log passed payload
            EventBusDataPresenter.LogPayload(payload);
            LogSL($"{GUID} caught and executed published event!");
        }

        private void OnTryEnchant(EventPayload payload)
        {
            if (payload == null) return;

            // try to retrieve passed event data
            EnchantmentMenu menu = payload.Get<EnchantmentMenu>("menu", null);

            // if event data is null log and stop execution
            if (menu == null)
            {
                LogSL("Mod gymmed.outward_game_settings event EnchantmentMenu@TryEnchant returned null for EnchantmentMenu");
                // log received payload for errors inspection
                EventBusDataPresenter.LogPayload(payload);
                return;
            }

            // Lets log success
            LogSL($"{GUID} successfully communicated with gymmed.outward_game_settings mod and passed menu!");

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
            LogSL($"{enchantment.Name} tried to be applied!");
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
        }

        //  Log message with prefix
        public static void LogMessage(string message)
        {
            Log.LogMessage($"{OutwardModPackTemplate.prefix} {message}");
        }

        // Log message through side loader, helps to see it
        // if you are using UnityExplorer and want to see live logs
        public static void LogSL(string message)
        {
            SL.Log($"{OutwardModPackTemplate.prefix} {message}");
        }

        // Gets mod dll location
        public static string GetProjectLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        // This is an example of a Harmony patch.
        // If you're not using this, you should delete it.
        [HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        public class ResourcesPrefabManager_Load
        {
            static void Postfix(ResourcesPrefabManager __instance)
            {
                // use Debug build for things you don't want to release
                #if DEBUG
                // provide class and method separated by @ for easier live debugging
                LogSL("ResourcesPrefabManager@Load called!");
                #endif

                // Log all registered events
                EventBusDataPresenter.LogRegisteredEvents();
                // Log all subsribers
                EventBusDataPresenter.LogAllModsSubsribers();
            }
        }
    }
}
