using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOE4
{
    class Program
    {
        static void Main(string[] args)
        {
            int result1 = 0;
            int result2 = 0;

            string fileloc = @"data\input.txt";

            List<Card> cards = new List<Card>();
            foreach (var line in File.ReadLines(fileloc))
            {
                var parts = line.Split(": ");
                int cardNum = Int32.Parse(parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]);

                var nums = parts[1].Split(" | ");
                var luckyNumbers = nums[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).OfType<string>().ToList();
                var numbers = nums[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).OfType<string>().ToList();

                cards.Add(new Card(cardNum, luckyNumbers, numbers));
            }

            var cardNumbers = cards.Select(c => c.CardNumber);

            //part1
            foreach (var card in cards)
            {
                result1 += card.Score();
            }

            Console.WriteLine(result1);

            //part2 
            for (int i = 0; i < cards.Count(); ++i)
            {
                var card = cards[i];
                int winningNumbersCount = card.WinningNumbersCount();
                var cardsToChange = cards
                    .Where(c => c.CardNumber > card.CardNumber && c.CardNumber <= card.CardNumber + winningNumbersCount)
                    .ToList();

                foreach(var changeCard in cardsToChange)
                {
                    changeCard.Copies += card.Copies;
                }
            }

            result2 = cards.Select(c => c.Copies).Sum();
            
            Console.WriteLine(result2);
        }

        public class Card
        {
            public Card(int cardNum, List<string> luckyNumbers, List<string> numbers)
            {
                CardNumber = cardNum;
                LuckyNumbers = luckyNumbers;
                Numbers = numbers;
                Copies = 1;
            }

            public int CardNumber { get; }
            public int Copies { get; set; }
            public List<string> LuckyNumbers { get; }
            public List<string> Numbers { get; }

            public int WinningNumbersCount()
            {
                int i = 0;
                foreach (var number in Numbers)
                {
                    if (LuckyNumbers.Contains(number)) i++;
                }
                return i;
            }

            public int Score()
            {
                int i = WinningNumbersCount();
                return i < 2 ? i : (int)Math.Pow(2, i - 1);
            }

            public override bool Equals(object obj)
            {
                Card card = obj as Card;

                if (card == null)
                {
                    return false;
                }

                return card.CardNumber == this.CardNumber;
            }

            public override int GetHashCode()
            {
                return this.CardNumber.GetHashCode();
            }
        }
    }
}
