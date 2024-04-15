using PeicongCheng_FinalProject_Solver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;


namespace PeicongCheng_FinalProject_Solver
{
    public partial class MainWindow : Window
    {
        
        GSDocument GSdocument = new GSDocument();
        SimplexDocument SPdocument = new SimplexDocument();
        GaussElimation gausselimation = new GaussElimation();
        SimplexMethod simplexalgorithm = new SimplexMethod();
        FileDocument filedocument = new FileDocument();
        
        int variablenum = 0;
        int numberofequation = 0;
        int numberofconstraint = 0;
        int algorithmchoice = 0;
        
        bool firsttime = true;//check if the coefficient textboxs have been created 
        bool executeclick = false;//check if the input has been executed
        bool justopen = false;//check if user just opened a solver.emp file
        bool firstopen;//This is where I made a mistake!!!!!! I only realized that I accidentially initialized 'firstopen' as true after the due!!! initialize 'firstopen' as true will totally crash this application, so I have to resubmit.
        bool tem;

        System.Windows.Controls.TextBox[,] MyCoeffTextBox;
        System.Windows.Controls.Label[,] MyLabel;
        System.Windows.Controls.TextBox[] MyBTextBox;//B means the RHS of the equation or constraints
        System.Windows.Controls.Label[] MyObjectivefunctionLabel;
        System.Windows.Controls.TextBox[] MyObjectivefunctionCoeff;
        System.Windows.Controls.ComboBox Ismaxobjectivefunction;
        System.Windows.Controls.ComboBox[] ConstraintsOperation;
        System.Windows.Controls.TextBox[] MyAnswerTextBox;
        System.Windows.Controls.Label[] MyAnswerLabel;
        System.Windows.Controls.TextBox OutputWindow;


        System.Windows.Controls.TextBox Temptextbox;
        System.Windows.Controls.Label Templabel;

        double[,] coeffdata;
        double[] bdata;
        double[] answer;
        double[] coeffofobjectivefunction;
        int ismax = 0;
        int[] constraintoperation;

        string CurrentFilePath = "NewSolver.emp";
        bool modified = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------------------------------------------
        //Menu Operation
        private void new_menuItem_Click(object sender, RoutedEventArgs e)
        {
            if (HandleSaveRequest())
            {
                StartNewText(sender, e);
            }
        }

        private void open_menuItem_Click(object sender, RoutedEventArgs e)
        {
            if (HandleSaveRequest())
            {
                StartLoad(sender, e);
            }
        }

        private void save_menuItem_Click(object sender, RoutedEventArgs e)
        {
            StartSave();

        }

        private void saveas_menuItem_Click(object sender, RoutedEventArgs e)
        {
            StartSave();
        }

        private void exit_menuItem_Click(object sender, RoutedEventArgs e)
        {
            if (HandleSaveRequest())
            {
                Close();
            }
        }

