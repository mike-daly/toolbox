using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rotateLeft
{
    class matrix
    {
        public int[][] val;

        public override string ToString()
        {
            return base.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }

        matrix RotateLeft(matrix mat)
        {
            for (int ring = 0; ring < mat.val.Length/2; ring++)
            {
                for( int i = 0;i< mat.val[ring].Length - 1;i++)
                {
                    // hold = upper left
                    // upper left = upper right
                    // upper right = lower right
                    // lower right = lower left
                    // lower left = hold
                    int hold = mat.val[ring + i][ring + i];
                    mat.val[ring + i][ring + i] = mat.val[ring+i][mat.val[ring].Length-1-ring-i];
                    mat.val[ring+i][mat.val[ring].Length-1-ring-i] = mat.val[]

                }

            }
        }
    }

    
}
