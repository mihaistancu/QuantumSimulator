using System;
using System.Numerics;

namespace QuantumSimulator
{
    public class Register
    {
        private Complex[] p;
        private readonly int n;

        public int Length()
        {
            return n;
        }

        public Register(int n)
        {
            this.n = n;
            p = new Complex[(int)Math.Pow(2, n)];
        }

        public Register(string qbits)
            :this(qbits.Length)
        {
            int index = Convert.ToInt32(qbits, 2);
            p[index] = 1;
        }

        public void Apply(Operation op, int index)
        {
            Operation bigun = new Identity();
            // I x I x ... x I x OP x I x I x ... x I
            for(int i = 0; i < n; ++i)
            {
                if (i != index)
                    bigun = bigun.TensorProduct(new Identity());
                else
                    bigun = bigun.TensorProduct(op);
            }
            Apply(bigun);
        }

        public void Apply(Operation operation)
        {
            var t = p.Clone() as Complex[];
            
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = 0;

                for (int j = 0; j < p.Length; j++)
                {
                    p[i] += operation.elements[i,j] * t[j];
                }
            }
        }

        public void Apply(Func<int, int> function)
        {
            var t = p.Clone() as Complex[];

            for (int i = 0; i < p.Length; i++)
            {
                var result = function(i);
                p[result] = t[i];
            }
        }

        public string Measure()
        {
            var result = "";

            for (int i = 0; i < n; i++)
            {
                result += Measure(i);
            }

            return result;
        }

        public int Measure(int index)
        {
            //compute probability to get 0
            double p0 = ProbabilityFor0(index);

            //get random bit with given probability
            var measuredBit = MeasurementResult(p0);

            Normalize(index, measuredBit);

            return measuredBit;
        }

        private double ProbabilityFor0(int index)
        {
            double result=0.0;
            for (int i = 0; i < p.Length; i++)
            {
                if (Is(0, i, index))
                {
                    result += p[i].Magnitude*p[i].Magnitude;
                }
            }
            return result;
        }

        static Random random = new Random();
        private static int MeasurementResult(double probabilityFor0)
        {
            //var random = new Random();
            var value = random.NextDouble();
            int measuredBit = value < probabilityFor0 ? 0 : 1;
            return measuredBit;
        }

        private void Normalize(int index, int measuredBit)
        {
            // create PD with p
            ProbabilityDistribution<int, Complex> pd = new ProbabilityDistribution<int, Complex>();
            for(int i = 0; i < p.Length; ++i)
            {
                pd.Add(i, p[i].Magnitude*p[i].Magnitude);
            }
            pd.RemoveIf(state => !Is(measuredBit, state, index));

            // normalize pd
            pd.Normalize((c => c.Magnitude * c.Magnitude), ((c, total) => c / Math.Sqrt(total)));

            // update p with pd
            for (int i = 0; i < p.Length; ++i)
            {
                p[i] = pd[i];
            }
        }

        private bool Is(int toCompare, int number, int bitIndex)
        {
            return toCompare == BitAt(number, bitIndex);
        }

        private int BitAt(int number, int index)
        {
            index = n - index -1; //voodoo
            int mask = 1 << index;
            int t = (number & mask);
            t = t >> index;
            return t;
        }
    }
}