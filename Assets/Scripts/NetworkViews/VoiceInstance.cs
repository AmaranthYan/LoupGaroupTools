using Photon;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LoupsGarous;

public class VoiceInstance : PunBehaviour
{
    private NetVoice m_NetVocie = null;
    private PhotonView m_PhotonView = null;
    private PhotonVoiceRecorder m_VoiceRecorder = null;
    private PhotonVoiceSpeaker m_VoiceSpeaker = null;

    VoiceInstanceModel m_VoiceInstance = new VoiceInstanceModel();

    void Awake()
    {
        m_NetVocie = FindObjectOfType<NetVoice>();
        m_PhotonView = GetComponent<PhotonView>();
        m_VoiceRecorder = GetComponent<PhotonVoiceRecorder>();
        m_VoiceSpeaker = GetComponent<PhotonVoiceSpeaker>();
    }

    void Start()
    {
        transform.SetParent(m_NetVocie.transform);
        m_VoiceInstance.Player = m_PhotonView.owner;
        m_VoiceInstance.IsLocal = m_VoiceInstance.Player.isLocal;
    }

    void OnDestroy()
    {        
        m_NetVocie.VoiceInstanceListView.DeleteItemFromList(m_VoiceInstance.Player.userId);
    }

    void Update()
    {
        FetchInGameStatus();
        UpdateVoiceInstanceDisplay();
    }

    private void FetchInGameStatus()
    {
        m_VoiceInstance.IsInGame = GameSessionService.IsInGame;
        if (GameSessionService.IsInGame)
        {
            PlayerIdentity identity = GameSessionService.GameSession.GetComponent<GameLogicBase>().PlayerIdentities.Values.FirstOrDefault(pi => m_VoiceInstance.Player.Equals(pi.Player));
            m_VoiceInstance.InGamePlayerNumber = identity != default(PlayerIdentity) ? identity.Number : -1;
        }
    }

    private void UpdateVoiceInstanceDisplay()
    {
        if (m_VoiceInstance.IsLocal)
        {
            if (m_VoiceRecorder.IsTransmitting)
            {
                if (!m_NetVocie.VoiceInstanceListView.AddItemToList(m_VoiceInstance.Player.userId, m_VoiceInstance))
                {
                    m_NetVocie.VoiceInstanceListView.UpdateItemInList(m_VoiceInstance.Player.userId, m_VoiceInstance);
                }
            }
            else
            {
                m_NetVocie.VoiceInstanceListView.DeleteItemFromList(m_VoiceInstance.Player.userId);
            }
        }
        else
        {
            if (m_VoiceSpeaker.IsPlaying)
            {
                if (!m_NetVocie.VoiceInstanceListView.AddItemToList(m_VoiceInstance.Player.userId, m_VoiceInstance))
                {
                    m_NetVocie.VoiceInstanceListView.UpdateItemInList(m_VoiceInstance.Player.userId, m_VoiceInstance);
                }
            }
            else
            {
                m_NetVocie.VoiceInstanceListView.DeleteItemFromList(m_VoiceInstance.Player.userId);
            }
        }
    }
}