using System.Collections.Generic;

using UnityEngine;

namespace StealthGame
{
    #region ButtonInput struct

    public struct ButtonInput
    {
        #region Public properties

        public bool IsActive { get; }
        public bool IsDown { get; }
        public bool IsUp { get; }
        public bool IsDoubleTap { get; }
        public float Value { get; }

        #endregion


        #region Constructors

        public ButtonInput(bool isActive, bool isDown, bool isUp, bool isDoubleTap, float value)
        {
            IsActive = isActive;
            IsDown = isDown;
            IsUp = isUp;
            IsDoubleTap = isDoubleTap;
            Value = value;
        }

        #endregion
    }

    #endregion

    public class PlayerInputController : MonoBehaviour
    {
        #region Show in inspector

        [SerializeField] private float _inputThreshold;
        [SerializeField] private float _doubleTapDelay;

        #endregion


        #region Public properties

        public ButtonInput HorizontalInput { get; private set; }
        public ButtonInput VerticalInput { get; private set; }
        public Vector3 MovementInput { get; private set; }
        public bool HasMovementInput { get; private set; }
        public ButtonInput SneakInput { get; private set; }
        public ButtonInput RunInput { get; private set; }
        public ButtonInput JumpInput { get; private set; }

        #endregion


        #region Update

        private void Update()
        {
            HorizontalInput = GetAxisInput("Horizontal");
            VerticalInput = GetAxisInput("Vertical");

            HasMovementInput = HorizontalInput.IsActive || VerticalInput.IsActive;

            MovementInput = new Vector3()
            {
                x = HorizontalInput.Value,
                y = 0,
                z = VerticalInput.Value
            };

            MovementInput = Vector3.ClampMagnitude(MovementInput, 1);

            SneakInput = GetButtonInput("Sneak");
            RunInput = GetButtonInput("Run");
            JumpInput = GetButtonInput("Jump");
     
        }

        #endregion


        #region Private methods

        private ButtonInput GetButtonInput(string buttonName)
        {
            bool isDoubleTap = GetDoubleTapStatus(buttonName, Input.GetButtonDown(buttonName));

            return new ButtonInput(
                Input.GetButton(buttonName),
                Input.GetButtonDown(buttonName),
                Input.GetButtonUp(buttonName),
                isDoubleTap,
                Input.GetButton(buttonName) ? 1 : 0);
        }

        private ButtonInput GetAxisInput(string axisName)
        {
            bool isActive = IsAxisActive(axisName);

            bool isDown = false;
            bool isUp = false;
            if (!_axisInputLastFrame.TryGetValue(axisName, out bool wasActive))
            {
                _axisInputLastFrame.Add(axisName, false);
                wasActive = false;
            }

            if (wasActive && !isActive)
            {
                isUp = true;
            }
            if (!wasActive && isActive)
            {
                isDown = true;
            }

            bool isDoubleTap = GetDoubleTapStatus(axisName, isDown);

            return new ButtonInput(
                isActive,
                isDown,
                isUp,
                isDoubleTap,
                Input.GetAxisRaw(axisName));
        }

        private bool GetDoubleTapStatus(string name, bool isDown)
        {
            bool isDoubleTap = false;
            if (_doubleTapTimes.TryGetValue(name, out float doubleTapTime))
            {
                if (isDown)
                {
                    isDoubleTap = Time.time < doubleTapTime;
                    doubleTapTime = Time.time + _doubleTapDelay;
                    _doubleTapTimes[name] = doubleTapTime;
                }
            }
            else
            {
                isDoubleTap = false;
                doubleTapTime = Time.time + _doubleTapDelay;
                _doubleTapTimes.Add(name, doubleTapTime);
            }

            return isDoubleTap;
        }

        private bool IsAxisActive(string axisName)
        {
            return Mathf.Abs(Input.GetAxisRaw(axisName)) > _inputThreshold;
        }

        #endregion


        #region Private

        private Dictionary<string, float> _doubleTapTimes = new Dictionary<string, float>();
        private Dictionary<string, bool> _axisInputLastFrame = new Dictionary<string, bool>();

        #endregion
    }
}