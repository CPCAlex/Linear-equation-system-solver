using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace PeicongCheng_FinalProject_Solver
{
    [Serializable]

    internal class GSDocument : Document
    {
        string totalequ;
        int tequ;
        string[,] stringcoeffofvar;
        string[] stringbcoeffofvar;


        public GSDocument() : base()
        {
            this.totalequ = string.Empty;
            this.tequ = 0;
            stringcoeffofvar = new string[tequ, base.tvar];
            stringbcoeffofvar = new string[tequ];
            SetTwoDStringZero(stringcoeffofvar);
            SetOneDStringZero(stringbcoeffofvar);
            SizeChange += GSVarChanged;
        }

        void SetTwoDStringZero(string[,] str)
        {
            for(int i = 0;i < str.GetLength(0);i++)
            {
                for(int j = 0;j<str.GetLength(1);j++)
                {
                    str[i,j] = "0";
                }
            }
        }

        void SetOneDStringZero(string[] str)
        {
            for (int i = 0; i < str.Length; i++)
                str[i] = "0";
        }

        void GSVarChanged(object sender, EventArgs e)
        {
            stringcoeffofvar = new string[tequ, tvar];
            SetTwoDStringZero(stringcoeffofvar);
        }

        public string Totalequ
        {
            get { return totalequ; }
            set
            {
                totalequ = value;
            }
        }

        public int Tequ
        {
            get { return tequ; }
            set
            {
                tequ = value;
                stringbcoeffofvar = new string[tequ];
                stringcoeffofvar = new string[tequ, base.tvar];
                SetTwoDStringZero(stringcoeffofvar);
                SetOneDStringZero(stringbcoeffofvar);
            }
        }

        public string[,] GetStringCoeffofvar
        {
            get { return stringcoeffofvar; }
            set { stringcoeffofvar = value; }
        }
        public string[] GetStringBCoeffofvar
        {
            get { return stringbcoeffofvar; }
            set { stringbcoeffofvar = value; }
        }

    }
}
