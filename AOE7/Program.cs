using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOE7
{
    class Program
    {
        static void Main(string[] args)
        {
            long result1 = 0;
            long result2 = 0;

            string fileloc = @"data\input.txt";
            List<HandBid> handBids1 = new List<HandBid>();
            List<HandBid> handBids2 = new List<HandBid>();

            foreach (var line in File.ReadLines(fileloc))
            {
                var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                handBids1.Add(new HandBid(parts[0], Int64.Parse(parts[1]), DefineHandType1));
                handBids2.Add(new HandBid(parts[0], Int64.Parse(parts[1]), DefineHandType2));
            }

            //part 1 and 2
            handBids1.Sort(Comparison1);
            handBids2.Sort(Comparison2);

            for (int i = 0; i < handBids1.Count; ++i)
            {
                int weight = (i + 1);
                result1 += weight * handBids1[i].Bid;
                result2 += weight * handBids2[i].Bid;
            }

            Console.WriteLine(result1);
            Console.WriteLine(result2);

        }

        // Index of character defines value of card
        private static List<char> cardsStrength1 = new List<char>() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
        private static List<char> cardsStrength2 = new List<char>() { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };

        // Hand type defines value of cards in hand
        public enum HandType
        {
            HighCard = 0,
            OnePair = 1,
            TwoPairs = 2,
            ThreeOfKind = 3,
            FullHouse = 4,
            FourOfKind = 5,
            FiveOfKind = 6,
        }

        static public HandType DefineHandType1(string hand)
        {
            var charHist = hand.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            return GetHandType(charHist.Values.ToList());
        }


        static public HandType DefineHandType2(string hand)
        {
            var charHist = hand.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

            int charHistCount = charHist.Count();
            if (charHistCount <= 5 && charHistCount > 1 && charHist.ContainsKey('J'))
            {
                int jCount = charHist['J'];
                charHist.Remove('J');
                char maxHist = charHist.OrderByDescending(kvp => kvp.Value).ThenByDescending(kvp => cardsStrength2.IndexOf(kvp.Key)).First().Key;
                charHist[maxHist] += jCount;
            }

            return GetHandType(charHist.Values.ToList());
        }

        static private HandType GetHandType(List<int> charsNum)
        {
            if (charsNum.Count == 1) return HandType.FiveOfKind;
            if (charsNum.Count == 2)
            {
                if (charsNum.Contains(4)) return HandType.FourOfKind;
                else return HandType.FullHouse;
            }
            if (charsNum.Count == 3)
            {
                if (charsNum.Contains(3)) return HandType.ThreeOfKind;
                else return HandType.TwoPairs;
            }
            if (charsNum.Count == 4) return HandType.OnePair;

            return HandType.HighCard;
        }

        static public int Comparison1(HandBid h1, HandBid h2)
        {
            return Comparison(h1, h2, cardsStrength1);
        }

        static public int Comparison2(HandBid h1, HandBid h2)
        {
            return Comparison(h1, h2, cardsStrength2);
        }

        static private int Comparison(HandBid h1, HandBid h2, List<char> cardsStrength)
        {
            if (h1 != null && h2 != null)
            {
                if (h1.HandType == h2.HandType)
                {
                    for (int i = 0; i < h1.Hand.Length; ++i)
                    {
                        var value1 = cardsStrength.IndexOf(h1.Hand[i]);
                        var value2 = cardsStrength.IndexOf(h2.Hand[i]);
                        if (value1 == value2) continue;
                        return value1.CompareTo(value2);
                    }

                    return 0;
                }

                return h1.HandType.CompareTo(h2.HandType);
            }
            else
                throw new ArgumentException("Object is not a HandBid");
        }

        public class HandBid
        {
            public string Hand { get; }
            public long Bid { get; }
            public HandType HandType { get; set; }

            public HandBid(string hand, long bid, Func<string, HandType> DefineHandType)
            {
                Hand = hand;
                Bid = bid;
                HandType = DefineHandType(hand);
            } 
        }
    }
}
