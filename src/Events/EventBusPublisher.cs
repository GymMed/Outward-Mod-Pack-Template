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
        public const string Event_NameFromOtherMod = "GUID";

        //other mods listener uid
        public const string OtherMod_Listener = "GUID";

        public static void SendYourMessage(object yourVariable)
        {
            var payload = new EventPayload
            {
                ["eventVariableName"] = yourVariable
            };
            EventBus.Publish(OtherMod_Listener, Event_NameFromOtherMod, payload);
        }
    }
}
