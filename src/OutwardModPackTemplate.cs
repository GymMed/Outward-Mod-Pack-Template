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
using OutwardModPackTemplate.Events;

// RENAME 'OutwardModPackTemplate' TO SOMETHING ELSE
namespace OutwardModPackTemplate
{
    [BepInPlugin(GUID, NAME, VERSION)]
    // if other mod pack is overwritting configs values you would add dependency to load your mod after it, to overwrite.
    // soft dependency means that your mod can work without it and should be loaded even if dependency is missing
    // don't forget to add dependencies to manifest.json(look into thunderstore documentation) for others to see them.
    [BepInDependency(SideLoader.SL.GUID, BepInDependency.DependencyFlags.SoftDependency)]
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

            // These EventBus classes are static because they act as simple helper modules.

            // EventBusRegister  → defines and exposes events so other mods can use them
            // EventBusSubscriber → adds listeners that handle events
            // EventBusPublisher → provides methods to fire (publish) events

            // Initialize the EventBus system:
            // 1. Register all events that other mods might access
            EventBusRegister.RegisterEvents();

            // 2. Add your event listeners (methods that react when events are triggered)
            EventBusSubscriber.AddSubscribers();

            // 3. Publishers are called only where the game logic needs them.
            //    For real-world examples, see the Outward Game Settings mod.
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
