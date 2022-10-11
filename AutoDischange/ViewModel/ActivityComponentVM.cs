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
using System.Windows.Controls;

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

        private string envEntrega;
        public string EnvEntrega
        {
            get { return envEntrega; }
            set
            {
                envEntrega = value;             
                OnPropertyChanged("EnvEntrega");
            }
        }

        public ActivityComponentVM()
        {
            ActivityComponentCommand = new ActivityComponentCommand(this);
            activityComponent = new ActivityComponent();
            ActivityVisible = true;
            EnvEntrega = "PRE";
            ActivityPathComponent = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); 
        }

        public async void ExcuteActivityExport(string pathUser)
        {
            try
            {
                Console.WriteLine(EnvEntrega);
                ActivityStatus = $"Leyendo paquetes";
                ActivityVisible = false;
                string ambiente;
                switch (EnvEntrega.ToUpper())
                {
                    case "PRE":
                        ambiente = "Pre";
                        // code block
                        break;
                    case "PRO":
                        ambiente = "Pro";
                        // code block
                        break;
                    default:
                        ambiente = "Pre Pro";
                        // code block
                        break;
                }
                //await DiffComponentHelper.DiffFiles(DiffComponent, pathUser);
                await ActivityHelper.ExportActivity(activityComponent, pathUser, ambiente);
                ActivityVisible = true;
                ActivityStatus = $"Tarea comparación de paquetes finalizada";
                MessageBox.Show("Listo ", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log4net.log.Error(ex.Message);
                ActivityVisible = true;
                ActivityStatus = $"Error al ejecutar Excel de entrega: { ex.Message}.";
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
