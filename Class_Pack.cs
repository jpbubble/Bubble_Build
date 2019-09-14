// Lic:
// Bubble Builder for C#
// Packing Class
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
// Version: 19.05.16
// EndLic





using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UseJCR6;
using TrickyUnits;

namespace Bubble {
    static class Pack {

        static TGINI Project => Bubble_Build.Project;
        static string storage => Project.C("JCRCOMPRESS");
        static TGINI EngineConfig => Bubble_Build.Engine[Bubble_Build.Project.C("Engine").ToUpper()].Config;

        static public void Hello() {
            MKL.Version("Bubble Builder Tool - Class_Pack.cs","19.05.16");
            MKL.Lic    ("Bubble Builder Tool - Class_Pack.cs","GNU General Public License 3");
        }

        static void MakeID(TJCRCreate jcr) {
            var ID = new StringBuilder("[rem]\nBubble Identification File\n\n[vars]\n");
            ID.Append($"BubbleID={Project.C("BubbleID")}\n");
            ID.Append($"BubbleEngine={Project.C("Engine")}\n");
            ID.Append($"Title={Project.C("Title")}\n");
            ID.Append($"Author={Project.C("Author")}\n");
            ID.Append($"License={Project.C("PROJECTLICENSE")}\n");
            ID.Append($"BuilderVersion={MKL.Newest}\n");
            ID.Append($"MakeDate={DateTime.Now.ToString()}\n");
            if (EngineConfig.List("RunModes").Count > 0) {
                if (Project.C("RunMode") == "") {
                    QCol.Green("Which \"Run Mode\" would you like to write your project in?\n");
                    foreach (string rm in EngineConfig.List("RunModes")) {
                        QCol.Doing(rm, EngineConfig.C($"Explain.{rm}"));
                    }
                    do {
                        Project.D("RunMode", "");
                        Bubble_Build.Ask("RunMode", "Please enter the runmode tag:", true);
                    } while (!EngineConfig.List("RunModes").Contains(Project.C("RunMode")));
                }
                ID.Append($"RunMode={Project.C("RunMode")}");
            }
            jcr.AddString(ID.ToString(), "ID/BUBBLEID", storage, "Bubble", "Generated by bubble");
        }

        static void ImportDependencies(TJCRCreate jcr) {
            QCol.Yellow("Checking Dependencies:\n");
            foreach(string dfile in Project.List("Import")) {
                var file= Dirry.AD(dfile);
                if (File.Exists(file)) {
                    var targetfile = $"{Dirry.AD(Project.C("OutFolder"))}/{qstr.StripDir(file)}";
                    var doit = !File.Exists(targetfile);
                    var source = new FileInfo(file);
                    FileInfo target = null;
                    if (!doit) target = new FileInfo(targetfile);
                    doit = doit || source.Length != target.Length;
                    doit = doit || source.LastWriteTime.ToLongDateString() != target.LastWriteTime.ToLongDateString();
                    source = null;
                    target = null;
                    if (doit) {
                        QCol.Doing("Importing", file);
                        File.Copy(file, targetfile,true);
                    } else
                        QCol.Doing("Kept unmodified", file);
                    jcr.Import(qstr.StripDir(file));
                } else {
                    QCol.QuickError($"Requested dependency '{file}' does not exist!");
                }
            }
            Console.WriteLine("");
        }

