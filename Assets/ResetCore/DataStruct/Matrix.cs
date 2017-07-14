using System;
using System.Text;

namespace ResetCore.DataStruct
{
    public class Matrix
    {
        public int row
        {
            get { return matrix.GetLength(0); }
        }
        public int line
        {
            get { return matrix.GetLength(1); }
        }

        private double[,] matrix;

        public double this[int row, int line]
        {
            get { return matrix[row, line]; }
            set { matrix[row, line] = value; }
        }

        public Matrix(double[,] matrix)
        {
            this.matrix = matrix;
        }
        public Matrix(Matrix matrix)
        {
            this.matrix = new double[matrix.row, matrix.line];
            for (int row = 0; row < matrix.row; row++)
            {
                for (int line = 0; line < matrix.line; line++)
                {
                    this.matrix[row, line] = matrix[row, line];
                }
            }
        }

        public Matrix(int row, int line)
        {
            this.matrix = new double[row, line];
        }

        public static Matrix operator *(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.line != matrixB.row)
            {
                throw new Exception();
            }
            double[,] resMartix = new double[matrixA.row, matrixB.line];

            for (int i = 0; i < matrixA.row; i++)
            {
                for (int j = 0; j < matrixB.line; j++)
                {
                    resMartix[i, j] = 0;
                    for (int n = 0; n < matrixA.line; n++)
                    {
                        resMartix[i, j] += matrixA[i, n] * matrixB[n, j];
                    }
                }
            }

            Matrix res = new Matrix(resMartix);
            return res;
        }

        public static Matrix operator *(double num, Matrix matrix)
        {

            double[,] resMartix = new double[matrix.row, matrix.line];
            for (int i = 0; i < matrix.row; i++)
            {
                for (int j = 0; j < matrix.line; j++)
                {
                    resMartix[i, j] = matrix[i, j] * num;
                }
            }
            return new Matrix(resMartix);
        }

        public static Matrix operator +(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.line != matrixB.line || matrixA.row != matrixB.row)
            {
                return null;
            }
            double[,] resMartix = new double[matrixA.row, matrixA.line];
            for (int i = 0; i < matrixA.row; i++)
            {
                for (int j = 0; j < matrixA.line; j++)
                {
                    resMartix[i, j] = matrixA[i, j] + matrixB[i, j];
                }
            }
            Matrix res = new Matrix(resMartix);
            return res;
        }

        public static Matrix operator -(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.line != matrixB.line || matrixA.row != matrixB.row)
            {
                return null;
            }
            double[,] resMartix = new double[matrixA.row, matrixA.line];
            for (int i = 0; i < matrixA.row; i++)
            {
                for (int j = 0; j < matrixA.line; j++)
                {
                    resMartix[i, j] = matrixA[i, j] - matrixB[i, j];
                }
            }
            Matrix res = new Matrix(resMartix);
            return res;
        }


        public Matrix Transpose()
        {
            //Console.Write(" 行:" + row + "列:" + line);
            //Console.Write("输出：\n" + ToString());
            double[,] resMartix = new double[line, row];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < line; j++)
                {
                    //Console.Write("\n" + "新数组：" + j + ":" + i + " 源数组：" + i + ":" + j );
                    resMartix[j, i] = matrix[i, j];
                }
            }
            //Console.Write("输出：\n" + new Matrix(resMartix).ToString());
            return new Matrix(resMartix);

        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("矩阵 " + row + "行:" + line + "列\n");
            for (int r = 0; r < row; r++)
            {
                for (int l = 0; l < line; l++)
                {
                    str.Append(" " + string.Format("{0:N3}", matrix[r, l]));
                }
                str.Append("\n");
            }

            return str.ToString();
        }
    }
}
