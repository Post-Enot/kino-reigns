using UnityEngine;
using UnityEngine.Localization;

namespace KinoCube.KinoReigns
{
    [CreateAssetMenu(fileName = "Dilemma", menuName = "Data Assets/Dilemma")]
    public sealed class Dilemma : ScriptableObject
    {
        [field: Header("Visual:")]
        [field: SerializeField] public Sprite ActorSprite { get; private set; }
        [field: SerializeField] public Color BackgroundColor { get; private set; }

        [field: Header("Text:")]
        [field: SerializeField] public LocalizedString Description { get; private set; }
        [field: SerializeField] public LocalizedString ActorName { get; private set; }

        [field: Header("Actions:")]
        [field: SerializeField] public DilemmaAction LeftAction { get; private set; }
        [field: SerializeField] public DilemmaAction RightAction { get; private set; }
    }
}
