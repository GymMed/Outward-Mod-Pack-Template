using OutwardModsCommunicator.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModPackTemplate.Events
{
    public static class EventBusPublisher
    {
        // Outward Game Settings mod has better examples.
        public const string Event_Name_From_Other_Mod = "GUID";

        //other mods listener uid
        public const string Other_Mod_Listener = "GUID";

        public static void SendYourMessage(object yourVariable)
        {
            var payload = new EventPayload
            {
                ["eventVariableName"] = yourVariable
            };
            EventBus.Publish(Other_Mod_Listener, Event_Name_From_Other_Mod, payload);
        }
    }
}
