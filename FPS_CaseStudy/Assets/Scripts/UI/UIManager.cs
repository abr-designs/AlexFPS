using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private Slider healthBar;

    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private Slider armourBar;

    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private TextMeshProUGUI ammoText;

    [SerializeField, Required, FoldoutGroup("Respawn Elements")]
    private GameObject respawnWindow;
    [SerializeField, Required, FoldoutGroup("Respawn Elements")]
    private TextMeshProUGUI respawnText;

    public void SetHealth(float health)
    {
        healthBar.value = health / 100f;
    }
    
    public void SetArmour(float armour)
    {
        armourBar.value = armour / 100f;
    }

    public void SetAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    public void ShowRespawn(bool state, string textDisplay)
    {
        respawnWindow.SetActive(state);
        
        if (state)
            respawnText.text = textDisplay;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //Need to ensure that this is disabled on start.
        ShowRespawn(false, string.Empty);
    }

}
