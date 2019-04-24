// Lic:
// Bubble Builder
// Builds a Bubble Project to a proper test version or a full release
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.04.24
// EndLic


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;

namespace Bubble {
    class Bubble_Build {

        public static TGINI Project=null;
        public static TGINI Config = null;
        static FlagParse Flags;
        static public bool BuildTest => Flags.GetBool("test");
        static public string[] WantProjects => Flags.Args;

        static void GetAllVersions() {
            MKL.Version("Bubble Builder Tool - Bubble_Build.cs","19.04.24");
            MKL.Lic    ("Bubble Builder Tool - Bubble_Build.cs","GNU General Public License 3");
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

        static public void Crash(string Error,string additional = "",int ExitCode=100) {
            Console.Beep();
            QCol.Red("ERROR! ");
            QCol.Yellow($"{Error}\n");
            if (additional != "") QCol.Cyan($"\n\n{additional}\n");
            Environment.Exit(ExitCode);
        }

        static public void Crash(Exception e) => Crash(e.Message, e.StackTrace, e.HResult);

        static void ParseCLI() {
            Flags.CrBool("test", false);
            if (!Flags.Parse()) Crash("Command line parse error");
            if (WantProjects.Length == 0) {
                QCol.Red("Usage: ");
                QCol.Yellow("Bubble_Build ");
                QCol.Cyan("[ -test ] ");
                QCol.Green("<Project>\n\n\n");
                QCol.White("Please note, that Build_Bubble is able to generate its project files and will ask and store anything it doesn't know\n");
                QCol.White("With the -test parameter a test version will be created which will be build very quickly, however, it will only be usable on the same computer it has been created on!");
                Environment.Exit(0);
            }
        }

        static void BuildProject(string prj) {
            if (Project!=null) { Crash("Build Project request, while a build is already running!", "This must be an internal error! Either caused bya bug, or somebody has been messing with the source code!\nEither way, this error is fatal!"); }
        }

        static void Main(string[] args) {
            Flags = new FlagParse(args);
            GetAllVersions();
            Head();
            ParseCLI();
            QCol.OriCol();
            foreach (string prj in WantProjects) BuildProject(prj);
        }
    }
}


