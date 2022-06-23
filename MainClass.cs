using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHTriangle
{
    public class MainClass
    {
        static void Main(string[] args)
        {
            // 输入列数和行数
            Console.WriteLine("输入杨辉三角上想表示数字的位置\n行数n和列数m(1<n<m<10000)\n以空格分割:");
            string inputStr = Console.ReadLine();
            int inputRows = 0;
            int inputCols = 0;

            // 以空格分割输入内容
            string[] inputStrSplit = inputStr.Split(' ');
            // 输入内容检查
            while (inputStrSplit.Length != 2
                || !int.TryParse(inputStrSplit[0], out inputRows)
                || !int.TryParse(inputStrSplit[1], out inputCols)
                || inputCols > inputRows)
            {
                // 输入列数和行数
                Console.WriteLine("输入内容有误，请重新输入\n行数n和列数m(1<n<m<10000)\n以空格分割:");
                inputStr = Console.ReadLine();
                inputStrSplit = inputStr.Split(' ');
            }

            // 输出杨辉三角中n行m列的数字
            OutPutResult(inputRows, inputCols);

            return;
        }

        /// <summary>
        /// 输出杨辉三角中n行m列的数字
        /// </summary>
        /// <param name="inputRows"></param>
        /// <param name="inputCols"></param>
        /// <param name="colsCalc"></param>
        public static void OutPutResult(int inputRows, int inputCols)
        {
            // *1
            // 杨辉三角中 第n行的第m个数和第n-m+1个数相等 
            // 例:第5行第4列 = 第5行第2列 
            // 若列数在右侧，则取得对应的左侧的列数
            int colsCalc = inputCols;
            if (2 * inputCols > inputRows + 1)
            {
                colsCalc = inputRows - inputCols + 1;
            }

            // 杨辉三角n行m列的数为C(n-1,m-1) 
            string result = String.Empty;
            if (inputRows < 60)
            {
                // 行数较小(大概以60行为分界)时，使用ulong类型直接计算
                result = CaculateCjk(inputRows - 1, colsCalc - 1).ToString();
            }
            else
            {
                // 行数较大时，视为大数计算
                result = GetCjkResultStr(inputRows - 1, colsCalc - 1);
            }
            Console.WriteLine($"杨辉三角{inputRows}行第{inputCols}列的数为:\n{result}");
            Console.ReadKey();
        }

        #region ulong直接计算结果
        /// <summary>
        /// ulong直接计算C(j,k)
        /// </summary>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns>C(j,k)结果</returns>
        public static ulong CaculateCjk(int j, int k)
        {
            // 此步可省略,*1运行后不会出现共同项，若无*1执行此项
            // 去共同项
            // 例：Factorial(10,2)/Factorial(5,1) 共同项为 3*4*5
            // 同：Factorial(10,5)/Factorial(2,1) 
            //int startNumF = j;
            //int endNumF = j - k;
            //int startNumS = k;
            //int tempNum = 0;
            //if (startNumS > endNumF)
            //{ 
            //    tempNum = endNumF;
            //    endNumF = startNumF;
            //    startNumF = tempNum;
            //}

            return Factorial((ulong)j, (ulong)(j - k)) / Factorial((ulong)k, 1);
        }

        /// <summary>
        /// 阶乘递归，从startNum+1 到 endNum 
        /// 例:Factorial(10,6) = 10*9*8*7*1
        /// </summary>
        /// <param name="endNum"></param>
        /// <param name="startNum"></param>
        /// <returns>阶乘结果</returns>
        public static ulong Factorial(ulong endNum,ulong startNum)
        {
            // 边界时返回结果
            if (endNum == startNum || endNum <= 0 || startNum <= 0)
            {
                return 1;
            }
            else
            {
                // 阶乘递归
                endNum *= Factorial(endNum - 1, startNum);
                return endNum;
            }  
        }
        #endregion

        #region 大数 计算结果
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns>字符串结果</returns>
        public static string GetCjkResultStr(int j, int k)
        {
            // 边界时直接返回结果
            if (j == 0 || k == 0)
            {
                return "1";
            }
            else if (k == 1)
            { 
                return j.ToString();
            }

            // 被除数（乘法算式）字符串取得
            string dividend = FactorialStr(j, j - k);
            // 除数（乘法算式）字符串取得
            string divisor = FactorialStr(k, 1);

            // 被除数（乘法算式）字符串分割
            string[] dividendSplit = dividend.Split('*');
            // 除数（乘法算式）字符串分割
            string[] divisorSplit = divisor.Split('*');

            // 被除数（乘法算式）结果取得
            string dividendRes = GetMultiplicationResult(dividendSplit);
            // 除数（乘法算式）结果取得
            string divisorRes = GetMultiplicationResult(divisorSplit);

            // 商结果取得
            return GetDivisionResult(dividendRes, divisorRes);
        }

        /// <summary>
        /// 取得除法结果
        /// </summary>
        /// <param name="dividend">被除数</param>
        /// <param name="divisor">除数</param>
        /// <returns>商</returns>
        public static string GetDivisionResult(string dividend, string divisor)
        {
            // ulong最大长度20位
            // 被除数大于19位时使用大数除法
            if (dividend.Length > 19)
            {
                return strDivision(dividend.ToCharArray(), divisor.ToCharArray());
            }
            // 被除数小于19位时，直接相除
            else
            {
                return (ulong.Parse(dividend) / ulong.Parse(divisor)).ToString();
            }
        }

        /// <summary>
        /// 取得乘法结果
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <returns>乘积</returns>
        public static string GetMultiplicationResult(string[] strArray)
        {
            for (int i = 1; i < strArray.Length - 1; i++)
            {
                // ulong最大长度20位
                // 乘积位数超出19时，使用大数乘法
                if (strArray[i].Length + strArray[i - 1].Length > 18)
                {
                    strArray[i] = strMultiplication(strArray[i].ToCharArray(), strArray[i - 1].ToCharArray());
                }
                // 乘积位数未超出19时，直接相乘
                else
                {
                    strArray[i] = (ulong.Parse(strArray[i]) * ulong.Parse(strArray[i - 1])).ToString();
                }
            }
            // 结果返回
            return strArray[strArray.Length - 2];
        }

        /// <summary>
        /// C(j,k)计算公式
        /// </summary>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns>C(j,k)计算公式</returns>
        public static string CaculateCjkFormula(int j, int k)
        {
            // 计算公式
            return FactorialStr(j, j - k)+ "/\n"+ FactorialStr(k, 1);
        }

        /// <summary>
        /// 字符串格式阶乘递归 从startNum+1 到 endNum 
        /// </summary>
        /// <param name="endNum">结束位</param>
        /// <param name="startNum">起始位</param>
        /// <returns>字符串相乘结果</returns>
        public static string FactorialStr(int endNum, int startNum)
        {
            if (endNum == startNum || endNum <= 0 || startNum <= 0)
                return "1";
            else
            {
                // 字符串格式阶乘递归
                string resultStr = endNum.ToString() + "*" + FactorialStr(endNum - 1, startNum);
                return resultStr;
            }
        }

        /// <summary>
        /// 大数乘法
        /// </summary>
        /// <param name="multiplier">乘数</param>
        /// <param name="multiplicand">被乘数</param>
        /// <returns>乘积</returns>
        public static string strMultiplication(char[] multiplierArray, char[] multiplicandArray)
        {
            // 各位乘积存储
            int[] result = new int[multiplierArray.Length + multiplicandArray.Length];

            // 两数各位相乘
            for (int i = 0; i < multiplierArray.Length; i++)
            {
                for (int j = 0; j < multiplicandArray.Length; j++)
                {
                    result[i + j + 1] += int.Parse(multiplierArray[i].ToString()) * int.Parse(multiplicandArray[j].ToString());
                }
            }

            // 进位处理
            for (int k = result.Length - 1; k > 0; k--)
            {
                if (result[k] >= 10)
                {
                    result[k-1] += result[k]/10;
                    result[k] %= 10;
                }
            }

            // 拼接字符
            StringBuilder resultStrBuilder = new StringBuilder();
            foreach (int temp in result)
            {
                resultStrBuilder.Append(temp);
            }

            // 删除起始位的0
            string resultstr = resultStrBuilder.ToString();
            if (resultstr.StartsWith("0"))
            { 
                resultstr = resultstr.Substring(1);
            }

            // 返回结果
            return resultstr;
        }

        /// <summary>
        /// 大数除法
        /// </summary>
        /// <param name="dividend">被除数</param>
        /// <param name="divisor">除数</param>
        /// <returns>商</returns>
        public static string strDivision(char[] dividendArray, char[] divisorArray)
        {
            // 自然数乘积的位数，等于两数的位数和，或 位数和-1
            // 可判定商的位数为 digits~digits+1 范围内
            int digits = dividendArray.Length - divisorArray.Length;

            // 若被除数大于除数*digits位，则商的位数+1
            if (compareCharArray(dividendArray, divisorArray, false) > 0)
            {
                digits++;
            }

            // 商初始化
            char[] result = new char[digits];
            for (int j = 0; j < result.Length; j++)
            {
                result[j] = '0';
            }

            for (int i = 0; i < digits; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    // 乘数设定
                    result[i] = j.ToString().ToCharArray()[0];
                    // 大数乘法
                    string mutiResult = strMultiplication(result, divisorArray);
                    int compareResult = compareCharArray(mutiResult.ToCharArray(), dividendArray);
                    if (compareResult > 0)
                    {
                        // 结果赋值
                        result[i] = (j - 1).ToString().ToCharArray()[0];
                        break ;
                    }
                    else if (compareResult == 0)
                    {
                        // 结果赋值
                        result[i] = j.ToString().ToCharArray()[0];
                        // 相等时跳出循环
                        goto gotoEnd;
                    }
                }
            }

        gotoEnd:

            // 结果拼接
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char res in result)
            {
                stringBuilder.Append(res);
            }
            // 返回结果
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 大数 大小比较
        /// </summary>
        /// <param name="charArrA"></param>
        /// <param name="charArrB"></param>
        /// <param name="flag">是否进行位数大小判断 默认判断</param>
        /// <returns>1：大于 0：等于 -1：小于</returns>
        public static int compareCharArray(char[] charArrA, char[] charArrB, bool flag = true)
        {
            // 判断位数长度
            if (flag)
            {
                if (charArrA.Length > charArrB.Length)
                {
                    return 1;
                }
                else if (charArrA.Length < charArrB.Length)
                {
                    return -1;
                }
            }

            // 按字符逐个判断大小
            for (int i = 0; i < charArrB.Length; i++)
            {
                if (charArrA[i] > charArrB[i])
                {
                    return 1;
                }
                else if (charArrA[i] < charArrB[i])
                {
                    return -1;
                }
            }
            return 0;
        }
        #endregion
    }
}
