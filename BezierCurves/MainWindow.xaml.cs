using OxyPlot;
using OxyPlot.Series;
using System;
using System.Windows;

namespace BezierCurves
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataPoint[] DataPoints = new DataPoint[4];
        DataPoint[] abcdefPoints = new DataPoint[5];
        LineSeries pointSeries_abcdef = new LineSeries();
        LineSeries ab_Line = new LineSeries();
        LineSeries bc_Line = new LineSeries();
        LineSeries de_Line = new LineSeries();
        LineSeries end_Line = new LineSeries();
        int scale = 0;

        double t = 0;

        PlotModel plt_model = new PlotModel();
        public MainWindow()
        {
            DataPoints[0] = new DataPoint(-1.41, 0);
            DataPoints[1] = new DataPoint(0, -2);
            DataPoints[2] = new DataPoint(1.41, 0);
            DataPoints[3] = new DataPoint(4, 14);

            LineSeries startSeries = new LineSeries();
            startSeries.Color = OxyColors.Gray;
            startSeries.Points.AddRange(DataPoints);


            ab_Line = new LineSeries();
            ab_Line.Color = OxyColors.Orange;
            ab_Line.Points.Add(DataPoints[0]);
            ab_Line.Points.Add(DataPoints[1]);

            bc_Line = new LineSeries();
            bc_Line.Color = OxyColors.Orange;
            bc_Line.Points.Add(DataPoints[1]);
            bc_Line.Points.Add(DataPoints[2]);

            de_Line.Color = OxyColors.Red;

            end_Line.Color = OxyColors.Purple;
            end_Line.MarkerSize = 5;

            pointSeries_abcdef = CreateLineseries(OxyColors.Green);
            abcdefPoints = new DataPoint[] { DataPoints[0], DataPoints[1], DataPoints[2], DataPoints[0], DataPoints[1], DataPoints[0] };
            pointSeries_abcdef.Points.AddRange(abcdefPoints);

            plt_model.Series.Add(startSeries);
            plt_model.Series.Add(pointSeries_abcdef);
            plt_model.Series.Add(ab_Line);
            plt_model.Series.Add(bc_Line);
            plt_model.Series.Add(de_Line);
            plt_model.Series.Add(end_Line);

            InitializeComponent();

            var myController = new PlotController();
            plt_end.Controller = myController;

            //  Customizing the bindings 
            myController.UnbindMouseDown(OxyMouseButton.Right);
            //Mouse

            myController.BindMouseDown(OxyMouseButton.Left, OxyPlot.PlotCommands.ZoomRectangle);

            myController.BindMouseDown(OxyMouseButton.Right, OxyPlot.PlotCommands.Track);

            //Keyboard

            myController.BindKeyDown(OxyKey.R, OxyPlot.PlotCommands.Reset);

            myController.BindKeyDown(OxyKey.W, OxyPlot.PlotCommands.PanDown);
            myController.BindKeyDown(OxyKey.A, OxyPlot.PlotCommands.PanRight);
            myController.BindKeyDown(OxyKey.S, OxyPlot.PlotCommands.PanUp);
            myController.BindKeyDown(OxyKey.D, OxyPlot.PlotCommands.PanLeft);

            plt_end.Model = plt_model;
        }
        public LineSeries CreateLineseries(OxyColor color)
        {
            LineSeries lineSeries = new LineSeries();
            lineSeries.LineStyle = LineStyle.None;
            lineSeries.MarkerType = MarkerType.Circle;
            lineSeries.MarkerFill = color;
            lineSeries.MarkerSize = 5;
            return lineSeries;
        }
        double Lerp(double a, double b, double t)
        {
            return (1f - t) * a + t * b;
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = (double)slider.Value;
            if (Math.Abs(t - value) >= 0.001)
            {
                t = value;
                for (int i = 0; i < 2; i++)
                {
                    ab_Line.Points[i] = abcdefPoints[i];
                }
                for (int i = 0; i < 2; i++)
                {
                    bc_Line.Points[i] = abcdefPoints[i + 1];
                }
                for (int i = 0; i < 2; i++)
                {
                    if (de_Line.Points.Count == 2)
                        de_Line.Points[i] = abcdefPoints[i + 3];
                    else
                        de_Line.Points.Add(abcdefPoints[i + 3]);

                }
                for (int i = 0; i < 3; i++)
                {
                    double x = Lerp(DataPoints[i].X, DataPoints[i + 1].X, t);
                    double y = Lerp(DataPoints[i].Y, DataPoints[i + 1].Y, t);
                    abcdefPoints[i] = new DataPoint(x, y);
                }
                for (int i = 0; i < 2; i++)
                {
                    double x = Lerp(abcdefPoints[i].X, abcdefPoints[i + 1].X, t);
                    double y = Lerp(abcdefPoints[i].Y, abcdefPoints[i + 1].Y, t);
                    abcdefPoints[i + 3] = new DataPoint(x, y);
                }

                double x_end = Lerp(abcdefPoints[3].X, abcdefPoints[4].X, t);
                double y_end = Lerp(abcdefPoints[3].Y, abcdefPoints[4].Y, t);
                abcdefPoints[5] = new DataPoint(x_end, y_end);

                for (int i = 0; i < 6; i++)
                {
                    pointSeries_abcdef.Points[i] = abcdefPoints[i];
                }


                int index = (int)(t * 99.999);
                if (index > scale)
                {
                    end_Line.Points.Add(new DataPoint(x_end, y_end));
                    scale++;
                }


                plt_model.Series[1] = pointSeries_abcdef;
                plt_model.Series[2] = ab_Line;
                plt_model.Series[3] = bc_Line;
                plt_model.Series[4] = de_Line;
                plt_model.Series[5] = end_Line;
                plt_end.Model = plt_model;
                plt_end.InvalidatePlot();
            }
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            bool areValuesGood = true;
            bool[] badData = new bool[4] { true, true, true, true };

            float px0, px1, px2, px3;
            float py0, py1, py2, py3;

            if (float.TryParse(x0.Text, out px0))
            {
                if (float.TryParse(y0.Text, out py0))
                {
                    DataPoints[0] = new DataPoint(px0, py0);
                    badData[0] = false;
                }
            }
            if (float.TryParse(x1.Text, out px1))
            {
                if (float.TryParse(y1.Text, out py1))
                {
                    DataPoints[1] = new DataPoint(px1, py1);
                    badData[1] = false;
                }
            }
            if (float.TryParse(x2.Text, out px2))
            {
                if (float.TryParse(y2.Text, out py2))
                {
                    DataPoints[2] = new DataPoint(px2, py2);
                    badData[2] = false;
                }
            }
            if (float.TryParse(x3.Text, out px3))
            {
                if (float.TryParse(y3.Text, out py3))
                {
                    DataPoints[3] = new DataPoint(px3, py3);
                    badData[3] = false;
                }
            }
            for (int i = 0; i < badData.Length; i++)
            {
                if (badData[i] == true)
                {
                    MessageBox.Show($"Please write correct value to x{i} and y{i} !\nExample: x{i} = 3,53 y{i} = 13");
                    areValuesGood = false;
                }
            }
            if (areValuesGood)
            {
                LineSeries startSeries = new LineSeries();
                startSeries.Color = OxyColors.Gray;
                startSeries.Points.AddRange(DataPoints);


                ab_Line.Points[0] = DataPoints[0];
                ab_Line.Points[1] = DataPoints[1];

                bc_Line.Points[0] = DataPoints[1];
                bc_Line.Points[1] = DataPoints[2];

                pointSeries_abcdef.Points.Clear();
                abcdefPoints = new DataPoint[] { DataPoints[0], DataPoints[1], DataPoints[2], DataPoints[0], DataPoints[1], DataPoints[0] };
                pointSeries_abcdef.Points.AddRange(abcdefPoints);

                end_Line.Points.Clear();

                plt_model.Series[0] = startSeries;
                plt_model.Series[1] = pointSeries_abcdef;
                plt_model.Series[2] = ab_Line;
                plt_model.Series[3] = bc_Line;
                plt_model.Series[4] = de_Line;
                plt_model.Series[5] = end_Line;

                double minX = DataPoints[0].X;
                double maxX = DataPoints[0].X;

                double minY = DataPoints[0].Y;
                double maxY = DataPoints[0].Y;

                for (int i = 0; i < DataPoints.Length; i++)
                {
                    double xVal = DataPoints[i].X;
                    double yVal = DataPoints[i].Y;

                    if (minX > xVal)
                        minX = xVal;

                    if (maxX < xVal)
                        maxX = xVal;

                    if (minY > yVal)
                        minY = yVal;

                    if (maxY < yVal)
                        maxY = yVal;
                }
                double offSetX = (maxX - minX) * 0.1;
                double offSetY = (maxY - minY) * 0.1;

                plt_model.Axes[0].Minimum = minX - offSetX;
                plt_model.Axes[0].Maximum = maxX + offSetX;
                plt_model.Axes[1].Minimum = minY - offSetY;
                plt_model.Axes[1].Maximum = maxY + offSetY;

                plt_end.Model = plt_model;

                slider.Value = 0;
                scale = 0;

                plt_end.InvalidatePlot();
            }
        }

        private void btn_Help_Click(object sender, RoutedEventArgs e)
        {
            PlotOption window = new PlotOption();
            window.Show();
        }
    }
}
