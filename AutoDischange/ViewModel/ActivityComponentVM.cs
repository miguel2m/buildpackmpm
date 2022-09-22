using AutoDischange.Model;
using AutoDischange.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel
{
    public class ActivityComponentVM : INotifyPropertyChanged
    {
        public ActivityComponentCommand ActivityComponentCommand { get; set; }

        private ActivityComponent activityComponent;
        public ActivityComponent ActivityComponent
        {
            get { return activityComponent; }
            set
            {
                activityComponent = value;
                OnPropertyChanged("ActivityComponent");
            }
        }
        private string activityPathComponent;
        public string ActivityPathComponent
        {
            get { return activityPathComponent; }
            set
            {
                activityPathComponent = value;
                ActivityComponent = new ActivityComponent
                {
                    PathStart = activityPathComponent,
                };
                OnPropertyChanged("ActivityPathComponent");
            }
        }

        

        private string activityStatus;

        public string ActivityStatus
        {
            get { return activityStatus; }
            set
            {
                activityStatus = value;
                OnPropertyChanged("ActivityStatus");
            }
        }

        private bool activityVisible;
        public bool ActivityVisible
        {
            get { return activityVisible; }
            set
            {
                activityVisible = value;
                OnPropertyChanged("ActivityVisible");
            }
        }

        public ActivityComponentVM()
        {
            ActivityComponentCommand = new ActivityComponentCommand(this);
            activityComponent = new ActivityComponent();
            ActivityVisible = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
