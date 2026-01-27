using System;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace My_program.Helpers
{
    public static class NumberFormatter
    {
        /// <summary>
        /// แปลง TextBox ให้แสดงเลขมีจุดคั่นพัน (real-time)
        /// </summary>
        /// <param name="textBox">TextBox ที่ต้องการใช้งาน</param>
        /// <param name="integerOnly">true = จำนวนเต็มเท่านั้น, false = รองรับทศนิยม</param>
        public static void ApplyNumberComma(TextBox textBox, bool integerOnly = false)
        {
            // จัดการ TextInput event สำหรับจัดรูปแบบตัวเลข (เมื่อพิมพ์)
            textBox.AddHandler(TextBox.TextInputEvent, (sender, e) =>
            {
                if (sender is TextBox tb)
                {
                    // ป้องกันการพิมพ์จุดทศนิยมซ้ำ
                    if (!integerOnly && e.Text == "." && !string.IsNullOrEmpty(tb.Text) && tb.Text.Contains("."))
                    {
                        e.Handled = true; // บล็อคการพิมพ์จุดซ้ำ
                        return;
                    }
                    
                    // ป้องกันการพิมพ์จุดถ้าเป็น integerOnly
                    if (integerOnly && e.Text == ".")
                    {
                        e.Handled = true;
                        return;
                    }
                    
                    Dispatcher.UIThread.Post(() =>
                    {
                        NumberFormat(tb, integerOnly);
                    }, DispatcherPriority.Background);
                }
            }, Avalonia.Interactivity.RoutingStrategies.Tunnel);

            // จัดการ KeyDown event สำหรับตรวจสอบการกดปุ่ม
            textBox.AddHandler(TextBox.KeyDownEvent, (sender, e) =>
            {
                if (sender is TextBox tb)
                {
                    // จัดการการลบ (Backspace หรือ Delete)
                    if (e.Key == Key.Back || e.Key == Key.Delete)
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            NumberFormat(tb, integerOnly);
                        }, DispatcherPriority.Background);
                    }
                    
                    if (integerOnly)
                    {
                        IntValidator(tb, e);
                    }
                    else
                    {
                        DoubleValidator(tb, e);
                    }
                }
            }, Avalonia.Interactivity.RoutingStrategies.Tunnel);

            // จัดการ TextChanged event สำหรับการเปลี่ยนแปลงใดๆ (รวม paste, cut)
            textBox.TextChanged += (sender, e) =>
            {
                if (sender is TextBox tb)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        NumberFormat(tb, integerOnly);
                    }, DispatcherPriority.Background);
                }
            };
        }

        /// <summary>
        /// จัดรูปแบบตัวเลขให้มีเครื่องหมายจุลภาค
        /// </summary>
        private static void NumberFormat(TextBox txtField, bool integerOnly)
        {
            if (string.IsNullOrEmpty(txtField.Text))
            {
                return;
            }

            // Save original caret position
            int originalCaretPos = txtField.CaretIndex;
            string originalText = txtField.Text;

            string inputNum = txtField.Text.Replace(",", "");
            string commafiedNum = "";
            char firstChar = inputNum.Length > 0 ? inputNum[0] : ' ';

            // จัดการเครื่องหมาย + หรือ -
            if (firstChar == '+' || firstChar == '-')
            {
                commafiedNum = commafiedNum + firstChar.ToString();
                inputNum = inputNum.Replace("+", "").Replace("-", "");
            }

            string[] splittedNum = inputNum.Split('.');
            string decimalNum = "";

            // แยกส่วนทศนิยม (รองรับเฉพาะจุดเดียว)
            if (!integerOnly && splittedNum.Length >= 2)
            {
                // ถ้ามีจุดหลายตัว เอาแค่ส่วนแรกและส่วนที่สอง (ตัดส่วนที่เกินทิ้ง)
                inputNum = splittedNum[0];
                decimalNum = "." + splittedNum[1];
            }
            else if (integerOnly)
            {
                // ถ้าเป็น integerOnly ให้เอาเฉพาะส่วนแรก (ไม่มีทศนิยม)
                inputNum = splittedNum[0];
            }
            else if (splittedNum.Length == 1)
            {
                // ไม่มีจุดทศนิยม
                inputNum = splittedNum[0];
            }

            // ใส่เครื่องหมายจุลภาค
            int numLength = inputNum.Length;
            for (int i = 0; i < numLength; i++)
            {
                if ((numLength - i) % 3 == 0 && i != 0)
                {
                    commafiedNum += ",";
                }
                commafiedNum += inputNum[i];
            }

            // ลบเครื่องหมายจุลภาคที่อยู่ตำแหน่งแรก (ถ้ามี)
            if (commafiedNum.Length > 0 && !char.IsDigit(commafiedNum[0]))
            {
                commafiedNum = commafiedNum.Substring(1);
            }

            string newText = commafiedNum + decimalNum;

            // Calculate new caret position
            int commasBeforeCaret = originalText.Substring(0, Math.Min(originalCaretPos, originalText.Length)).Count(c => c == ',');
            int commasInNew = newText.Substring(0, Math.Min(originalCaretPos, newText.Length)).Count(c => c == ',');
            int newCaretPos = originalCaretPos + (commasInNew - commasBeforeCaret);

            txtField.Text = newText;
            txtField.CaretIndex = Math.Min(Math.Max(newCaretPos, 0), txtField.Text.Length);
        }

        /// <summary>
        /// ตรวจสอบให้รับเฉพาะตัวเลขจำนวนเต็ม
        /// </summary>
        private static void IntValidator(TextBox txtField, KeyEventArgs e)
        {
            // อนุญาตเฉพาะตัวเลข, backspace, delete, enter, arrow keys
            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                e.Handled = true; // บล็อคปุ่มจุด
            }
        }

        /// <summary>
        /// ตรวจสอบให้รับตัวเลขทศนิยม
        /// </summary>
        private static void DoubleValidator(TextBox txtField, KeyEventArgs e)
        {
            // บล็อคจุดทศนิยมเพิ่มเติมถ้ามีอยู่แล้ว
            if ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && !string.IsNullOrEmpty(txtField.Text) && txtField.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// แปลง string แบบมีจุดคั่น กลับเป็น double
        /// </summary>
        public static double ParseNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            
            string clean = text.Replace(",", "");
            if (double.TryParse(clean, out double result))
                return result;
            return 0;
        }

        /// <summary>
        /// แปลง double เป็น string แบบมีจุดคั่นพัน
        /// </summary>
        public static string Format(double number)
        {
            return number.ToString("#,0.##");
        }
    }
}
