using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace calculator
{
    public partial class FormMain : Form
    {
        public enum SymbolType
        {
            Number,
            Operator,
            SpecialOp,
            DecimalPoint,
            backspace,
            PlusMinusSign,
            cancellAll,
            cancellEntry,
            undefined
        }
        public struct BtnStruct
        {
            public char Content;
            public SymbolType type;
            public bool IsBold;
            public bool IsNumber;
            public BtnStruct(char c, SymbolType t = SymbolType.undefined, bool b = false, bool n=false)
            {
                this.Content = c;
                this.type = t;
                this.IsBold = b;
                this.IsNumber = n;
            }
        }
        float lblResultBaseFontSize;
        const int lblResultWidthMargin = 24;
        const int lblResultMaxDigit = 25;
        char lastOperator = ' ';
        decimal op1, op2, result;
        BtnStruct lastButtonClicked;

        private BtnStruct[,] buttons =
        {
            { new BtnStruct('%',SymbolType.SpecialOp), new BtnStruct('\u0152',SymbolType.cancellEntry), new BtnStruct('C',SymbolType.cancellAll), new BtnStruct('\u232B',SymbolType.backspace) },
            { new BtnStruct('\u215F',SymbolType.SpecialOp), new BtnStruct('\u00B2',SymbolType.SpecialOp), new BtnStruct('\u221A',SymbolType.SpecialOp), new BtnStruct('\u00F7',SymbolType.Operator )},
            { new BtnStruct('7',SymbolType.Number,true,true), new BtnStruct('8',SymbolType.Number,true,true), new BtnStruct('9',SymbolType.Number,true,true), new BtnStruct('\u00D7',SymbolType.Operator) },
            { new BtnStruct('4',SymbolType.Number,true,true), new BtnStruct('5',SymbolType.Number,true,true), new BtnStruct('6',SymbolType.Number,true,true), new BtnStruct('-',SymbolType.Operator) },
            { new BtnStruct('1',SymbolType.Number,true,true), new BtnStruct('2',SymbolType.Number,true,true), new BtnStruct('3',SymbolType.Number,true,true), new BtnStruct('+',SymbolType.Operator) },
            { new BtnStruct('\u00B1',SymbolType.PlusMinusSign,true), new BtnStruct('0',SymbolType.Number,true,true), new BtnStruct(',',SymbolType.DecimalPoint,true), new BtnStruct('=',SymbolType.Operator) },
        };

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MakeButtons(buttons.GetLength(0), buttons.GetLength(1));
            lblResultBaseFontSize = lbl_result.Font.Size;
        }

        private void MakeButtons(int rows, int cols)
        {
            int btnWidth = 80;
            int btnHeight = 60;
            int posX = 0;
            int posY = 116;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Button myButton = new Button();
                    FontStyle fs = buttons[i, j].IsBold ? FontStyle.Bold : FontStyle.Regular;
                    myButton.Font = new Font("Segoe UI", 16, fs);
                    myButton.BackColor = buttons[i, j].IsBold ? Color.White : Color.Transparent;
                    myButton.Text = buttons[i, j].Content.ToString();
                    myButton.Width = btnWidth;
                    myButton.Height = btnHeight;
                    myButton.Top = posY;
                    myButton.Left = posX;
                    myButton.Tag = buttons[i, j];
                    myButton.Click += Button_Click;
                    this.Controls.Add(myButton);
                    posX += myButton.Width;
                }
                posX = 0;
                posY += btnHeight;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            BtnStruct clickedButtonStruct = (BtnStruct)clickedButton.Tag;

            switch (clickedButtonStruct.type)
            {
                case SymbolType.Number:
                    if (lbl_result.Text == "0" || lastButtonClicked.type == SymbolType.Operator)
                    {
                        lbl_result.Text = "";
                    }
                    lbl_result.Text += clickedButton.Text;

                    break;
                case SymbolType.SpecialOp:
                    SpecialManageOperator(clickedButtonStruct);
                    break;
                case SymbolType.Operator:
                    if (lastButtonClicked.type == SymbolType.Operator && clickedButtonStruct.Content != '=')
                    {
                        lastOperator = clickedButtonStruct.Content;
                        
                    }
                    else
                    {
                       
                        ManageOperator(clickedButtonStruct);
                      
                    }
                  
                    break;
              

                case SymbolType.DecimalPoint:
                    if (lbl_result.Text.IndexOf(",") == -1)
                    {
                        lbl_result.Text += clickedButton.Text;
                    }
                    break;
                case SymbolType.PlusMinusSign:
                    if (lbl_result.Text != "0")
                    {
                        if (lbl_result.Text.IndexOf("-") == -1)
                        {
                            lbl_result.Text = "-" + lbl_result.Text;
                        }
                        else
                        {
                            lbl_result.Text = lbl_result.Text.Substring(1);
                        }
                    }
                    break;
                case SymbolType.backspace:
                    if (lastButtonClicked.type != SymbolType.Operator)
                    {
                        lbl_result.Text = lbl_result.Text.Substring(0, lbl_result.Text.Length - 1);
                        if (lbl_result.Text.Length == 0 || lbl_result.Text == "-0" || lbl_result.Text == "-")
                        {
                            lbl_result.Text = "0";
                        }
                    }
                  
                    break;
                case SymbolType.cancellAll:
                    NewMethod();

                    break;
                case SymbolType.cancellEntry:
                    if(lastButtonClicked.Content == '=')
                    {
                      
                        NewMethod();
                    }
                    else
                    {
                        lbl_result.Text = "0";
                    }
                    break;
                case SymbolType.undefined:
                    break;
                default:
                    break;
            }
            if(clickedButtonStruct.type != SymbolType.backspace && clickedButtonStruct.type != SymbolType.PlusMinusSign)
                lastButtonClicked = clickedButtonStruct;

        }

        private void NewMethod()
        {
            op1 = 0;
            op2 = 0;
            result = 0;
            lastOperator = ' ';
            lbl_result.Text = "0";
        }
        private void SpecialManageOperator(BtnStruct clickedButtonStruct)
        {
            if (clickedButtonStruct.type == SymbolType.SpecialOp)
            {
                op2 = decimal.Parse(lbl_result.Text);
                switch (clickedButtonStruct.Content)
                {
                    case '%':
                        result = (op1 *op2) / 100;
                        break;
                    case '\u215F': // 1/x
                        result = 1 / op2;
                        break;
                    case '\u00B2':
                        result = op2 * op1;
                        break;
                    case '\u221A':
                        result = (decimal)Math.Sqrt((double)op2);
                        break;

                    default:
                        break;
                }
                //op1 = result;
                lbl_result.Text = result.ToString();

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void ManageOperator(BtnStruct clickedButtonStruct)
        {
         
            
            
                if (lastOperator == ' ')
                {
                    op1 = decimal.Parse(lbl_result.Text);
                label1.Text = op1.ToString();
                lastOperator = clickedButtonStruct.Content;
                }
                else
                {
                    if (lastButtonClicked.Content != '=')
                {
                    op2 = decimal.Parse(lbl_result.Text);

                    label3.Text = op2.ToString();
                }
                       
                switch (lastOperator)
                    {
                        case '+':
                        label2.Text = "+";
                        result = op1 + op2;
                            break;

                        case '-':
                        label2.Text = "-";
                        result = op1 - op2;
                            break;

                        case '\u00D7':
                        label2.Text = "*";
                        result = op1 * op2;
                            break;

                        case '\u00F7':
                        label2.Text = "/";
                        result = op1 / op2;
                            break;

                        default:
                            break;
                    }
                    op1 = result;
                    if (clickedButtonStruct.Content != '=')
                    {
                        lastOperator = clickedButtonStruct.Content;
                        if (lastButtonClicked.Content == '=') op2 = 0;
                    }
                    lbl_result.Text = result.ToString();
                }
            
        }

        private void lbl_result_TextChanged(object sender, EventArgs e)
        {
            if (lbl_result.Text == "-")
            {
                lbl_result.Text = "0";
                return;
            }
            if (lbl_result.Text.Length > 0)
            {
                decimal num = decimal.Parse(lbl_result.Text); string stOut = "";
                NumberFormatInfo nfi = new CultureInfo("it-IT", false).NumberFormat;
                int decimalSeparatorPosition = lbl_result.Text.IndexOf(",");
                nfi.NumberDecimalDigits = decimalSeparatorPosition == -1 ? 0
                  : lbl_result.Text.Length - decimalSeparatorPosition - 1;
                stOut = num.ToString("N", nfi);
                if (lbl_result.Text.IndexOf(",") == lbl_result.Text.Length - 1) stOut += ",";
                lbl_result.Text = stOut;


            }

            if (lbl_result.Text.Length > lblResultMaxDigit)
                lbl_result.Text = lbl_result.Text.Substring(0, lblResultMaxDigit);

            int textWidth = TextRenderer.MeasureText(lbl_result.Text, lbl_result.Font).Width;
            float newSize = lbl_result.Font.Size * (((float)lbl_result.Size.Width - lblResultWidthMargin) / textWidth);
            if (newSize > lblResultBaseFontSize)
            {
                newSize = lblResultBaseFontSize;
            }
            lbl_result.Font = new Font("Segoe UI", newSize, FontStyle.Regular);
        }
    }
}
