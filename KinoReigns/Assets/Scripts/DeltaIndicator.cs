using UnityEngine;
using UnityEngine.UI;

namespace KinoCube.KinoReigns
{
    public sealed class DeltaIndicator : MonoBehaviour
    {
        [SerializeField] private Image _smallIndicator;
        [SerializeField] private Image _largeIndicator;
        [SerializeField] private int _threshold = 5;
        [SerializeField] private float _alphaFactor = 2.0f;

        private const float _defaultIndicatorAlpha = 0.0f;

        public void ChangeIndicatorAlpha(int delta, float alpha)
        {
            alpha *= _alphaFactor;
            int deltaAbs = Mathf.Abs(delta);
            if (deltaAbs > _threshold)
            {
                SetAlpha(_smallIndicator, _defaultIndicatorAlpha);
                SetAlpha(_largeIndicator, alpha);
            }
            else if (deltaAbs > 0)
            {
                SetAlpha(_largeIndicator, _defaultIndicatorAlpha);
                SetAlpha(_smallIndicator, alpha);
            }
            else
            {
                SetAlpha(_smallIndicator, _defaultIndicatorAlpha);
                SetAlpha(_largeIndicator, _defaultIndicatorAlpha);
            }
        }

        private void SetAlpha(Image image, float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}
