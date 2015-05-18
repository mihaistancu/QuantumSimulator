using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace QuantumSimulator
{
    public class Identity: Operation
    {        
        public Identity()
        {
            elements = new Complex[,]
            {
                {1,0},
                {0,1}
            };
        }
    }
}
