using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
//using System.Windows.Forms;

namespace PeicongCheng_FinalProject_Solver
{
    internal class SimplexMethod
    {
        int variablenum;
        double[] answer;
        int numberofconstraint;
        TextBox[,] MyCoeffTextBox;
        TextBox[] MyBTextBox;
        double[,] coeffdata;
        double[] bdata;
        TextBox[] MyAnswerTextBox;
        TextBox OutputWindow;
        TextBox[] CoeffObjectiveFunction;
        double[] coeffofobjectivefunction;
        int ismax;//0->max, 1->min
        int[] constraintoperation;//0-> <=, 1-> =, 2-> >=
        
        double maxnum;
        int[] choosenindex;
        double[] judgementindex;
        public SimplexMethod() { }

        public SimplexMethod(int variablenum, double[] answer, int numberofconstraint, TextBox[,] MyCoeffTextBox, TextBox[] MyBTextBox, double[,] coeffdata, double[] bdata, TextBox[] MyAnswerTextBox, TextBox OutputWindow, TextBox[] CoeffObjectiveFunction, double[] coeffofobjectivefunction, int ismax, int[] constraintoperation)
        {
            this.variablenum = variablenum;
            this.answer = answer;
            this.numberofconstraint = numberofconstraint;
            this.MyCoeffTextBox = MyCoeffTextBox;
            this.MyBTextBox = MyBTextBox;
            this.coeffdata = coeffdata;
            this.bdata = bdata;
            this.MyAnswerTextBox = MyAnswerTextBox;
            this.OutputWindow = OutputWindow;
            this.CoeffObjectiveFunction = CoeffObjectiveFunction;
            this.coeffofobjectivefunction = coeffofobjectivefunction;
            this.ismax = ismax;
            this.constraintoperation = constraintoperation;
            this.maxnum = 9999999999;
            this.choosenindex = new int[numberofconstraint];
            this.judgementindex = null;
        }

        public TextBox[] MYANSWERTEXTBOX
        {
            get { return MyAnswerTextBox; }
        }

        public TextBox OUTPUTWINDOW
        {
            get { return OutputWindow; }
        }

        public bool CheckValid()
        {

            for (int i = 0; i < variablenum; i++)
                answer[i] = 0;

            //------------
            TextBox tempTextBox;

            bool judgevalidconstraint = true;//if all the coefficient of the variables in one eqaution are 0, this equation is invalid
            bool[] judgebalidvariable = new bool[variablenum];
            for (int i = 0; i < variablenum; i++)
                judgebalidvariable[i] = true;
            //
            for(int i = 0;i< variablenum; i++)
            {

                tempTextBox = CoeffObjectiveFunction[i];
                try
                {
                    coeffofobjectivefunction[i] = double.Parse(tempTextBox.Text);
                    if (coeffofobjectivefunction[i] != 0) { judgevalidconstraint = false;  }
                    if(Math.Abs(coeffofobjectivefunction[i]) >= maxnum/100) maxnum = (Math.Abs(coeffofobjectivefunction[i]))*99999;
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("In objective function, the input of the coefficient of X" + (i + 1).ToString() + " can not be null!\nPlease reenter!");
                    return false;
                }
                catch (FormatException)
                {
                    MessageBox.Show("In objective function, the input of the coefficient of X" + (i + 1).ToString() + " is illegal!\nPlease reenter!");
                    return false;

                }


               
            }
            if (judgevalidconstraint == true)
            {
                MessageBox.Show("All coefficients of the variables in objective function are 0!\nPlease check and reenter!");
                return false;
            }
            for (int i = 0; i < numberofconstraint; i++)
            {
                judgevalidconstraint = true;

                for (int j = 0; j < variablenum; j++)
                {
                    tempTextBox = MyCoeffTextBox[i, j];
                    try
                    {
                        coeffdata[i, j] = double.Parse(tempTextBox.Text);
                        if (coeffdata[i, j] != 0) { judgevalidconstraint = false; judgebalidvariable[j] = false; }
                    }
                    catch (ArgumentNullException)
                    {
                        MessageBox.Show("the input of the coefficient of X" + (j + 1).ToString() + " in No." + (i + 1).ToString() + " constraint can not be null!\nPlease reenter!");
                        return false;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("the input of the coefficient of X" + (j + 1).ToString() + " in No." + (i + 1).ToString() + " constraint is illegal!\nPlease reenter!");
                        return false;

                    }


                }

                if (judgevalidconstraint == true)
                {
                    MessageBox.Show("No." + (i + 1).ToString() + " constraint is invalid because all coefficients of the variables are 0!\nPlease check and reenter!");
                    return false;
                }



                tempTextBox = MyBTextBox[i];
                try
                {
                    bdata[i] = double.Parse(tempTextBox.Text);
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("the input of right hand side in No." + (i + 1).ToString() + " constraint can not be null!\nPlease reenter!");
                    return false;
                }
                catch (FormatException)
                {
                    MessageBox.Show("the input of right hand side in No." + (i + 1).ToString() + " constraint is illegal!\nPlease reenter!");
                    return false;

                }


            }

            for (int i = 0; i < variablenum; i++)
                if (judgebalidvariable[i] == true)
                {
                    MessageBox.Show("All the coefficients of variable X" + (i + 1).ToString() + " in constraints are 0, which is meaningless!\nPlease verify your Total Variables!");
                    return false;
                }
            return true;
        }

