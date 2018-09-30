using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterHealth : MonoBehaviour
{
    public Slider m_Slider;                             // The slider to represent how much health the character currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    private CharacterManager _charManager;
    private int currentHealth;
    private int maxHealth;

    private void OnEnable()
    {
        _charManager = GetComponent<CharacterManager>();
        if (_charManager != null)
        {
            SetHealthUI();
        }
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
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, _charManager.CurrentHealth / _charManager.StartingHealth);
    }
}
