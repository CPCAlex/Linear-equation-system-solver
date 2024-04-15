using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// use to store and iofstream the data in GSDocument and SimplexDocument
namespace PeicongCheng_FinalProject_Solver
{
    [Serializable]
    internal class FileDocument
    {
        GSDocument gs;
        SimplexDocument sp;
        public FileDocument() { }
        public FileDocument(GSDocument gs, SimplexDocument sp)
        {
            this.gs = gs;
            this.sp = sp;
        }

        public GSDocument GS
        {
            get { return gs; }
            set { gs = value; }
        }

        public SimplexDocument SP
        {
            get { return sp; }
            set { sp = value; }
        }
    }
}
