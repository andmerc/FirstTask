using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Task_3
{
    public class VeryLongNumber : IComparable<VeryLongNumber>, IEquatable<VeryLongNumber>
    {
        private List<int> chunks;
        private int sign;

        private readonly Regex ValidString = new Regex(@"-?\d+", RegexOptions.Compiled);

        public VeryLongNumber(string number)
        {
            if(number == null)
            {
                throw new ArgumentNullException();
            }
            if (!ValidString.IsMatch(number))
            {
                throw new FormatException();
            }

            chunks = new List<int>();

            string num;
            if (number[0]=='-')
            {
                sign = -1;
                num = number.Substring(1);
            }
            else
            {
                sign = 1;
                num = number;
            }

            while (num.Length > 0)
            {
                chunks.Add(int.Parse(num.Substring(num.Length - 3 > 0 ? num.Length - 3 : 0)));
                num = num.Substring(0, num.Length - 3 > 0 ? num.Length - 3 : 0);
            }

            Normalize();
        }
        public VeryLongNumber(int number) : this(number.ToString())
        {

        }
        public VeryLongNumber()
        {
            chunks = new List<int>();
            sign = 0;
        }
        
        private void Normalize()
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                chunks[i] = Math.Abs(chunks[i]);
            }
            for (int i = chunks.Count - 1; i > 0 && chunks[i] == 0; i--)
            {
                chunks.RemoveAt(i);
            }
            if (chunks.Count == 1 && chunks[0] == 0)
            {
                sign = 0;
            }
        }

        public static VeryLongNumber Abs(VeryLongNumber number)
        {
            if(number.sign == 0)
            {
                return number;
            }
            var absNum = new VeryLongNumber();
            for(int i = 0; i < number.chunks.Count; i++)
            {
                absNum.chunks.Add(number.chunks[i]);
            }
            absNum.sign = 1;
            return absNum;
        }

        public static VeryLongNumber operator +(VeryLongNumber leftOperand, VeryLongNumber rightOperand)
        {
            if (leftOperand.sign == 0)
            {
                return rightOperand;
            }
            else if (rightOperand.sign == 0)
            {
                return leftOperand;
            }
            var result = new VeryLongNumber(); int sum = 0;
            if(Abs(leftOperand) > Abs(rightOperand))
            {
                result.sign = leftOperand.sign;
            }
            else
            {
                result.sign = rightOperand.sign;
            }

            for (int i = 0; i < leftOperand.chunks.Count || i < rightOperand.chunks.Count || sum != 0; i++)
            {
                if(i < leftOperand.chunks.Count)
                {
                    sum += leftOperand.chunks[i] * leftOperand.sign;
                }
                if (i < rightOperand.chunks.Count)
                {
                    sum += rightOperand.chunks[i] * rightOperand.sign;
                }
                if(Math.Sign(sum) == result.sign || sum == 0)
                {
                    result.chunks.Add(sum % 1000);
                    sum /= 1000;
                }
                else
                {
                    result.chunks.Add((sum + 1000 * result.sign) % 1000);
                    sum = (sum - 1000 * result.sign) / 1000;
                }
            }
            result.Normalize();
            return result;
        }
        public static VeryLongNumber operator -(VeryLongNumber leftOperand, VeryLongNumber rightOperand)
        {
            if (leftOperand.sign == 0)
            {
                var res = rightOperand;
                res.sign *= -1;
                return res;
            }
            else if (rightOperand.sign == 0)
            {
                return leftOperand;
            }
            var result = new VeryLongNumber(); int residual = 0;
            if (Abs(leftOperand) > Abs(rightOperand))
            {
                result.sign = leftOperand.sign;
            }
            else
            {
                result.sign = -1 * rightOperand.sign;
            }
            for (int i = 0; i < leftOperand.chunks.Count || i < rightOperand.chunks.Count || residual > 0; i++)
            {
                if (i < leftOperand.chunks.Count)
                {
                    residual += leftOperand.chunks[i] * leftOperand.sign;
                }
                if (i < rightOperand.chunks.Count)
                {
                    residual -= rightOperand.chunks[i] * rightOperand.sign;
                }
                if (Math.Sign(residual) == result.sign || residual == 0)
                {
                    result.chunks.Add(residual % 1000);
                    residual /= 1000;
                }
                else
                {
                    result.chunks.Add((residual + 1000 * result.sign) % 1000);
                    residual = (residual - 1000 * result.sign) / 1000;
                }
            }
            result.Normalize();
            return result;
        }
        public static VeryLongNumber operator *(VeryLongNumber leftOperand, VeryLongNumber rightOperand)
        {
            if (leftOperand.sign == 0 || rightOperand.sign == 0)
            {
                return new VeryLongNumber("0");
            }
            var result = new VeryLongNumber();
            int product;
            for (int i = 0; i < leftOperand.chunks.Count; i++)
            {
                product = 0;
                for (int j = 0; j < rightOperand.chunks.Count; j++)
                {
                    product += leftOperand.chunks[i] * rightOperand.chunks[j];
                    if(result.chunks.Count < i + j + 1)
                    {
                        result.chunks.Add(product % 1000);
                    }
                    else
                    {
                        product += 1000 * ((result.chunks[i + j] + product % 1000) / 1000);
                        result.chunks[i+j] = (result.chunks[i + j] + product % 1000) % 1000;
                    }
                    product /= 1000;
                }
                if(product > 0) result.chunks.Add(product);
            }
            result.sign = leftOperand.sign * rightOperand.sign;
            result.Normalize();
            return result;
        }
        public static VeryLongNumber operator /(VeryLongNumber leftOperand, VeryLongNumber rightOperand)
        {
            if (rightOperand.sign == 0)
            {
                throw new DivideByZeroException();
            }
            else if (leftOperand.sign == 0 || Abs(leftOperand) < Abs(rightOperand))
            {
                return new VeryLongNumber("0");
            }
            VeryLongNumber result = new VeryLongNumber("0"),
                           minuend = new VeryLongNumber(leftOperand.ToString()) { sign = 1 },
                           subtrahend = new VeryLongNumber(rightOperand.ToString()) { sign = 1 },
                           factor = new VeryLongNumber("1");
            int quotient;
            for (int i = 0; i < minuend.ToString().Length-subtrahend.ToString().Length; i++)
            {
                factor *= new VeryLongNumber("10");
            }
            while(minuend >= subtrahend)
            {
                quotient = 0;
                while (minuend >= subtrahend * factor * new VeryLongNumber(quotient + 1))
                {
                    quotient++;
                }
                result *= new VeryLongNumber("10");
                result += new VeryLongNumber(quotient);
                minuend -= subtrahend * factor * new VeryLongNumber(quotient);
                if (minuend >= subtrahend && factor > new VeryLongNumber("1"))
                {
                    factor = new VeryLongNumber(factor.ToString().Substring(0, factor.ToString().Length - 1));
                }
            }
            result *= factor;

            result.sign = leftOperand.sign * rightOperand.sign;
            result.Normalize();
            return result;
        }
        public static VeryLongNumber operator %(VeryLongNumber leftOperand, VeryLongNumber rightOperand)
        {
            return leftOperand - leftOperand / rightOperand * rightOperand;
        }

        public static VeryLongNumber operator -(VeryLongNumber number)
        {
            return new VeryLongNumber(number.ToString()) { sign = -1 * number.sign };
        }
        public static VeryLongNumber operator ++(VeryLongNumber number)
        {
            return number + new VeryLongNumber("1");
        }
        public static VeryLongNumber operator --(VeryLongNumber number)
        {
            return number - new VeryLongNumber("1");
        }

        public static bool operator <(VeryLongNumber n1, VeryLongNumber n2)
        {
            return n1.CompareTo(n2) == -1;
        }
        public static bool operator >(VeryLongNumber n1, VeryLongNumber n2)
        {
            return n1.CompareTo(n2) == 1;
        }
        public static bool operator <=(VeryLongNumber n1, VeryLongNumber n2)
        {
            return n1.CompareTo(n2) == -1 || n1.Equals(n2);
        }
        public static bool operator >=(VeryLongNumber n1, VeryLongNumber n2)
        {
            return n1.CompareTo(n2) == 1 || n1.Equals(n2);
        }
        public static bool operator !=(VeryLongNumber n1, VeryLongNumber n2)
        {
            return !n1.Equals(n2);
        }
        public static bool operator ==(VeryLongNumber n1, VeryLongNumber n2)
        {
            return n1.Equals(n2);
        }

        public int CompareTo(VeryLongNumber number)
        {
            if (Math.Sign(chunks.Count - number.chunks.Count) != 0)
            {
                return Math.Sign(chunks.Count * sign - number.chunks.Count * number.sign);
            }
            else
            {
                int totalSign = 0, i = chunks.Count;
                while (i-- > 0 && totalSign == 0)
                {
                    totalSign = Math.Sign(chunks[i] * sign - number.chunks[i] * number.sign);
                }
                return totalSign;
            }
        }
        public bool Equals(VeryLongNumber number)
        {
            return GetHashCode() == number.GetHashCode() && CompareTo(number) == 0;
        }

        public override string ToString()
        {
            string stringView;
            if(sign == -1)
            {
                stringView = "-";
            }
            else
            {
                stringView = "";
            }
            for (int i = chunks.Count-1; i >= 0; i--)
            {
                if (i != chunks.Count - 1)
                {
                    for (int j = 0; j < 3 - chunks[i].ToString().Length; j++)
                    {
                        stringView += "0";
                    }
                }
                stringView += chunks[i].ToString();
            }
            return stringView;
        }
        public override int GetHashCode()
        {
            return 107 * chunks.Count * sign + 511;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as VeryLongNumber);
        }
    }
}