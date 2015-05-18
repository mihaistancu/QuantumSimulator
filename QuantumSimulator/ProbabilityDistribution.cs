using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantumSimulator
{
    public class ProbabilityDistribution<T,U> where U:new()
    {
        private Dictionary<T, U> p = new Dictionary<T, U>();
        public void Add(T value, U probability)
        {
            p[value] = probability;
        }
        // p1, p2, p3... pi
        // sum(p(i)) = 1

        // sum(p(i)) = N
        // N = 1 <=> pi = pi/N

        public void Normalize(Func<U,double> pFunc, Func<U, double, U> pDiv  )
        {
            double total = p.Sum(e => pFunc(e.Value));
            var keys = p.Keys.ToList();
            foreach (var k in keys)
            {
                p[k] = pDiv(p[k], total);
            }
        }

        public void Remove(T value)
        {
            p.Remove(value);
        }

        public U this[T value]
        {
            get { 
                if (p.ContainsKey(value))
                    return p[value];
                else
                    return new U();
            }
        }

        public void RemoveIf(Func<T, bool> condition)
        {
            p = p.Where(kp => !condition(kp.Key)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}