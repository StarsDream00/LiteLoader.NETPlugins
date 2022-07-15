using System;

using LLNET.Event.Effective;
using LLNET.Event.Effective.NativeEvents;

using ExampleEffectEvent;

namespace ExamplePlugin.Examples
{
    internal class ExampleEffectEvent : IExample
    {
        public void Execute()
        {
            //Register event
            EventManager.RegisterEvent<ExampleCustomEvent>();
            EventManager.RegisterEvent<ExampleCancellableEvent>();
            //Register listener
            EventManager.RegisterListener<ExampleListener>();
            EventManager.RegisterListener<ExampleNativeListner>();
        }
    }
}

namespace ExampleEffectEvent
{
    //Create a new custom Event
    public class ExampleCustomEvent : EventBase
    {
        public ExampleCustomEvent(int m1, string m2)
        {
            Member_1 = m1;
            Member_2 = m2;
        }

        public int Member_1;
        public string Member_2;
        //...More member

        //Finally, use 'EventManager.RegisterEvent<Tevent>' to register this event.


        //And call this event by your own functions.
        public static void RaiseEvent()
        {
            var ev = new ExampleCustomEvent(233, "QAQ");
            EventManager.CallEvent(ev);
        }
    }


    //Create a new cancellable event
    //Based on ICancellable interface
    public class ExampleCancellableEvent : EventBase, ICancellable
    {
        public ExampleCancellableEvent()
        {
        }

        public static void RaiseEvent()
        {
            var ev = new ExampleCancellableEvent();

            var ret = EventManager.CallEvent(ev);

            switch (ret)
            {
                case EventCode.SUCCESS:
                    {
                        Console.WriteLine("Call this event successfully.");
                        break;
                    }
                case EventCode.UNREGISTERED:
                    {
                        Console.WriteLine("This event are unregistered.");
                        break;
                    }
                case EventCode.CATCHED_EXCEPTIONS:
                    {
                        Console.WriteLine("Some exceptions were thrown when calling this event.");

                        var exceptions = EventManager.GetLastCallingExceptions();

                        foreach (var exception in exceptions)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                    break;
            }
        }
    }


    //Create a new listener
    public class ExampleListener : IEventListener
    {
        //Default event handler
        [EventHandler]
        public static void ExampleCustomEvent_Listener1(ExampleCustomEvent ev)
        {
            Console.WriteLine(ev.Member_1 + ev.Member_2);
        }

        //Define the priority of the handler.

        //The order that calling handlers is from the first to the last as:
        //EventPriority.LOWEST
        //EventPriority.LOW
        //EventPriority.NORMAL
        //EventPriority.HIGH
        //EventPriority.HIGHEST
        //EventPriority.MONITOR
        [EventHandler(Priority = EventPriority.MONITOR)]
        public static void ExampleCustomEvent_Listener2(ExampleCustomEvent ev)
        {
            Console.WriteLine(ev.Member_1 + ev.Member_2);
        }

        //If ignoreCancelled is true and the event is cancelled, the method is not called. Otherwise, the method is always called.
        //Just like nukkit EventHandler.
        //Default value is false.
        [EventHandler(IgnoreCancelled = true)]
        public static void ExampleCustomEvent_Listener3(ExampleCustomEvent ev)
        {
            Console.WriteLine(ev.Member_1 + ev.Member_2);
        }

        //If you explicit declare this property as true, you can use ref parameter.
        [EventHandler(CanModifyEvent = true)]
        public static void ExampleCustomEvent_Listener4(ref ExampleCustomEvent ev)
        {
            Console.WriteLine(ev.Member_1 + ev.Member_2);
        }



        public ExampleListener()
        {
        }

        // And you can use Instance handler if this class has default constructor,
        // but it is a little inefficiency (by reflection).
        int member1;
        int member2;
        [EventHandler]
        public void ExampleCustomEvent_Listener5(ref ExampleCustomEvent ev)
        {
            Console.WriteLine(ev.Member_1 + ev.Member_2);
            Console.WriteLine(member1 + member2);
        }
    }


    public class ExampleNativeListner: IEventListener
    {
        //Native enent listener's parameter is ref only.
        [EventHandler]
        public static void Listener(in PlayerJumpEvent ev)
        {
            //...
        }

        [EventHandler(CanModifyEvent = true)]
        public unsafe static void Listener1(ref PlayerJumpEvent ev)
        {
            MC.Player player = new(null);
            //...
        }
    }
}
