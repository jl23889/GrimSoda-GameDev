using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterStamina : MonoBehaviour
{
    private GameObject _playerUI;
    private Slider m_Slider;                              // The slider to represent how much Stamina the character currently has.
    private Image m_FillImage;                           // The image component of the slider.
    private Color m_FullStaminaColor = Color.yellow;       // The color the Stamina bar will be when on full Stamina.
    private Color m_ZeroStaminaColor = Color.red;         // The color the Stamina bar will be when on no Stamina.
    private CharacterManager _charManager;
    private int currentStamina;
    private int maxStamina;

    private void OnEnable()
    {
        _playerUI = GetComponent<CharacterControl>()._playerUI;
        m_Slider = _playerUI.transform.GetChild(4).GetComponent<Slider>();
        m_FillImage = m_Slider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        _charManager = GetComponent<CharacterManager>();
        if (_charManager != null)
        {
            SetStaminaUI();
        }

        m_FillImage.color = m_FullStaminaColor;
    }

    void Update()
    {
        SetStaminaUI();
    }

    private void SetStaminaUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = _charManager.CurrentStamina;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting Stamina.
        m_FillImage.color = Color.Lerp(m_ZeroStaminaColor, m_FullStaminaColor, (_charManager.CurrentStamina * 1.0f) / (_charManager.StartingStamina * 1.0f));

    }
}
