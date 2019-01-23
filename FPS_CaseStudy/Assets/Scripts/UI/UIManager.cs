using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private TextMeshProUGUI healthText;

    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private TextMeshProUGUI armourText;

    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private TextMeshProUGUI ammoText;
    
    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private Image gunSprite;
    [SerializeField,Required,FoldoutGroup("Player Elements")]
    private CanvasGroup bloodSplatterCanvasGroup;

    [SerializeField, Required, FoldoutGroup("Respawn Elements")]
    private GameObject respawnWindow;
    [SerializeField, Required, FoldoutGroup("Respawn Elements")]
    private TextMeshProUGUI respawnText;

    public void SetHealth(float health)
    {
        //healthBar.value = health / 100f;
        healthText.text = ((int) health).ToString();
        bloodSplatterCanvasGroup.alpha = 1.5f - (health / 100f);
    }
    
    public void SetArmour(float armour)
    {
        //armourBar.value = armour / 100f;
        armourText.text = ((int) armour).ToString();
    }

    public void SetAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    public void SetGunSprite(Sprite sprite)
    {
        gunSprite.sprite = sprite;
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

        bloodSplatterCanvasGroup.alpha = 0f;
    }

    private bool isClearingBlood;
    
    public void AddBlood(float amount)
    {
        Debug.Log("Adding Amount: " + amount);
        bloodSplatterCanvasGroup.alpha += amount;

        //I dont like how this turned out, im going to use the same idea that Doom used, and just keep the blood up
        //if (!isClearingBlood)
        //    StartCoroutine(ClearBloodCoroutine());
    }

    private IEnumerator ClearBloodCoroutine()
    {
        isClearingBlood = true;

        while (bloodSplatterCanvasGroup.alpha > 0f)
        {
            bloodSplatterCanvasGroup.alpha -= Time.deltaTime / 3f;
            yield return null;
        }

        isClearingBlood = false;
    }

}
