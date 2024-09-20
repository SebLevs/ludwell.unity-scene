using System;
using UnityEngine.UIElements;

namespace Ludwell.UIToolkitElements.Editor
{
    public class DualStateButton : ButtonWithIcon, IDisposable
    {
        public new class UxmlFactory : UxmlFactory<DualStateButton>
        {
        }

        private DualStateButtonState _currentState;

        public DualStateButtonState StateOne { get; private set; }
        public DualStateButtonState StateTwo { get; private set; }

        public void Initialize(DualStateButtonState one, DualStateButtonState two)
        {
            StateOne = one;
            var oneAction = one.OnClick;
            StateOne.OnClick = SwitchToStateTwo;
            StateOne.OnClick += oneAction;

            StateTwo = two;
            var twoAction = two.OnClick;
            StateTwo.OnClick = SwitchToStateOne;
            StateTwo.OnClick += twoAction;

            SwitchState(StateOne);
        }

        public void Dispose()
        {
            StateOne.OnClick -= SwitchToStateTwo;
            StateTwo.OnClick -= SwitchToStateOne;
        }

        public void SwitchState(DualStateButtonState state)
        {
            if (_currentState == null)
            {
                _currentState = state;
                _currentState.Enter();
                return;
            }

            if (_currentState == state) return;

            _currentState.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public void SwitchToStateOne()
        {
            SwitchState(StateOne);
        }

        public void SwitchToStateTwo()
        {
            SwitchState(StateTwo);
        }
    }
}