        static public void Go() {
            TJCRCreate jcr=null;
            var outJCR = Dirry.AD($"{Bubble_Build.Project.C("OutFolder")}/{Bubble_Build.Project.C("ExeName")}.Bubble.JCR").Replace('\\','/');
            QCol.Doing("Creating JCR", outJCR);
            QCol.Doing("Storage", storage);
            Console.WriteLine();
            try {
                Directory.CreateDirectory(Dirry.AD(Project.C("OutFolder")));
                jcr = new TJCRCreate(outJCR, storage, Project.C("BubbleID"));
                if (jcr == null) throw new Exception(JCR6.JERROR);
                MakeID(jcr);
                ImportDependencies(jcr);
                QCol.Yellow("Packing raw files:\n");
                foreach (string tname in Gather.Gathered.Keys) {
                    QCol.Doing(" Adding", tname, "\r");
                    if (JCR6.Recognize(Gather.Gathered[tname]).ToUpper() != "NONE") {
                        QCol.Doing("Merging", tname,"\t"); QCol.Cyan($"Recognized as {JCR6.Recognize(Gather.Gathered[tname])}\n");
                        var j = JCR6.Dir(Gather.Gathered[tname]);
                        foreach (TJCREntry e in j.Entries.Values) {
                            QCol.Doing(" Adding", $"{tname}/{e.Entry}", "\r");
                            if (JCR6.CompDrivers.ContainsKey(e.Storage)) {
                                var s = j.JCR_B(e.Entry);
                                var targetname = $"{tname}/{e.Entry}";
                                jcr.AddBytes(s,targetname, storage);
                                var te = jcr.Entries[targetname.ToUpper()];
                                var ratio = (int)(((float)e.CompressedSize / e.Size) * 100);
                                QCol.Blue(qstr.Right($"   {ratio}% ", 5));
                                switch (te.Storage) {
                                    case "Store":
                                        QCol.White("\r     Stored:\n"); break;
                                    case "lzma":
                                        QCol.Green("Compressed:\n"); break;
                                    case "zlib":
                                        QCol.Green("Deflated:\n"); break;
                                    default:
                                        QCol.Magenta($"{storage}:\n"); break;
                                }
                            } else {
                                QCol.QuickError($"Unknown compression method: {e.Storage}");
                            }
                        }
                        QCol.Red("END OF MERGE!\n");
                    } else {
                        jcr.AddFile(Gather.Gathered[tname], tname, storage);
                        var e = jcr.Entries[tname.ToUpper()];
                        var ratio = (int)(((float)e.CompressedSize / e.Size) * 100);
                        QCol.Blue(qstr.Right($"   {ratio}% ", 5));
                        switch (e.Storage) {
                            case "Store":
                                QCol.White("\r     Stored:\n"); break;
                            case "lzma":
                                QCol.Green("Compressed:\n"); break;
                            case "zlib":
                                QCol.Green("Deflated:\n"); break;
                            default:
                                QCol.Magenta($"{storage}:\n"); break;
                        }
                    }
                }
                    Bubble_Build.Project.CL("ALIAS");
                foreach (string alias in Bubble_Build.Project.List("ALIAS")) {
                    if (alias != "") {
                        var ori = "";
                        var tar = "";
                        var sep = false;
                        var skip = 0;
                        for (int ai = 0; ai < alias.Length; ai++) {
                            if (ai < alias.Length - 4 && qstr.Mid(alias, ai + 1, 4) == " => ") {
                                sep = true;
                                skip = 3;
                            } else if (skip > 0)
                                skip--;
                            else if (!sep)
                                ori += alias[ai];
                            else
                                tar += alias[ai];
                        }
                        if (!sep)
                            QCol.QuickError($"Misformed alias request: {alias}");
                        else {
                            QCol.Yellow("Alias: ");
                            QCol.Red(ori);
                            QCol.White(" => ");
                            QCol.Green($"{tar}\n");
                            jcr.Alias(ori, tar);
                            if (JCR6.JERROR != "") QCol.QuickError($"Alias failed!\n{JCR6.JERROR}");
                        }
                    }
                }
            } catch (Exception E) {
                QCol.QuickError($"PACK ERROR: {E.Message}");
                QCol.Cyan($"{E.StackTrace}\n\n");
                return;
            } finally {
                if (jcr != null) {
                    QCol.Doing("Finalizing","");
                    jcr.Close();
                }
                Thread.Sleep(2500);
            }
        }
    }
}





