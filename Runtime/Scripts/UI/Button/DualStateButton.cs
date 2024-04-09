using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DualStateButton : ButtonWithIcon
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
            StateOne.OnClick += SwitchToStateTwo;

            StateTwo = two;
            StateTwo.OnClick += SwitchToStateOne;

            SwitchState(StateOne);
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

        private void SwitchToStateOne()
        {
            SwitchState(StateOne);
        }

        private void SwitchToStateTwo()
        {
            SwitchState(StateTwo);
        }
    }
}