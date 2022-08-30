using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBuild.Models;

namespace AutoBuild.DAL
{
    class MockService : IMockService
    {


        public List<MockUser> GetMockUser()
        {
            var mock = new List<MockUser>();
            for (int i = 0; i < 20; i++)
            {
                mock.Add(new MockUser()
                {
                    ID = i,
                    FirstName = Guid.NewGuid().ToString(),
                    SecondName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    isEneable = i%2==0,
                    LastLogin = DateTime.Now.AddDays(i)
                } ) ;
            }
            return mock;
        }
    }
}
