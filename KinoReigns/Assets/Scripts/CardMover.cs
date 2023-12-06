using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace KinoCube.KinoReigns
{
    public sealed class CardMover : MonoBehaviour
    {
        public enum Statuses : byte
        {
            Idle = 0,
            Drag = 1,
            Return = 2,
            ThrowLeft = 3,
            ThrowRight = 4
        }

        [Header("Component References:")]
        [SerializeField] private CardSwapControl _cardSwapControl;
        [SerializeField] private GameObject _card;
        [SerializeField] private CardDetector _leftThrowPointCardDetector;
        [SerializeField] private CardDetector _rightThrowPointCardDetector;

        [Header("Params:")]
        [SerializeField] private float _maxSpeed = 2.0f;
        [SerializeField] private float _unitToDegrees = 1f;
        [SerializeField] private float _returnDuration = 1.0f;
        [SerializeField] private AnimationCurve _returnAnimationCurve;

        [Header("Events:")]
        [SerializeField] private UnityEvent _cardThrowedLeft;
        [SerializeField] private UnityEvent _cardThrowedRight;
        [SerializeField] private UnityEvent _cardThrowed;

        public Statuses Status { get; private set; }
        public float CardDeltaX_Abs => Mathf.Abs(CardDeltaX);
        public float CardDeltaX
        {
            get
            {
                Vector3 delta = _card.transform.position - _cardStartPosition;
                return delta.x;
            }
        }
        public float CardDeltaX_Sign => Mathf.Sign(CardDeltaX);

        public event Action CardThrowedLeft;
        public event Action CardThrowedRight;
        public event Action CardThrowed;

        private float EulerAngleZ => -(_card.transform.position.x * _unitToDegrees);
        private Quaternion UpdatedRotation => Quaternion.Euler(0, 0, EulerAngleZ);

        private Vector3 _velocity;
        private Vector3 _cardStartPosition;
        private Quaternion _cardStartRotation;
        private Coroutine _coroutine;

        private void Awake()
        {
            _cardStartPosition = _card.transform.position;
            _cardStartRotation = _card.transform.rotation;
            _leftThrowPointCardDetector.CardDetected += HandleLeftThrowCardDetectedEvent;
            _rightThrowPointCardDetector.CardDetected += HandleRightThrowCardDetectedEvent;
        }

        private void HandleLeftThrowCardDetectedEvent(CardTag card)
        {
            InvokeCardThrowedLeftEvent();
            InvokeCardThrowedEvent();
        }

        private void HandleRightThrowCardDetectedEvent(CardTag card)
        {
            InvokeCardThrowedRightEvent();
            InvokeCardThrowedEvent();
        }

        public void ResetCardPositionAndRotation()
        {
            StopCurrentCoroutine();
            _card.transform.SetPositionAndRotation(_cardStartPosition, _cardStartRotation);

        }

        public void SetStatus(Statuses newStatus)
        {
            switch (newStatus)
            {
                case Statuses.Idle:
                    StopCurrentCoroutine();
                    break;

                case Statuses.Drag:
                    SetStatus(newStatus, RoutineDrag());
                    break;

                case Statuses.Return:
                    SetStatus(newStatus, RoutineReturn());
                    break;

                case Statuses.ThrowLeft:
                    SetStatus(newStatus, RoutineThrowLeft());
                    break;

                case Statuses.ThrowRight:
                    SetStatus(newStatus, RoutineThrowRight());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus));
            }
        }

        private void SetStatus(Statuses newStatus, IEnumerator routine)
        {
            if (newStatus == Status)
            {
                return;
            }
            StopCurrentCoroutine();
            _coroutine = StartCoroutine(routine);
        }

        private void StopCurrentCoroutine()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }

        private IEnumerator RoutineThrowLeft()
        {
            yield return RoutineSmoothDamp(_leftThrowPointCardDetector.transform.position);
            _coroutine = null;
        }

        private IEnumerator RoutineThrowRight()
        {
            yield return RoutineSmoothDamp(_rightThrowPointCardDetector.transform.position);
            _coroutine = null;
        }

        private IEnumerator RoutineReturn()
        {
            yield return RoutineLerp(
                _card.transform.position,
                _cardStartPosition,
                _returnDuration,
                _returnAnimationCurve);
            _coroutine = null;
        }

        private IEnumerator RoutineLerp(
            Vector3 startPosition,
            Vector3 finalPosition,
            float duration,
            AnimationCurve animationCurve)
        {
            float timeStart = Time.time;
            float timeLeftNormalized;
            do
            {
                float timeLeft = Time.time - timeStart;
                timeLeftNormalized = timeLeft / duration;
                float value = animationCurve.Evaluate(timeLeftNormalized);
                _card.transform.position = Vector3.Lerp(startPosition, finalPosition, value);
                UpdateRotation();
                yield return null;
            }
            while (timeLeftNormalized < 1);
        }

        private IEnumerator RoutineDrag()
        {
            _velocity = Vector3.zero;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                Vector3 targetPosition = new(
                _cardStartPosition.x + _cardSwapControl.PointerDeltaWorldPosition.x,
                _cardStartPosition.y + _cardSwapControl.PointerDeltaWorldPosition.y,
                _cardStartPosition.z);
                UpdatePosition(targetPosition);
                UpdateRotation();
            }
        }

        private Vector3 RoundVector3(Vector3 value)
        {
            const int digitsCount = 1;
            return new Vector3(
                x: Mathf.Round(Mathf.RoundToInt(value.x * 10 * digitsCount)),
                y: Mathf.Round(Mathf.RoundToInt(value.y * 10 * digitsCount)),
                z: Mathf.Round(Mathf.RoundToInt(value.z * 10 * digitsCount)));
        }

        private bool IsEqual(Vector3 a, Vector3 b)
        {
            a = RoundVector3(a);
            b = RoundVector3(b);
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }

        private IEnumerator RoutineSmoothDamp(Vector3 targetPosition)
        {
            while (!IsEqual(_card.transform.position, targetPosition))
            {
                yield return new WaitForEndOfFrame();
                UpdatePosition(targetPosition);
                UpdateRotation();
            }
        }

        private void UpdatePosition(Vector3 targetPosition)
        {
            _card.transform.position = CardSmoothDamp(targetPosition);
        }

        private Vector3 CardSmoothDamp(Vector3 targetPosition)
        {
            return Vector3.SmoothDamp(
                _card.transform.position,
                targetPosition,
                ref _velocity,
                _maxSpeed);
        }

        private void UpdateRotation()
        {
            _card.transform.rotation = UpdatedRotation;
        }

        private void InvokeCardThrowedLeftEvent()
        {
            _cardThrowedLeft?.Invoke();
            CardThrowedLeft?.Invoke();
        }

        private void InvokeCardThrowedRightEvent()
        {
            _cardThrowedRight?.Invoke();
            CardThrowedRight?.Invoke();
        }

        private void InvokeCardThrowedEvent()
        {
            _cardThrowed?.Invoke();
            CardThrowed?.Invoke();
        }
    }
}
