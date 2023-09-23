using System;
using System.Collections.Generic;
using System.Text;

namespace simplexe
{
    class Utility
    {
        public void EliminateRow()
        {

        }

        public void checkBase()
        {

        }

        public static int getNegatifCN(List<double> z)
        {
            for(int i = 0; i< z.Count;i++)
            {
                if(z[i] < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int geMinToEnterBase(List<List<double>> matrice,int entreDansLaBase,int nbVariable)
        {
            var index = 1;
            for(int i = 1;i<matrice.Count - 1;i++)
            {
                var a = matrice[index][nbVariable - 1] / matrice[index][entreDansLaBase];
                var b = matrice[i + 1][nbVariable - 1] / matrice[i + 1][entreDansLaBase];
                if(a < 0)
                {
                    index++;
                }
                if (a > 0 && b >0)
                {
                    Console.WriteLine("Positif:" + a + " " + b);
                    index = Compare(a,b) ? index : (i + 1);
                }
            }
            Console.WriteLine("INDEX : " + (index - 1));
            return index - 1;
        }

        private static bool Compare(double a,double b)
        {
            if(a > b)
            {
                return false;
            }
            return true;
        }

        // Elimination de ligne pour former une nouvelle matrice
        public static List<List<double>> EliminateRow(List<List<double>> matrice,int baseCol,int baseLigne)
        {
            var _matrice = matrice;
            var tmp_base = _matrice[baseLigne][baseCol];
            for (int i = 0;i< matrice.Count; i++)
            {

                var BaseElimination = -_matrice[i][baseCol] / _matrice[baseLigne][baseCol];
                for (int j=0;j< _matrice[i].Count;j++)
                {
                    if(i!=baseLigne)
                    {
                        _matrice[i][j] = _matrice[i][j] + BaseElimination* _matrice[baseLigne][j];
                    }else
                    {
                        _matrice[i][j] = (1 / tmp_base) * _matrice[i][j];
                    }
                }
            }
            return _matrice;
        }


        
    }
}
