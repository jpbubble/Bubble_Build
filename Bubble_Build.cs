using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;

namespace Bubble {
    class Bubble_Build {

        public static TGINI Project;
        static FlagParse Flags;

        static void GetAllVersions() {
            Dirry.InitAltDrives(); // Must be done anyway, and we'll get the version in the process!
            GINI.Hello();
            FileList.Hello();
            qstr.Hello();
            QuickStream.Hello();
            QCol.Hello();
        }

        static void Head() {            
            QCol.Red("Bubble Builder  ");
            QCol.Green($"Version {MKL.Newest}\n");
            QCol.Magenta("Coded by: Jeroen P. Broks\n");
            QCol.Yellow($"(c) {MKL.CYear(2019)} Jeroen P. Broks, Licensed under the terms of the GPL3\n\n");
        }

        static void Main(string[] args) {
            Flags = new FlagParse(args);
            GetAllVersions();
            Head();
            QCol.OriCol();
        }
    }
}
