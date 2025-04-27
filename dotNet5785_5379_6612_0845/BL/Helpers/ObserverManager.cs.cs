//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BL.Helpers
//{
//    internal class ObserverManager
//    {
//    }
//}
namespace Helpers;

/// &lt;summary&gt;
/// This class is a helper class allowing to manage observers for different logical entities
/// in the Business Logic (BL) layer.
/// It offers infrastructure to support observers as follows:
/// &lt;list type="bullet"&gt;
/// &lt;item&gt;an event delegate for list observers - wherever there may be a change in the
/// presentation of the list of entities&lt;/item&gt;
/// &lt;item&gt;a hash table of delegates for individual entity observers - indexed by appropriate entity ID&lt;/item&gt;
/// &lt;/list&gt;
/// &lt;/summary&gt;
class ObserverManager //stage 5
{
    /// &lt;summary&gt;
    /// event delegate for list observers - it's called whenever there may be need to update the presentation
    /// of the list of entities
    /// &lt;/summary&gt;
    private event Action? _listObservers;
    /// &lt;summary&gt;
    /// Hash table (Dictionary) of individual entity delegates.&lt;br/&gt;
    /// The index (key) is the ID of an entity.&lt;br/&gt;
    /// If there are no observers for a specific entity instance - there will not be entry in the hash
    /// table for it, thus providing memory effective storage for these observers
    /// &lt;/summary&gt;
    private readonly Dictionary&lt;int, Action?&gt; _specificObservers = new ();

    /// &lt;summary&gt;
    /// Add an observer on change in list of entities that may effect the list presentation
    /// &lt;/summary&gt;
    /// &lt;param name="observer"&gt;Observer method (usually from Presentation Layer) to be added&lt;/param&gt;
    internal void AddListObserver(Action observer) =&gt; _listObservers += observer;
    /// &lt;summary&gt;
    /// Remove an observer on change in list of entities that may effect the list presentation
    /// &lt;/summary&gt;
    /// &lt;param name="observer"&gt;Observer method (usually from Presentation Layer) to be removed&lt;/param&gt;
    internal void RemoveListObserver(Action observer) =&gt; _listObservers -= observer;

    /// &lt;summary&gt;
    /// Add an observer on change in an instance of entity that may effect the entity instance presentation
    /// &lt;/summary&gt;
    /// &lt;param name="id"&gt;the ID value for the entity instance to be observed&lt;/param&gt;
    /// &lt;param name="observer"&gt;Observer method (usually from Presentation Layer) to be added&lt;/param&gt;
    internal void AddObserver(int id, Action observer)
    {
        if (_specificObservers.ContainsKey(id)) // if there are already observers for the ID
            _specificObservers[id] += observer; // add the given observer
        else // there is the first observer for the ID
            _specificObservers[id] = observer; // create hash table entry for the ID with the given observer
    }

    /// &lt;summary&gt;
    /// Remove an observer on change in an instance of entity that may effect the entity instance presentation
    /// &lt;/summary&gt;
    /// &lt;param name="id"&gt;the ID value for the observed entity instance&lt;/param&gt;
    /// &lt;param name="observer"&gt;Observer method (usually from Presentation Layer) to be removed&lt;/param&gt;
    internal void RemoveObserver(int id, Action observer)
    {
        // First, lets check that there are any observers for the ID
        if (_specificObservers.ContainsKey(id) & amp; &amp; _specificObservers[id] is not null)
        {
            Action? specificObserver = _specificObservers[id]; // Reference to the delegate element for the ID
            specificObserver -= observer; // Remove the given observer from the delegate
            if (specificObserver?.GetInvocationList().Length == 0) // if there are no more observers for the ID
                _specificObservers.Remove(id); // then remove the hash table entry for the ID
        }
    }

    /// &lt;summary&gt;
    /// Notify all the observers that there is a change for one or more entities in the list
    /// that may affect the whole list presentation
    /// &lt;/summary&gt;
    internal void NotifyListUpdated() =&gt; _listObservers?.Invoke();

    /// &lt;summary&gt;
    /// Notify observers of an e specific entity  that there was some change in the entity
    /// &lt;/summary&gt;
    /// &lt;param name="id"&gt;a specific entity ID&lt;/param&gt;
    internal void NotifyItemUpdated(int id)
    {
        if (_specificObservers.ContainsKey(id))
            _specificObservers[id]?.Invoke();
    }

}

