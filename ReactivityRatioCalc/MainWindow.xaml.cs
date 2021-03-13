using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace ReactivityRatioCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _r1;
        private double _r2;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                var m1Vals = M1Vals.Text.Split(",");
                var m2Vals = M2Vals.Text.Split(",");
                //var testM1Vals = ".58667,.58667,.58667,.17472,.17472,.17472,.44263,.44263,.44263,.49878,.49878,.49878,.13753,.13753,.13522,.13522";
                //var testM2Vals = ".68831,.71541,.72123,.362168,.386959,.388711,.604341,.627048,.648347,.642779,.616691,.624412,.303973,.304759,.314079,.306983";
                //var testM1Vals = ".58667,.58667,.58667,.17472,.17472,.17472,.44263,.44263,.44263,.49878,.49878,.49878,.13753,.13753,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522,.13522";
                //var testM2Vals = ".68831,.71541,.72123,.362168,.386959,.388711,.604341,.627048,.648347,.642779,.616691,.624412,.303973,.304759,.314079,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983,.306983";
                //var testM1Vals = ".677,.576,.476,.381,.281,.188";
                //var testM2Vals = ".681,.567,.443,.288,.208,.112";
                //var M1= Array.ConvertAll(testM1Vals.Split(','), double.Parse);
                //var M2 = Array.ConvertAll(testM2Vals.Split(','), double.Parse);
                double[] M1 = Array.ConvertAll(m1Vals, double.Parse);
                double[] M2 = Array.ConvertAll(m2Vals, double.Parse);

                if (M1.Length == M2.Length)
                    Calculate(M1, M2);
                else
                    txtBlockOutput.Text = $"Please ensure inputs are the length. # of M1:{M1.Length}, # of M2:{M2.Length}";
            }
            catch (Exception ex)
            {
                txtBlockOutput.Text = ex.Message;
            }
            
        }
        private void Calculate(double[] M1, double[] M2)
        {
            txtBlockOutput.Text = "Calculating...";
            bool debug = true;
            int size = M1.Length;
            double[] eta = new double[size];
            double[] ksi = new double[size];
            double[] gee = new double[size];
            double[] ef = new double[size];
            double [] t = new double[] { 12.71, 4.3, 3.18, 2.78, 2.57, 2.45, 2.37, 2.31, 2.26, 2.23, 2.2, 2.18, 2.16, 2.14, 2.13, 2.12, 2.11, 2.1, 2.09, 2.09 };

            double vari = 0;
            double sumasi = 0;
            double sumi = 0;
            double suma = 0;
            double sumsqi = 0;
            double sumnegi = 0;
            double sum2i = 0;
            double fmin = 0;
            double fmax= 0;


            for (var ct = 0; ct<M1.Length; ct++)
            {
                double m20 = 1 - M1[ct];
                double x = M1[ct] / m20;
                double p1 = M2[ct];
                double p2 = 1 - p1;
                double y = p1 / p2;
                double g = ((y - 1) * x) / y;
                double f = Math.Pow(x, 2)/y;

                if (ct == 0)
                    fmin = f;
                else if (fmin > f)
                    fmin = f;
                if (ct == 0)
                    fmax = f;
                else if (fmax < f)
                    fmax = f;

                gee[ct] = g;
                ef[ct] = f;
                if(debug)
                    Console.WriteLine($"M1value({ct+1}): {M1[ct]}M2value({ct+1}): {M2[ct]} Fmin= {fmin,17:F15} Fmax= {fmax,17:F15}F= {f,17:F15} gee({ct + 1})= {gee[ct],17:F15} ef({ct+1})= {ef[ct],17:F15}");
            }

            for(var m=0;m< size; m++)
            {
                double alpha = Math.Sqrt(fmin * fmax);

                double a = gee[m] / (alpha + ef[m]);
                double si = ef[m] / (alpha + ef[m]);

                sumasi = (a * si) + sumasi;
                sumi = si + sumi;
                suma = a + suma;
                sumsqi = Math.Pow(si, 2) + sumsqi;
                sumnegi = (1 - si) + sumnegi;
                sum2i = si * (1 - si) + sum2i;
                eta[m] = a;
                ksi[m] = si;
                if(debug)
                    Console.WriteLine($"{m+1}: alpha={alpha,17:F15}  a={a,17:F15} si={si,17:F15}: sumasi={sumasi,17:F15} sumi= {sumi,17:F15} suma= {suma,17:F15} sumsqi= {sumsqi,17:F15} sumnegi= {sumnegi,17:F15} sum2i= {sum2i,17:F15} eta({m+1})={eta[m],17:F15} ksi({m+1})={ksi[m],17:F15}");
                

                if (m == size-1)
                {

                    _r1 = (sumasi * (size - sumi) - suma * (sumi - sumsqi)) / ((size * sumsqi) - (Math.Pow(sumi, 2)));
                    _r2 = (alpha * ((sumasi * sumi) - (suma * sumsqi))) / ((size * sumsqi) - (Math.Pow(sumi, 2)));
                    txtBlockOutput.Text = $"Results: R1: {_r1}, R2: {_r2}";
                    //At this point its done. the rest is a while loop to calculate the deltr1 deltr2 which at this point is not needed.
                    

                    for (var ct=0;ct<size-1;ct++)
                    {
                        var smin = Math.Pow(((eta[ct]) - (_r1 * ksi[ct])) + ((_r2 / alpha) * (1 - ksi[ct])), 2);
                        vari = smin + vari; 
                    }
                    Console.Write($"Value for length of t: {t.Length}");
                    Console.Write($"Value of size: {size}");
                    double d = sumsqi * (Math.Pow(sumnegi, 2) - (Math.Pow(sum2i, 2)));
                    double tVal = 0;
                    //t was a data source that supplied but didn't  account for data sets over 20, if they exist hard code the value as the last possible data point.
                    //TODO figure out how d is calcuated and then extend it out past 20 for enchanced accurary. 
                    if (size > 19)
                        tVal = 2.09;
                    else
                        tVal = t[m - 2];


                    double deltr1 = tVal * Math.Sqrt((vari / size - 2)) * ((Math.Pow(sumnegi, 2) / d));
                    double deltr2 = alpha * tVal * Math.Sqrt((vari / size - 2)) * (sumsqi / d);
                    
                }
            }
        }
        
    }
}

