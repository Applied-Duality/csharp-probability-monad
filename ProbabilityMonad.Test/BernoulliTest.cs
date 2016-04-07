﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProbabilityMonad;
using System.Diagnostics;
using static ProbabilityMonad.Base;
using System.Linq;
using System.Collections.Generic;
using static ProbabilityMonad.Test.Models.CoinExt;
using ProbabilityMonad.Test.Models;

namespace ProbabilityMonad.Test
{
    [TestClass]
    public class BernoulliTest
    {
        [TestMethod]
        public void CoinFlip_SumsToOne()
        {
            var coin = BernoulliF(Prob(0.5));
            Assert.AreEqual(1, coin.Explicit.Weights.Select(p => p.Prob.Value).Sum());
        }

        [TestMethod]
        public void CoinToss_InferWeighted()
        {
            // We put a prior on the coin being fair with Pr(0.8).
            // If it's biased, we put a prior on its weight with a Beta distribution.
            var prior = from isFair in Bernoulli(Prob(0.8))
                             from headsProb in isFair ? Return(0.5) : Beta(5, 1)
                             select headsProb;

            // Some coinflips from a fair coin
            var fair = new List<Coin> { Heads, Tails, Tails, Heads, Heads, Tails, Heads, Tails };

            // The posterior weight distribution given these fair flips
            var posteriorWeight = FlipsUpdate(fair, prior);

            // Use a weighted prior to infer the posterior
            var inferredWeight = posteriorWeight.Prior().SampleNParallel(100);
            Debug.WriteLine(Histogram.Weighted(inferredWeight));

            // Do the same with a biased coin
            var biased = new List<Coin> { Tails, Tails, Tails, Tails, Tails, Tails, Heads, Tails };
            var inferredBiasedWeight = FlipsUpdate(biased, prior).Prior().SampleNParallel(100);
            Debug.WriteLine(Histogram.Weighted(inferredBiasedWeight));
        }

        [TestMethod]
        public void CoinToss_Exact()
        {
            // There's an 80% chance we have a fair coin and a 20% chance of a biased coin
            // with an 80% chance of tails.
            var prior = from isFair in BernoulliF(Prob(0.8))
                             from headsProb in isFair ? UniformF(0.5) : UniformF(0.2)
                             select headsProb;

            // Some coinflips from a fair coin
            var fair = new List<Coin> { Heads, Tails, Tails, Heads, Heads, Tails, Heads, Tails };

            // The posterior weight distribution given these fair flips
            var exactPosterior = FlipsUpdateExact(fair, prior);
            Debug.WriteLine(exactPosterior.Histogram());

            // Do the same with a biased coin
            var biased = new List<Coin> { Tails, Tails, Tails, Tails, Tails, Tails, Heads, Tails };
            var biasedPosterior = FlipsUpdateExact(biased, prior);
            Debug.WriteLine(biasedPosterior.Histogram());
        }

    }
}

