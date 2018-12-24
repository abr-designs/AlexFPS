// GENERATED AUTOMATICALLY FROM 'Assets/Input Settings/FPSControls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class FPSControls : InputActionAssetReference
{
    public FPSControls()
    {
    }
    public FPSControls(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_Move = m_Gameplay.GetAction("Move");
        m_Gameplay_Look = m_Gameplay.GetAction("Look");
        m_Gameplay_Jump = m_Gameplay.GetAction("Jump");
        m_Gameplay_Fire = m_Gameplay.GetAction("Fire");
        m_Gameplay_Sprint = m_Gameplay.GetAction("Sprint");
        m_Gameplay_Crouch = m_Gameplay.GetAction("Crouch");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Gameplay = null;
        m_Gameplay_Move = null;
        m_Gameplay_Look = null;
        m_Gameplay_Jump = null;
        m_Gameplay_Fire = null;
        m_Gameplay_Sprint = null;
        m_Gameplay_Crouch = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Gameplay
    private InputActionMap m_Gameplay;
    private InputAction m_Gameplay_Move;
    private InputAction m_Gameplay_Look;
    private InputAction m_Gameplay_Jump;
    private InputAction m_Gameplay_Fire;
    private InputAction m_Gameplay_Sprint;
    private InputAction m_Gameplay_Crouch;
    public struct GameplayActions
    {
        private FPSControls m_Wrapper;
        public GameplayActions(FPSControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move { get { return m_Wrapper.m_Gameplay_Move; } }
        public InputAction @Look { get { return m_Wrapper.m_Gameplay_Look; } }
        public InputAction @Jump { get { return m_Wrapper.m_Gameplay_Jump; } }
        public InputAction @Fire { get { return m_Wrapper.m_Gameplay_Fire; } }
        public InputAction @Sprint { get { return m_Wrapper.m_Gameplay_Sprint; } }
        public InputAction @Crouch { get { return m_Wrapper.m_Gameplay_Crouch; } }
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
    }
    public GameplayActions @Gameplay
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new GameplayActions(this);
        }
    }
}
