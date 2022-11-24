using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    public class DischangeChangeset
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Changeset { get; set; }
        public string Branch { get; set; }
    }

    public class DischangePath
    {
        [PrimaryKey , AutoIncrement]
        public int Id { get; set; }
        public string Path { get; set; }
        public string Rama { get; set; }
    }

    public class ListComponent
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Path { get; set; }
        public string Branch { get; set; }
        public string changeset { get; set; }
    }

    public class DiffComponent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PathStart { get; set; }
        public string PathEnd { get; set; }
        public string PathDownload { get; set; }
    }

    public class BranchUse
    {
        public bool UseBranch { get; set; }
        public string NameBranch { get; set; }
    }   
}
