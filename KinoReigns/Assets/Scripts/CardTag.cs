using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace KinoCube.KinoReigns
{
    public sealed class CardTag : MonoBehaviour
    {
        [Header("View Component References:")]
        [SerializeField] private Image _actor;
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _dilemmaDescriptionField;
        [SerializeField] private TMP_Text _actorNameField;
        [SerializeField] private TMP_Text _leftActionField;
        [SerializeField] private TMP_Text _rightActionField;

        [Header("Params:")]
        [SerializeField] private float _speedFactor = 1.0f;

        private const float _startRotationInDegrees = 90.0f;

        public Dilemma Dilemma { get; private set; }

        private Coroutine _coroutine;
        private Collider2D Collider
        {
            get
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider2D>();
                }
                return _collider;
            }
        }

        private Collider2D _collider;

        public void InitCard(Dilemma dilemma)
        {
            Dilemma = dilemma;
            _actor.sprite = dilemma.ActorSprite;
            _background.color = dilemma.BackgroundColor;
            GetStringAsync(dilemma.Description, (str) => _dilemmaDescriptionField.text = str);
            GetStringAsync(dilemma.ActorName, (str) => _actorNameField.text = str);
            GetStringAsync(dilemma.LeftAction.ActionDescription, (str) => _leftActionField.text = str);
            GetStringAsync(dilemma.RightAction.ActionDescription, (str) => _rightActionField.text = str);
            transform.rotation = Quaternion.Euler(_startRotationInDegrees, 0, 0);
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = StartCoroutine(Routine());
        }

        private void GetStringAsync(LocalizedString localizedString, Action<string> onCompleted)
        {
            var asyncOperation = localizedString.GetLocalizedStringAsync();
            if (asyncOperation.IsDone)
            {
                onCompleted(asyncOperation.Result);
            }
            else
            {
                asyncOperation.Completed += (context) => onCompleted(context.Result);
            }
        }

        private IEnumerator Routine()
        {
            Collider.enabled = false;
            const float startRotationInDegrees = _startRotationInDegrees;
            const float endRotationInDegrees = 0;
            float startTime = Time.time;
            float timeLeft;
            do
            {
                timeLeft = (Time.time - startTime) * _speedFactor;
                float rotationInDegrees = Mathf.Lerp(startRotationInDegrees, endRotationInDegrees, timeLeft);
                transform.rotation = Quaternion.Euler(0, rotationInDegrees, 0);
                yield return null;
            }
            while (timeLeft < 1);
            _coroutine = null;
            Collider.enabled = true;
        }
    }
}
