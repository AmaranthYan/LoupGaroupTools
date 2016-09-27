using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

class SplashScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_Screens = new GameObject[0];
    [SerializeField]
    private float m_ScreenFadeInTime = 0.5f;
    [SerializeField]
    private float m_ScreenDisplayTime = 1.0f;

    private int m_ScreenIndex = 0;
    private float m_FadeAlpha = 1.0f;
    private Texture2D m_FadeTexture = null;

    void Awake()
    {
        m_FadeTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        m_FadeTexture.SetPixel(0, 0, Color.black);
    }
    
    void OnGUI()
    {
        if (Event.current.type != EventType.Repaint)
            return;
        if (m_FadeAlpha > 0.0f)
        {
            m_FadeAlpha -= Mathf.Clamp01(Time.deltaTime / m_ScreenFadeInTime);
            if (m_FadeAlpha < 0.0f)
            {
                m_FadeAlpha = 0.0f;
                Invoke("LoadNextScreen", m_ScreenDisplayTime);
            }
            else
            {
                GUI.color = new Color(0, 0, 0, m_FadeAlpha);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_FadeTexture);
            }
        }
    }

    private void LoadNextScreen()
    {
        if (m_ScreenIndex >= m_Screens.Count() - 1)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            m_Screens[m_ScreenIndex++].SetActive(false);
            m_FadeAlpha = 1.0f;
        }
    }
}
