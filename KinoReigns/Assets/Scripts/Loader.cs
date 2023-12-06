using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace KinoCube.KinoReigns
{
    public sealed class Loader : MonoBehaviour
    {
        [Header("Events:")]
        [SerializeField] private UnityEvent _loadStarted;
        [SerializeField] private UnityEvent _loadFinished;

        public event Action LoadStarted;
        public event Action LoadFinished;

        private Coroutine _coroutineLoadLocalization;

        private void Start()
        {
            if (_coroutineLoadLocalization == null)
            {
                _coroutineLoadLocalization = StartCoroutine(RoutineLoadLocalization());
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private IEnumerator RoutineLoadLocalization()
        {
            InvokeLoadStartedEvent();
            yield return LocalizationSettings.InitializationOperation;
            InvokeLoadFinishedEvent();
            _coroutineLoadLocalization = null;
        }

        private void InvokeLoadStartedEvent()
        {
            _loadStarted?.Invoke();
            LoadStarted?.Invoke();
        }

        private void InvokeLoadFinishedEvent()
        {
            _loadFinished?.Invoke();
            LoadFinished?.Invoke();
        }
    }
}
