using AutoDischange.Model;
using AutoDischange.ViewModel.Commands;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public async void ExcuteActivityExport(string pathUser)
        {
            try
            {
                ActivityStatus = $"Leyendo paquetes";
                ActivityVisible = false;
                //await DiffComponentHelper.DiffFiles(DiffComponent, pathUser);
                await ActivityHelper.ExportActivity(activityComponent, pathUser);
                ActivityVisible = true;
                ActivityStatus = $"Tarea comparación de paquetes finalizada";
                MessageBox.Show("Listo ", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ActivityVisible = true;
                activityStatus = $"Error al ejecutar Excel de entrega: { ex.Message}.";
                MessageBox.Show("Error al ejecutar Excel de entrega: " + ex.Message, "Error al ejecutar Excel de entrega", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
