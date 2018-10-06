using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterStamina : MonoBehaviour
{
    public Slider m_Slider;                             // The slider to represent how much Stamina the character currently has.
    private Image m_FillImage;                           // The image component of the slider.
    public Color m_FullStaminaColor = Color.green;       // The color the Stamina bar will be when on full Stamina.
    public Color m_ZeroStaminaColor = Color.red;         // The color the Stamina bar will be when on no Stamina.
    private CharacterManager _charManager;
    private int currentStamina;
    private int maxStamina;

    private void OnEnable()
    {
        m_FillImage = m_Slider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        _charManager = GetComponent<CharacterManager>();
        if (_charManager != null)
        {
            SetStaminaUI();
        }
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
        m_FillImage.color = Color.Lerp(m_ZeroStaminaColor, m_FullStaminaColor, _charManager.CurrentStamina / _charManager.StartingStamina);
    }
}
