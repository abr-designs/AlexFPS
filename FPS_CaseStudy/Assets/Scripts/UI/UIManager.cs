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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
