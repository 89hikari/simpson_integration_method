using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private double A = -10, B = 10, C = -10, D = 10, Alpha, Beta, Gamma, Delta, Mu, Del, Epsilon, a, b, sh, nMaximum;
        private int n;

        private readonly PointPairList _axis = new PointPairList();
        private readonly PointPairList _graphline = new PointPairList();


        public Form1()
        {
            InitializeComponent();

            radioButton1.Checked = true;
            radioButton_Alph.Checked = true;

            _axis.Add(0, 0);

            Graph.GraphPane.AddCurve("", _axis, Color.Red, SymbolType.None);

			Graph.GraphPane.Title.Text = "f(x) = Alpha * sin(Beta/(x-Gamma)^2) + Delta * cos(Epsilon/(x-Mu)^2)";

            Graph.GraphPane.XAxis.Cross = 0.0; 
            Graph.GraphPane.YAxis.Cross = 0.0; 

            Graph.GraphPane.YAxis.MajorGrid.IsVisible = true;

            Graph.GraphPane.XAxis.Scale.IsSkipFirstLabel = true;
            Graph.GraphPane.XAxis.Scale.IsSkipLastLabel = true;
            Graph.GraphPane.YAxis.Scale.IsSkipFirstLabel = true;
            Graph.GraphPane.YAxis.Scale.IsSkipLastLabel = true;
            Graph.GraphPane.XAxis.Scale.IsSkipCrossLabel = true;
            Graph.GraphPane.YAxis.Scale.IsSkipCrossLabel = true;

            Graph.GraphPane.XAxis.Title.IsVisible = false;
            Graph.GraphPane.YAxis.Title.IsVisible = false;

            Graph.GraphPane.XAxis.Scale.Min = A;
            Graph.GraphPane.XAxis.Scale.Max = B;
            Graph.GraphPane.YAxis.Scale.Min = C;
            Graph.GraphPane.YAxis.Scale.Max = D;

            Graph.Invalidate(); 
            Graph.AxisChange();
        }

        private void Build_up_Click(object sender, EventArgs e)
        {
            Graph.GraphPane.CurveList.Clear();

			if (radioButton1.Checked)
                Delta = 1.0;
            if (radioButton2.Checked)
                Delta = 0.1;
            if (radioButton3.Checked)
                Delta = 0.01;
            if (radioButton4.Checked)
                Delta = 0.001;

            A = (double)Par_A.Value; 
            B = (double)Par_B.Value;
            C = (double)Par_C.Value;
            D = (double)Par_D.Value;
            a = (double)numeric_a.Value;
            b = (double)numeric_b.Value;
            n = (int)numeric_points.Value;

            if (A >= B)
            {
                double temp = A;
                A = B;
                B = temp;
                Par_A.Value = (decimal)A;
                Par_B.Value = (decimal)B;
            }
            if (C >= D)
            {
                double temp = C;
                C = D;
                D = temp;
                Par_C.Value = (decimal)C;
                Par_D.Value = (decimal)D;
            }
            if (a >= b)
            {
                double temp = a;
                a = b;
                numeric_a.Value = (decimal)a;
                b = temp;
                numeric_b.Value = (decimal)b;
            }

            sh = (Math.Abs(A) + Math.Abs(B)) / 600;

            Alpha = (double)numericAlpha.Value;
            Beta = (double)numericBeta.Value;
            Gamma = (double)numericGamma.Value;
            Epsilon = (double)numericEpsilon.Value;
            Del = (double)numericDel.Value;
            Mu = (double)numericMu.Value;

            Graph.GraphPane.XAxis.Scale.Min = A;
            Graph.GraphPane.XAxis.Scale.Max = B;
            Graph.GraphPane.YAxis.Scale.Min = C;
            Graph.GraphPane.YAxis.Scale.Max = D;

            Graph.AxisChange();
            Graph.Invalidate();

            draw_graph();
        }

        private void Clear_Click(object sender, EventArgs e)
        {

            Graph.GraphPane.CurveList.Clear();
            nMaximum = 0;
            nMax.Text = Convert.ToString(nMaximum);

            Graph.AxisChange();
            Graph.Invalidate();
        }

        private void Param_Alph_CheckedChanged(object sender, EventArgs e)
        {
            Alph_box.Visible = true;
            Bet_box.Visible = false;
            Gam_box.Visible = false;
            Eps_box.Visible = false;
            DelBox.Visible = false;
            MuBox.Visible = false;
        }
        private void Param_Bet_CheckedChanged(object sender, EventArgs e)
        {
            Alph_box.Visible = false;
            Bet_box.Visible = true;
            Gam_box.Visible = false;
            Eps_box.Visible = false;
            DelBox.Visible = false;
            MuBox.Visible = false;
        }
        private void Param_Gamma_CheckedChanged(object sender, EventArgs e)
        {
            Alph_box.Visible = false;
            Bet_box.Visible = false;
            Gam_box.Visible = true;
            Eps_box.Visible = false;
            DelBox.Visible = false;
            MuBox.Visible = false;
        }
        private void Param_Del_CheckedChanged(object sender, EventArgs e)
        {
            Alph_box.Visible = false;
            Bet_box.Visible = false;
            Gam_box.Visible = false;
            Eps_box.Visible = false;
            DelBox.Visible = true;
            MuBox.Visible = false;
        }
        private void Param_Eps_CheckedChanged(object sender, EventArgs e)
        {
            Alph_box.Visible = false;
            Bet_box.Visible = false;
            Gam_box.Visible = false;
            Eps_box.Visible = true;
            DelBox.Visible = false;
            MuBox.Visible = false;
        }

        private void Param_Mu_CheckedChanged(object sender, EventArgs e)
        {
            Alph_box.Visible = false;
            Bet_box.Visible = false;
            Gam_box.Visible = false;
            Eps_box.Visible = false;
            DelBox.Visible = false;
            MuBox.Visible = true;
        }

        double Fx(double x, double al, double bet, double gam, double del, double eps, double mu)
        {
            var step1 = Math.Pow(x - gam, 2);
            var step2 = Math.Pow(x - mu, 2);
            if (Double.IsInfinity(step1) || step1 < 0.001 || Double.IsInfinity(step2) || step2 < 0.001)
                return 0;
            return al * Math.Sin(bet / step1) + del * Math.Cos(eps / step2);
        }

        double SimpsonMethod(double par, int nCount)
        {
            double h = (Math.Abs(b) + Math.Abs(a)) / nCount;
	        double intSum1 = 0, intSum2 = 0, intSum3 = 0;
	        double j, k, l;

			if (radioButton_Alph.Checked)
            {
                for (double x = a; x < b; x += h)
                {
	                j = 2 * x - 2;
	                k = 2 * x - 1;
	                l = 2 * x;
                    intSum1 += Fx(j, par, Beta, Gamma, Del, Epsilon, Mu);
                    intSum2 += Fx(k, par, Beta, Gamma, Del, Epsilon, Mu);
                    intSum3 += Fx(l, par, Beta, Gamma, Del, Epsilon, Mu);
                }
                return h * (intSum1 + 4 * intSum2 + intSum3) / 3;
            }

			if (radioButton_Bet.Checked)
            {
                for (double x = a; x < b; x += h)
				{
					j = 2 * x - 2;
					k = 2 * x - 1;
					l = 2 * x;
                    intSum1 += Fx(j, Alpha, par, Gamma, Del, Epsilon, Mu);
                    intSum2 += Fx(k, Alpha, par, Gamma, Del, Epsilon, Mu);
                    intSum3 += Fx(l, Alpha, par, Gamma, Del, Epsilon, Mu);
                }
				return h * (intSum1 + 4 * intSum2 + intSum3) / 3;
            }

			if (radioButton_Gamma.Checked)
            {
                for (double x = a; x < b; x += h)
				{
					j = 2 * x - 2;
					k = 2 * x - 1;
					l = 2 * x;

                    intSum1 += Fx(j, Alpha, Beta, par, Del, Epsilon, Mu);
                    intSum2 += Fx(k, Alpha, Beta, par, Del, Epsilon, Mu);
                    intSum3 += Fx(l, Alpha, Beta, par, Del, Epsilon, Mu);

                }
				return h * (intSum1 + 4 * intSum2 + intSum3) / 3;
            }

            if (radioButton_Del.Checked)
            {
                for (double x = a; x < b; x += h)
                {
                    j = 2 * x - 2;
                    k = 2 * x - 1;
                    l = 2 * x;
                    intSum1 += Fx(j, Alpha, Beta, Gamma, par, Epsilon, Mu);
                    intSum2 += Fx(k, Alpha, Beta, Gamma, par, Epsilon, Mu);
                    intSum3 += Fx(l, Alpha, Beta, Gamma, par, Epsilon, Mu);
                }
                return h * (intSum1 + 4 * intSum2 + intSum3) / 3;
            }

            if (radioButton_Epsilon.Checked)
            {
                for (double x = a; x < b; x += h)
				{
					j = 2 * x - 2;
					k = 2 * x - 1;
					l = 2 * x;
                    intSum1 += Fx(j, Alpha, Beta, Gamma, Del, par, Mu);
                    intSum2 += Fx(k, Alpha, Beta, Gamma, Del, par, Mu);
                    intSum3 += Fx(l, Alpha, Beta, Gamma, Del, par, Mu);
                }
				return h * (intSum1 + 4 * intSum2 + intSum3) / 3;
            }

            if (radioButton_Mu.Checked)
            {
                for (double x = a; x < b; x += h)
                {
                    j = 2 * x - 2;
                    k = 2 * x - 1;
                    l = 2 * x;
                    intSum1 += Fx(j, Alpha, Beta, Gamma, Del, Epsilon, par);
                    intSum2 += Fx(k, Alpha, Beta, Gamma, Del, Epsilon, par);
                    intSum3 += Fx(l, Alpha, Beta, Gamma, Del, Epsilon, par);
                }
                return h * (intSum1 + 4 * intSum2 + intSum3) / 3;
            }
            return 0;
        }

        double Runge(double par)
        {
            int nCount = n;
            double int2N = SimpsonMethod(par, nCount * 2);
            double intn = SimpsonMethod(par, (nCount * 2) - 2);

            while (Delta < (1.0 / 3.0) * Math.Abs(int2N - intn))
            {
                nCount = nCount * 2;
                int2N = SimpsonMethod(par, nCount * 2);
                intn = SimpsonMethod(par, nCount);
            }

            if (nCount > nMaximum)
                nMaximum = nCount;

            return int2N;
        }

        void draw_graph()
        {
            _graphline.Clear();
            nMaximum = 0;
			Graph.GraphPane.YAxis.MajorGrid.IsVisible = true;
            Graph.GraphPane.XAxis.MajorGrid.IsVisible = true;
            Graph.GraphPane.XAxis.Scale.Min = A;
            Graph.GraphPane.XAxis.Scale.Max = B;
            Graph.GraphPane.YAxis.Scale.Min = C;
            Graph.GraphPane.YAxis.Scale.Max = D;
			for (double par = A; par <= B; par += sh)
				_graphline.Add(par, Runge(par));

			nMax.Text = Convert.ToString(nMaximum);

            LineItem mygraphline = Graph.GraphPane.AddCurve("", _graphline, Color.Red, SymbolType.None);
            mygraphline.Line.Width = 2;

            mygraphline.Line.IsSmooth = true;
            Graph.AxisChange();
            Graph.Invalidate();

        }
    }
}
