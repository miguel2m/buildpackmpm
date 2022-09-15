using AutoDischange.Model;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class PackageCommand :ICommand
    {
        public DischangeVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public PackageCommand(DischangeVM vm)
        {
            ViewModel = vm;
        }
        public bool CanExecute(object parameter)
        {
            //LOGICA PARA BLOQUEAR EL BOTON EXPORTAR LISTA DE LOS COMPONENTES
            //SE DEBE AVERIGUAR QUE LA LISTA DE LOS CHANGESET ESTE CREADO
            DischangeChangeset changesetList = parameter as DischangeChangeset;
            
            return changesetList != null ?  true : false;
        }

        public void Execute(object parameter)
        {
            

            //ViewModel.copyToJenkins();

        }
    }
}
