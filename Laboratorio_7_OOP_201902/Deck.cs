using Laboratorio_7_OOP_201902.Cards;
using Laboratorio_7_OOP_201902.Enums;
using Laboratorio_7_OOP_201902.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laboratorio_7_OOP_201902
{
    public class Deck:ICharacteristics
    {

        private List<Card> cards;

        public Deck()
        {
        
        }

        public List<Card> Cards { get => cards; set => cards = value; }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }
        public void DestroyCard(int cardId)
        {
            cards.RemoveAt(cardId);
        }

        

        public void Shuffle()
        {
            Random random = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        public List<string> GetCharacteristics()
        {
            List<String> characteristics = new List<string> { };

            string totalCards =Convert.ToString(Cards.Count());
            characteristics.Add(totalCards);

            IEnumerable<Card> auxtotalCardsMelee =
                from card in Cards
                where card.Type == EnumType.melee
                select card;

            string totalCardsMelee = Convert.ToString(auxtotalCardsMelee.Count());
            characteristics.Add(totalCardsMelee);

            IEnumerable<Card> auxtotalCardsRange =
                from card in Cards
                where card.Type == EnumType.range
                select card;

            string totalCardsRange = Convert.ToString(auxtotalCardsRange.Count());
            characteristics.Add(totalCardsRange);

            IEnumerable<Card> auxtotalCardsLongRange =
                from card in Cards
                where card.Type == EnumType.longRange
                select card;

            string totalCardsLongRange = Convert.ToString(auxtotalCardsLongRange.Count());
            characteristics.Add(totalCardsLongRange);

            IEnumerable<Card> auxtotalCardsBuff =
                from card in Cards
                where card.Type == EnumType.buff
                select card;

            string totalCardsBuff = Convert.ToString(auxtotalCardsBuff.Count());
            characteristics.Add(totalCardsBuff);

            IEnumerable<Card> auxtotalCardsWeather =
                from card in Cards
                where card.Type == EnumType.weather
                select card;

            string totalCardsWeather = Convert.ToString(auxtotalCardsWeather.Count());
            characteristics.Add(totalCardsWeather);

            IEnumerable<CombatCard> auxtotalAttackPointMelee =
                from card in auxtotalCardsMelee
                select card as CombatCard;

            IEnumerable<int> auxauxtotalAttackPointMelee =
               from card in auxtotalAttackPointMelee
               select card.AttackPoints;

            string totalAttackPointMelee = Convert.ToString(auxauxtotalAttackPointMelee.Sum());
            characteristics.Add(totalAttackPointMelee);

            IEnumerable<CombatCard> auxtotalAttackPointRange =
                from card in auxtotalCardsRange
                select card as CombatCard;

            IEnumerable<int> auxauxtotalAttackPointRange =
               from card in auxtotalAttackPointRange
               select card.AttackPoints;

            string totalAttackPointRange= Convert.ToString(auxauxtotalAttackPointRange.Sum());
            characteristics.Add(totalAttackPointRange);

            IEnumerable<CombatCard> auxtotalAttackPointLongRange =
                from card in auxtotalCardsLongRange
                select card as CombatCard;

            IEnumerable<int> auxauxtotalAttackPointLongRange =
               from card in auxtotalAttackPointLongRange
               select card.AttackPoints;
            string totalAttackPointLongRange = Convert.ToString(auxauxtotalAttackPointRange.Sum());
            characteristics.Add(totalAttackPointLongRange);

            string totalAttackPoint =Convert.ToString( auxauxtotalAttackPointMelee.Sum() + auxauxtotalAttackPointRange.Sum() + auxauxtotalAttackPointRange.Sum());
            characteristics.Add(totalAttackPoint);

            return characteristics;
        }
    }
}
