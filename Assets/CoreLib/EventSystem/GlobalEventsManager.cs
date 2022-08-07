using System;
using System.Collections;
using System.Collections.Generic;
using Utils;

namespace Core
{
    public class EventsManager<T> : SingletonSpawningMonoBehaviour<EventsManager<T>>
    {
        #region public interface

        public delegate void SubscribeListenerCallback(T type, object sender, object context);

        static public void Subscribe(T type, SubscribeListenerCallback callback)
        {
            Instance.Subscribe_Internal(type, callback);
        }

        static public void UnSubscribe(T type, SubscribeListenerCallback callback)
        {
            Instance.UnsuscribeToEventtype_Internal(type, callback);
        }

        static public void Send(T type, object sender, object context)
        {
            Instance.Send_Internal(type, sender, context, false);
        }

        static public void SendImmediate(T type, object sender, object context)
        {
            Instance.Send_Internal(type, sender, context, true);
        }

        #endregion

        private struct EventDetails
        {
            public T type;
            public object sender;
            public object context;
        };


        private void Subscribe_Internal(T type, SubscribeListenerCallback callback)
        {
            if (listeners.ContainsKey(type))
                listeners[type] += callback;
            else
                listeners.Add(type, callback);
        }

        private void UnsuscribeToEventtype_Internal(T type, SubscribeListenerCallback callback)
        {
            if (listeners.ContainsKey(type))
                listeners[type] -= callback;
        }

        private void Update () 
        {
            foreach ( EventDetails ed in eventsToBeFiredInNextFrame )
            {
                Send_Immediate ( ed );
            }
            eventsToBeFiredInNextFrame.Clear();
        }

        private void Send_Immediate ( EventDetails evnt )
        {
            SubscribeListenerCallback actionListener;
            listeners.TryGetValue(evnt.type, out actionListener);

            if(actionListener != null)
            {
                actionListener(evnt.type, evnt.sender, evnt.context);
                return;
            } 

            Core.QLogger.LogWarning (string.Format("EventsManager.Send found unhandled Event. [ Category: {0}   Sender: {1}  Context: {2} ]",
                                         evnt.type, (evnt.sender == null ? "null" : evnt.sender.ToString()), (evnt.context == null ? "null" : evnt.context.ToString())));
        }
        private void Send_Internal(T type, object sender, object context, bool immediate = false )
        {
            EventDetails ed = new EventDetails();
            ed.type = type;
            ed.sender = sender;
            ed.context = context;
 
            if ( immediate )    
                Send_Immediate ( ed );
            else 
                eventsToBeFiredInNextFrame.Add ( ed );
        }

        List <EventDetails> eventsToBeFiredInNextFrame = new List<EventDetails>();
        Dictionary<T, SubscribeListenerCallback> listeners = new Dictionary<T, SubscribeListenerCallback>();
    }
}
