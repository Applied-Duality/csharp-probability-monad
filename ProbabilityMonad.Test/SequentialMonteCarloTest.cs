﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProbabilityMonad.Base;
using ProbabilityMonad.Test.Models;
using System.Diagnostics;
using static ProbabilityMonad.Test.Models.LinearRegression;
using static ProbabilityMonad.Test.Models.HiddenMarkovModel;

namespace ProbabilityMonad.Test
{
    [TestClass]
    public class SequentialMonteCarloTest
    {
        [TestMethod]
        public void Smc_Dice()
        {
            // Exact posterior conditioned die
            var twoRollsExact = Dice.ConditionalDieExact(2);
            Debug.WriteLine(twoRollsExact.Histogram());

            // Posterior inferred with SMC
            var twoRolls = Dice.ConditionalDie(2);
            var smcPosterior = twoRolls.SmcMultiple(100, 100);
            var sampled = smcPosterior.Sample();
            var weights = sampled.MapSample(sum => (double) sum)
                                 .Weights;

            var klDivergence = KullbackLeibner.KLDivergence(sampled, twoRollsExact, d => d);
            Assert.IsTrue(klDivergence < 0.5);
            Debug.WriteLine($"KL divergence: {klDivergence}");
            Debug.WriteLine(Histogram.Weighted(weights, scale: 60));
        }

        [TestMethod]
        public void Smc_LinReg()
        {
            var prior = from a in Normal(0, 100)
                        from b in Normal(0, 100)
                        select new Param(a, b);

            var sandLinReg = CreateLinearRegression(prior, BeachSandData);

            var smcLinReg = sandLinReg.SmcMultiple(1000, 50).Sample();

            var paramA = smcLinReg.MapSample(param => param.a);
            var paramB = smcLinReg.MapSample(param => param.b);

            Debug.WriteLine(Histogram.Weighted(paramA));
            Debug.WriteLine(Histogram.Weighted(paramB));
        }

        [TestMethod]
        public void Smc_HiddenMarkov()
        {
            var hmmModel = Hmm(ObservedHmmData1);

            var smcHmmSamples = hmmModel.SmcMultiple(100, 500).Sample();

            var topSamples = Samples(smcHmmSamples
                .GroupBy(ip => ShowLatentList(ip.Item))
                .OrderByDescending(group => group.First().Prob.Value)
                .Take(5).Select(g => g.First()));

            Debug.WriteLine(Histogram.Finite(topSamples, ShowLatentList));
        }
    }
}