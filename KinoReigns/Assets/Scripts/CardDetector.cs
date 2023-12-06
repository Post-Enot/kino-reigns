using System;
using UnityEngine;
using UnityEngine.Events;

namespace KinoCube.KinoReigns
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public sealed class CardDetector : MonoBehaviour
    {
        [Header("Events:")]
        [SerializeField] private UnityEvent<CardTag> _cardDetected;
        [SerializeField] private UnityEvent<CardTag> _cardLost;

        public event Action<CardTag> CardDetected;
        public event Action<CardTag> CardLost;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out CardTag cardTag))
            {
                InvokeCardDetectedEvent(cardTag);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out CardTag cardTag))
            {
                InvokeCardLostEvent(cardTag);
            }
        }

        private void InvokeCardDetectedEvent(CardTag cardTag)
        {
            _cardDetected?.Invoke(cardTag);
            CardDetected?.Invoke(cardTag);
        }

        private void InvokeCardLostEvent(CardTag cardTag)
        {
            _cardLost?.Invoke(cardTag);
            CardLost?.Invoke(cardTag);
        }
    }
}
