using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.Scripting;
using UnityEngine.XR;
using WebXR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WebXR.XRITBridge
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(stateType = typeof(XRSimulatedControllerState), commonUsages = new[] {"LeftHand", "RightHand"}, isGenericTypeOfDevice = false, displayName = "WebXR Controller", updateBeforeRender = true)]
    [Preserve]
    public class WebXRInteractionController : XRController
    {
#if UNITY_EDITOR
        static WebXRInteractionController()
        {
            InputSystem.RegisterLayout<WebXRInteractionController>(
                matches: new InputDeviceMatcher()
                    .WithInterface("WebXRController"));
        }
#endif

        internal WebXRControllerHand ident;

        protected override void FinishSetup()
        {
            base.FinishSetup();

            WebXRManager.OnHeadsetUpdate += OnHeadsetUpdate;
            WebXRManager.OnControllerUpdate += OnControllerUpdate;
        }

        private float height = float.MinValue;
        private float pressMargin = 0.5f;
        private float degreeOffset = 30f;

        private void OnHeadsetUpdate(
            Matrix4x4 leftProjectionMatrix,
            Matrix4x4 rightProjectionMatrix,
            Quaternion leftRotation,
            Quaternion rightRotation,
            Vector3 leftPosition,
            Vector3 rightPosition)
        {
            if (height <= 0f)
                height = ((leftPosition + rightPosition) * 0.5f).y;
        }

        private void OnControllerUpdate(WebXRControllerData data)
        {
            if ((WebXRControllerHand)data.hand != ident)
                return;

            ushort buttons = 0;

            if (data.squeeze >= 1f - pressMargin)
                buttons |= 1 << (int)ControllerButton.GripButton;
            if (data.trigger >= 1f - pressMargin)
                buttons |= 1 << (int)ControllerButton.TriggerButton;
            if (data.buttonATouched)
                buttons |= 1 << (int)ControllerButton.PrimaryTouch;
            if (data.buttonBTouched)
                buttons |= 1 << (int)ControllerButton.SecondaryTouch;
            if (data.thumbstickTouched)
                buttons |= 1 << (int)ControllerButton.Primary2DAxisTouch;
            if (data.touchpadTouched)
                buttons |= 1 << (int)ControllerButton.Secondary2DAxisTouch;

            buttons |= 1 << (int)ControllerButton.UserPresence;

            var state = new XRSimulatedControllerState
            {
                primary2DAxis = new(data.thumbstickX, data.thumbstickY),
                trigger = data.trigger,
                grip = data.squeeze,
                secondary2DAxis = new(data.touchpadX, data.touchpadY),
                buttons = buttons,
                batteryLevel = 0f,
                trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation),
                isTracked = data.enabled,
                devicePosition = data.position - new Vector3(0f, height, 0f),
                deviceRotation = data.rotation * Quaternion.Euler(degreeOffset, 0f, 0f)
            };

            InputSystem.QueueStateEvent(this, state);
        }
    }

    public class WebXRControllerAdapter : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_WEBGL
            DontDestroyOnLoad(this);

            var left = InputSystem.AddDevice<WebXRInteractionController>("Left");
            left.ident = WebXRControllerHand.LEFT;
            InputSystem.SetDeviceUsage(left, UnityEngine.InputSystem.CommonUsages.LeftHand);

            var right = InputSystem.AddDevice<WebXRInteractionController>("Right");
            right.ident = WebXRControllerHand.RIGHT;
            InputSystem.SetDeviceUsage(right, UnityEngine.InputSystem.CommonUsages.RightHand);
#endif
        }
    }
}