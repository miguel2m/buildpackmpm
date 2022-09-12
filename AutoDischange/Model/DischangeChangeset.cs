﻿using SQLite;
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
    }
}
