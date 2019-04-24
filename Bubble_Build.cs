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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;

namespace Bubble {
    class Bubble_Build {

        public static TGINI Project;
        static FlagParse Flags;

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

        static void Main(string[] args) {
            Flags = new FlagParse(args);
            GetAllVersions();
            Head();
            QCol.OriCol();
        }
    }
}


