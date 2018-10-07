using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterHealth : MonoBehaviour
{
    public Slider m_Slider;                             // The slider to represent how much health the character currently has.
    private Image m_FillImage;
    private Color m_FullHealthColor = Color.green;        // The color the health bar will be when on full health.
    private Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    private CharacterManager _charManager;
    private int currentHealth;
    private int maxHealth;

    private void OnEnable()
    {
        m_FillImage = m_Slider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        _charManager = GetComponent<CharacterManager>();
        if (_charManager != null)
        {
            SetHealthUI();
        }

        m_FillImage.color = m_FullHealthColor;
    }

    void Update()
    {
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = _charManager.CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, ((_charManager.CurrentHealth*1.0f) / (_charManager.StartingHealth * 1.0f)));

    }
}
