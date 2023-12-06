using System;
using UnityEngine;
using UnityEngine.Localization;

namespace KinoCube.KinoReigns
{
    [Serializable]
    public sealed class DilemmaAction
    {
        [field: Header("Text:")]
        [field: SerializeField] public LocalizedString ActionDescription { get; private set; }

        [field: Header("Params Change:")]
        [field: Range(ClubParams.NegativeMaxParamValue, ClubParams.MaxParamValue)]
        [field: SerializeField]
        public int Money { get; private set; }
        [field: Range(ClubParams.NegativeMaxParamValue, ClubParams.MaxParamValue)]
        [field: SerializeField]
        public int Audience { get; private set; }
        [field: Range(ClubParams.NegativeMaxParamValue, ClubParams.MaxParamValue)]
        [field: SerializeField]
        public int Team { get; private set; }
        [field: Range(ClubParams.NegativeMaxParamValue, ClubParams.MaxParamValue)]
        [field: SerializeField]
        public int Confidence { get; private set; }
    }
}