        void TransferMintoMax()
        {
            for(int i = 0;i<variablenum;i++)
            {
                coeffofobjectivefunction[i] *= -1;
            }
            for(int i =0;i<numberofconstraint;i++)
            {
                if (constraintoperation[i] == 1)
                    continue;
                for(int j = 0;j<variablenum;j++)
                {
                    coeffdata[i,j] *= -1;
                }
                constraintoperation[i] = (constraintoperation[i] == 0)?2:0;
                bdata[i] *= -1;
            }
        }

        double[,] PositiveRHS()
        {
            int greaterthancount = 0;
            double[,] slack;
            for(int i = 0;i < numberofconstraint;i++)
            {
                greaterthancount++;
                if (bdata[i] < 0)
                {
                    for (int j = 0; j < variablenum; j++)
                    {
                        coeffdata[i, j] *= -1;
                    }
                    bdata[i] *= -1;
                    if (constraintoperation[i] != 1)
                        constraintoperation[i] = (constraintoperation[i] == 0) ? 2 : 0;

                }
                if (constraintoperation[i] == 2) greaterthancount++;
            }
            slack = new double[numberofconstraint, greaterthancount];
            int row = 0;
            int col = 0;
            for(int i = 0;i<numberofconstraint;i++)
            {
                if (constraintoperation[i] != 2)
                {
                    slack[row, col] = 1;
                    row++;
                    col++;
                    continue;
                }
                else
                {
                    slack[row, col] = -1;
                    slack[row, col+1] = 1;
                    row++;
                    col += 2;
                }
            }

            return slack;
        }

        double[,] ResizeConCoeff(double[,] slack)
        {
            double[,] temp;
            temp = new double[numberofconstraint, variablenum + slack.GetLength(1)];
            for(int i = 0;i<numberofconstraint;i++)
            {
                for (int j = 0; j < variablenum;j++)
                    temp[i,j] = coeffdata[i,j];
                for (int j = 0; j < slack.GetLength(1); j++)
                    temp[i, j + variablenum] = slack[i, j];
            }
            return temp;

        }

        double[] ResizeObjectCOeff(double[] objectivefunctioncoeff, double[,] slack)
        {
            double[] temp;
            temp = new double[variablenum + slack.GetLength(1)];
            for(int i = 0;i<variablenum;i++)
                temp[i] = objectivefunctioncoeff[i];
            int index = variablenum;
            for(int i = 0;i<numberofconstraint;i++)
            {
                if (constraintoperation[i] == 0)
                {
                    temp[index] = 0;
                    choosenindex[i] = index;
                    index++;
                }
                else if (constraintoperation[i] == 1)
                {
                    temp[index] = -maxnum;
                    choosenindex[i] = index;
                    index++;
                }
                else
                {
                    temp[index] = 0;
                    temp[index + 1] = -maxnum;
                    choosenindex[i] = index + 1;
                    index += 2;
                }
            }
            return temp;
        }

        void JudgeMentInitialization()
        {
            double[] choosencoeff = new double[numberofconstraint];
            for(int i = 0;i<numberofconstraint;i++)
            {
                choosencoeff[i] = coeffofobjectivefunction[choosenindex[i]];
            }
            for(int i = 0;i<coeffofobjectivefunction.Length;i++)
            {
                judgementindex[i] = coeffofobjectivefunction[i];
                for(int j = 0;j<numberofconstraint;j++)
                {
                    judgementindex[i] -= coeffdata[j,i] * choosencoeff[j];
                }
            }
        }

