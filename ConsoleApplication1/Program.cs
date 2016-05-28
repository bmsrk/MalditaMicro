using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            var a = new BLL.WebServiceProxy();
            var e = a.Read("PC616");
            var c = a.Read("PD527");
        }
    }
}
