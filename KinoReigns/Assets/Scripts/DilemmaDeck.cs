using System.Collections.Generic;
using UnityEngine;

namespace KinoCube.KinoReigns
{
    public sealed class DilemmaDeck : MonoBehaviour
    {
        [SerializeField] private List<Dilemma> _cards;

        public Dilemma GetNextCard()
        {
            int cardIndex = Random.Range(0, _cards.Count);
            return _cards[cardIndex];
        }
    }
}
