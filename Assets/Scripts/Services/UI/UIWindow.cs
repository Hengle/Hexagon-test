using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWeld.Binding;

namespace Hexagon.Services.UI
{
    public class UIWindow : MonoBehaviour
    {
        public enum Transition {
            Instant,
            Fade
        }

        public enum VisualState {
            Shown,
            Hidden
        }

        private CanvasGroup _canvasGroup;


        [SerializeField] private Transition _transition = Transition.Instant;
        [SerializeField] private float _transitionDuration = 0.1f;
        [SerializeField] private VisualState _startingState = VisualState.Hidden;
        private VisualState _currentVisualState = VisualState.Hidden;


        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                if (_active) Show();
                else Hide();
            }
        }

        private bool _active;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            ApplyInitialVisualState(_startingState);
        }

#if UNITY_EDITOR
        protected void OnValidate() {
            _transitionDuration = Mathf.Max(_transitionDuration, 0f);

            // Apply starting state
            if (_canvasGroup != null) {
                _canvasGroup.alpha = (this._startingState == VisualState.Hidden) ? 0f : 1f;
            }
        }
#endif

        private void Show()
        {
            // Check if the window is already shown
            if (this._currentVisualState == VisualState.Shown)
                return;

            // Transition
            this.EvaluateAndTransitionToVisualState(VisualState.Shown);
        }

        private void Hide() {

            // Check if the window is already hidden
            if (this._currentVisualState == VisualState.Hidden)
                return;

            // Transition
            this.EvaluateAndTransitionToVisualState(VisualState.Hidden);
        }

        protected void EvaluateAndTransitionToVisualState(VisualState state) {
            float targetAlpha = (state == VisualState.Shown) ? 1f : 0f;          

            // Do the transition
            if (this._transition == Transition.Fade) {

                // Tween the alpha
                this.StartAlphaTween(targetAlpha, _transitionDuration, true);
            }
            else {
                // Set the alpha directly
                this.SetCanvasAlpha(targetAlpha);

            }

            // Save the state
            this._currentVisualState = state;

            // If we are transitioning to show, enable the canvas group raycast blocking
            if (state == VisualState.Shown) {
                _canvasGroup.blocksRaycasts = true;
            }
        }

        public void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale) {
            if (this._canvasGroup == null)
                return;

            LeanTween.alphaCanvas(_canvasGroup, targetAlpha, duration);

        }

        protected void SetCanvasAlpha(float alpha) {
            if (this._canvasGroup == null)
                return;

            // Set the alpha
            this._canvasGroup.alpha = alpha;

            // If the alpha is zero, disable block raycasts
            // Enabling them back on is done in the transition method
            if (alpha == 0f) {
                this._canvasGroup.blocksRaycasts = false;
                //this.m_CanvasGroup.interactable = false;
            }
        }

        protected virtual void ApplyInitialVisualState(VisualState state) {
            float targetAlpha = (state == VisualState.Shown) ? 1f : 0f;

            // Set the alpha directly
            this.SetCanvasAlpha(targetAlpha);

            // Save the state
            this._currentVisualState = state;

            // If we are transitioning to show, enable the canvas group raycast blocking
            if (state == VisualState.Shown) {
                this._canvasGroup.blocksRaycasts = true;
                //this.m_CanvasGroup.interactable = true;
            }
        }

    }
}


