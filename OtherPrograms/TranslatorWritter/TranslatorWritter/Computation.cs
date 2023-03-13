using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace TranslatorWritter {
    internal static class Computation {
        private static readonly Timer timerDownloading=new Timer();

        public static void DownloadString(ref DownloadDataCompletedEventHandler eventDone, string name) { 
            WebClient client = new WebClient();

            bool downloading = false;
            timerDownloading.Interval = 5000;

            timerDownloading.Tick += delegate {
                if (downloading) {
                    client.CancelAsync();
                }
                timerDownloading.Stop();
            };
            timerDownloading.Interval = 5000;
            timerDownloading.Start();
            downloading = true;
            client.DownloadDataAsync(new Uri("https://cs.wiktionary.org/wiki/" + Uri.EscapeDataString(name)));
            client.DownloadDataCompleted += eventDone;
        }

        public static NodeHTML findNode(string html, string id) { 
            if (!html.Contains(id)) return null;
            int idPos = html.IndexOf("id=\""+id+"\"");
            string next=html.Substring(idPos);
            int tagStart=next.IndexOf('>');

            string[] singles = { "input" }; 

            List<NodeHTML> needToClose=new List<NodeHTML>();
            NodeHTML node=new NodeHTML(){ Tag="?" };

            CreateNode(tagStart, node);
            return node;
                      
            int CreateNode(int Start, NodeHTML parent) {
                bool insideTag=false;
                string tag=""; 
                string inside="";

                for (int i=Start; i<html.Length; i++) { 
                    char ch=html[i];
                   
                    if (ch=='<') {
                        insideTag=true;
                        tag="";
                    } else if (ch=='>') {
                        insideTag=false;

                        //tag
                        if (tag.Length==0) throw new Exception();

                        int pos = tag.IndexOf(" ");
                        string name;
                        if (pos>0) name=tag.Substring(0, pos);
                        else name=tag;

                        // </div>
                        if (tag.StartsWith("/")) { 
                            if (needToClose.Count>0) { 
                                if (inside=="") parent.Childs.Add(new NodeText{ Text=inside});
                                needToClose[needToClose.Count-1].End=i;
                                needToClose.RemoveAt(needToClose.Count-1);
                            } else return -1;
                        }

                        // <input>
                        bool selfClose=false;
                        foreach (string single in singles) { 
                            if (single==name) {
                                selfClose=true;
                                break;    
                            }
                        }

                        if (selfClose) {
                            parent.Childs.Add(new NodeHTML{Start=Start, End=i, Tag=tag });
                        } else {
                            NodeHTML building=new NodeHTML{Start=Start, End=-1, Tag=tag };
                            parent.Childs.Add(building);
                            needToClose.Add(building);

                            while (true) {
                                int n=CreateNode(i+1, building);
                                if (n==-1) break;
                            }
                        }
                    }

                    if (insideTag) { 
                        tag+=ch;
                    }else inside+=ch;
                } 

                if (inside=="") parent.Childs.Add(new NodeText{ Text=inside});                    
                return -1;
             
                 // (HTMLNode, type, end index), type==0 same level, 1==go deep
            }
        }
     
        public static bool FindTableInHTML(string html, string tableName, ref List<Table> tables) {
            string _html=html;
            while (_html.Contains("<table class=\""+tableName+"\">")) {
                Table table=new Table(){ Rows=new List<Row>()};
                int tableStart = _html.IndexOf("<table class=\""+tableName+"\">")+("<table class=\""+tableName+"\">").Length;
                string fndForEnd=_html.Substring(tableStart);
                int tableEnd = fndForEnd.IndexOf("</table>")+tableStart;
                string rawTable = _html.Substring(tableStart, tableEnd-tableStart);
                
                // Caption
                if (rawTable.Contains("<caption>")){ 
                    int startCaption=rawTable.IndexOf("<caption>")+"<caption>".Length;
                    int endCaption=rawTable.IndexOf("</caption>");

                    table.Caption=rawTable.Substring(startCaption,endCaption-startCaption);
                    table.Caption=table.Caption.Replace("\n","");
                }

                // Remove tbody
                if (rawTable.Contains("<tbody")){ 
                    int start = rawTable.IndexOf("<tbody");
                    rawTable = rawTable.Substring(start);
                    rawTable = rawTable.Substring(rawTable.IndexOf(">")+1);

                    int len   = rawTable.IndexOf("</tbody>");

                    rawTable = rawTable.Substring(0, len);
                }

                // Rows
                while (rawTable.Contains("<tr")) {
                    int startTr=rawTable.IndexOf("<tr");
                    string rawRow = rawTable.Substring(startTr);
                    int endTag=rawRow.IndexOf('>')+1;
                    rawRow=rawRow.Substring(endTag);

                    int endTr=rawRow.IndexOf("</tr>");
                    rawRow = rawRow.Substring(0, endTr);

                    Row row = new Row(){ Cells=new List<Cell>(), html=rawRow};

                    // Cells
                    
                    while (true) {
                        int td=rawRow.Contains("<td") ? rawRow.IndexOf("<td"): int.MaxValue;
                        int th=rawRow.Contains("<th") ? rawRow.IndexOf("<th"): int.MaxValue;
                        if (th==int.MaxValue && td==int.MaxValue)break;

                        string cellTag=td<th ? "td":"th";
                      
                        int start=rawRow.IndexOf("<"+cellTag);
                        string cell=rawRow.Substring(start);
                        cell=cell.Substring(cell.IndexOf(">")+1);

                        int end=cell.IndexOf("</"+cellTag+">");
                        cell = cell.Substring(0, end);
                    
                        row.Cells.Add(new Cell{ Text=InsidePlainText(cell.Replace("\n", "").Replace(" / ", ",")).Replace("/", ",").Replace("&#91;","[").Replace("&#93;","]"), html=cell });
                        rawRow = rawRow.Substring(rawRow.IndexOf("</"+cellTag+">")+("</"+cellTag+">").Length);
                     
                    }
                    table.Rows.Add(row);
                    rawTable = rawTable.Substring(rawTable.IndexOf("</tr>")+"</tr>".Length);
                }

                tables.Add(table);
                _html=_html.Substring(tableEnd+"</table>".Length);
            }
            return true;

            string InsidePlainText(string txt) { 
                string plain="";
                bool adding=true;
                for (int i=0; i<txt.Length; i++) { 
                    char ch =txt[i];
                    if (ch == '<') { 
                        adding=false;
                        continue;
                    } else if (ch == '>') { 
                        adding=true;
                        continue;
                    }
                    if (adding) plain+=ch;
                }
                return plain;
            }
        }

        public static bool FindTableInListByName(List<Table> tables, string name, out Table rTable) { 
            foreach (Table table in tables) { 
                if (table.Caption==name) {
                    rTable=table;
                    return true;
                }
            }
            rTable=null;
            return false;
        }
    }

    class Table{ 
        public string Caption;
        public List<Row> Rows;
    }

    class Row{ 
        public string html;
        public List<Cell> Cells;
    }

    class Cell{ 
        public string html;
        public string Text;    
    }

    class NodeHTML : Node{ 
        public string Tag;
        public List<Node> Childs=new List<Node>();
        public int Start, End;

        public bool FindText(string text) { 
            foreach (Node node in Childs) { 
                switch (node){
                    case NodeHTML h:
                        if (h.FindText(text)) return true;
                        break;

                    case NodeText t:
                        if (t.Text.Contains(text)) return true;
                        break;
                }
            }
            return false;
        }
    }
    
    class NodeText : Node{ 
        public string Text;
    }
    
    abstract class Node { }
}
