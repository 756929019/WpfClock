using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using ApiService;

namespace WpfClock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //托盘
        System.Windows.Forms.NotifyIcon notifyIcon;
        bool isDragMove = false;
        bool isDesktopStop = false;
        double oldHeight = 0;
        bool isAnimation = false;
        //计时器
        public MainWindow()
        {
            InitializeComponent();
            #region 托盘
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.BalloonTipText = "天干地支";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "乾坤八卦";
            //this.notifyIcon.Icon = new System.Drawing.Icon(@"weather_clock.ico");
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            //打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("Open");
            open.Click += new EventHandler(Show);
            //隐藏菜单项
            System.Windows.Forms.MenuItem hide = new System.Windows.Forms.MenuItem("Hide");
            hide.Click += new EventHandler(Hide);
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("Exit");
            exit.Click += new EventHandler(Close);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, hide, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) this.Show(o, e);
            });
            #endregion

            #region 时间
            Time2Angle dt2as = new Time2Angle();
            dt2as.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(dt2as_PropertyChanged);
            this.DataContext = dt2as;
            DrawScale(dt2as.IsChecked12_24);
            #endregion

            #region 天气
            Dictionary<string,string> dicPros = ApiService.ApiService.GetProvinceList();
            cmbProvince.ItemsSource = dicPros;
            cmbProvince.SelectedIndex = 20;
            #endregion
            this.LocationChanged += new EventHandler(MainWindow_LocationChanged);
            this.MouseEnter += new MouseEventHandler(MainWindow_MouseEnter);
            this.MouseLeave += new MouseEventHandler(MainWindow_MouseLeave);
        }

        void MainWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isDesktopStop && !isAnimation)
            {
                newHeightAnimation(this.Height, 2);
                this.Top = -1;
            }
        }

        void newHeightAnimation(double fval,double tval)
        {
            isAnimation = true;
            #region 高变化动画
            DoubleAnimation heightAnimation = new DoubleAnimation(fval, tval, new Duration(TimeSpan.FromSeconds(0.3)));
            heightAnimation.Completed += new EventHandler(heightAnimation_Completed);
            this.BeginAnimation(Window.HeightProperty, heightAnimation, HandoffBehavior.Compose);
            #endregion
        }

        void heightAnimation_Completed(object sender, EventArgs e)
        {
            isAnimation = false;
        }

        void MainWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isDesktopStop && !isAnimation)
            {
                newHeightAnimation(2, oldHeight);
                this.Top = -1;
            }
        }

        void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (isDragMove)
            {
                //拖动中
            }
        }

        void dt2as_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked12_24")
            { 
                //重绘刻度表盘
                DrawScale((sender as Time2Angle).IsChecked12_24);
            }
        }

        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            //this.ShowInTaskbar = true;
            this.Activate();
        }

        private void Hide(object sender, EventArgs e)
        {
            //this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //进行拖放移动
            isDragMove = true;
            this.DragMove();
            //拖动完成
            double mx = (e.Source as FrameworkElement).PointToScreen(Mouse.GetPosition(e.Source as FrameworkElement)).X;
            double my = (e.Source as FrameworkElement).PointToScreen(Mouse.GetPosition(e.Source as FrameworkElement)).Y;

            if (my <= 0)
            {
                oldHeight = this.Height;
                newHeightAnimation(this.Height, 2);
                //this.Height = 2;
                this.Top = -1;
                isDesktopStop = true;
            }
            isDragMove = false;
        }

        /// <summary>
        /// 画刻度
        /// </summary>
        private void DrawScale(Boolean IsChecked12_24)
        {
            rootGrid.Children.Clear();

            double ScaleStartAngle = 270;
            double ScaleSweepAngle = 360;
            double MajorDivisionsCount = 12;
            double MinorDivisionsCount = 5;

            double MaxValue = 12;

            if (IsChecked12_24)
            {
                MaxValue = 24;
                MajorDivisionsCount = 24;
            }
            double MinValue = 0;
            int ScaleValuePrecision = 0;
            Size MajorTickSize = new Size(10, 3);
            Size ScaleLabelSize = new Size(40, 20);
            Color MajorTickColor = Colors.LightGray;
           
            double ScaleLabelFontSize = 8;
            Color ScaleLabelForeground = Colors.LightGray;
            Color MinorTickColor = Colors.LightGray;
            Size MinorTickSize = new Size(3, 1);

            double ScaleRadius = 100;
            double ScaleLabelRadius = 80;

            //大刻度角度
            Double majorTickUnitAngle = ScaleSweepAngle / MajorDivisionsCount;

            //小刻度角度
            Double minorTickUnitAngle = ScaleSweepAngle / MinorDivisionsCount;

            //刻度单位值
            Double majorTicksUnitValue = (MaxValue - MinValue) / MajorDivisionsCount;
            majorTicksUnitValue = Math.Round(majorTicksUnitValue, ScaleValuePrecision);

            
            Double minvalue = MinValue; ;
            for (Double i = ScaleStartAngle; i <= (ScaleStartAngle + ScaleSweepAngle); i = i + majorTickUnitAngle)
            {
                //大刻度、刻度值角度
                Double i_radian = (i * Math.PI) / 180;

                #region 大刻度
                Rectangle majortickrect = new Rectangle();
                majortickrect.Height = MajorTickSize.Height;
                majortickrect.Width = MajorTickSize.Width;
                majortickrect.Fill = new SolidColorBrush(MajorTickColor);
                Point p = new Point(0.5, 0.5);
                majortickrect.RenderTransformOrigin = p;
                majortickrect.HorizontalAlignment = HorizontalAlignment.Center;
                majortickrect.VerticalAlignment = VerticalAlignment.Center;

                TransformGroup majortickgp = new TransformGroup();
                RotateTransform majortickrt = new RotateTransform();
                majortickrt.Angle = i;
                majortickgp.Children.Add(majortickrt);
                TranslateTransform majorticktt = new TranslateTransform();

                //在这里画点中心为（0,0）
                majorticktt.X = (int)((ScaleRadius) * Math.Cos(i_radian));
                majorticktt.Y = (int)((ScaleRadius) * Math.Sin(i_radian));

              
                majortickgp.Children.Add(majorticktt);
                majortickrect.RenderTransform = majortickgp;
                rootGrid.Children.Add(majortickrect);
                #endregion
                
                #region 刻度值

                TranslateTransform majorscalevaluett = new TranslateTransform();
                //在这里画点中心为（0,0）
                majorscalevaluett.X = (int)((ScaleLabelRadius) * Math.Cos(i_radian));
                majorscalevaluett.Y = (int)((ScaleLabelRadius) * Math.Sin(i_radian));
                //刻度值显示
                TextBlock tb = new TextBlock();

                tb.Height = ScaleLabelSize.Height;
                tb.Width = ScaleLabelSize.Width;
                tb.FontSize = ScaleLabelFontSize;
                tb.Foreground = new SolidColorBrush(ScaleLabelForeground);
                tb.TextAlignment = TextAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;

                if (Math.Round(minvalue, ScaleValuePrecision) <= Math.Round(MaxValue, ScaleValuePrecision))
                {
                    minvalue = Math.Round(minvalue, ScaleValuePrecision);
                    if (minvalue > 0)
                    {
                        tb.Text = minvalue.ToString();
                    }
                    minvalue = minvalue + majorTicksUnitValue;
                }
                else
                {
                    break;
                }

                tb.RenderTransform = majorscalevaluett;
                rootGrid.Children.Add(tb);
              
                #endregion

                #region 小刻度
                Double onedegree = ((i + majorTickUnitAngle) - i) / (MinorDivisionsCount);
                if ((i < (ScaleStartAngle + ScaleSweepAngle)) && (Math.Round(minvalue, ScaleValuePrecision) <= Math.Round(MaxValue, ScaleValuePrecision)))
                {
                    //绘制小刻度
                    for (Double mi = i + onedegree; mi < (i + majorTickUnitAngle); mi = mi + onedegree)
                    {
                        Rectangle mr = new Rectangle();
                        mr.Height = MinorTickSize.Height;
                        mr.Width = MinorTickSize.Width;
                        mr.Fill = new SolidColorBrush(MinorTickColor);
                        mr.HorizontalAlignment = HorizontalAlignment.Center;
                        mr.VerticalAlignment = VerticalAlignment.Center;
                        Point p1 = new Point(0.5, 0.5);
                        mr.RenderTransformOrigin = p1;

                        TransformGroup minortickgp = new TransformGroup();
                        RotateTransform minortickrt = new RotateTransform();
                        minortickrt.Angle = mi;
                        minortickgp.Children.Add(minortickrt);
                        TranslateTransform minorticktt = new TranslateTransform();

                        //计算角度
                        Double mi_radian = (mi * Math.PI) / 180;
                        //刻度点
                        minorticktt.X = (int)((ScaleRadius) * Math.Cos(mi_radian));
                        minorticktt.Y = (int)((ScaleRadius) * Math.Sin(mi_radian));

                        minortickgp.Children.Add(minorticktt);
                        mr.RenderTransform = minortickgp;
                        rootGrid.Children.Add(mr);
                    }
                }
                #endregion

                //#region 天干地支[子、丑、寅、卯、辰、巳、午、未、申、酉、戌、亥]
                //TranslateTransform majorscalevaluett_tgdz = new TranslateTransform();
                ////在这里画点中心为（0,0）
                //majorscalevaluett_tgdz.X = (int)((ScaleLabelRadius_Tgdz) * Math.Cos(i_radian));
                //majorscalevaluett_tgdz.Y = (int)((ScaleLabelRadius_Tgdz) * Math.Sin(i_radian));
                ////刻度值显示
                //TextBlock tb_tgdz = new TextBlock();

                //tb_tgdz.Height = ScaleLabelSize.Height;
                //tb_tgdz.Width = ScaleLabelSize.Width;
                //tb_tgdz.FontSize = ScaleLabelFontSize;
                //tb_tgdz.Foreground = new SolidColorBrush(ScaleLabelForeground);
                //tb_tgdz.TextAlignment = TextAlignment.Center;
                //tb_tgdz.VerticalAlignment = VerticalAlignment.Center;
                //tb_tgdz.HorizontalAlignment = HorizontalAlignment.Center;
                //if (int.Parse((minvalue - majorTicksUnitValue).ToString()) < tgdz.Length)
                //{
                //    tb_tgdz.Text = tgdz[int.Parse((minvalue - majorTicksUnitValue).ToString())];
                //}
                //tb_tgdz.RenderTransform = majorscalevaluett_tgdz;
                //rootGrid.Children.Add(tb_tgdz);
                //#endregion
            }

            #region 天干地支[子、丑、寅、卯、辰、巳、午、未、申、酉、戌、亥]
            string[] tgdz = new string[] { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
            Color ScaleLabelForeground_tgdz = Colors.LightGray;
            Size ScaleLabelSize_tgdz = new Size(40, 20);
            double ScaleLabelFontSize_tgdz = 8;

            double MajorDivisionsCount_tgdz = 12;
            double MaxValue_tgdz = 12;
            double ScaleLabelRadius_tgdz = 70;
            //刻度角度
            Double majorTickUnitAngle_tgdz = ScaleSweepAngle / MajorDivisionsCount_tgdz;

            //刻度单位值
            Double majorTicksUnitValue_tgdz = (MaxValue_tgdz - MinValue) / MajorDivisionsCount_tgdz;
            majorTicksUnitValue_tgdz = Math.Round(majorTicksUnitValue_tgdz, ScaleValuePrecision);
            Double minvalue_tgdz = MinValue;
            for (Double i = ScaleStartAngle; i <= (ScaleStartAngle + ScaleSweepAngle); i = i + majorTickUnitAngle_tgdz)
            {
                Double i_radian = (i * Math.PI) / 180;

                TranslateTransform majorscalevaluett = new TranslateTransform();
                //在这里画点中心为（0,0）
                majorscalevaluett.X = (int)((ScaleLabelRadius_tgdz) * Math.Cos(i_radian));
                majorscalevaluett.Y = (int)((ScaleLabelRadius_tgdz) * Math.Sin(i_radian));

                //刻度值显示
                TextBlock tb = new TextBlock();

                tb.Height = ScaleLabelSize_tgdz.Height;
                tb.Width = ScaleLabelSize_tgdz.Width;
                tb.FontSize = ScaleLabelFontSize_tgdz;
                tb.Foreground = new SolidColorBrush(ScaleLabelForeground_tgdz);
                tb.TextAlignment = TextAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;

                if (Math.Round(minvalue_tgdz, ScaleValuePrecision) <= Math.Round(MaxValue_tgdz, ScaleValuePrecision))
                {
                    minvalue_tgdz = Math.Round(minvalue_tgdz, ScaleValuePrecision);
                    if (int.Parse((minvalue_tgdz).ToString()) < tgdz.Length)
                    {
                        tb.Text = tgdz[int.Parse((minvalue_tgdz).ToString())];
                    }
                    minvalue_tgdz = minvalue_tgdz + majorTicksUnitValue_tgdz;
                }
                else
                {
                    break;
                }

                tb.RenderTransform = majorscalevaluett;
                rootGrid.Children.Add(tb);
            }

            #endregion

            #region 八卦
            string[] bg = new string[] { "乾", "巽", "坎", "艮", "坤", "震", "离", "兑" };
            string[] bg_tag = new string[] { "离", "坤", "兑", "乾", "坎", "艮", "震", "巽" };
            Color ScaleLabelForeground_bg = Colors.Red;
            Color ScaleLabelForeground_bg_tag = Colors.LightGray;
            Size ScaleLabelSize_bg = new Size(40, 20);
            double ScaleLabelFontSize_bg = 12;
            double ScaleLabelFontSize_bg_tag = 8;

            double MajorDivisionsCount_bg = 8;
            double MaxValue_bg = 8;
            double ScaleLabelRadius_bg = 113;
            //刻度角度
            Double majorTickUnitAngle_bg = ScaleSweepAngle / MajorDivisionsCount_bg;

            //刻度单位值
            Double majorTicksUnitValue_bg = (MaxValue_bg - MinValue) / MajorDivisionsCount_bg;
            majorTicksUnitValue_bg = Math.Round(majorTicksUnitValue_bg, ScaleValuePrecision);
            Double minvalue_bg = MinValue;
            for (Double i = ScaleStartAngle; i <= (ScaleStartAngle + ScaleSweepAngle); i = i + majorTickUnitAngle_bg)
            {
                Double i_radian = (i * Math.PI) / 180;
                Double i_radian_ta = ((i+8) * Math.PI) / 180;

                TranslateTransform majorscalevaluett = new TranslateTransform();
                //在这里画点中心为（0,0）
                majorscalevaluett.X = (int)((ScaleLabelRadius_bg) * Math.Cos(i_radian));
                majorscalevaluett.Y = (int)((ScaleLabelRadius_bg) * Math.Sin(i_radian));

                TranslateTransform majorscalevaluett_tag = new TranslateTransform();
                //在这里画点中心为（0,0）
                majorscalevaluett_tag.X = (int)((ScaleLabelRadius_bg) * Math.Cos(i_radian_ta));
                majorscalevaluett_tag.Y = (int)((ScaleLabelRadius_bg) * Math.Sin(i_radian_ta));
                //刻度值显示
                TextBlock tb = new TextBlock();

                tb.Height = ScaleLabelSize_bg.Height;
                tb.Width = ScaleLabelSize_bg.Width;
                tb.FontSize = ScaleLabelFontSize_bg;
                tb.Foreground = new SolidColorBrush(ScaleLabelForeground_bg);
                tb.TextAlignment = TextAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;

                TextBlock tb_tag = new TextBlock();

                tb_tag.Height = ScaleLabelSize_bg.Height;
                tb_tag.Width = ScaleLabelSize_bg.Width;
                tb_tag.FontSize = ScaleLabelFontSize_bg_tag;
                tb_tag.Foreground = new SolidColorBrush(ScaleLabelForeground_bg_tag);
                tb_tag.TextAlignment = TextAlignment.Center;
                tb_tag.VerticalAlignment = VerticalAlignment.Center;
                tb_tag.HorizontalAlignment = HorizontalAlignment.Center;

                if (Math.Round(minvalue_bg, ScaleValuePrecision) <= Math.Round(MaxValue_bg, ScaleValuePrecision))
                {
                    minvalue_bg = Math.Round(minvalue_bg, ScaleValuePrecision);
                    if (int.Parse((minvalue_bg).ToString()) < bg.Length)
                    {
                        tb.Text = bg[int.Parse((minvalue_bg).ToString())];
                        tb_tag.Text = bg_tag[int.Parse((minvalue_bg).ToString())];
                    }
                    minvalue_bg = minvalue_bg + majorTicksUnitValue_bg;
                }
                else
                {
                    break;
                }

                tb.RenderTransform = majorscalevaluett;
                tb_tag.RenderTransform = majorscalevaluett_tag;
                rootGrid.Children.Add(tb);
                rootGrid.Children.Add(tb_tag);
            }

            #endregion
        }

        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProvince.SelectedItem != null)
            {
                KeyValuePair<string,string> kvp = (KeyValuePair<string,string>)cmbProvince.SelectedItem;
                Dictionary<string, string> dicCitys = ApiService.ApiService.GetCityList(kvp.Key);
                cmbCity.ItemsSource = dicCitys;

                cmbCounty.ItemsSource = null;

                cmbCity.SelectedIndex = 0;
            }
        }

        private void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCity.SelectedItem != null)
            {
                KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)cmbCity.SelectedItem;
                Dictionary<string, string> dicCountys = ApiService.ApiService.GetCountyList(kvp.Key);
                cmbCounty.ItemsSource = dicCountys;
                cmbCounty.SelectedIndex = 0;
            }
        }

        private void cmbCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCounty.SelectedItem != null)
            {
                KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)cmbCounty.SelectedItem;
                
               
                WeatherRealTime wrt = ApiService.ApiService.GetWeatherRealTime(kvp.Key);
                if (wrt == null)
                {
                    wrt = new WeatherRealTime();
                }

                WeatherRealTime wrt2 = ApiService.ApiService.GetWeatherToday(kvp.Key);
                if (wrt2 != null)
                {
                    wrt.city = wrt2.city;
                    wrt.cityid = wrt2.cityid;
                    wrt.temp1 = wrt2.temp1;
                    wrt.temp2 = wrt2.temp2;
                    wrt.weather = wrt2.weather;
                    wrt.img1 = wrt2.img1;
                    wrt.img2 = wrt2.img2;
                    wrt.ptime = wrt2.ptime;
                }
                gridWeather.DataContext = wrt;
            }
        }
    }
}
