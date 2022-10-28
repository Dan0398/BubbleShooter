//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Scripts/Services/Inputs/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""BaseMap"",
            ""id"": ""f49ae12d-43c9-431f-b85f-34e5b47c510b"",
            ""actions"": [
                {
                    ""name"": ""ClickPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c2a7fa73-a40b-472f-99c4-197442d32915"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Clicked"",
                    ""type"": ""Button"",
                    ""id"": ""750e1f2e-7a5c-4a18-a382-5557316c1f12"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchBubbles"",
                    ""type"": ""Button"",
                    ""id"": ""d9026663-96d2-492b-99d2-980c993eaea8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f34db17c-7dee-4486-9133-63338b4081cf"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Base"",
                    ""action"": ""ClickPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2747c153-34e9-4cba-b256-56ded9f1e760"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Clicked"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b42392c-e6bd-4453-9e4b-c6a18b091331"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchBubbles"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Base"",
            ""bindingGroup"": ""Base"",
            ""devices"": [
                {
                    ""devicePath"": ""<Pointer>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // BaseMap
        m_BaseMap = asset.FindActionMap("BaseMap", throwIfNotFound: true);
        m_BaseMap_ClickPosition = m_BaseMap.FindAction("ClickPosition", throwIfNotFound: true);
        m_BaseMap_Clicked = m_BaseMap.FindAction("Clicked", throwIfNotFound: true);
        m_BaseMap_SwitchBubbles = m_BaseMap.FindAction("SwitchBubbles", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // BaseMap
    private readonly InputActionMap m_BaseMap;
    private IBaseMapActions m_BaseMapActionsCallbackInterface;
    private readonly InputAction m_BaseMap_ClickPosition;
    private readonly InputAction m_BaseMap_Clicked;
    private readonly InputAction m_BaseMap_SwitchBubbles;
    public struct BaseMapActions
    {
        private @Controls m_Wrapper;
        public BaseMapActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ClickPosition => m_Wrapper.m_BaseMap_ClickPosition;
        public InputAction @Clicked => m_Wrapper.m_BaseMap_Clicked;
        public InputAction @SwitchBubbles => m_Wrapper.m_BaseMap_SwitchBubbles;
        public InputActionMap Get() { return m_Wrapper.m_BaseMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BaseMapActions set) { return set.Get(); }
        public void SetCallbacks(IBaseMapActions instance)
        {
            if (m_Wrapper.m_BaseMapActionsCallbackInterface != null)
            {
                @ClickPosition.started -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnClickPosition;
                @ClickPosition.performed -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnClickPosition;
                @ClickPosition.canceled -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnClickPosition;
                @Clicked.started -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnClicked;
                @Clicked.performed -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnClicked;
                @Clicked.canceled -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnClicked;
                @SwitchBubbles.started -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnSwitchBubbles;
                @SwitchBubbles.performed -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnSwitchBubbles;
                @SwitchBubbles.canceled -= m_Wrapper.m_BaseMapActionsCallbackInterface.OnSwitchBubbles;
            }
            m_Wrapper.m_BaseMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ClickPosition.started += instance.OnClickPosition;
                @ClickPosition.performed += instance.OnClickPosition;
                @ClickPosition.canceled += instance.OnClickPosition;
                @Clicked.started += instance.OnClicked;
                @Clicked.performed += instance.OnClicked;
                @Clicked.canceled += instance.OnClicked;
                @SwitchBubbles.started += instance.OnSwitchBubbles;
                @SwitchBubbles.performed += instance.OnSwitchBubbles;
                @SwitchBubbles.canceled += instance.OnSwitchBubbles;
            }
        }
    }
    public BaseMapActions @BaseMap => new BaseMapActions(this);
    private int m_BaseSchemeIndex = -1;
    public InputControlScheme BaseScheme
    {
        get
        {
            if (m_BaseSchemeIndex == -1) m_BaseSchemeIndex = asset.FindControlSchemeIndex("Base");
            return asset.controlSchemes[m_BaseSchemeIndex];
        }
    }
    public interface IBaseMapActions
    {
        void OnClickPosition(InputAction.CallbackContext context);
        void OnClicked(InputAction.CallbackContext context);
        void OnSwitchBubbles(InputAction.CallbackContext context);
    }
}