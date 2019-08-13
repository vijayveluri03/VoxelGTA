using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

namespace Core.EventSystem
{

    public class GlobalEventsManager : SingletonSpawningMonoBehaviour<GlobalEventsManager>
    {
        #region public interface

        public delegate void SuscribeListenerCallback(string action, object sender, object context);

        static public void SuscribeToEventCategory(string category, SuscribeListenerCallback callback)
        {
            Instance.SuscribeToEventCategory_Internal(category, callback);
        }

        static public void UnsuscribeToEventCategory(string category, SuscribeListenerCallback callback)
        {
            Instance.UnsuscribeToEventCategory_Internal(category, callback);
        }

        static public void Send(string category, string action, object sender, object context, bool immediate = false )
        {
            Instance.Send_Internal(category, action, sender, context, immediate );
        }

        #endregion

        private struct EventDetails
        {
            public string category;
            public string action;
            public object sender;
            public object context;
        };


        private void SuscribeToEventCategory_Internal(string category, SuscribeListenerCallback callback)
        {
            if (listeners.ContainsKey(category))
                listeners[category] += callback;
            else
                listeners.Add(category, callback);
            
            //SuscribeListenerCallback actionListener;
            //listeners.TryGetValue(category, out actionListener);

            //if (actionListener == null)
            //{
            //    listeners.Add(category, callback);
            //    return;
            //}

            //actionListener += callback;
        }

        private void UnsuscribeToEventCategory_Internal(string category, SuscribeListenerCallback callback)
        {
            if (listeners.ContainsKey(category))
                listeners[category] -= callback;
            

            //SuscribeListenerCallback actionListener;
            //listeners.TryGetValue(category, out actionListener);

            //if (actionListener != null)
            //{
            //    actionListener -= callback;
            //}
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
            SuscribeListenerCallback actionListener;
            listeners.TryGetValue(evnt.category, out actionListener);

            if(actionListener != null)
            {
                actionListener(evnt.action, evnt.sender, evnt.context);
                return;
            } 

            if ( QLogger.CanLogInfo ) QLogger.LogInfo (string.Format("GlobalEventsManager.Send found unhandled Event. [ Category: {0}   Action: {1}  Sender: {2}  Context: {3} ]",
                                         evnt.category, evnt.action, (evnt.sender == null ? "null" : evnt.sender.ToString()), (evnt.context == null ? "null" : evnt.context.ToString())));
        }
        private void Send_Internal(string category, string action, object sender, object context, bool immediate = false )
        {
            EventDetails ed = new EventDetails();
            ed.category = category;
            ed.action = action; 
            ed.sender = sender;
            ed.context = context;
 
            if ( immediate )    
                Send_Immediate ( ed );
            else 
                eventsToBeFiredInNextFrame.Add ( ed );
        }

        List <EventDetails> eventsToBeFiredInNextFrame = new List<EventDetails>();
        Dictionary<string, SuscribeListenerCallback> listeners = new Dictionary<string, SuscribeListenerCallback>();
    }

}
