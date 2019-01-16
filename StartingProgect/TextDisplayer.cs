using System;
using System.Windows.Controls;

namespace StartingProgect
{
    public class TextDisplayer
    {
        public static void DisplayDoubleMas(RichTextBox richTextBox, double [][] mas)
        {
            for (int i = 0; i < mas.Length; i++)
            {
                richTextBox.AppendText("i = " + i + "  ");
                for (int j = 0; j < mas[i].Length; j++)
                {
                    richTextBox.AppendText("   " + mas[i][j].ToString() + "   ");
                }
                richTextBox.AppendText("\n");
            }
        }
        public static void DisplayIntMas(RichTextBox richTextBox, int[][] mas)
        {
            for (int i = 0; i < mas.Length; i++)
            {
                richTextBox.AppendText("i = " + i + "  ");
                for (int j = 0; j < mas[i].Length; j++)
                {
                    richTextBox.AppendText("   " + mas[i][j].ToString() + "   ");
                }
                richTextBox.AppendText("\n");
            }
        }
    }
}
