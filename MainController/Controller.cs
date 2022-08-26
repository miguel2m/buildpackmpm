using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace MainController
{
    public class Controller
    {
        // Create a list of parts.
        List<MainModel> Chagetsets = new List<MainModel>();

        public Controller()
        {
            MainModel model1 = new MainModel("1","test1", "test1");
            MainModel model2 = new MainModel("2", "test2", "test2");
            MainModel model3 = new MainModel("3", "test3", "test3");
            MainModel model4 = new MainModel("4", "test4", "test4");
            MainModel model5 = new MainModel("5", "test5", "test5");
            MainModel model6 = new MainModel("6", "test6", "test6");

            Chagetsets.Add(model1);
            Chagetsets.Add(model2);
            Chagetsets.Add(model3);
            Chagetsets.Add(model4);
            Chagetsets.Add(model5);
            Chagetsets.Add(model6);
        }
        public void GetData ()
        {

        }
    }
}
