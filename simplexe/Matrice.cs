using System;
using System.Collections.Generic;
using System.Text;

namespace simplexe
{
    class Matrice
    {
        // protected double[] z { get; set; }
        public List<List<double>> matrice { get; set; }
        public int nbVariable { get; set; }
        public int nbContrainte { get; set; }
        public List<int> baseIndex { get; set; }

        private int choice = 0;
        public string[] signeContrainte { get; set; }

        public Matrice()
        {
            this.baseIndex = new List<int>();
        }

        public void init()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Supposons que les variables X1,x2,....,XN sont tous POSITIFS pour l'application");
            Console.WriteLine("++++++++++++ET ON NE LES PRECISERA PLUS DANS LE CONTRAINTE :P :P :P++++++++++++");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.ResetColor();
            this.initNbVariable();
            this.initNbContrainte();
            this.initMatriceRange();
            this.initZ();
            this.initContrainte();
            if(Array.Exists(this.signeContrainte,e => e == "="))
            {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("DESOLE, ne peut pas etre resolu pour le moment car besoin d'une base inversible :(");
                return;
            }
            // Affichage de la matrice et des contraintes
            this.printZ();
            this.printContrainte();
            this.generateX();
            this.printBaseIndex();
            this.DoSimplexe();
        }



        // Initialisation du nombre de contrainte
        public void initNbContrainte()
        {
            Console.Write("Entrer le nombre de contrainte du programme : ");
            
            this.nbContrainte = Convert.ToInt32(Console.ReadLine()) + 1;
            this.signeContrainte = new string[5];
        }

        // Initialisation du nombre de variable
        public void initNbVariable()
        {

            Console.Write("Entrer le nombre de variable du programme lineaire : ");
            this.nbVariable = Convert.ToInt32(Console.ReadLine()) + 1;

            Console.WriteLine("Nombre de variable : " + (this.nbVariable - 1));
        }

        public void initMatriceRange()
        {
            // this.z = new double[this.nbVariable];
            this.matrice = new List<List<double>>();

            for (int i = 0; i < this.nbContrainte; i++)
            {
                this.matrice.Add(new List<double>());
            }

        }

        public void initZ()
        {
            List<int> choices = new List<int>(){ 1, 2 };
            // Choix entre maximiser ou minimiser
            while(!choices.Contains(this.choice))
            {
                Console.Write("Choisir entre minimiser ou maximiser : 1-Minimiser 2-Maximiser ==> ");
                this.choice = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Choix :" + this.choice);
            }

            Console.WriteLine("Insertion de z a "+ (this.choice == 1 ? "Minimiser" : "Maximiser") +" : ");
            // Insertion de Z a la ligne 0 de la matrice
            for(int i = 0;i<this.nbVariable - 1;i++)
            {
                Console.Write("Entrer x[" + i + "] : ");
                double z_temp = Convert.ToDouble(Console.ReadLine());
                if(this.choice == 2)
                {
                    z_temp = z_temp * -1;
                }
                this.matrice[0].Add(z_temp);
            }
        }

        public string getSigne(int ?s)
        {
            switch(s)
            {
                case 0:
                    return ">";

                case 1:
                    return ">=";

                case 2:
                    return "<";

                case 3:
                    return "<=";

                case 4:
                    return "=";

                default:
                    return "=";

            }
        }

        public void multiplyRowPerNegatif(int index)
        {
            for(int i =0;i<this.nbVariable;i++)
            {
                this.matrice[index][i] = this.matrice[index][i] * -1;
            }
        }

        // Insertion des contraintes dans la matrice
        public void initContrainte()
        {

            Console.WriteLine("Insertion de sous contrainte");
            for (int i = 1; i < this.nbContrainte; i++)
            {
                for(int j = 0; j < this.nbVariable; j++)
                {
                    if (j == (this.nbVariable - 1))
                    {
                        Console.Write("Choisir le signe entre 0: > , 1: >= ,2: <, 3:<= ,4= :");
                        
                        this.signeContrainte[i - 1] = this.getSigne(Convert.ToInt32(Console.ReadLine()));
                    

                        Console.Write("Entrer la valeur de b[" + (i + 1) + "] = ");
                        this.matrice[i].Add(Convert.ToDouble(Console.ReadLine()));
                        if (this.signeContrainte[i - 1] == ">" || this.signeContrainte[i - 1] == ">=")
                        {
                            // Si signe > || >= multiplier par -1 et changer la signe a <= :)
                            this.multiplyRowPerNegatif(i);
                            this.signeContrainte[i - 1] = "<=";
                        }
                        break;
                    }

                    Console.Write("Entrer la valeur de x[" + i + "][" + j + "] = ");
                    this.matrice[i].Add(Convert.ToDouble(Console.ReadLine()));
                }
            }

        }

