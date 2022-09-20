﻿using AutoDischange.Model;
using AutoDischange.ViewModel.Commands;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel
{
    public class DiffComponentVM : INotifyPropertyChanged
    {
        public DiffComponentCommand DiffComponentCommand { get; set; }


        private DiffComponent diffComponent;
        public DiffComponent DiffComponent
        {
            get { return diffComponent; }
            set
            {
                diffComponent = value;
                OnPropertyChanged("DiffComponent");
            }
        }
        private string pathStartComponent;
        public string PathStartComponent
        {
            get { return pathStartComponent; }
            set
            {
                pathStartComponent = value;
                DiffComponent = new DiffComponent
                {
                    PathStart = pathStartComponent,
                    PathEnd = this.PathEndComponent,
                };
                OnPropertyChanged("PathStartComponent");
            }
        }

        private string pathEndComponent;
        public string PathEndComponent
        {
            get { return pathEndComponent; }
            set
            {
                pathEndComponent = value;
                DiffComponent = new DiffComponent
                {
                    PathStart = this.pathStartComponent,
                    PathEnd = pathEndComponent,
                };
                OnPropertyChanged("PathEndComponent");
            }
        }

        private string diffStatus;

        public string DiffStatus
        {
            get { return diffStatus; }
            set
            {
                diffStatus = value;
                OnPropertyChanged("DiffStatus");
            }
        }

        private bool diffVisible;
        public bool DiffVisible
        {
            get { return diffVisible; }
            set
            {
                diffVisible = value;
                OnPropertyChanged("DiffVisible");
            }
        }

        public DiffComponentVM()
        {
            DiffComponentCommand = new DiffComponentCommand(this);
            DiffVisible = true;
        }



        public event EventHandler AddInput;

        public void GenericInput(string pathUser)
        {




            DiffVisible = false;
            DiffComponentHelper.DiffFiles(DiffComponent, pathUser);
            DiffVisible = true;

            //var fileStart = DiffComponentHelper.GetFileList(DiffComponent.PathStart);
            //foreach (var item in fileStart)
            //{
            //    //string fileName =Path.GetFileName(item);
            //    //Console.WriteLine(fileName);
            //    //var fileFound = DiffComponentHelper.GetSearchFileList(DiffComponent.PathEnd,fileName);
            //    //Console.WriteLine("fileFound");
            //    //foreach (var item2 in fileFound)
            //    //{

            //    //    Console.WriteLine(item2);
            //    //}
            //    //Console.WriteLine("Item");
            //    //Console.WriteLine(item);
            //}
            //Console.WriteLine("Path End" + DiffComponent.PathEnd);
            //var fileEnd = DiffComponentHelper.GetFileList(DiffComponent.PathEnd);
            //foreach (var item in fileEnd)
            //{
            //    Console.WriteLine(item);
            //}
            //AddInput?.Invoke(this, new EventArgs());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
