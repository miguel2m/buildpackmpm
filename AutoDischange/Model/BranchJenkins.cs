using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    public class BranchJenkins
    {
        private int codBranch;
        public int CodBranch
        {
            get { return codBranch; }
            set { codBranch = value; }
        }
        private string nameBranch;
        public string NameBranch
        {
            get { return nameBranch; }
            set { nameBranch = value; }
        }

    }
}
