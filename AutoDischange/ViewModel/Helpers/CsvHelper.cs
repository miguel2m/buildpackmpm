using AutoDischange.Model;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class CsvHelper
    {
        //READ Local Branches Utiles

        public static async Task<bool> ReadCSVBranch_Utiles(string filePath)
        {
            bool resultMethod = false;

            FileHelperEngine<BranchUtilData> engine = new FileHelperEngine<BranchUtilData>();

            //To read use
            var result = engine.ReadFile(filePath);
            foreach (BranchUtilData item in result)
            {
                BranchJenkinsExcel branchUtilData = new BranchJenkinsExcel
                {
                    Name = item.Branch
                };
                List<BranchJenkinsExcel> notebooks = DatabaseHelper.Read<BranchJenkinsExcel>().Where(n => n.Name == branchUtilData.Name).ToList();

                if (notebooks.Count() == 0)
                {
                    resultMethod = await DatabaseHelper.InsertBranchUtil(branchUtilData);
                }
                else
                {
                    branchUtilData.Id = notebooks[0].Id;
                    resultMethod = await DatabaseHelper.InsertReplaceBranchUtil(branchUtilData);
                }
            }

            return resultMethod;
        }

        //READ Local DIS_Changes
        public static async Task<bool> ReadCSVDIS_Changes(string filePath)
        {
            //string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, "DIS_Changes.xlsx");

            bool resultMethod = false;
            //List<DischangePath> DischangeChangesets = new List<DischangePath>();

            var engine = new FileHelperEngine<DisChangeData>();

            // To Read Use:
            var result = engine.ReadFile(filePath);
            foreach (DisChangeData item in result)
            {
                DischangePath DischangeChangeset = new DischangePath
                {
                    Path = item.PathData,
                };

                var notebooks = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path == DischangeChangeset.Path).ToList();


                if (notebooks.Count() == 0)
                {
                    resultMethod = await DatabaseHelper.InsertDischange(DischangeChangeset);
                }
                else
                {
                    DischangeChangeset.Id = notebooks[0].Id;
                    resultMethod = await DatabaseHelper.InsertReplaceDischange(DischangeChangeset);
                }
            }
            return resultMethod;
        }

        //READ Local DIS_Changes
        public static async Task<List<DischangeChangeset>> ReadCSVChangeset(string filePath)
        {
            //string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, "DIS_Changes.xlsx");
            bool resultMethod = false;
            List<DischangeChangeset> DischangeChangesets = new List<DischangeChangeset>();
            //List<DischangePath> DischangeChangesets = new List<DischangePath>();

            var engine = new FileHelperEngine<ChangesetData>();
            // To Read Use:
            var result = engine.ReadFile(filePath);
            int count = 0;
            var notexbooks = DatabaseHelper.Read<DischangeChangeset>();
            if (notexbooks.Count() > 0)
            {
                for (int i = 0; i < notexbooks.Count; i++)
                {
                    DatabaseHelper.Delete<DischangeChangeset>(notexbooks[i]);
                }
            }
            foreach (ChangesetData item in result)
            {
                DischangeChangeset DischangeChangeset = new DischangeChangeset
                {
                    Id = count,
                    Changeset = item.Changeset,
                    Branch = item.Branch,
                };

                var notebooks = (DatabaseHelper.Read<DischangeChangeset>()).Where(n => n.Changeset == DischangeChangeset.Changeset).ToList();

                if (notebooks.Count() == 0)
                {
                    resultMethod = await DatabaseHelper.InsertChangeset(DischangeChangeset);
                }
                else
                {
                    DischangeChangeset.Id = notebooks[0].Id;
                    resultMethod = await DatabaseHelper.InsertReplaceChangeset(DischangeChangeset);
                }

                DischangeChangesets.Add(DischangeChangeset);

                //if (await DatabaseHelper.InsertReplaceChangeset(DischangeChangeset))
                //{
                //    DischangeChangesets.Add(DischangeChangeset);
                //}
            }
            return DischangeChangesets;


        }
    }

    [DelimitedRecord(";")]
    public class DisChangeData
    {
        public string PathData;

        public string HashData;

        public string SizeData;
        

    };

    [DelimitedRecord(",")]
    public class ChangesetData
    {
        public string Changeset;

        public string Branch;
    }

    [DelimitedRecord(",")]
    public class BranchUtilData
    {
        public string Branch;

        public bool Util;
    }
}



