using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace events
{
    public class EventBus : MonoBehaviour
    {
        private readonly Dictionary<Type, List<Action<BasicEvent>>> subscribers = new Dictionary<Type, List<Action<BasicEvent>>>(50);
        private static EventBus instance;

        //For debug/profile
        private readonly Dictionary<Type, float> publishTimes = new Dictionary<Type, float>(50);
        
        private void Awake()
        {
            //Requires initialization before all other objects on scene
            instance = this;
        }

        private static EventBus GetInstance()
        {
            return instance;
        }

        public static void Subscribe(Type eventType, Action<BasicEvent> handler)
        {
            var bus = GetInstance();
            if (!bus)
            {
                Debug.LogWarning($"EventBus was not yet instantiated, failed to subscribe to type {eventType}");
                return;
            }
            
            if (!bus.subscribers.ContainsKey(eventType))
            {
                bus.subscribers.Add(eventType, new List<Action<BasicEvent>>(50) { handler });
            }
            else if (!bus.subscribers[eventType].Contains(handler))
            {
                bus.subscribers[eventType].Add(handler);
            }
        }

        public static void Unsubscribe(Type eventType, Action<BasicEvent> handler)
        {
            var bus = GetInstance();
            if (!bus)
            {
                //Debug.LogWarning($"EventBus was not yet instantiated, failed to unsubscribe for type {eventType}");
                return;
            }

            if (bus.subscribers.ContainsKey(eventType))
            {
                bus.subscribers[eventType].Remove(handler);
            }
        }

        public static void Publish(BasicEvent e)
        {
            var bus = GetInstance();
            if (!bus)
            {
                Debug.LogWarning($"EventBus was not yet instantiated, failed to publish {e.GetType()}");
                return;
            }
            
            var type = e.GetType();
            if (!bus.subscribers.ContainsKey(type))
            {
                return;
            }

//Debug/profile code
//            var startTime = Time.time;
            
            var subscribersByType = bus.subscribers[type];
            for (var i = 0; i < subscribersByType.Count; i++)
            {
                subscribersByType[i].Invoke(e);
            }

//Debug/profile code
/*
            var timeSpent = Time.time - startTime;
            if (!bus.publishTimes.ContainsKey(type))
            {
                bus.publishTimes.Add(type, timeSpent);
            }
            else
            {
                bus.publishTimes[type] = Mathf.Max(bus.publishTimes[type], timeSpent);
            }
*/
        }

        //Debug/profile code
        public void DumpProfileData()
        {
            Debug.Log($"[{publishTimes.Count}] events have subscribers. Their publish times:");
            Debug.Log($"{string.Join(",\n", publishTimes.Select(entry => $"{entry.Key.ToString()}={entry.Value:0.000}"))}");
        }
    }
}