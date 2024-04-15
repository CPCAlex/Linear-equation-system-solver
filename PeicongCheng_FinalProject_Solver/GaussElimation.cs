using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PeicongCheng_FinalProject_Solver
{
    internal class GaussElimation
    {
        int variablenum;
        double[] answer;
        int numberofequation;
        TextBox[,] MyCoeffTextBox;
        TextBox[] MyBTextBox;
        double[,] coeffdata;
        double[] bdata;
        TextBox[] MyAnswerTextBox;
        TextBox OutputWindow;
        Label[] MyAnswerLabel;
        Grid Mygrid;

        public GaussElimation() { }

        public GaussElimation(int variablenum, double[] answer, int numberofequation, TextBox[,] MyCoeffTextBox, TextBox[] MyBTextBox, double[,] coeffdata, double[] bdata, TextBox[] MyAnswerTextBox, TextBox OutputWindow, Label[] MyAnswerLabel, Grid mygrid)
        {
            this.variablenum = variablenum;
            this.answer = answer;
            this.numberofequation = numberofequation;
            this.MyCoeffTextBox = MyCoeffTextBox;
            this.MyBTextBox = MyBTextBox;
            this.coeffdata = coeffdata;
            this.bdata = bdata;
            this.MyAnswerTextBox = MyAnswerTextBox;
            this.OutputWindow = OutputWindow;
            this.MyAnswerLabel = MyAnswerLabel;
            this.Mygrid = mygrid;
        }
        

        public TextBox[,] MYCOEFFTEXTBOX
        {
            get { return MyCoeffTextBox; }
            
        }

        public TextBox[] MYBTEXTBOX
        {
            get { return MyBTextBox; }
        }

        public TextBox[] MYANSWERTEXTBOX
        {
            get { return MyAnswerTextBox; }
        }

        public TextBox OUTPUTWINDOW
        {
            get { return OutputWindow; }
        }

        public Label[] MYANSWERLABEL
        {
            get { return MyAnswerLabel; }
        }

        public Grid MYGRID
        {
            get { return Mygrid; }
        }
        
        

        public bool CheckValid()
        {

            for (int i = 0; i < variablenum; i++)
                answer[i] = 0;

            //------------
            TextBox tempTextBox;

            bool judgevalidequation;//if all the coefficient of the variables in one eqaution are 0, this equation is invalid
            bool[] judgebalidvariable = new bool[variablenum];
            for (int i = 0; i < variablenum; i++)
                judgebalidvariable[i] = true;
            for (int i = 0; i < numberofequation; i++)
            {
                judgevalidequation = true;

                for (int j = 0; j < variablenum; j++)
                {
                    tempTextBox = MyCoeffTextBox[i, j];
                    try
                    {
                        coeffdata[i, j] = double.Parse(tempTextBox.Text);
                        if (coeffdata[i, j] != 0) { judgevalidequation = false; judgebalidvariable[j] = false; }
                    }
                    catch (ArgumentNullException)
                    {
                        MessageBox.Show("the input of the coefficient of X" + (j + 1).ToString() + " in No." + (i + 1).ToString() + " equation can not be null!\nPlease reenter!");
                        return false;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("the input of the coefficient of X" + (j + 1).ToString() + " in No." + (i + 1).ToString() + " equation is illegal!\nPlease reenter!");
                        return false;

                    }
                }

                if (judgevalidequation == true)
                {
                    MessageBox.Show("No." + (i + 1).ToString() + " equation is invalid because all coefficients of the variables are 0!\nPlease check and reenter!");
                    return false;
                }


                tempTextBox = MyBTextBox[i];
                try
                {
                    bdata[i] = double.Parse(tempTextBox.Text);
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("the input of right hand side in No." + (i + 1).ToString() + " equation can not be null!\nPlease reenter!");
                    return false;
                }
                catch (FormatException)
                {
                    MessageBox.Show("the input of right hand side in No." + (i + 1).ToString() + " equation is illegal!\nPlease reenter!");
                    return false;
                }

            }

            for (int i = 0; i < variablenum; i++)
                if (judgebalidvariable[i] == true)
                {
                    MessageBox.Show("All the coefficients of variable X" + (i + 1).ToString() + " are 0, which is meaningless!\nPlease verify your Total Variables!");
                    return false;
                }
            return true;
        }

        //core code of Gauss Elimation
        public bool Gauss_Jordan_Algorithm()
        {

            if (!CheckValid()) return false;
            //---------------------
            int index = 0;
            //core iteration of Gauss Elimation
            for (int i = 0; i < variablenum; i++)
            {
                if (i >= numberofequation)
                    break;
                if (RowShift(i, index))
                {
                    index++;
                    continue;
                }
                double division;
                if (coeffdata[i - index, i] != 1)
                {
                    division = coeffdata[i - index, i];
                    for (int j = i; j < variablenum; j++)
                    {
                        coeffdata[i - index, j] /= division;
                    }
                    bdata[i - index] /= division;
                }

                double times;
                for (int j = i - index + 1; j < numberofequation; j++)
                {
                    times = coeffdata[j, i];
                    for (int k = i; k < variablenum; k++)
                    {
                        coeffdata[j, k] -= times * coeffdata[i - index, k];
                    }
                    bdata[j] -= times * bdata[i - index];
                }
            }
            int count = 0;
            for (int i = variablenum - index; i < numberofequation; i++)
            {
                if (bdata[i] != 0)
                {
                    OutputWindow.Text = "No Solution!";
                    for (int j = 0; j < variablenum; j++)
                    {

                        if (MyAnswerLabel[j] != null)
                        {
                            Mygrid.Children.Remove(MyAnswerLabel[j]);
                        }

                        if (MyAnswerTextBox[j] != null)
                        {
                            Mygrid.Children.Remove(MyAnswerTextBox[j]);
                        }
                    }
                    return true;
                }
                count++;
            }

            int duplicateindex = index + (numberofequation - variablenum);
            if (duplicateindex == 0 || count == numberofequation - variablenum)
            {
                for (int i = variablenum - 1; i >= 0; i--)
                {
                    answer[i] = bdata[i];
                    for (int j = variablenum - 1; j > i; j--)
                    {
                        answer[i] -= answer[j] * coeffdata[i, j];
                    }

                    MyAnswerTextBox[i].Text = answer[i].ToString("0.00000000");
                }
                OutputWindow.Text = "Find unique solution!";
                return true;
            }

            List<int> check = new List<int>();
            int temp = 0;
            OutputWindow.Text = "There are infinite solutions\n";
            count = 0;
            for (int i = variablenum - index - 1; i >= 0; i--)
            {
                if (i >= numberofequation)
                    continue;
                check = new List<int>();
                count += CheckDuplic(check, i);
                if (temp == count - 1)
                {
                    answer[i] = bdata[i] / coeffdata[i, i];
                    MyAnswerTextBox[i].Text = answer[i].ToString("0.00000000");
                    continue;
                }
                string outputtext = "X" + (i + 1).ToString();
                string outputtext2 = coeffdata[i, i].ToString() + "X" + (i + 1).ToString();
                temp = count;
                answer[i] = 1 / coeffdata[i, i];
                MyAnswerTextBox[i].Text = answer[i].ToString("0.00000000");


                for (int j = 0; j < check.Count; j++)
                {
                    if (j != check.Count - 1)
                        answer[check[j]] = 1 / coeffdata[i, check[j]];
                    else
                        answer[check[j]] = (bdata[i] - check.Count) / coeffdata[i, check[j]];
                    MyAnswerTextBox[check[j]].Text = answer[check[j]].ToString();
                    outputtext += ", X" + (check[j] + 1).ToString();
                    outputtext2 += "+" + coeffdata[i, check[j]].ToString() + "X" + (check[j] + 1).ToString();
                }
                outputtext2 += "=" + bdata[i];
                OutputWindow.Text += outputtext + " are changeable, as long as they meet:" + outputtext2 + "\n";
            }

            return true;
        }

        public int CheckDuplic(List<int> check, int row)
        {
            int count = 1;

            for (int i = row + 1; i < variablenum; i++)
            {
                if (check.Contains(i))
                    continue;
                if (coeffdata[row, i] != 0)
                {
                    count++;
                    check.Add(i);
                }
            }

            return count;

        }

        public bool RowShift(int row, int index)
        {

            if (coeffdata[row - index, row] != 0)
            {
                return false;
            }
            double temp;
            for (int i = row - index + 1; i < numberofequation; i++)
            {
                if (coeffdata[i, row] != 0)
                {
                    for (int j = row; j < variablenum; j++)
                    {
                        temp = coeffdata[i, j];
                        coeffdata[i, j] = coeffdata[row - index, j];
                        coeffdata[row - index, j] = temp;
                    }
                    temp = bdata[i];
                    bdata[i] = bdata[row - index];
                    bdata[row - index] = temp;
                    return false;

                }
            }
            return true;
        }

    }
}
