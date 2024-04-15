using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace PeicongCheng_FinalProject_Solver
{
    [Serializable]

    internal class Document
    {
        int algorithmchoice;
        string totalvar;
        protected int tvar = 0;
        string[] stringanswer;
        bool firsttime;
        public event EventHandler SizeChange;//the size of the coefficients in equation system or constranits are dependent on number of variables; thus, it is necessary to create an event to handle the size change of these filed in child class.
        public Document() 
        {
            algorithmchoice = 0;
            this.totalvar = string.Empty;
            stringanswer = new string[tvar];
            SetOneDStringZero(stringanswer);
            firsttime = true;
        }

        public int AlgorithmChoice
        {
            get { return algorithmchoice; }
            set { algorithmchoice = value; }
        }

        public string TotalVar
        {
            get { return totalvar; }
            set 
            { 
                totalvar = value;
                
            }
        }

        public int Tvar
        {
            get { return tvar; }
            set
            { 
                tvar = value;
                stringanswer = new string[tvar];
                SetOneDStringZero(stringanswer);
                SizeChanged(EventArgs.Empty);
            }
        }
        protected void SizeChanged(EventArgs e)
        {
            EventHandler handler = SizeChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public string[] GetStringAnswer
        {
            get { return stringanswer; }
            set { stringanswer = value; }
        }

        public bool FirstTime
        {
            get { return firsttime; }
            set { firsttime = value; }
        }
        private void SetOneDStringZero(string[] str)
        {
            for(int i = 0;i<str.Length;i++)
            {
                str[i] = "0";
            }
        }
    }
}
