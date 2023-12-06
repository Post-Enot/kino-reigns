using System;
using UnityEngine;

namespace KinoCube.KinoReigns
{
    public sealed class ClubParams : MonoBehaviour
    {
        public const int MinParamValue = 0;
        public const int MaxParamValue = 100;
        public const int DefaultParamValue = MaxParamValue / 2;
        public const int NegativeMaxParamValue = -MaxParamValue;

        [field: Header("Params:")]
        [field: Range(MinParamValue, MaxParamValue)][field: SerializeField]
        public int Money { get; set; } = DefaultParamValue;
        [field: Range(MinParamValue, MaxParamValue)][field: SerializeField]
        public int Audience { get; set; } = DefaultParamValue;
        [field: Range(MinParamValue, MaxParamValue)][field: SerializeField]
        public int Team { get; set; } = DefaultParamValue;
        [field: Range(MinParamValue, MaxParamValue)][field: SerializeField]
        public int Confidence { get; set; } = DefaultParamValue;

        public bool SomeParamsOutOfRange
        {
            get
            {
                return !(ParamInRange(Money) &&
                    ParamInRange(Audience) &&
                    ParamInRange(Team) &&
                    ParamInRange(Confidence));
            }
        }

        public void ResetParams()
        {
            Money = DefaultParamValue;
            Audience = DefaultParamValue;
            Team = DefaultParamValue;
            Confidence = DefaultParamValue;
        }

        private bool ParamInRange(int value)
        {
            return (value > MinParamValue) && (value < MaxParamValue);
        }
    }
}
