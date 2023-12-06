using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace KinoCube.KinoReigns
{
    public sealed class CardSwapControl : MonoBehaviour
    {
        [Header("Component References:")]
        [SerializeField] private Camera _mainCamera;

        [Header("Params:")]
        [SerializeField] private float _senseThreshold = 0.1f;

        [Header("Events:")]
        [SerializeField] private UnityEvent _swapStarted;
        [SerializeField] private UnityEvent<Vector2> _swapUpdated;
        [SerializeField] private UnityEvent _swapCanceled;

        public Vector2 PointerDeltaWorldPosition { get; private set; }

        public event Action SwapStarted;
        public event Action<Vector2> SwapUpdated;
        public event Action SwapCanceled;

        private Vector2 PointerScreenPosition => Pointer.current.position.value;
        private Vector2 PointerWorldPosition => _mainCamera.ScreenToWorldPoint(PointerScreenPosition);

        private Coroutine _coroutine = null;

        public void HandleDragActionStartedEvent()
        {
            if (IsPointOverlapCard(PointerWorldPosition))
            {
                InvokeSwapStartedEvent();
                _coroutine = StartCoroutine(Routine());
            }
        }

        public void HandleDragActionCanceledEvent()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
                PointerDeltaWorldPosition = Vector2.zero;
                InvokeSwapCanceledEvent();
            }
        }

        private IEnumerator Routine()
        {
            Vector2 pointerStartWorldPosition = PointerWorldPosition;
            yield return null;

            while (true)
            {
                PointerDeltaWorldPosition = PointerWorldPosition - pointerStartWorldPosition;

                InvokeSwapUpdatedEvent();

                if (!IsGreaterThanThreshold(PointerDeltaWorldPosition.x))
                {
                    yield return null;
                    continue;
                }
                yield return null;
            }
        }

        private void InvokeSwapStartedEvent()
        {
            _swapStarted?.Invoke();
            SwapStarted?.Invoke();
        }

        private void InvokeSwapUpdatedEvent()
        {
            _swapUpdated?.Invoke(PointerDeltaWorldPosition);
            SwapUpdated?.Invoke(PointerDeltaWorldPosition);
        }

        private void InvokeSwapCanceledEvent()
        {
            _swapCanceled?.Invoke();
            SwapCanceled?.Invoke();
        }

        private bool IsPointOverlapCard(Vector2 pointWorldPosition)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(pointWorldPosition);
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out CardTag _))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsGreaterThanThreshold(float mouseDeltaWorldX)
        {
            float absMouseDeltaWorldX = Mathf.Abs(mouseDeltaWorldX);
            return absMouseDeltaWorldX > _senseThreshold;
        }
    }
}
