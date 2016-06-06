using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomListView : MonoBehaviour {
    [SerializeField]
    private ToggleGroup m_ToggleGroup = null;
    public ToggleGroup ToggleGroup { get { return m_ToggleGroup; } }
    [SerializeField]
    private RoomView m_RoomPrefab = null;

    private Dictionary<string, RoomView> m_CurrentRoomList = new Dictionary<string, RoomView>();
    private string m_SelectedRoom = null;

    public UnityTypedEvent.StringEvent onSelectedRoomChange = new UnityTypedEvent.StringEvent();

    public void UpdateRoomList(RoomInfo[] roomInfo)
    {
        Dictionary<string, RoomView> newRoomList = new Dictionary<string, RoomView>();
        foreach (RoomInfo rI in roomInfo)
        {
            if (m_CurrentRoomList.ContainsKey(rI.name))
            {
                newRoomList[rI.name] = m_CurrentRoomList[rI.name];
                newRoomList[rI.name].UpdateRoomInfo(rI);
                m_CurrentRoomList.Remove(rI.name);
            }
            else
            {
                RoomView roomInstance = GameObject.Instantiate<RoomView>(m_RoomPrefab);
                roomInstance.transform.SetParent(this.transform);
                roomInstance.transform.localPosition = Vector3.zero;
                roomInstance.transform.localScale = Vector3.one;
                roomInstance.InitRoomView(this);
                roomInstance.UpdateRoomInfo(rI);
                newRoomList[rI.name] = roomInstance;
            }
        }

        foreach (RoomView rV in m_CurrentRoomList.Values)
        {
            GameObject.Destroy(rV.gameObject);
        }
        m_CurrentRoomList.Clear();

        m_CurrentRoomList = newRoomList;
    }

	public void SelectRoomInList(string name, bool isSelected)
    {
        if (isSelected)
        {
            m_SelectedRoom = name;
        }
        else
        {
            if (!string.IsNullOrEmpty(m_SelectedRoom) && m_SelectedRoom.Equals(name))
            {
                m_SelectedRoom = null;
            }
        }
        onSelectedRoomChange.Invoke(m_SelectedRoom);
    }
}
