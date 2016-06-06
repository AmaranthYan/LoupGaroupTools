using Photon;
using UnityEngine.Events;
using System;
using System.Collections;

public class UnityTypedEvent {
    [Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [Serializable]
    public class StringEvent : UnityEvent<string> { }

    [Serializable]
    public class PhotonRoomInfoArrayEvent : UnityEvent<RoomInfo[]> { }
}
