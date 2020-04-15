using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;

namespace WpfClock
{
    public sealed class Time2Angle : INotifyPropertyChanged
    {
        
        public double Hour 
        {
            get { return _hour; }
            set
            {
                _hour = value;
                OnPropertyChanged("Hour");
            }
        }
        public double Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                OnPropertyChanged("Minute");
            }
        }
        public double Second
        {
            get { return _second; }
            set
            {
                _second = value;
                OnPropertyChanged("Second");
            }
        }

        public string TimeTxt
        {
            get { return _timetxt; }
            set
            {
                _timetxt = value;
                OnPropertyChanged("TimeTxt");
            }
        }
        public Boolean IsChecked12_24
        {
            get { return _ischecked12_24; }
            set
            {
                _ischecked12_24 = value;
                OnPropertyChanged("IsChecked12_24");
                initialDateTime();
                //值更改后触发事件重绘表盘
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public Time2Angle()
        {
            initialDateTime();
            _tiemr = new Timer(1000);
            _tiemr.Elapsed += _tiemr_Elapsed;
            _tiemr.Start();
        }

        ~Time2Angle()
        {
            _tiemr.Stop();
            _tiemr.Dispose();
        }

        void _tiemr_Elapsed(object sender, ElapsedEventArgs e)
        {
            initialDateTime();
        }
        
        void initialDateTime()
        {
            DateTime dt = DateTime.Now;
            if (_ischecked12_24)
            {
                if (dt.Minute % 2 == 0)
                {
                    //秒针转动,秒针绕一圈360度，共120秒，所以1秒转动3度
                    Second = (dt.Second * 3) + 180;
                }
                else
                {
                    //秒针转动,秒针绕一圈360度，共120秒，所以1秒转动3度
                    Second = dt.Second * 3;
                }

                if (dt.Hour % 2 == 0)
                {
                    //分针转动,分针绕一圈360度，共1200分，所以1分转动3度
                    Minute = (dt.Minute * 3) + 180;
                }
                else
                {
                    //分针转动,分针绕一圈360度，共1200分，所以1分转动3度
                    Minute = dt.Minute * 3;
                }
              
                //时针转动,时针绕一圈360度，共24时，所以1时转动15度。
                //另外同一个小时内，随着分钟数的变化(绕一圈120分钟2小时），时针也在缓慢变化（转动15度，15/(120/2)=0.25)
                if (int.Parse(DateTime.Now.ToString("HH")) > 12)
                {
                    Hour = ((dt.Hour * 15) + (dt.Minute * 0.25));
                }
                else
                {
                    Hour = (dt.Hour * 15) + (dt.Minute * 0.25);
                }
            }
            else
            {
                //秒针转动,秒针绕一圈360度，共60秒，所以1秒转动6度
                Second = dt.Second * 6;
                //分针转动,分针绕一圈360度，共60分，所以1分转动6度
                Minute = dt.Minute * 6;
                //时针转动,时针绕一圈360度，共12时，所以1时转动30度。
                //另外同一个小时内，随着分钟数的变化(绕一圈60分钟），时针也在缓慢变化（转动30度，30/60=0.5)
                Hour = (dt.Hour * 30) + (dt.Minute * 0.5);
            }
            TimeTxt = DateTime.Now.ToString("HH:mm:ss");
        }

        private void OnPropertyChanged(string info)
        {
           
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        private double _hour, _minute, _second;

        private string  _timetxt;

        private Boolean _ischecked12_24 = false;

        private Timer _tiemr;
    }
}
