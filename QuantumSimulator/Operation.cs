using System.Numerics;

namespace QuantumSimulator
{
    public class Operation
    {
        public Complex[,] elements;
        
        //public Complex At(int i, int j, int n)
        //{
        //    if (n == 1)
        //    {
        //        return 1;
        //    }

        //    int di = 0, dj = 0;

        //    if (i >= n/2)
        //    {
        //        di = 1;
        //        i = i - n/2;
        //    }

        //    if (j >= n/2)
        //    {
        //        dj = 1;
        //        j = j - n/2;
        //    }

        //    return elements[di, dj]*At(i, j, n/2);
        //}

        public Operation TensorProduct(Operation operation)
        {
            int dim_a = this.elements.GetLength(0);
            int dim_b = operation.elements.GetLength(0);
            int resultSize =dim_a * dim_b;
            Operation result = new Operation();
            result.elements = new Complex[resultSize, resultSize];

            for (int row_a = 0; row_a < dim_a; ++row_a)
            {
                for (int col_a = 0; col_a < dim_a; ++col_a)
                {
                    int row_start = row_a * dim_b;
                    int col_start = col_a * dim_b;
                    Complex a = this.elements[row_a, col_a];

                    for (int row_b = 0; row_b < dim_b; ++row_b)
                    {
                        for (int col_b = 0; col_b < dim_b; ++col_b)
                        {
                            int row = row_start + row_b;
                            int col = col_start + col_b;
                            Complex b = operation.elements[row_b, col_b];
                            Complex value = a * b;
                            result.elements[row, col] = value;
                        }
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            string result = "";

            for (int i=0;i<elements.GetLength(0); i++)
            {
                for (int j = 0; j < elements.GetLength(0); j++)
                {
                    result += elements[i, j] + " ";
                }
                result += "\n";
            }

            return result;
        }

        public void Multiply(int m)
        {
            for (int i = 0; i < elements.GetLength(0); i++)
            {
                for (int j = 0; j < elements.GetLength(0); j++)
                {
                    elements[i,j] *= m;
                }
            }
        }   
    }
}