        public void printZ()
        {

            Console.WriteLine("----------------------------------------------------------------------------------");
            Console.Write("z = ");
            for (int i = 0; i < this.nbVariable - 1; i++)
            {
                Console.Write(this.matrice[0][i] + "x" + i + " ");
            }

        }


        public void printContrainte()
        {

            Console.WriteLine("-----------------------------Les contraintes-------------------------------------");
            for (int i = 1; i < this.nbContrainte; i++)
            {

                for (int j = 0; j < this.nbVariable; j++)
                {
                    // Si la valeur == 0, alors ne pas afficher
                        if (j == this.nbVariable - 1)
                        {
                            Console.Write(this.signeContrainte[i - 1] + " " + this.matrice[i][j]);
                            Console.WriteLine();
                            break;

                        }
                        Console.Write(this.matrice[i][j] + "x" + j + " ");



                }
            }
        }

        private void generateX()
        {

            for (int i = 1; i < this.nbContrainte; i++)
            {
                switch(this.signeContrainte[i - 1])
                {
                    case ">":
                    case ">=":
                        this.matrice[i].Insert((this.nbVariable - 1),-1);
                        this.loadEmptyVar(this.nbVariable - 1,i);
                        this.baseIndex.Add(this.nbVariable - 1);
                        this.nbVariable++;
                        break;
                    case "<":
                    case "<=":
                        this.matrice[i].Insert((this.nbVariable - 1), 1);
                        this.loadEmptyVar(this.nbVariable - 1,i);
                        this.baseIndex.Add(this.nbVariable - 1);
                        this.nbVariable++;
                        break;
                }
            }
        }

        // Charger les X qui n'ont pas encore de valeur sur les autres contraintes
        // Ex x1 de ligne 3 n'a pas de valeur alors 0x3
        private void loadEmptyVar(int index,int rowContrainte)
        {

            for (int i = 0; i< this.nbContrainte;i++)
            {
                    if(i!=rowContrainte)
                    {
                        this.matrice[i].Insert(index, 0);
                    }
                }

        }

        private void printBaseIndex()
        {
            Console.WriteLine("Les bases : ");
            for (int i = 0; i< this.baseIndex.Count;i++)
            {
                Console.Write(this.baseIndex[i] + " , ");
            }
        }
        public void DoSimplexe()
        {
            while(Utility.getNegatifCN(this.matrice[0]) >= 0)
            {
                this.printZ();

                this.printContrainte();

                var entreDansLaBase = Utility.getNegatifCN(this.matrice[0]);
                if (entreDansLaBase == -1)
                {
                    return;
                }
                // Sortir de la base
                var sortDelaBase = Utility.geMinToEnterBase(this.matrice, entreDansLaBase, this.nbVariable);

                Console.WriteLine("X" + sortDelaBase + "sort de la base");


                // Entre en base
                // var baseLigne = this.baseIndex.IndexOf(sortDelaBase);
                this.baseIndex[sortDelaBase] = entreDansLaBase;
                var baseLigne = this.baseIndex.IndexOf(entreDansLaBase);
                Console.WriteLine("======================Base apres nouveau base======================");
                this.printBaseIndex();

                // Console.WriteLine("Base ligne :" + baseLigne + " Base colonne : " + baseCol);
                this.matrice = Utility.EliminateRow(this.matrice, entreDansLaBase, baseLigne + 1);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=============================RESULTAT==============================");
            Console.ResetColor();
            this.printZ();
            this.printContrainte();

            for(int i = 0;i<this.baseIndex.Count;i++)
            {
                Console.WriteLine("X[" + (this.baseIndex[i] + 1) + "] = " + this.matrice[this.baseIndex[i] + 1][this.nbVariable - 1]);
            }

        }
    }
}
