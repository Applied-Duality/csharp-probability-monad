﻿using System;
using static ProbCSharp.ProbBase;
using System.Security.Cryptography;

namespace ProbCSharp
{
    /// <summary>
    /// Normal distribution
    /// </summary>
    public class NormalC : SampleableDist<double>
    {
        public double Mean { get; }
        public double Variance { get; }
        public Random Gen {get;}
        public NormalC(double mean, double variance, Random gen)
        {
            Mean = mean;
            Variance = variance;
            Gen = gen;
        }

        public Func<double> Sample
        {
            get
            {
                return () => MathNet.Numerics.Distributions.Normal
                        .WithMeanVariance(Mean, Variance, Gen)
                        .Sample();
            }
        }
    }

    /// <summary>
    /// Beta distribution
    /// </summary>
    public class BetaC : SampleableDist<double>
    {
        public double alpha;
        public double beta;
        public MathNet.Numerics.Distributions.Beta dist;
        public BetaC(double alpha, double beta, Random gen)
        {
            this.alpha = alpha;
            this.beta = beta;
            dist = new MathNet.Numerics.Distributions.Beta(alpha, beta, gen);
        }

        public Func<double> Sample
        {
            get { return () => dist.Sample(); }
        }
    }
}
