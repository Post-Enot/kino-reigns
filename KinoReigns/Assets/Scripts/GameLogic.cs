using System;
using UnityEngine;
using UnityEngine.Events;

namespace KinoCube.KinoReigns
{
    public sealed class GameLogic : MonoBehaviour
    {
        [Header("Temp:")]
        [SerializeField] private GameObject _uiObject;
        [SerializeField] private GameObject _winUI;
        [SerializeField] private Dilemma _looseCard;
        [SerializeField] private int _cardForWin = 10;

        [Header("View Component References:")]
        [SerializeField] private ParamIndicator _moneyIndicator;
        [SerializeField] private ParamIndicator _audienceIndicator;
        [SerializeField] private ParamIndicator _teamIndicator;
        [SerializeField] private ParamIndicator _confidenceIndicator;

        [Header("Component References:")]
        [SerializeField] private ClubParams _clubParams;
        [SerializeField] private CardSwapControl _cardSwapControl;
        [SerializeField] private CardMover _cardMover;
        [SerializeField] private DilemmaDeck _cardDeck;
        [SerializeField] private CardTag _card;

        [Header("Events:")]
        [SerializeField] private UnityEvent<int> _cardsLeftCountUpdated;

        [Header("Params:")]
        [SerializeField] private float _thresholdForThrowCard = 1.0f;

        public event Action<int> CardsLeftCountUpdated;

        private const int _looseCardCode = -3;

        private int _cardsLeft;

        public void StartGame()
        {
            _uiObject.SetActive(true);
            ChooseNextCard();
            SubscribeOnCardSwapEvents();
            _moneyIndicator.SetValueWithoutAnimation(_clubParams.Money);
            _audienceIndicator.SetValueWithoutAnimation(_clubParams.Audience);
            _teamIndicator.SetValueWithoutAnimation(_clubParams.Team);
            _confidenceIndicator.SetValueWithoutAnimation(_clubParams.Confidence);
        }

        private void HandleCardSwapStartedEvent()
        {
            _cardMover.SetStatus(CardMover.Statuses.Drag);
        }

        private void HandleCardDragCanceledEvent()
        {
            if (_cardMover.CardDeltaX_Abs >= _thresholdForThrowCard)
            {
                UnsubscribeFromCardSwapEvents();
                _cardMover.CardThrowed += HandleCardThrowedEvent;

                if (_cardMover.CardDeltaX > 0)
                {
                    ChangeParams(_card.Dilemma.RightAction);
                    _cardMover.SetStatus(CardMover.Statuses.ThrowRight);
                }
                else
                {
                    ChangeParams(_card.Dilemma.LeftAction);
                    _cardMover.SetStatus(CardMover.Statuses.ThrowLeft);
                }
            }
            else
            {
                _cardMover.SetStatus(CardMover.Statuses.Return);
            }
        }

        private void ChangeParams(DilemmaAction dilemmaAction)
        {
            _moneyIndicator.ValueAddition(dilemmaAction.Money);
            _audienceIndicator.ValueAddition(dilemmaAction.Audience);
            _teamIndicator.ValueAddition(dilemmaAction.Team);
            _confidenceIndicator.ValueAddition(dilemmaAction.Confidence);

            _clubParams.Money += dilemmaAction.Money;
            _clubParams.Audience += dilemmaAction.Audience;
            _clubParams.Team += dilemmaAction.Team;
            _clubParams.Confidence += dilemmaAction.Confidence;
        }

        private void ChooseNextCard()
        {
            if (_cardsLeft == _looseCardCode)
            {
                _cardsLeft = 0;
                _clubParams.ResetParams();
                _moneyIndicator.SetValueWithoutAnimation(_clubParams.Money);
                _audienceIndicator.SetValueWithoutAnimation(_clubParams.Audience);
                _teamIndicator.SetValueWithoutAnimation(_clubParams.Team);
                _confidenceIndicator.SetValueWithoutAnimation(_clubParams.Confidence);
                InitNextCard();
            }
            else if (_clubParams.SomeParamsOutOfRange)
            {
                _cardsLeft = _looseCardCode;
                _card.InitCard(_looseCard);
            }
            else if (_cardsLeft >= _cardForWin)
            {
                _uiObject.SetActive(false);
                _winUI.SetActive(true);
            }
            else
            {
                InitNextCard();
            }
        }

        private void HandleCardThrowedEvent()
        {
            _cardMover.CardThrowed -= HandleCardThrowedEvent;
            SubscribeOnCardSwapEvents();
            _cardMover.ResetCardPositionAndRotation();
            ChooseNextCard();
        }

        private void InitNextCard()
        {
            Dilemma card = _cardDeck.GetNextCard();
            _card.InitCard(card);
            _cardsLeft += 1;
            InvokeCardsLeftCountUpdated();
        }

        private void SubscribeOnCardSwapEvents()
        {
            _cardSwapControl.SwapStarted += HandleCardSwapStartedEvent;
            _cardSwapControl.SwapCanceled += HandleCardDragCanceledEvent;
        }

        private void UnsubscribeFromCardSwapEvents()
        {
            _cardSwapControl.SwapStarted -= HandleCardSwapStartedEvent;
            _cardSwapControl.SwapCanceled -= HandleCardDragCanceledEvent;
        }

        private void InvokeCardsLeftCountUpdated()
        {
            _cardsLeftCountUpdated?.Invoke(_cardsLeft);
            CardsLeftCountUpdated?.Invoke(_cardsLeft);
        }
    }
}