        bool PositiveJudgement()
        {
            for (int i = 0; i < judgementindex.Length; i++)
            {
                if (judgementindex[i] > 0) { return false; }
            }
            return true;
        }

        bool ErrorJudgement(int choosencol)
        {
            for(int i = 0;i<numberofconstraint;i++)
            {
                if (coeffdata[i, choosencol] > 0)
                {
                    return false;
                }
            }
            return true;
        }

        int ChoosenCol()
        {
            double temp = 0;
            int choosencol = 0 ;
            for(int i = 0;i<judgementindex.Length;i++)
            {
                if (judgementindex[i] > temp)
                {
                    temp = judgementindex[i];
                    choosencol = i;
                }
            }
            return choosencol;
        }

        int ChoosenRow(int choosencol)
        {
            int choosenrow = 0;
            double temp = maxnum;
            for(int i = 0;i<numberofconstraint;i++)
            {
                if (coeffdata[i, choosencol] <= 0)
                    continue;
                if(temp > bdata[i] / coeffdata[i,choosencol])
                {
                    temp = bdata[i] / coeffdata[i,choosencol];
                    choosenrow = i;
                }
            }



            return choosenrow;
        }

        void ChoosenRowNormalization(int choosenrow, int choosencol)
        {
            double division = coeffdata[choosenrow, choosencol];

            for(int i = 0; i< coeffdata.GetLength(1);i++)
            {
                coeffdata[choosenrow, i] /= division;

            }
            bdata[choosenrow] /= division;
        }

        void RowOperation(int choosenrow, int choosencol)
        {
            double index;
            for(int i = 0;i<coeffdata.GetLength(0);i++)
            {
                if (i == choosenrow) continue;
                index = coeffdata[i, choosencol]; 
                for(int j = 0; j < coeffdata.GetLength(1);j++)
                {
                    coeffdata[i, j] -= index * coeffdata[choosenrow, j];
                }
                bdata[i] -= index * bdata[choosenrow];
            }
        }

        void OutputSolution()
        {
            double temp = 0;
            for(int i = 0;i<choosenindex.Length;i++)
            {
                if (choosenindex[i] < variablenum) //_____
                {
                    temp += answer[choosenindex[i]] * coeffofobjectivefunction[choosenindex[i]];
                }
            }
            if(ismax == 0)
            {
                OutputWindow.Text += "The maximium result: " + temp.ToString("0.0000000");
            }
            else
            {
                temp *= -1;
                OutputWindow.Text += "The minimium result: " + temp.ToString("0.0000000");

            }
        }

        //core code of Simplex Algorithm
        public bool Simplex_Algorithm()
        {
            double[,] slack;
            bool isover = false;
            if (!CheckValid()) return false;

            if(ismax != 0) TransferMintoMax();

            slack = PositiveRHS();

            coeffdata = ResizeConCoeff(slack);
            coeffofobjectivefunction = ResizeObjectCOeff(coeffofobjectivefunction, slack);

            int choosencol = 0;
            int choosenrow = 0;
            judgementindex = new double[variablenum + slack.GetLength(1)];
            //the iteration process of SImplex Algortihm
            while(isover == false)
            {
                JudgeMentInitialization();
               
                isover = PositiveJudgement();
                if(isover == true) { break; }
                choosencol = ChoosenCol();
                isover = ErrorJudgement(choosencol);
                if(isover == true) 
                {
                    OutputWindow.Text = "No solution!";
                    return true; 
                }
                choosenrow = ChoosenRow(choosencol);
                choosenindex[choosenrow] = choosencol;
                ChoosenRowNormalization(choosenrow, choosencol);
                RowOperation(choosenrow, choosencol);
            }
            
            for(int i = 0;i<variablenum;i++)
            {
                answer[i] = 0;
            }
            for(int i = 0;i<numberofconstraint;i++)
            {
                if (choosenindex[i] < variablenum)
                    answer[choosenindex[i]] = bdata[i];
            }


            for(int i = 0;i<variablenum;i++)
            {
                MyAnswerTextBox[i].Text = answer[i].ToString("0.00000000");
            }

            OutputWindow.Text = "Find Solution!\n";
            OutputSolution();
            //choosencol = 


            return true;

        }

        
    }
}