        private void about_menuItem_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "This application was developed by Peicong, Cheng.\n\nFor any advice, contact me by pcgkg@umsystem.edu";
            string caption = "About";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);

        }

        private void StartNewText(object sender, RoutedEventArgs e)
        {
            if(algorithmchoice == 0)
            {
                if (SPdocument.Tcon != 0)
                    RemoveSPCoeff();
            }
            else
            {
                if(GSdocument.Tequ != 0)
                    RemoveGSCoeff();
            }
            ResetAllButton_Click(sender, e);
            AlgorithmChoice.SelectedIndex = 0;
            algorithmchoice = 0;
            TotalConstraintsLabel.Visibility = Visibility.Hidden;
            TotalConstraintsText.Visibility = Visibility.Hidden;
            TotalEquationLabel.Visibility = Visibility.Visible;
            TotalEquationText.Visibility = Visibility.Visible;

            variablenum = 0;
            numberofequation = 0;
            numberofconstraint = 0;
            SPdocument = new SimplexDocument();
            GSdocument = new GSDocument();
            TotalEquationText.Text = string.Empty;
            TotalVariablesText.Text = string.Empty;

            MyCoeffTextBox = null;
            MyLabel = null;
            MyBTextBox = null;
            MyObjectivefunctionLabel = null;
            MyObjectivefunctionCoeff = null;
            Ismaxobjectivefunction = null;
            ConstraintsOperation = null;
            MyAnswerTextBox = null;
            MyAnswerLabel = null;
            OutputWindow = null;


            coeffdata = null;
            bdata = null;
            answer = null;
            coeffofobjectivefunction = null;
            ismax = 0;
            constraintoperation = null;
            justopen= false;
            firstopen= false;

            firsttime = true;//remember to initialize to 0 when open a file
            executeclick = false;
            saveas_menuItem.IsEnabled = true;
            modified = false;
        }

        private void UnsetModifedstate()
        {
            modified = false;
            save_menuItem.IsEnabled = false;
        }

        private void SetModifedstate()
        {
            modified = true;
            save_menuItem.IsEnabled = true;
        }

        private bool HandleSaveRequest()
        {
            if (!modified) return true;

            string MessageBoxText = "You have modified the content. \nDo you want save it?";
            string caption = "LinearEquationSolver";

            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(MessageBoxText, caption, button, icon);
            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes:
                    return StartSave();


                case MessageBoxResult.No:
                    return true;

                case MessageBoxResult.Cancel:
                    return false;
            }

            return false;
        }

        private void StartLoad(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();

            openfiledialog.Filter = "Linear Equation Solver file (*.emp)|*.emp";
            openfiledialog.Multiselect = false;
            if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadSolverInformation(openfiledialog.FileName, sender, e);
            }
        }

        private void LoadSolverInformation(string filepath, object sender, RoutedEventArgs e)
        {
            StartNewText(sender, e);
            var fStreamSolver = new FileStream(filepath, FileMode.Open);
            var binFormatterSolver = new BinaryFormatter();
            filedocument = (FileDocument)binFormatterSolver.Deserialize(fStreamSolver);
            GSdocument = filedocument.GS;
            SPdocument = filedocument.SP;
            justopen = true;
            algorithmchoice = GSdocument.AlgorithmChoice;
            if (algorithmchoice == 0)
            {
                TotalConstraintsText.Visibility = Visibility.Hidden;
                TotalConstraintsLabel.Visibility = Visibility.Hidden;
                TotalEquationLabel.Visibility = Visibility.Visible;
                TotalEquationText.Visibility = Visibility.Visible;
                numberofequation = GSdocument.Tequ;
                variablenum = GSdocument.Tvar;
                TotalVariablesText.Text = GSdocument.TotalVar;
                TotalEquationText.Text = GSdocument.Totalequ;

                SetUpGSPCoeff();
                firsttime = GSdocument.FirstTime;
            }

            else
            {
                TotalConstraintsText.Visibility = Visibility.Visible;
                TotalConstraintsLabel.Visibility = Visibility.Visible;
                TotalEquationLabel.Visibility = Visibility.Hidden;
                TotalEquationText.Visibility = Visibility.Hidden;
                numberofconstraint = SPdocument.Tcon;
                variablenum = SPdocument.Tvar;
                
                TotalVariablesText.Text = SPdocument.TotalVar;
                TotalConstraintsText.Text = SPdocument.TotalCon;
                firstopen = true;

                AlgorithmChoice.SelectedIndex = 1;
                firstopen = false;

                SetUpSPCoeff();
                for (int i = 0; i < numberofconstraint; i++)
                    constraintoperation[i] = SPdocument.GetConstraintsOperation[i];
                ismax = SPdocument.IsMax;
                firsttime = SPdocument.FirstTime;

            }
            executeclick = false;

            fStreamSolver.Close();
            CurrentFilePath = filepath;
            UnsetModifedstate();
        }

        private bool StartSave()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            if (CurrentFilePath != "")
            {
                saveFileDialog.FileName = System.IO.Path.GetFileName(CurrentFilePath);
                saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(CurrentFilePath);
            }
            saveFileDialog.DefaultExt = ".emp";
            saveFileDialog.Filter = "Solver Information file (*.emp)|*.emp";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                SaveSolverInformation(saveFileDialog.FileName);
                UnsetModifedstate();
                return true;
            }
            return false;
        }

        private void SaveSolverInformation(string filepath)
        {
            var fStreamSolver = new FileStream(filepath, FileMode.Create);

            var binFormatterSolver = new BinaryFormatter();
            filedocument = new FileDocument(GSdocument, SPdocument);
            binFormatterSolver.Serialize(fStreamSolver, filedocument);

            fStreamSolver.Close();
            CurrentFilePath = filepath;
        }


        //---------------------------------------------------------------------------
        //Linear Equation System Solver Core Code

        private void TotalVariablesText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(algorithmchoice == 0)
            {
                GSdocument.TotalVar = TotalVariablesText.Text;
            }
            else
            {
                SPdocument.TotalVar = TotalVariablesText.Text;
            }
            SetModifedstate();
            //textchange = true;
        }

        private void TotalEquationText_TextChanged(object sender, TextChangedEventArgs e)
        {
            GSdocument.Totalequ = TotalEquationText.Text;
            SetModifedstate();
            //textchange = true;
        }



        private void MyCoeffTextBox_TextChange(object sender, TextChangedEventArgs e)
        {

            System.Windows.Controls.TextBox ChangedTextBox = sender as System.Windows.Controls.TextBox;

            string[] coordinates = ChangedTextBox.Name.Split('_');
            
            int row = int.Parse(coordinates[1]);
            int column = int.Parse(coordinates[2]);
            if(algorithmchoice == 0)
            {
                GSdocument.GetStringCoeffofvar[row,column] = ChangedTextBox.Text;
            }
            else
            {
                SPdocument.GetStringCoeffofcon[row,column] = ChangedTextBox.Text;
            }
            SetModifedstate();
        }

        private void MyBCoeffTextBox_TextChange(object sender, TextChangedEventArgs e)
        {

            System.Windows.Controls.TextBox ChangedTextBox = sender as System.Windows.Controls.TextBox;

            string[] coordinates = ChangedTextBox.Name.Split('_');
            int index = int.Parse(coordinates[1]);
            if(algorithmchoice == 0)
            {
                GSdocument.GetStringBCoeffofvar[index] = ChangedTextBox.Text;
            }
            else
            {
                SPdocument.GetStringBCoeffofcon[index] = ChangedTextBox.Text;
            }
            SetModifedstate();
        }

        private void MyObjectiveCoeffTextBox_TextChange(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox ChangedTextBox = sender as System.Windows.Controls.TextBox;

            string[] coordinates = ChangedTextBox.Name.Split('_');
            int index = int.Parse(coordinates[1]);
            SPdocument.GetStringCoeffofObjectiveFunction[index] = ChangedTextBox.Text;
            SetModifedstate();
        }

        private void ResetAllButton_Click(object sender, RoutedEventArgs e)
        {
            TotalVariablesText.Text = string.Empty;
            if(algorithmchoice == 0)
            {
                TotalEquationText.Text = string.Empty;
                GSdocument.Tequ = 0;
                GSdocument.Tvar = 0;
            }
            else
            {
                TotalConstraintsText.Text = string.Empty;
                SPdocument.Tcon = 0;
                SPdocument.Tvar = 0;
            }
            if(algorithmchoice == 0)
            {
                RemoveGSCoeff();
            }
            else
            {
                RemoveSPCoeff();
            }
            if (executeclick == true)
            {
                RemoveAnswer();
            }

            firsttime = true;
            SetModifedstate();
        }
        private void ResetCoeffButton_Click(object sender, RoutedEventArgs e)
        {
            int row;
            int col;
            if (MyCoeffTextBox != null)
            {
                row = MyCoeffTextBox.GetLength(0);
                col = MyCoeffTextBox.GetLength(1);
            }
            else
            {
                row = 0; col = 0;
            }

            if (firsttime == true)
                return;
            bool judge = false;
            for (int i = 0; i < row; i++)
            {

                for (int j = 0; j < col; j++)
                {
                    Temptextbox = MyCoeffTextBox[i, j];
                    if (Temptextbox.Text != "0")
                        judge = true;
                    Temptextbox.Text = "0";
                }
                Temptextbox = MyBTextBox[i];
                if (Temptextbox.Text != "0")
                    judge = true;
                Temptextbox.Text = "0";
            }

            if(algorithmchoice == 1)
            {
                Ismaxobjectivefunction.SelectedIndex = 0;
                for(int i = 0;i<col;i++)
                {
                    MyObjectivefunctionCoeff[i].Text = "0";
                }
            }
            if (judge == true)
                SetModifedstate();
            if (executeclick == true)
            {
                RemoveAnswer();
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlgorithmChoice.SelectedIndex == 0)
            {
                if (GSdocument.Tvar.ToString() == TotalVariablesText.Text && GSdocument.Tequ.ToString() == TotalEquationText.Text)
                    return;
                Gauss_Jordan_Initialization();

            }
            else
            {
                if (SPdocument.Tvar.ToString() == TotalVariablesText.Text && SPdocument.Tcon.ToString() == TotalConstraintsText.Text)
                    return;
                Simplex_Algorithm_Initialization();
            }
        }

        void Gauss_Jordan_Initialization()
        {
            if (firsttime == false)
            {
                RemoveGSCoeff();
                if (executeclick == true)
                {
                    RemoveAnswer();
                }

            }
            try
            {
               
                variablenum = int.Parse(TotalVariablesText.Text);
                Exception variableerror = new Exception("For Total Variable, Please enter value between 1 to 10, and you have entered " + variablenum);

                if (variablenum > 10 || variablenum < 1)
                    throw variableerror;
                numberofequation = int.Parse(TotalEquationText.Text);
                Exception numberofequerror = new Exception("For Number of Equation, Please enter value between 1 to 10, and you have entered " + numberofequation);

                if (numberofequation <= 0 || numberofequation > 10)
                    throw numberofequerror;

            }
            catch (ArgumentNullException)
            {
                System.Windows.MessageBox.Show("the number cannot be null!");
                return;
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("the number must be positive number!");
                return;

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return;
            }

            GSdocument.Tvar = variablenum;
            GSdocument.Tequ = numberofequation;
            firsttime = false;
            GSdocument.FirstTime = false;

            SetUpGSPCoeff();

            SetModifedstate();

        }

        void Simplex_Algorithm_Initialization()
        {
            if (firsttime == false)
            {
                RemoveSPCoeff();

                if (executeclick == true)
                {
                    RemoveAnswer();
                }

            }

            try
            {

                variablenum = int.Parse(TotalVariablesText.Text);
                Exception variableerror = new Exception("For Total Variable, Please enter value between 1 to 10, and you have entered " + variablenum);

                if (variablenum > 10 || variablenum < 1)
                    throw variableerror;
                numberofconstraint = int.Parse(TotalConstraintsText.Text);
                Exception numberofconerror = new Exception("For Number of Constraint, Please enter value between 1 to 10, and you have entered " + numberofconstraint);

                if (numberofconstraint <= 0 || numberofconstraint > 10)
                    throw numberofconerror;

            }
            catch (ArgumentNullException)
            {
                System.Windows.MessageBox.Show("the number cannot be null!");
                return;
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("the number must be positive number!");
                return;

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return;
            }
            ismax = 0;

            SPdocument.Tvar = variablenum;
            SPdocument.Tcon = numberofconstraint;
            firsttime = false;
            SPdocument.FirstTime = false;

            SetUpSPCoeff();

            SetModifedstate();
        }
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {

            if(algorithmchoice == 0)
            {

                if (TotalVariablesText.Text != GSdocument.Tvar.ToString())
                {
                    System.Windows.MessageBox.Show("You have changed the Total Variable! Please press Generate button first!");
                    return;
                }
                if (TotalEquationText.Text != GSdocument.Tequ.ToString())
                {
                    System.Windows.MessageBox.Show("You have changed the Number of Equation! Please press Generate button first!");
                    return;
                }


                if (executeclick == true)
                    RemoveAnswer();
                Gauss_Jordan_AnswerInitialization();
                gausselimation = new GaussElimation(variablenum, answer, numberofequation, MyCoeffTextBox, MyBTextBox, coeffdata, bdata, MyAnswerTextBox, OutputWindow, MyAnswerLabel, Mygrid);
                bool temp = gausselimation.Gauss_Jordan_Algorithm();
                Mygrid = gausselimation.MYGRID;
                OutputWindow = gausselimation.OUTPUTWINDOW;
                MyAnswerLabel = gausselimation.MYANSWERLABEL;
                MyAnswerTextBox = gausselimation.MYANSWERTEXTBOX;
                if (!temp)
                {
                    
                    RemoveAnswer();
                    return;
                }
                executeclick = true;
            }
            else
            {

                if (TotalVariablesText.Text != SPdocument.Tvar.ToString())
                {
                    System.Windows.MessageBox.Show("You have changed the Total Variable! Please press Generate button first!");
                    return;
                }
                if (TotalConstraintsText.Text != SPdocument.Tcon.ToString())
                {
                    System.Windows.MessageBox.Show("You have changed the Number of Constraints! Please press Generate button first!");
                    return;
                }


                if (executeclick == true)
                    RemoveAnswer();
                Simplex_Algorithm_AnswerInitialization();
                simplexalgorithm = new SimplexMethod(variablenum, answer, numberofconstraint, MyCoeffTextBox, MyBTextBox, coeffdata, bdata, MyAnswerTextBox, OutputWindow, MyObjectivefunctionCoeff, coeffofobjectivefunction, ismax, constraintoperation);
                bool temp = simplexalgorithm.Simplex_Algorithm();
                MyAnswerTextBox = simplexalgorithm.MYANSWERTEXTBOX;
                OutputWindow = simplexalgorithm.OUTPUTWINDOW;
                if(temp == false)
                {
                    for(int i = 0; i < MyAnswerTextBox.Length;i++)
                    {
                        if (MyAnswerLabel[i] != null)
                        {
                            Mygrid.Children.Remove(MyAnswerLabel[i]);
                        }

                        if (MyAnswerTextBox[i] != null)
                        {
                            Mygrid.Children.Remove(MyAnswerTextBox[i]);
                        }
                    }
                    return;
                }
                executeclick = true;
            }
            SetModifedstate();


        }

        void Gauss_Jordan_AnswerInitialization()
        {
            int gridrow = 1 + numberofequation;
            int gridcol = 1;

            for (int i = 0; i < variablenum; i++)
            {
                MyAnswerLabel[i] = new System.Windows.Controls.Label();
                MyAnswerLabel[i].Name = "Answer" + i.ToString();
                MyAnswerLabel[i].Content = "X" + (i + 1).ToString() + " = ";
                Grid.SetRow(MyAnswerLabel[i], gridrow);
                Grid.SetColumn(MyAnswerLabel[i], gridcol);
                Mygrid.Children.Add(MyAnswerLabel[i]);
                gridcol++;
                MyAnswerTextBox[i] = new System.Windows.Controls.TextBox();

                MyAnswerTextBox[i].Name = "Answer_" + i.ToString();

                Grid.SetRow(MyAnswerTextBox[i], gridrow);

                Grid.SetColumn(MyAnswerTextBox[i], gridcol);
                Grid.SetColumnSpan(MyAnswerTextBox[i], 2);
                MyAnswerTextBox[i].TextWrapping = TextWrapping.NoWrap;
                MyAnswerLabel[i].Width = 45;
                MyAnswerTextBox[i].Height = 20;
                Mygrid.Children.Add(MyAnswerTextBox[i]);



                gridcol += 2;

            }
            OutputWindow = new System.Windows.Controls.TextBox();
            OutputWindow.Name = "OutputWindow";
            OutputWindow.Text = string.Empty;
            Grid.SetRow(OutputWindow, gridrow + 1);
            Grid.SetRowSpan(OutputWindow, 4);
            Grid.SetColumn(OutputWindow, 1);
            Grid.SetColumnSpan(OutputWindow, 15);
            OutputWindow.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            OutputWindow.VerticalAlignment = VerticalAlignment.Top;
            OutputWindow.TextWrapping = TextWrapping.NoWrap;
            OutputWindow.Width = 500;
            OutputWindow.Height = 100;
            OutputWindow.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            OutputWindow.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            Mygrid.Children.Add(OutputWindow);
        }


        void Simplex_Algorithm_AnswerInitialization()
        {
            int gridrow = 2 + numberofconstraint;
            int gridcol = 1;

            for (int i = 0; i < variablenum; i++)
            {
                MyAnswerLabel[i] = new System.Windows.Controls.Label();
                MyAnswerLabel[i].Name = "Answer" + i.ToString();
                MyAnswerLabel[i].Content = "X" + (i + 1).ToString() + " = ";
                Grid.SetRow(MyAnswerLabel[i], gridrow);
                Grid.SetColumn(MyAnswerLabel[i], gridcol);
                Mygrid.Children.Add(MyAnswerLabel[i]);
                gridcol++;
                MyAnswerTextBox[i] = new System.Windows.Controls.TextBox();

                MyAnswerTextBox[i].Name = "Answer_" + i.ToString();

                Grid.SetRow(MyAnswerTextBox[i], gridrow);

                Grid.SetColumn(MyAnswerTextBox[i], gridcol);
                Grid.SetColumnSpan(MyAnswerTextBox[i], 2);
                MyAnswerTextBox[i].TextWrapping = TextWrapping.NoWrap;
                MyAnswerLabel[i].Width = 45;
                MyAnswerTextBox[i].Height = 20;
                Mygrid.Children.Add(MyAnswerTextBox[i]);



                gridcol += 2;

            }
            OutputWindow = new System.Windows.Controls.TextBox();
            OutputWindow.Name = "OutputWindow";
            OutputWindow.Text = string.Empty;
            Grid.SetRow(OutputWindow, gridrow + 1);
            Grid.SetRowSpan(OutputWindow, 4);
            Grid.SetColumn(OutputWindow, 1);
            Grid.SetColumnSpan(OutputWindow, 15);
            OutputWindow.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            OutputWindow.VerticalAlignment = VerticalAlignment.Top;
            OutputWindow.TextWrapping = TextWrapping.NoWrap;
            OutputWindow.Width = 500;
            OutputWindow.Height = 100;
            OutputWindow.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            OutputWindow.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            Mygrid.Children.Add(OutputWindow);
        }
        private void AlgorithmChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            algorithmchoice = AlgorithmChoice.SelectedIndex;
            GSdocument.AlgorithmChoice = algorithmchoice;
            if (firstopen == true)
                return;
            if (algorithmchoice == 0)
            {
                
                RemoveAnswer();
                
                tem = SPdocument.FirstTime;
                RemoveSPCoeff();
                SPdocument.FirstTime = tem;
                TotalEquationText.Text = GSdocument.Totalequ;
                TotalVariablesText.Text = GSdocument.TotalVar;
                TotalConstraintsLabel.Visibility = Visibility.Hidden;
                TotalConstraintsText.Visibility = Visibility.Hidden;
                TotalEquationLabel.Visibility = Visibility.Visible;
                TotalEquationText.Visibility = Visibility.Visible;
                variablenum = GSdocument.Tvar;
                numberofequation = GSdocument.Tequ;
                SetUpGSPCoeff();
                firsttime = GSdocument.FirstTime;
            }
            else
            {
                
                RemoveAnswer();
                tem = GSdocument.FirstTime;
                RemoveGSCoeff();
                GSdocument.FirstTime = tem;
                TotalVariablesText.Text = SPdocument.TotalVar;
                TotalConstraintsText.Text = SPdocument.TotalCon;
                TotalConstraintsLabel.Visibility = Visibility.Visible;
                TotalConstraintsText.Visibility = Visibility.Visible;
                TotalEquationLabel.Visibility = Visibility.Hidden;
                TotalEquationText.Visibility = Visibility.Hidden;
                
                TotalVariablesText.Text = SPdocument.TotalVar;
                TotalConstraintsText.Text = SPdocument.TotalCon;
                variablenum = SPdocument.Tvar;
                numberofconstraint = SPdocument.Tcon;
                SetUpSPCoeff();
                for (int i = 0; i < numberofconstraint; i++)
                    constraintoperation[i] = SPdocument.GetConstraintsOperation[i];
                ismax = SPdocument.IsMax;
                firsttime = SPdocument.FirstTime;
            }
        }

        // read data from GSdocument
        void SetUpGSPCoeff()
        {
            if (GSdocument.FirstTime == true) return;
            MyCoeffTextBox = new System.Windows.Controls.TextBox[numberofequation, variablenum];
            MyLabel = new System.Windows.Controls.Label[numberofequation, variablenum];
            MyBTextBox = new System.Windows.Controls.TextBox[numberofequation];

            MyAnswerTextBox = new System.Windows.Controls.TextBox[variablenum];
            MyAnswerLabel = new System.Windows.Controls.Label[variablenum];

            coeffdata = new double[numberofequation, variablenum];
            bdata = new double[numberofequation];
            answer = new double[variablenum];
            int gridrow;
            int gridcol;
            if (GSdocument.FirstTime == true) return;

            for (int i = 0; i < GSdocument.Tequ; i++)
            {
                //leftmargin = 40;
                //topmargin = 60 + i * 30;
                gridrow = 1 + i;
                gridcol = 1;
                for (int j = 0; j < GSdocument.Tvar; j++)
                {
                    MyCoeffTextBox[i, j] = new System.Windows.Controls.TextBox();
                    MyCoeffTextBox[i, j].Name = "_" + i.ToString() + "_" + j.ToString();
                    MyCoeffTextBox[i, j].Text = GSdocument.GetStringCoeffofvar[i, j];
                    Grid.SetRow(MyCoeffTextBox[i, j], gridrow);
                    Grid.SetColumn(MyCoeffTextBox[i, j], gridcol);
                    MyCoeffTextBox[i, j].TextWrapping = TextWrapping.NoWrap;
                    MyCoeffTextBox[i, j].Width = 45;
                    MyCoeffTextBox[i, j].Height = 20;
                    MyCoeffTextBox[i, j].TextChanged += MyCoeffTextBox_TextChange;
                    Mygrid.Children.Add(MyCoeffTextBox[i, j]);
                    gridcol++;

                    MyLabel[i, j] = new System.Windows.Controls.Label();
                    MyLabel[i, j].Name = "x" + i.ToString() + j.ToString();
                    if (j != variablenum - 1)
                        MyLabel[i, j].Content = "X" + (j + 1).ToString() + " + ";
                    else
                        MyLabel[i, j].Content = "X" + (j + 1).ToString() + " = ";
                    Grid.SetRow(MyLabel[i, j], gridrow);
                    Grid.SetColumn(MyLabel[i, j], gridcol);
                    Mygrid.Children.Add(MyLabel[i, j]);
                    gridcol++;
                }
                MyBTextBox[i] = new System.Windows.Controls.TextBox();
                MyBTextBox[i].Name = "_" + i.ToString() + "_b";
                MyBTextBox[i].Text = GSdocument.GetStringBCoeffofvar[i];
                Grid.SetRow(MyBTextBox[i], gridrow);
                Grid.SetColumn(MyBTextBox[i], gridcol);
                MyBTextBox[i].TextWrapping = TextWrapping.NoWrap;
                MyBTextBox[i].Width = 45;
                MyBTextBox[i].Height = 20;
                MyBTextBox[i].TextChanged += MyBCoeffTextBox_TextChange;

                Mygrid.Children.Add(MyBTextBox[i]);

            }
            Mygrid.UpdateLayout();
        }

        private void comboBox_IsmaxIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Controls.ComboBox ComboboxIsMax = sender as System.Windows.Controls.ComboBox;
            ismax = ComboboxIsMax.SelectedIndex;
            SPdocument.IsMax = ismax;
            SetModifedstate();
        }

        private void comboBox_ConstraintOperationChanged(object sender, EventArgs e)
        {
            System.Windows.Controls.ComboBox ComboBoxOperation = sender as System.Windows.Controls.ComboBox;
            string[] coordinates = ComboBoxOperation.Name.Split('_');

            int index = int.Parse(coordinates[1]);
            constraintoperation[index] = ComboBoxOperation.SelectedIndex;
            SPdocument.GetConstraintsOperation[index] = ComboBoxOperation.SelectedIndex;
            SetModifedstate();
        }

        // read data from SimplexDocument
        void SetUpSPCoeff()
        {
            if (SPdocument.FirstTime == true) return;
            MyCoeffTextBox = new System.Windows.Controls.TextBox[numberofconstraint, variablenum];
            MyLabel = new System.Windows.Controls.Label[numberofconstraint, variablenum];
            MyBTextBox = new System.Windows.Controls.TextBox[numberofconstraint];

            MyObjectivefunctionCoeff = new System.Windows.Controls.TextBox[variablenum];
            MyObjectivefunctionLabel = new System.Windows.Controls.Label[variablenum];
            ConstraintsOperation = new System.Windows.Controls.ComboBox[numberofconstraint];

            MyAnswerTextBox = new System.Windows.Controls.TextBox[variablenum];
            MyAnswerLabel = new System.Windows.Controls.Label[variablenum];

            coeffdata = new double[numberofconstraint, variablenum];
            bdata = new double[numberofconstraint];
            answer = new double[variablenum];
            coeffofobjectivefunction = new double[variablenum];
            
            constraintoperation = new int[numberofconstraint];

            for (int i = 0; i < numberofconstraint; i++)
                constraintoperation[i] = SPdocument.GetConstraintsOperation[i];
            ismax = SPdocument.IsMax;

            int gridrow = 1;
            int gridcol = 1;
            if (SPdocument.FirstTime == true) return;

            Ismaxobjectivefunction = new System.Windows.Controls.ComboBox();
            Ismaxobjectivefunction.Name = "ComboboxObject";
            Ismaxobjectivefunction.Items.Add("Max");
            Ismaxobjectivefunction.Items.Add("Min");
            Ismaxobjectivefunction.SelectedIndex = SPdocument.IsMax;
            Ismaxobjectivefunction.SelectionChanged += comboBox_IsmaxIndexChanged;
            Grid.SetRow(Ismaxobjectivefunction, gridrow);
            Grid.SetColumn(Ismaxobjectivefunction, gridcol);
            Grid.SetColumnSpan(Ismaxobjectivefunction, 2);
            Ismaxobjectivefunction.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Ismaxobjectivefunction.Width = 80;
            Ismaxobjectivefunction.Height = 20;
            Mygrid.Children.Add(Ismaxobjectivefunction);
            gridcol += 2;
            for (int i = 0;i<SPdocument.Tvar;i++)
            {
                MyObjectivefunctionCoeff[i] = new System.Windows.Controls.TextBox();
                MyObjectivefunctionCoeff[i].Name = "objectivefunction_" + i.ToString();
                MyObjectivefunctionCoeff[i].Text = SPdocument.GetStringCoeffofObjectiveFunction[i];
                Grid.SetRow(MyObjectivefunctionCoeff[i], gridrow);
                Grid.SetColumn(MyObjectivefunctionCoeff[i], gridcol);
                
                MyObjectivefunctionCoeff[i].TextWrapping = TextWrapping.NoWrap;
                MyObjectivefunctionCoeff[i].Width = 45;
                MyObjectivefunctionCoeff[i].Height = 20;
                MyObjectivefunctionCoeff[i].TextChanged += MyObjectiveCoeffTextBox_TextChange;
                Mygrid.Children.Add(MyObjectivefunctionCoeff[i]);
                gridcol++;

                MyObjectivefunctionLabel[i] = new System.Windows.Controls.Label();
                MyObjectivefunctionLabel[i].Name = "label_" + i.ToString();
                if (i != variablenum - 1)
                    MyObjectivefunctionLabel[i].Content = "X" + (i + 1).ToString() + " + ";
                else
                    MyObjectivefunctionLabel[i].Content = "X" + (i + 1).ToString();
                Grid.SetRow(MyObjectivefunctionLabel[i], gridrow);
                Grid.SetColumn(MyObjectivefunctionLabel[i], gridcol);
                Mygrid.Children.Add(MyObjectivefunctionLabel[i]);
                gridcol++;
            }

            for (int i = 0; i < SPdocument.Tcon; i++)
            {
                gridrow = 2 + i;
                gridcol = 1;
                for (int j = 0; j < SPdocument.Tvar; j++)
                {
                    MyCoeffTextBox[i, j] = new System.Windows.Controls.TextBox();
                    MyCoeffTextBox[i, j].Name = "_" + i.ToString() + "_" + j.ToString();
                    MyCoeffTextBox[i, j].Text = SPdocument.GetStringCoeffofcon[i, j];
                    Grid.SetRow(MyCoeffTextBox[i, j], gridrow);
                    Grid.SetColumn(MyCoeffTextBox[i, j], gridcol);
                    MyCoeffTextBox[i, j].TextWrapping = TextWrapping.NoWrap;
                    MyCoeffTextBox[i, j].Width = 45;
                    MyCoeffTextBox[i, j].Height = 20;
                    MyCoeffTextBox[i, j].TextChanged += MyCoeffTextBox_TextChange;
                    Mygrid.Children.Add(MyCoeffTextBox[i, j]);
                    gridcol++;

                    MyLabel[i, j] = new System.Windows.Controls.Label();
                    MyLabel[i, j].Name = "x" + i.ToString() + j.ToString();
                    if (j != variablenum - 1)
                        MyLabel[i, j].Content = "X" + (j + 1).ToString() + " + ";
                    else
                        MyLabel[i, j].Content = "X" + (j + 1).ToString();
                    Grid.SetRow(MyLabel[i, j], gridrow);
                    Grid.SetColumn(MyLabel[i, j], gridcol);
                    Mygrid.Children.Add(MyLabel[i, j]);
                    gridcol++;
                }
                ConstraintsOperation[i] = new System.Windows.Controls.ComboBox();
                ConstraintsOperation[i].Name = "ComboBox_" + i.ToString();
                ConstraintsOperation[i].Items.Add("<=");
                ConstraintsOperation[i].Items.Add("=");
                ConstraintsOperation[i].Items.Add(">=");

                ConstraintsOperation[i].SelectedIndex = SPdocument.GetConstraintsOperation[i];
                ConstraintsOperation[i].SelectionChanged += comboBox_ConstraintOperationChanged;
                Grid.SetRow(ConstraintsOperation[i], gridrow);
                Grid.SetColumn(ConstraintsOperation[i], gridcol);
                ConstraintsOperation[i].Width = 45;
                ConstraintsOperation[i].Height = 20;
                Mygrid.Children.Add(ConstraintsOperation[i]);
                gridcol++;

                MyBTextBox[i] = new System.Windows.Controls.TextBox();
                MyBTextBox[i].Name = "_" + i.ToString() + "_b";
                MyBTextBox[i].Text = SPdocument.GetStringBCoeffofcon[i];
                Grid.SetRow(MyBTextBox[i], gridrow);
                Grid.SetColumn(MyBTextBox[i], gridcol);
                Grid.SetColumnSpan(MyBTextBox[i], 2);
                MyBTextBox[i].TextWrapping = TextWrapping.NoWrap;
                MyBTextBox[i].Width = 45;
                MyBTextBox[i].Height = 20;
                MyBTextBox[i].TextChanged += MyBCoeffTextBox_TextChange;

                Mygrid.Children.Add(MyBTextBox[i]);

            }
            Mygrid.UpdateLayout();
        }

        //Remove all the components related to Simplex Algorithm
        void RemoveSPCoeff()
        {
            if (MyCoeffTextBox == null || MyObjectivefunctionCoeff == null)
                return;
            int row = numberofconstraint;
            int col = variablenum;

            if (Ismaxobjectivefunction != null)
                Mygrid.Children.Remove(Ismaxobjectivefunction);
            for (int i = 0; i < col; i++)
            {
                if (MyObjectivefunctionCoeff[i] != null)
                    Mygrid.Children.Remove(MyObjectivefunctionCoeff[i]);
                if (MyObjectivefunctionLabel[i] != null)
                    Mygrid.Children.Remove(MyObjectivefunctionLabel[i]);
            }
            

            for (int i = 0; i < row; i++)
            {

                for (int j = 0; j < col; j++)
                {

                    if (MyCoeffTextBox[i, j] != null)
                    {
                        Mygrid.Children.Remove(MyCoeffTextBox[i, j]);
                    }
                    if (MyLabel[i, j] != null)
                    {
                        Mygrid.Children.Remove(MyLabel[i, j]);
                    }
                }
                if (ConstraintsOperation[i] != null)
                    Mygrid.Children.Remove(ConstraintsOperation[i]);

                if (MyBTextBox[i] != null)
                {
                    Mygrid.Children.Remove(MyBTextBox[i]);
                }

            }

            firsttime = true;
            SPdocument.FirstTime = true;
            Mygrid.UpdateLayout();

        }

        //Remove all the components related to Gauss Elimation
        void RemoveGSCoeff()
        {
            if (MyCoeffTextBox == null)
                return;

            int row = numberofequation;
            int col = variablenum;
            for (int i = 0; i < row; i++)
            {

                for (int j = 0; j < col; j++)
                {
                    if (MyCoeffTextBox[i, j] != null)
                    {
                        Mygrid.Children.Remove(MyCoeffTextBox[i, j]);
                    }

                    if (MyLabel[i, j] != null)
                    {
                        Mygrid.Children.Remove(MyLabel[i, j]);
                    }
                }

                if (MyBTextBox[i] != null)
                {
                    Mygrid.Children.Remove(MyBTextBox[i]);
                }
            }
            firsttime = true;
            GSdocument.FirstTime = true;
            Mygrid.UpdateLayout();
        }

        //Remove all the components related to solution
        void RemoveAnswer()
        {
            if(MyAnswerTextBox == null) return;
            int row = variablenum;
            for (int i = 0; i < row; i++)
            {
                if (MyAnswerLabel[i] != null)
                {
                    Mygrid.Children.Remove(MyAnswerLabel[i]);
                }

                if (MyAnswerTextBox[i] != null)
                {
                    Mygrid.Children.Remove(MyAnswerTextBox[i]);
                }
            }
            if (OutputWindow != null)
            {
                Mygrid.Children.Remove(OutputWindow);
            }
            executeclick = false;
            Mygrid.UpdateLayout();
        }

        private void TotalConstraintsText_TextChanged(object sender, TextChangedEventArgs e)
        {
            SPdocument.TotalCon = TotalConstraintsText.Text;
            SetModifedstate();
            //textchange = true;

        }

        private void AlgorithmChoice_Initialized(object sender, EventArgs e)
        {
            AlgorithmChoice.SelectedIndex = 0;
        }

        private void TotalConstraintsLabel_Initialized(object sender, EventArgs e)
        {
            TotalConstraintsLabel.Visibility = Visibility.Hidden;

        }

        private void TotalConstraintsText_Initialized(object sender, EventArgs e)
        {
            TotalConstraintsText.Visibility = Visibility.Hidden;
        }
    }

}
