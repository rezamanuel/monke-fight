// using UnityEngine;
// using System;
// using System.Collections.Generic;
// namespace Monke.Infrastructure
// {
//     public class MatchPhaseMessageChannel<T> : IMessageChannel<MatchPhase>
//     {
//         private List<System.Action<MatchPhase>> _subscribers;
//         public bool IsDisposed { get; private set; }

//         public void Dispose()
//         {
//             IsDisposed = true;
//         }

//         public bool HasBufferedMessage => false;

//         public MatchPhase BufferedMessage => default;

//         public void Publish(MatchPhase message)
//         {
//             // Check if the object has been disposed
//             if (IsDisposed)
//             {
//                 Debug.LogError("MatchPhaseMessageChannel<" + typeof(MatchPhase).Name + "> this message channel has been disposed.");
//             }

//             // Example: Notify all subscribed handlers about the published message
//             // Assuming there's a list of subscribers stored in a field named `_subscribers`
//             foreach (var handler in _subscribers)
//             {
//                 handler(message);
//             }
//         }
//         public void Unsubscribe(System.Action<MatchPhase> handler)
//         {
//             // Check if the handler is null
//             if (handler == null)
//             {
//                 Debug.LogError("MatchPhaseMessageChannel<" + typeof(MatchPhase).Name + "> Handler cannot be null.");
//             }

//             // Remove the handler from the list of subscribers if it exists
//             _subscribers.Remove(handler);
//         }

//         public IDisposable Subscribe(System.Action<MatchPhase> handler)
//         {
//             // Check if the handler is null
//             if (handler == null)
//             {
//                 throw new ArgumentNullException(nameof(handler), "Handler cannot be null.");
//             }

//             // Add the handler to the list of subscribers
//             _subscribers.Add(handler);

//             // Return an IDisposable that removes the handler from the subscribers list when disposed
//             return new Unsubscriber<MatchPhase>(_subscribers, handler);
//         }

//         // Helper class to handle the unsubscription
//         private class Unsubscriber<MatchPhase> : IDisposable
//         {
//             private List<System.Action<MatchPhase>> _subscribers;
//             private System.Action<MatchPhase> _handler;

//             public Unsubscriber(List<System.Action<MatchPhase>> subscribers, System.Action<MatchPhase> handler)
//             {
//                 _subscribers = subscribers;
//                 _handler = handler;
//             }

//             public void Dispose()
//             {
//                 if (_handler != null && _subscribers.Contains(_handler))
//                 {
//                     _subscribers.Remove(_handler);
//                 }
//             }
//         }
//     }
// }