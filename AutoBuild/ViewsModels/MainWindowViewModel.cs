using AutoBuild.DAL;
using AutoBuild.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoBuild.ViewsModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        #region properties

        private List<MockUser> mockUsers;
        public List<MockUser> MockUsers
        {

            get
            {
                return mockUsers;
            }
            set
            {
                if (mockUsers == value)
                {
                    return;
                }
                mockUsers = value;
                OnPropertyChanged("MockUsers");
            }
        }
        #endregion

        #region ICommand

        private ICommand mockUserCommand;

        public ICommand MockUserCommand
        {
            get
            {
                if (mockUserCommand == null )
                {
                    mockUserCommand = new RelayCommand((param => this.MockUserCommandExcute()), null ) ;
                }

                return mockUserCommand;
            }
        }

        

        #endregion

        public MainWindowViewModel()
        {

        }

        private void MockUserCommandExcute()
        {
            var mockService = new MockService();
            var users = mockService.GetMockUser();
            MockUsers = new List<MockUser>(users);
        }
    }
}
