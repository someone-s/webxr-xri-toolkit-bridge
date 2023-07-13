using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.Scripting;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using WebXR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WebXR.XRITBridge
{
#if UNITY_EDITOR
    [InitializeOnLoad] // Call static class constructor in editor.
#endif
    [InputControlLayout(stateType = typeof(XRSimulatedHMDState), isGenericTypeOfDevice = false, displayName = "WebXR HMD", updateBeforeRender = true)]
    [Preserve]
    public class WebXRInteractionHeadset : XRHMD
    {
#if UNITY_EDITOR
        static WebXRInteractionHeadset()
        {
            InputSystem.RegisterLayout<WebXRInteractionHeadset>(
                matches: new InputDeviceMatcher()
                    .WithInterface("WebXRHeadset"));
        }
#endif

        protected override void FinishSetup()
        {
            base.FinishSetup();

            WebXRManager.OnXRChange += OnXRChange;
            WebXRManager.OnHeadsetUpdate += OnHeadsetUpdate;
        }

        private float height = float.MinValue;
        private Camera mainCamera = null;
        private Camera leftCamera = null;
        private Camera rightCamera = null;

        private void OnXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            switch (state)
            {
                case WebXRState.VR:
                case WebXRState.AR:
                    mainCamera = Camera.main;
                    mainCamera.enabled = false;

                    GameObject leftGameObject = new("Left Camera");
                    leftGameObject.transform.SetParent(mainCamera.transform, false);
                    leftCamera = leftGameObject.AddComponent<Camera>();
                    leftCamera.rect = leftRect;

                    GameObject rightGameObject = new("Right Camera");
                    rightGameObject.transform.SetParent(mainCamera.transform, false);
                    rightCamera = rightGameObject.AddComponent<Camera>();
                    rightCamera.rect = rightRect;

                    break;
                default:
                    mainCamera.enabled = true;

                    GameObject.Destroy(leftCamera.gameObject);
                    GameObject.Destroy(rightCamera.gameObject);

                    break;
            }
        }

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

            XRSimulatedHMDState state = new();

            state.isTracked = true;
            state.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);


            state.leftEyePosition = leftPosition - new Vector3(0f, height, 0f);
            state.leftEyeRotation = leftRotation;

            state.rightEyePosition = rightPosition - new Vector3(0f, height, 0f);
            state.rightEyeRotation = rightRotation;

            state.devicePosition = (state.leftEyePosition + state.rightEyePosition) * 0.5f;
            state.deviceRotation = Quaternion.Slerp(state.leftEyeRotation, state.rightEyeRotation, 0.5f);

            state.centerEyePosition = state.devicePosition;
            state.centerEyeRotation = state.deviceRotation;

            if (leftCamera != null)
            {
                leftCamera.transform.localPosition = Quaternion.Inverse(state.leftEyeRotation) * (state.leftEyePosition - state.centerEyePosition);
                leftCamera.projectionMatrix = leftProjectionMatrix;
            }

            if (rightCamera != null)
            {
                rightCamera.transform.localPosition = Quaternion.Inverse(state.rightEyeRotation) * (state.rightEyePosition - state.centerEyePosition);
                rightCamera.projectionMatrix = rightProjectionMatrix;
            }

            InputSystem.QueueStateEvent(this, state);
        }
    }

    public class WebXRHeadsetAdapter : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_WEBGL
            DontDestroyOnLoad(this);

            InputSystem.AddDevice<WebXRInteractionHeadset>();
#endif
        }
    }
}