using System;
using System.Text;
using System.Linq;

namespace checker
{
    public static class PolandWrite
    {
        public static bool AreBracketsCorrect(string expression)
        {
            Stack<char> brackets = new Stack<char>();
            for (int i = 0; i < expression.Length; i++)
            {
                switch (expression[i])
                {
                    case ')':
                        if (brackets.Count == 0) return false;
                        brackets.Pop();
                        break;
                    case '(':
                        brackets.Push(expression[i]);
                        break;
                    case ' ':
                        return false;
                    case '.':
                        return false;
                    default: break;
                }

                if (i < expression.Length - 1 && (expression[i] == '*' || expression[i] == '+' || expression[i] == '-' || expression[i] == '/'))
                {
                    if (expression[i] != '-' && (i == 0)) return false;
                    if (!Char.IsDigit(expression[i + 1]) && expression[i + 1] != '(') return false;
                }

                if (i == expression.Length - 1 && !Char.IsDigit(expression[i]) && expression[i] != ')') return false;
                if (i < expression.Length - 1 && (expression[i] == '(' && expression[i + 1] == ')')) return false;
            }

            return brackets.Count == 0;
        }

        static void Main()
        {
            var expression = Console.ReadLine();

            if (!AreBracketsCorrect(expression)) throw new Exception("Некорректное выражение");
            var a = Count(ref expression);
            var final = new StackForCount();
            for (int i = 0; i < a.Count; i++)
            {
                Console.Write(a[i] + " ");
                final.Add(a[i]);
            }

            Console.WriteLine("\n" + final.Pop());
        }

        public static List<String> Count(ref string expression)
        {
            Stack<string> polski = new Stack<string>();
            List<String> result = new List<String>();
            for (int i = 0; i < expression.Length;)
            {
                if (expression[i] == '(')
                {
                    AddFromBrackets(ref expression, i, polski);
                    i++;
                    continue;
                }

                if (expression[i] == '*' || expression[i] == '/')
                {
                    i = MultiplyOrDivide(ref expression, i, polski);
                    continue;
                }

                else if (expression[i] == '-' || expression[i] == '+')
                {
                    if (i == 0 || expression[i - 1] == '(')
                    {
                        i++;
                        continue;
                    }

                    i = AddOrSubtract(ref expression, i, polski);
                }

                else i++;
            }

            while (polski.Count() > 0)
            {
                result.Add(polski.Pop());
            }

            result.Reverse();
            return result;
        }

        public static string Number(string expression, int indexof, string way)
        {
            var i = new StringBuilder();
            int minuscounter = 0;
            int dotcounter = 0;
            while (indexof >= 0 && indexof < expression.Length && (Char.IsDigit(expression[indexof]) || expression[indexof] == ' ' || expression[indexof] == '-' || expression[indexof] == ','))
            {
                if (expression[indexof] == '-')
                {
                    if (minuscounter == 0)
                    {
                        minuscounter = 1;
                        i.Append(expression[indexof]);
                    }
                    else break;
                }
                else if (expression[indexof] == ',')
                {
                    if (dotcounter == 0)
                    {
                        dotcounter = 1;
                        i.Append(expression[indexof]);
                    }
                    else throw new Exception();
                }
                else i.Append(expression[indexof]);
                if (way == "right") indexof++;
                else indexof--;
            }

            return (way == "right") ? i.ToString() : new string(i.ToString().ToCharArray().Reverse().ToArray());
        }

        public static void AddFromBrackets(ref string expression, int index, Stack<string> final)
        {
            int indexoffinal = index + 1;
            StackForBrackets check = new StackForBrackets();
            check.Add('(');
            for (int j = index + 1; ;)
            {
                if (expression[j] == '(')
                {

                    AddFromBrackets(ref expression, j + 1, final);
                    continue;
                }

                if (expression[j] == ')')
                {
                    indexoffinal = j;
                    break;
                }
                if (expression[j] == '*' || expression[j] == '/')
                {
                    j = MultiplyOrDivide(ref expression, j, final);
                    continue;
                }

                else if (expression[j] == '+' || expression[j] == '-')
                {
                    if (j == 0 || expression[j - 1] == '(')
                    {
                        j++;
                        continue;
                    }

                    j = AddOrSubtract(ref expression, j, final);
                }

                else j++;
            }

            expression = expression.Remove(index, 1);
            expression = expression.Remove(indexoffinal - 1, 1);
        }

        public static int MultiplyOrDivide(ref string expression, int j, Stack<string> final)
        {
            var two = Number(expression, j + 1, "right");
            if (two.Count() == 0)
            {

                AddFromBrackets(ref expression, j + 1, final);
                return j;
            }

            else
            {
                var one = Number(expression, j - 1, "left");
                if (one[one.Count() - 1] != ' ') final.Push(one);
                if (two[two.Count() - 1] != ' ') final.Push(two);
                final.Push(expression[j].ToString());

                if (expression[j] == '*')
                {
                    expression = expression.Remove(j - one.Length, one.Length + two.Length + 1);
                    expression = expression.Insert(j - one.Length, (double.Parse(two.Replace(" ", "")) * double.Parse(one.Replace(" ", ""))).ToString() + " ");
                }

                else
                {
                    expression = expression.Remove(j - one.Length, one.Length + two.Length + 1);
                    expression = expression.Insert(j - one.Length, (double.Parse(one.Replace(" ", "")) / double.Parse(two.Replace(" ", ""))).ToString() + " ");
                }

                return j - one.Length;
            }
        }

        public static int AddOrSubtract(ref string expression, int j, Stack<string> final)
        {
            var two = Number(expression, j + 1, "right");
            if (two.Count() == 0)
            {

                AddFromBrackets(ref expression, j + 1, final);
                return j;
            }

            if (j + two.Count() + 1 < expression.Length && (expression[j + two.Count() + 1] == '*' || expression[j + two.Count() + 1] == '/'))
            {
                MultiplyOrDivide(ref expression, j + two.Count() + 1, final);
                return j;
            }

            var one = Number(expression, j - 1, "left");
            if (one[one.Count() - 1] != ' ') final.Push(one);
            if (two[two.Count() - 1] != ' ') final.Push(two);
            final.Push(expression[j].ToString());
            var a = expression[j];
            expression = expression.Remove(j - one.Length, one.Length + two.Length + 1);
            if (a == '+') expression = expression.Insert(j - one.Length, (double.Parse(two.Replace(" ", "")) + double.Parse(one.Replace(" ", ""))).ToString() + " ");
            else expression = expression.Insert(j - one.Length, (double.Parse(one.Replace(" ", "")) - double.Parse(two.Replace(" ", ""))).ToString() + " ");
            return j - one.Length;
        }
    }

    public class StackForBrackets
    {
        public Stack<char> brackets = new Stack<char>();

        public void Add(char bracket)
        {
            if (bracket == '(') brackets.Push(bracket);
            else brackets.Pop();
        }
        public int Count() { return this.brackets.Count; }
    }

    public class StackForCount
    {
        public Stack<string> count = new Stack<string>();
        public void Add(string add)
        {

            try
            {
                var a = double.Parse(add.Replace(" ", ""));
                count.Push(add);
            }
            catch
            {
                var a = double.Parse(count.Pop());
                var b = double.Parse(count.Pop());
                if (add == "*") count.Push((a * b).ToString());
                if (add == "/") count.Push((b / a).ToString());
                if (add == "+") count.Push((b + a).ToString());
                if (add == "-") count.Push((b - a).ToString());
            }
        }

        public string Pop()
        {
            return count.Pop();
        }
        public int Count() { return count.Count; }
    }
}