using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KinoCube.KinoReigns
{
    public sealed class ParamIndicator : MonoBehaviour
    {
        [Serializable]
        public sealed class AnimationData
        {
            [field: SerializeField] public float Duration { get; set; }
            [field: SerializeField] public AnimationCurve AnimationCurve { get; set; }
        }

        public struct AnimationFrameData
        {
            public AnimationFrameData(
                float timeLeft,
                float timeLeftNormalized,
                float animationCurveValue)
            {
                TimeLeft = timeLeft;
                TimeLeftNormalized = timeLeftNormalized;
                AnimationCurveValue = animationCurveValue;
            }

            public float TimeLeft { get; set; }
            public float TimeLeftNormalized { get; set; }
            public float AnimationCurveValue { get; set; }
            public readonly bool IsAnimationEnd => TimeLeftNormalized < 1;
        }

        [Header("Component References:")]
        [SerializeField] private Image _indicator;
        [SerializeField] private DeltaIndicator _deltaIndicator;

        [Header("Params:")]
        [SerializeField] private AnimationData _fillIncreaseAnimationData;
        [SerializeField] private AnimationData _fillDecreaseAnimationData;
        [SerializeField] private AnimationData _colorIncreaseAnimationData;
        [SerializeField] private AnimationData _colorDecreaseAnimationData;

        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _increaseAnimationColor;
        [SerializeField] private Color _decreaseAnimationColor;

        public int Value { get; private set; }
        public float NormalizedValue => (float)Value / ClubParams.MaxParamValue;
        public DeltaIndicator DeltaIndicator => _deltaIndicator;

        private Coroutine _fillCoroutine;
        private Coroutine _colorCoroutine;

        public void SetValueWithoutAnimation(int value)
        {
            Value = value;
            _indicator.fillAmount = NormalizedValue;
        }

        public void ValueAddition(int delta)
        {
            if (delta == 0)
            {
                return;
            }
            if (delta > 0)
            {
                _fillCoroutine = StartCoroutine(RoutineFillIncreaseAnimation(delta));
                _colorCoroutine = StartCoroutine(RoutineColorIncreaseAnimation());
            }
            else
            {
                _fillCoroutine = StartCoroutine(RoutineFillDecreaseAnimation(delta));
                _colorCoroutine = StartCoroutine(RoutineColorDecreaseAnimation());
            }
        }

        private IEnumerator RoutineColorIncreaseAnimation()
        {
            float startTime = Time.time;
            AnimationFrameData animationFrameData;
            do
            {
                animationFrameData = CalculateValueFactor(
                    startTime,
                    _colorIncreaseAnimationData.Duration,
                    _colorIncreaseAnimationData.AnimationCurve);
                _indicator.color = Color.Lerp(
                    _defaultColor,
                    _increaseAnimationColor,
                    animationFrameData.AnimationCurveValue);
                yield return null;
            }
            while (animationFrameData.IsAnimationEnd);
        }

        private IEnumerator RoutineColorDecreaseAnimation()
        {
            float startTime = Time.time;
            AnimationFrameData animationFrameData;
            do
            {
                animationFrameData = CalculateValueFactor(
                    startTime,
                    _colorIncreaseAnimationData.Duration,
                    _colorIncreaseAnimationData.AnimationCurve);
                _indicator.color = Color.Lerp(
                    _defaultColor,
                    _decreaseAnimationColor,
                    animationFrameData.AnimationCurveValue);
                yield return null;
            }
            while (animationFrameData.IsAnimationEnd);
        }

        private IEnumerator RoutineFillIncreaseAnimation(int delta)
        {
            float startTime = Time.time;
            float startValue = Value;
            float endValue = Value + delta;
            Value += delta;
            AnimationFrameData animationFrameData;
            do
            {
                animationFrameData = CalculateValueFactor(
                    startTime,
                    _fillIncreaseAnimationData.Duration,
                    _fillIncreaseAnimationData.AnimationCurve);
                float indicatorValue = Mathf.Lerp(
                    startValue,
                    endValue,
                    animationFrameData.AnimationCurveValue);
                float indicatorValueNormalized = indicatorValue / ClubParams.MaxParamValue;
                _indicator.fillAmount = indicatorValueNormalized;
                yield return null;
            }
            while (animationFrameData.IsAnimationEnd);
        }

        private IEnumerator RoutineFillDecreaseAnimation(int delta)
        {
            float startTime = Time.time;
            float startValue = Value;
            float endValue = Value + delta;
            Value += delta;
            AnimationFrameData animationFrameData;
            do
            {
                animationFrameData = CalculateValueFactor(
                    startTime,
                    _fillIncreaseAnimationData.Duration,
                    _fillIncreaseAnimationData.AnimationCurve);
                float indicatorValue = Mathf.Lerp(
                    startValue,
                    endValue,
                    animationFrameData.AnimationCurveValue);
                float indicatorValueNormalized = indicatorValue / ClubParams.MaxParamValue;
                _indicator.fillAmount = indicatorValueNormalized;
                yield return null;
            }
            while (animationFrameData.IsAnimationEnd);
        }

        private static AnimationFrameData CalculateValueFactor(
            float startTime,
            float duration,
            AnimationCurve animationCurve)
        {
            float timeLeft = Time.time - startTime;
            float timeLeftNormalizedUnclamped = timeLeft / duration;
            float timeLeftNormalized = Mathf.Clamp01(timeLeftNormalizedUnclamped);
            float animationCurveValue = animationCurve.Evaluate(timeLeftNormalized);
            return new AnimationFrameData(timeLeft, timeLeftNormalized, animationCurveValue);
        }
    }
}
