using System;
using System.Collections;
using KinoCube.KinoReigns.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace KinoCube.KinoReigns
{
    public sealed class InputFacade : MonoBehaviour
    {
        [Header("Events:")]
        [SerializeField] private UnityEvent<Vector2> _dragActionStarted;
        [SerializeField] private UnityEvent<Vector2> _dragActionUpdated;
        [SerializeField] private UnityEvent<Vector2> _dragActionCanceled;

        public event Action<Vector2> DragActionStarted;
        public event Action<Vector2> DragActionUpdated;
        public event Action<Vector2> DragActionCanceled;

        private Vector2 PointerPosition => Pointer.current.position.value;

        private InputActions _inputActions;
        private Coroutine _coroutine;

        private void Awake()
        {
            _inputActions = new InputActions();
            _inputActions.Main.DragAction.started += HandleDragActionStartedEvent;
            _inputActions.Main.DragAction.canceled += HandleDragActionCanceledEvent;
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {

            _inputActions.Disable();
        }

        private void OnDestroy()
        {
            _inputActions.Main.DragAction.started -= HandleDragActionStartedEvent;
            _inputActions.Main.DragAction.canceled -= HandleDragActionCanceledEvent;
        }

        private void HandleDragActionStartedEvent(InputContext context)
        {
            InvokeDragActionStartedEvent(PointerPosition);
            _coroutine = StartCoroutine(Routine());
        }

        private void HandleDragActionCanceledEvent(InputContext context)
        {
            InvokeDragActionCanceledEvent(PointerPosition);
            StopCoroutine(_coroutine);
        }

        private void InvokeDragActionStartedEvent(Vector2 pointerWorldPosition)
        {
            _dragActionStarted?.Invoke(pointerWorldPosition);
            DragActionStarted?.Invoke(pointerWorldPosition);
        }

        private void InvokeDragActionUpdatedEvent(Vector2 pointerWorldPosition)
        {
            _dragActionUpdated?.Invoke(pointerWorldPosition);
            DragActionUpdated?.Invoke(pointerWorldPosition);
        }

        private void InvokeDragActionCanceledEvent(Vector2 pointerWorldPosition)
        {
            _dragActionCanceled?.Invoke(pointerWorldPosition);
            DragActionCanceled?.Invoke(pointerWorldPosition);
        }

        private IEnumerator Routine()
        {
            do
            {
                yield return null;
                InvokeDragActionUpdatedEvent(PointerPosition);
            }
            while (true);
        }
    }
}
