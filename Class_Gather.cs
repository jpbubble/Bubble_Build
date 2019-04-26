// Lic:
// Bubble builder
// The Gathering
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
// Version: 19.04.26
// EndLic



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TrickyUnits;

namespace Bubble {
    static class Gather {
        static public void Hello() {
            MKL.Lic    ("Bubble Builder Tool - Class_Gather.cs","GNU General Public License 3");
            MKL.Version("Bubble Builder Tool - Class_Gather.cs","19.04.26");
        }

        static public SortedDictionary<string, string> Gathered = new SortedDictionary<string, string>();
        static List<string> Working;
        static List<string> Future;
        static SortedDictionary<string, string> RequestedLibraries = new SortedDictionary<string, string>();

        static void Queue(string t,string f,string fromdir, string todir="") {
            var todir2 = todir;
            if (todir != "" && (!qstr.Suffixed(todir2, "/"))) todir += "/";
            Gathered[$"{todir2}{qstr.RemPrefix(f,fromdir+"/")}"] = f;
            QCol.Doing($"Queued {t}", f);
        }

        static void AnalyseScript(string script,List<string> FutLib,string fromdir, string todir) {
            QCol.Doing("Analysing", script);
            try {
                var usepref = "";
                switch (qstr.ExtractExt(script).ToLower()) {
                    case "lua":
                        usepref = "-- #USE "; break;
                    default:
                        throw new Exception("Unknown script type!");
                }
                var ln = QuickStream.LoadLines(script);
                for (int lnum=0;lnum<ln.Length;lnum++) {
                    var tl = ln[lnum].Trim();
                    if (qstr.Prefixed(tl, usepref)) {
                        var libname = tl.Substring(usepref.Length);
                        if (libname == "") throw new Exception($"Invalid #USE call in line #{lnum+1}");
                        if (qstr.Prefixed(libname.ToUpper(),"LIBS/")) {
                            var reqlib = libname.Substring(5).ToLower();
                            if (!RequestedLibraries.ContainsKey(reqlib)) {
                                var foundas = "";
                                foreach (string s in Bubble_Build.Config.List("LibraryPath")) {
                                    var check = Dirry.AD($"{s}/{reqlib}.blb");
                                    //Console.WriteLine($"Checking: {check}");
                                    if (Directory.Exists(check)) {
                                        foundas = check;
                                        break;
                                    }
                                }
                                if (foundas == "") throw new Exception($"Requested Library ({reqlib}) has not been found in line {lnum+1}");
                                RequestedLibraries[reqlib] = foundas;
                                FutLib.Add(foundas);
                                QCol.Doing("- Requested library", foundas);
                            }
                        }
                    }
                }
                Queue("script", script, fromdir, todir);
            } catch (Exception E) {
                QCol.QuickError($"I couldn't read the script -- {E.Message}");
            }
        }

        static void GatherDir(string dir, List<string> FUtL,string todir) {            
            QCol.Doing("Gathering", dir);            
            var tree = FileList.GetTree(dir);
            if (tree==null) {
                QCol.QuickError($"I could not get the tree from directory {dir}");
                return;
            }
            foreach(string f in tree) {
                var cf = f.ToUpper();
                if (qstr.ExtractDir(cf) == "LIBS")
                    QCol.QuickError($"I cannot queue {f}. 'Libs' is a reserved folder!");
                else if (qstr.ExtractDir(cf) == "ID")
                    QCol.QuickError($"I cannot queue {f}. 'ID' is a reserved folder!");
                else if (qstr.Prefixed(cf, "__"))
                    QCol.QuickError($"I cannot queue {f}. '__' is a reserved prefix, meant for possible future functionality!");
                else {
                    var fp = $"{dir}/{f}";
                    switch (qstr.ExtractExt(cf)) {
                        case "PNG":
                        case "JPG":
                        case "BMP":
                            Queue("Image",fp,dir,todir);
                            break;
                        case "OGG":
                        case "MP3":
                            Queue("Audio", fp,dir,todir);
                            break;
                        case "GINI":
                            Queue("GINI", fp,dir,todir);
                            break;
                        case "LUA":
                            AnalyseScript(fp,FUtL,dir,todir);
                            break;
                        default:
                            Queue("file", fp,dir,todir);
                            break;
                    }
                }
            }
        }

        static public void Start() {
            Gathered.Clear();
            RequestedLibraries.Clear();
            
            Future = new List<string>(Bubble_Build.InputDirectories);
            var todir = "";
            while(Future.Count>0) {
                Working = Future;
                Future = new List<string>();
                foreach (string d in Working)
                    GatherDir(Dirry.AD(d), Future,todir);
                todir = "Libs/";
            }
        }



    }
}



