// Lic:
// Bubble Builder
// Extract Engine
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
// Version: 19.04.27
// EndLic

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UseJCR6;
using TrickyUnits;

namespace Bubble {
    static class EngineExtract {
        static TGINI Project => Bubble_Build.Project;
        static string chosen => Bubble_Build.Project.C("Engine").ToUpper();
        static Bubble_Build.TEngine Engine => Bubble_Build.Engine[chosen];
        static TJCRDIR JCR => Bubble_Build.EnginesJCR;
        static Dictionary<string, string> Replacements = new Dictionary<string, string>();
        


        static public void Hello() {
            MKL.Lic    ("Bubble Builder Tool - Class_EngineExtract.cs","GNU General Public License 3");
            MKL.Version("Bubble Builder Tool - Class_EngineExtract.cs","19.04.27");
        }

        static void SetReplacements() {
            Replacements.Clear();
            foreach(string l in Engine.Config.List("Replace")) {
                var s = qstr.Split(l, " => ");
                if (s.Length != 2)
                    QCol.QuickError($"Replacement syntax error: {l}");
                else {
                    Replacements[s[0]] = s[1].Replace("{ExeName}", Project.C("ExeName"));
                }
            }
        }

        static public void Go() {
            SetReplacements();
            Console.WriteLine("\n\n");
            QCol.Doing("Installing engine",chosen);
            foreach(TJCREntry E in JCR.Entries.Values) {
                var path = qstr.ExtractDir(E.Entry).Replace("\\","/"); if (qstr.Right(path, 1) != "/") path += "/";
                var splitpath = path.ToUpper().Split('/');
                var extractas = E.Entry.Substring($"Engine_bin/{chosen}/".Length);
                foreach (string k in Replacements.Keys)
                    extractas = extractas.Replace(k.Trim(), Replacements[k].Trim());
                //Console.WriteLine($"Entry {E.Entry}; P{path}; L{splitpath.Length}; e{splitpath[1]};  ea{extractas} ");
                if (splitpath.Length >= 2 && splitpath[1]==chosen && extractas.ToUpper()!="BUILDDATA.GINI") {
                    QCol.Doing("  Extracting", extractas, "\r");
                    try {
                        QCol.Magenta("r\r");
                        var data = JCR.JCR_B(E.Entry);
                        QCol.Magenta("w\r");
                        var extractfile = Dirry.AD($"{Project.C("OUTFOLDER")}/{extractas}");
                        var extractdir = qstr.ExtractDir(extractfile);
                        Directory.CreateDirectory(extractdir);
                        QuickStream.SaveBytes(extractfile, data);
                        QCol.Doing("   Extracted", extractas);
                    } catch (Exception ex) {
                        QCol.Doing("     Skipped", extractas);
                        QCol.QuickError($"Extraction failed!\n.NET reported: {ex.Message}\nJCR6 reported: {JCR6.JERROR}\n");
                    }
                }
            }
        }
    }

}

