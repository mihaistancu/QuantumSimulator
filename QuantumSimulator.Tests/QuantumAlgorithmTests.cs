using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace QuantumSimulator.Tests
{
    [TestClass]
    public class QuantumAlgorithmTests
    {
        [TestMethod]
        public void x()
        {
            Identity i1 = new Identity();
            Identity i2 = new Identity();

            Operation bigun = i1.TensorProduct(i2);
            Assert.AreEqual(4, bigun.elements.GetLength(0));
            for (int i = 0; i < 4; ++i)
            {

                for (int j = 0; j < 4; ++j)
                {
                    if (i == j)
                        Assert.AreEqual(1, bigun.elements[i, j]);
                    else
                        Assert.AreEqual(0, bigun.elements[i, j]);
                }
            }

            Operation evenbiggerun = bigun.TensorProduct(new Identity());

            Assert.AreEqual(8, evenbiggerun.elements.GetLength(0));
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    if (i == j)
                        Assert.AreEqual(1, evenbiggerun.elements[i, j]);
                    else
                        Assert.AreEqual(0, evenbiggerun.elements[i, j]);
                }
            }
        }

        [TestMethod]
        public void measuretest()
        {
            Random r = new Random();
            var values = new Dictionary<string, int>();
            var control = new Dictionary<int, int>();
            control.Add(0, 0);
            control.Add(1, 0);
            for (int k = 0; k < 10000; k++)
            {

                var d = r.NextDouble();
                if (d < 0.5)
                    control[0]++;
                else
                    control[1]++;
                var register = new Register("0");
                register.Apply(new Hadamard());

                var result = register.Measure();

                if (!values.ContainsKey(result))
                    values.Add(result, 0);
                values[result]++;
            }
        }

        [TestMethod]
        public void y()
        {
            var i = new Identity();
            var h = new Hadamard();

            var result = h.TensorProduct(i);
        }

        [TestMethod]
        public void SimonsAlgorithm()
        {
            Dictionary<string, int> values = new Dictionary<string, int>();

            for (int k = 0; k < 1000; k++)
            {
                var register = new Register("0000");
                Operation hadamard = new Hadamard().TensorProduct(new Hadamard())
                    .TensorProduct(new Identity()).TensorProduct(new Identity());

                register.Apply(hadamard);
                register.Apply(Uf4);
                register.Measure(2);
                register.Measure(3);
                register.Apply(hadamard);

                string result = register.Measure();
                if (!values.ContainsKey(result))
                    values.Add(result, 0);
                values[result]++;
            }

            Assert.AreEqual(4, values.Keys.Count());
        }

        private int Uf4(int x)
        {
            switch (x)
            {
                case 0:
                    return 1;
                case 1:
                    return 0; //function must be a permutation
                case 4:
                    return 6;
                case 6:
                    return 4; //function must be a permutation
                case 8:
                    return 9;
                case 9:
                    return 8; //function must be a permutation
                case 12:
                    return 14;
                case 14:
                    return 12; //function must be a permutation
            }
            return x;
        }

        private int Uf2(int x)
        {
            switch (x) // s = 110                 
            {
                //  x   0  |  x  f(x)              
                case 0:
                    return 0 + 4; // 000 000 | 000 100              
                case 8:
                    return 8 + 6; // 001 000 | 001 110              
                case 16:
                    return 16 + 3; // 010 000 | 010 011              
                case 24:
                    return 24 + 1; // 011 000 | 011 001              
                case 32:
                    return 32 + 3; // 100 000 | 100 011              
                case 40:
                    return 40 + 1; // 101 000 | 101 001              
                case 48:
                    return 48 + 4; // 110 000 | 110 100
                case 56:
                    return 56 + 6; // 111 000 | 111 110
            }
            return x;
        }

        private const string u = "101110";

        [TestMethod]
        public void BernsteinVaziraniAlgorithm()
        {
            var register = new Register("0000001");
            Operation hadamard = new Hadamard(register.Length());

            for (int i = 0; i < register.Length() - 1; i++)
            {
                hadamard = hadamard.TensorProduct(new Hadamard());
            }

            register.Apply(hadamard);
            register.Apply(Uf);
            register.Apply(hadamard);

            string result = register.Measure();
            Assert.IsTrue(result.StartsWith(u));
        }

        private int CountBits(int a)
        {
            return Convert.ToString(a, 2).ToCharArray().Count(b => b == '1');
        }

        private int Uf(int x)
        {
            int iu = Convert.ToInt32(u, 2);

            var f = (1 & x) ^ (CountBits(iu & (x >> 1))%2);

            x = x >> 1;

            return (x << 1) + (f > 0 ? 1 : 0);
        }

        [TestMethod]
        public void GroversAlgorithm()
        {

            var register = new Register("00000001");
            int n = register.Length();
            Operation hadamard = new Hadamard();

            register.Apply(hadamard, n - 1);

            int significant_qbits = n - 1;
            for (int i = 0; i < significant_qbits - 1; i++)
            {
                hadamard = hadamard.TensorProduct(new Hadamard());
            }
            hadamard = hadamard.TensorProduct(new Identity());

            Operation bigun = new Identity();
            for (int i = 0; i < significant_qbits - 1; ++i)
                bigun = bigun.TensorProduct(new Identity());
            bigun.Multiply(-1);
            bigun.elements[0, 0] = 1;
            bigun = bigun.TensorProduct(new Identity());

            int times = (int) Math.Sqrt(Math.Pow(2, (n - 1)));
            register.Apply(hadamard);
            for (int i = 0; i < times; ++i)
            {
                register.Apply(Uf5);

                register.Apply(hadamard);
                register.Apply(bigun);
                register.Apply(hadamard);
            }

            var result = register.Measure();
        }

        private int Uf5(int x)
        {
            const int needle = 42;

            if (x == needle * 2)
            {
                return needle * 2 + 1;
            }
            if (x == needle * 2 + 1)
            {
                return needle * 2;
            }
            return x;
        }
    }
}