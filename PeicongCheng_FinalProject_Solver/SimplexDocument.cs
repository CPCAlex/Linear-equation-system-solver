using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;

namespace PeicongCheng_FinalProject_Solver
{
    [Serializable]

    internal class SimplexDocument : Document
    {
        int ismax = 0;//true -> max; false -> min
        string totalcon;
        int tcon;
        string[,] stringcoeffofcon;
        string[] stringbcoeffofcon;
        string[] stringcoeffofobjectivefunction;
        int[] constraintsoperation;

        public SimplexDocument() : base()
        {
            this.totalcon = string.Empty;
            this.tcon = 0;
            this.ismax = 0;
            stringcoeffofcon = new string[tcon, tvar];
            stringbcoeffofcon = new string[tcon];
            stringcoeffofobjectivefunction = new string[tvar];
            constraintsoperation = new int[tcon];
            SetTwoDStringZero(stringcoeffofcon);
            SetOneDStringZero(stringbcoeffofcon);
            SetOneDStringZero(stringcoeffofobjectivefunction);
            SetOneDIntZero(constraintsoperation);
            SizeChange += SPVarChanged;
        }

        void SetOneDIntZero(int[] num)
        {
            for (int i = 0; i < num.Length; i++)
            {
                num[i] = 0;
            }
        }

        void SetTwoDStringZero(string[,] str)
        {
            for (int i = 0; i < str.GetLength(0); i++)
            {
                for (int j = 0; j < str.GetLength(1); j++)
                {
                    str[i, j] = "0";
                }
            }
        }

        void SetOneDStringZero(string[] str)
        {
            for (int i = 0; i < str.Length; i++)
                str[i] = "0";
        }

        void SPVarChanged(object sender, EventArgs e)
        {
            stringcoeffofcon = new string[tcon, tvar];
            stringcoeffofobjectivefunction = new string[tvar];
            SetTwoDStringZero(stringcoeffofcon);
            SetOneDStringZero(stringcoeffofobjectivefunction);
        }
        public string TotalCon
        {
            get { return totalcon; }
            set
            {
                totalcon = value;

            }
        }
        public int Tcon
        {
            get { return tcon; }
            set
            {
                tcon = value;
                stringcoeffofcon = new string[tcon, base.tvar];
                stringbcoeffofcon = new string[tcon];
                constraintsoperation = new int[tcon];
                
                SetOneDIntZero(constraintsoperation);
                SetTwoDStringZero(stringcoeffofcon);
                SetOneDStringZero(stringbcoeffofcon);
            }
        }
        public string[,] GetStringCoeffofcon
        {
            get { return stringcoeffofcon; }
            set { stringcoeffofcon = value; }
        }
        public string[] GetStringBCoeffofcon
        {
            get { return stringbcoeffofcon; }
            set { stringbcoeffofcon = value; }
        }

        public string[] GetStringCoeffofObjectiveFunction
        {
            get { return stringcoeffofobjectivefunction; }
            set { stringcoeffofobjectivefunction = value; }
        }

        public int[] GetConstraintsOperation
        {
            get { return constraintsoperation; }
            set { constraintsoperation = value; }
        }

        public int IsMax
        {
            get { return ismax; }
            set { ismax = value; }
        }
    }
}
