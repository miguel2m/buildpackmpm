using AutoBuild.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoBuild.DAL
{
    public interface IMockService
    {
        List<MockUser> GetMockUser();

    }
}
