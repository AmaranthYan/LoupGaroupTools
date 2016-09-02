using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using LoupsGarous;

public class UnityTypedEvent
{
    [Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [Serializable]
    public class IntEvent : UnityEvent<int> { }
    [Serializable]
    public class StringEvent : UnityEvent<string> { }
    [Serializable]
    public class StringListEvent : UnityEvent<List<string>> { }
    [Serializable]
    public class HashtableEvent : UnityEvent<Hashtable> { }
    [Serializable]
    public class OrderedDictionaryEvent : UnityEvent<OrderedDictionary> { }
    [Serializable]
    public class SpriteEvent : UnityEvent<Sprite> { }
    [Serializable]
    public class RectTransformEvent : UnityEvent<RectTransform> { }
    [Serializable]
    public class KeyCodeEvent : UnityEvent<KeyCode> { }
    
    [Serializable]
    public class PlayerIdentityEvent : UnityEvent<PlayerIdentity> { }

    [Serializable]
    public class StringAndPlayerIdentityEvent : UnityEvent<string, PlayerIdentity> { }

    //[Serializable]
    //public class ScrollListItemViewEvent : UnityEvent<ScrollListItemView> { }
    //[Serializable]
    //public class ScrollListItemViewListEvent : UnityEvent<List<ScrollListItemView>> { }
    
    [Serializable]
    public class PhotonRoomInfoArrayEvent : UnityEvent<RoomInfo[]> { }
    [Serializable]
    public class PhotonPhotonPlayerEvent : UnityEvent<PhotonPlayer> { }
}
