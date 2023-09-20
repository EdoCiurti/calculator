using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            DecimalPoint,
            backspace,
            PlusMinusSign,
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

        private BtnStruct[,] buttons =
        {
            { new BtnStruct('%'), new BtnStruct('\u0152'), new BtnStruct('C'), new BtnStruct('\u232B',SymbolType.backspace) },
            { new BtnStruct('\u215F'), new BtnStruct('\u00B2'), new BtnStruct('\u221A'), new BtnStruct('\u00F7') },
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
                    if (lbl_result.Text == "0")
                    {
                        lbl_result.Text = "";
                    }
                    lbl_result.Text += clickedButton.Text;
                    break;
                case SymbolType.Operator:
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
                    lbl_result.Text = lbl_result.Text.Substring(0,lbl_result.Text.Length - 1);
                    if(lbl_result.Text.Length == 0 || lbl_result.Text == "0")
                    {
                        lbl_result.Text = "0";
                    }

                    break;
                case SymbolType.undefined:
                    break;
                default:
                    break;
            }
        }
    }
}
