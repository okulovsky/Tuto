using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Youtube
{
    public class Join3<TInner, TMiddle, TOuter, TResult>
        where TInner : class
        where TMiddle : class
        where TResult : class
        where TOuter : class
    {
        public Func<TInner, TMiddle, bool> InnerComparator;
        public Func<TOuter, TMiddle, bool> OuterComparator;
        public Func<TInner, TOuter, TMiddle> CreateLink;
        public Func<TInner, TMiddle, TOuter, Status, TResult> CreateResult;
        public Func<TInner, TOuter, double> GetMatch;
        public List<TInner> Inner;
        public List<TMiddle> Middle;
        public List<TOuter> Outer;
        public List<TResult> Result;

        TResult Account(TInner innerValue, TMiddle middleValue, TOuter outerValue, Status status)
        {
            var result = CreateResult(innerValue, middleValue, outerValue, status);
            Result.Add(result);
            if (innerValue!=null) Inner.Remove(innerValue);
            if (middleValue!=null) Middle.Remove(middleValue);
            if (outerValue!=null) Outer.Remove(outerValue);
            return result;
        }

        const double MatchLowerLimit = 0.5;

       

        TResult FindMatchThroughPub(TInner fin)
        {
            var pub = Middle.FirstOrDefault(z => InnerComparator(fin,z));
            if (pub != null)
            {
                var clip = Outer.FirstOrDefault(z => OuterComparator(z,pub));
                if (clip == null)
                {
                    var newMatch = FindNewMatch(fin);
                    if (newMatch == null)
                        return Account(fin, pub, null, Status.DeletedFromYoutube);
                    else
                        return newMatch;
                }
                return Account(
                        fin,
                        pub,
                        clip,
                        clip == null ? Status.DeletedFromYoutube : Status.MatchedOld);

            }
            return null;
        }

        TResult FindNewMatch(TInner fin)
        {
            var bestMatch = Outer
                        .Select(z => Tuple.Create(z, GetMatch(fin, z)))
                        .OrderByDescending(z => z.Item2)
                        .FirstOrDefault();
            //the match is being installed right now
            if (bestMatch != null && bestMatch.Item2 > MatchLowerLimit)
            {
                var pub1 = CreateLink(fin, bestMatch.Item1);
                return Account(fin, pub1, bestMatch.Item1, Status.MatchedNew);
            }
            return null;
        }

        public List<TResult> Run()
        {
            Result = new List<TResult>();
            while (Inner.Count != 0)
            {
                if (Inner[0].ToString() == "Строки")
                    Console.Write("!");
                var fin = Inner[0];

                var match = FindMatchThroughPub( fin);
                if (match == null) match = FindNewMatch( fin);
                if (match == null) match = Account(fin, null, null, Status.NotFoundAtYoutube);
            }

            while (Outer.Count != 0)
            {
                var clip = Outer[0];
                var pub = Middle.FirstOrDefault(z => OuterComparator(clip, z));
                Account(null,pub, clip,
                    pub == null ? Status.NotExpectedAtYoutube : Status.DeletedFromTuto);
            }

            while (Middle.Count != 0)
            {
                Account(null, Middle[0], null, Status.DeletedFromBoth);
            }

            return Result;
        }
    }


}