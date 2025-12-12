using OutwardModsCommunicator.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModPackTemplate.Events
{
    public static class EventBusRegister
    {
        public static void RegisterEvents()
        {
            // Let's register our listener event to provide what kind of parameters we expect for others to see
            // This is not required. Just helper for others and good practice.
            EventBus.RegisterEvent(
                OutwardModPackTemplate.EVENTS_LISTENER_GUID, 
                EventBusSubscriber.Event_ExecuteMyCode, 
                "My code/method description", 
                ("callerGUID", typeof(string), "Optional variable for printing caller id.")
            );
        }
    }
}
