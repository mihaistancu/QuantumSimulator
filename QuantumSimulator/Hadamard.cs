using System;
using System.Numerics;

namespace QuantumSimulator
{
    public class Hadamard: Operation
    {
        public Hadamard()
        {
            elements = new Complex[,]
            {
                {1/Math.Sqrt(2), 1/Math.Sqrt(2)},
                {1/Math.Sqrt(2), -1/Math.Sqrt(2)}
            };
        }
    }
}