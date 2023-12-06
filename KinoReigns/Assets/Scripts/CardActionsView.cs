using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KinoCube.KinoReigns
{
    public sealed class CardActionsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _leftActionTextField;
        [SerializeField] private TMP_Text _rightActionTextField;
        [SerializeField] private Image _background;
        [SerializeField] private float _alphaThreshold;
        [SerializeField] private CardMover _cardMover;
        [SerializeField] private CardTag _cardTag;

        [Header("Indicators:")]
        [SerializeField] private ParamIndicator _moneyIndicator;
        [SerializeField] private ParamIndicator _teamIndicator;
        [SerializeField] private ParamIndicator _audienceIndicator;
        [SerializeField] private ParamIndicator _confidenceIndicator;

        private void LateUpdate()
        {

            float alpha = _cardMover.CardDeltaX_Abs / _alphaThreshold;

            Color backgroundColor = _background.color;
            Color leftActionTextFieldColor = _leftActionTextField.color;
            Color rightActionTextFieldColor = _rightActionTextField.color;

            if (_cardMover.CardDeltaX < 0)
            {
                rightActionTextFieldColor.a = 0;
                leftActionTextFieldColor.a = alpha;
                _teamIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.LeftAction.Team, alpha);
                _moneyIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.LeftAction.Money, alpha);
                _audienceIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.LeftAction.Audience, alpha);
                _confidenceIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.LeftAction.Confidence, alpha);
            }
            else
            {
                leftActionTextFieldColor.a = 0;
                rightActionTextFieldColor.a = alpha;
                _teamIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.RightAction.Team, alpha);
                _moneyIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.RightAction.Money, alpha);
                _audienceIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.RightAction.Audience, alpha);
                _confidenceIndicator.DeltaIndicator.ChangeIndicatorAlpha(_cardTag.Dilemma.RightAction.Confidence, alpha);
            }


            _leftActionTextField.color = leftActionTextFieldColor;
            _rightActionTextField.color = rightActionTextFieldColor;
            backgroundColor.a = alpha;
            _background.color = backgroundColor;

            RotateTextField(_leftActionTextField);
            RotateTextField(_rightActionTextField);
        }

        private void RotateTextField(TMP_Text textField)
        {
            Vector3 textFieldRotationInVector3 = textField.transform.rotation.eulerAngles;
            textFieldRotationInVector3.z = 0;
            textField.transform.rotation = Quaternion.Euler(textFieldRotationInVector3);
        }
    }
}
