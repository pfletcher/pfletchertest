using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using MyExtensions;

namespace mdtodita
{
    public partial class MdToDitaForm: Form
    {
        List<documentToConvert> Documents = new List<documentToConvert>();
        public MdToDitaForm()
        {
            InitializeComponent();
        }

        private void buttonReadFiles_Click(object sender, EventArgs e)
        {
            var reader = new XmlTextReader("files.xml");
            while(reader.Read())
            {
                switch (reader.NodeType)
                {
                    
                    case XmlNodeType.Element:
                        //appendLine(reader.Name);
                        switch (reader.Name)
                        {
                            case "file":
                                {
                                    var curDocument = new documentToConvert();
                                    while (reader.Read())
                                    {
                                        //Break out at end of file node (hack)
                                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "file")
                                        {
                                            break;
                                        }
                                        switch (reader.NodeType)
                                        {
                                            case XmlNodeType.Element:
                                                switch (reader.Name)
                                                {
                                                    case "filename":
                                                        reader.Read();
                                                        curDocument.filename = reader.Value;
                                                        break;
                                                    case "topicId":
                                                        reader.Read();
                                                        curDocument.topicId = reader.Value;
                                                        break;
                                                    case "outputFilename":
                                                        reader.Read();
                                                        curDocument.outputFilename = reader.Value;
                                                        break;
                                                    case "title":
                                                        reader.Read();
                                                        curDocument.title = reader.Value;
                                                        break;
                                                }
                                                break;
                                        }
                                    }
                                    Documents.Add(curDocument);
                                    appendLine(String.Format("Document read: {0}", curDocument.title));
                                }
                                break;
                        }
                        break;
                    case XmlNodeType.Text:
                        //appendLine(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        //appendLine(reader.Name);
                        break;

                }
            }
        }
        void appendLine(string text)
        {
            richTextConsole.AppendText(text + Environment.NewLine);
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            foreach (documentToConvert doc in Documents)
            {
                StreamReader file = new StreamReader(doc.filename);
                List<String> inputFileLines = new List<String>();
                List<String> outputFileLines = new List<String>();
                string line;
                //TODO: Replace all these flags with an enum, since only one should be true at a time
                bool inATable = false;
                bool inAList = false;
                bool inAParagraph = false;
                bool inADLEntry = false;
                bool inACodeBlock = false;
                bool ignoredTitle = false;  //Ignore the first instance of a top-level header; convert the second into a bold para

                // Read file into list
                while ((line = file.ReadLine()) != null) {
                    inputFileLines.Add(line);
                }

                //DITA header
                outputFileLines.Add("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                outputFileLines.Add("<!DOCTYPE topic PUBLIC \" -//OASIS//DTD DITA Topic//EN\" \"topic.dtd\">");
                outputFileLines.Add(String.Format("<topic id=\"{0}\">", doc.topicId));
                outputFileLines.Add(String.Format("  <title><ph keyref=\"cnap-tm\" /> {0}</title>", doc.title));
                outputFileLines.Add("  <body>");
                outputFileLines.Add("<!--  **********************************************");
                outputFileLines.Add("      *");
                outputFileLines.Add("      * This document was created with a tool.");
                outputFileLines.Add("      * Do not modify this document by hand.");
                outputFileLines.Add("      *");
                outputFileLines.Add("      *********************************************** -->");


                //Parse md file into output
                for (int lineNumber = 0; lineNumber < inputFileLines.Count; lineNumber++)
                {
                    string lineToProcess = inputFileLines[lineNumber];

                    //Is this part of a table (that starts with two spaces and uses a hash as a separator)?
                    if (inATable)
                    {
                        if (lineToProcess.StartsWith("  ") && lineToProcess.Contains(" #")) //Still in a table; add line and move to next
                        {
                            outputFileLines.Add(formatTableRow(FormatDita(lineToProcess.Substring(2))));
                            continue;
                        }
                        else  //No longer in table; close table and continue parsing line
                        {
                            outputFileLines.Add("    </simpletable>");
                            inATable = false;
                        }
                    }
                    //Is this part of a definition list (that starts with a dash)?
                    if (inAList)
                    {
                        if (inADLEntry) //Multi-line, indented text
                        {
                            if (String.IsNullOrWhiteSpace(lineToProcess))
                            {
                                outputFileLines.Add("          </dd>");
                                outputFileLines.Add("        </dlentry>");
                                inADLEntry = false;
                                continue;
                            }
                            else if (lineToProcess.StartsWith("  ")) //Rest of DL definition
                            {
                                outputFileLines.Add("          " + FormatDita(lineToProcess.Substring(2)));
                                continue;
                            }
                            else if (lineToProcess.StartsWith("- ")) //New DL entry just stuck right up against the prev one
                            {
                                outputFileLines.Add("      </dlentry>");                             
                                outputFileLines.Add("      <dlentry>");
                                outputFileLines.Add("        <dt>");
                                outputFileLines.Add("          " + FormatDita(lineToProcess.Substring(2)));
                                outputFileLines.Add("        </dt>");
                                continue;
                            }

                        }
                        if (lineToProcess.StartsWith("- ")) //New DL entry and term
                        {
                            outputFileLines.Add("      <dlentry>");
                            outputFileLines.Add("        <dt>");
                            outputFileLines.Add("          " + FormatDita(lineToProcess.Substring(2)));    
                            outputFileLines.Add("        </dt>");
                            continue;
                        }
                        
                        else if (lineToProcess.StartsWith("  ")) //New DL definition
                        {
                            outputFileLines.Add("        <dd>");
                            outputFileLines.Add("          " + FormatDita(lineToProcess.Substring(2)));
                            inADLEntry = true;
                            continue;
                        }
                        
                        else //Something else besides a list; close list and move on
                        {
                            outputFileLines.Add("    </dl>");
                            inAList = false;
                        }
                    }

                    if (inAParagraph)
                    {
                        if (String.IsNullOrWhiteSpace(lineToProcess))   //Close paragraph
                        {
                            outputFileLines.Add("    </p>");
                            inAParagraph = false;
                            continue;
                        }
                        else if (lineToProcess.StartsWith("=="))     //Title- we're getting this from the xml file though, so we'll not do anything
                        {
                            if (ignoredTitle == false)  //First instance: remove title and rely on title from files.xml
                            {
                                //Remove the previous line and paragraph tag and do nothing with this one 
                                outputFileLines.RemoveAt(outputFileLines.Count - 1);
                                outputFileLines.RemoveAt(outputFileLines.Count - 1);
                                inAParagraph = false;
                                ignoredTitle = true;
                                continue;
                            }
                            else    //Subsequent instance: Convert previous line to bold header (it will already have a starting p tag; close out paragraph). 
                            {
                                outputFileLines[outputFileLines.Count - 1] = String.Format("    <b>{0}</b></p>", outputFileLines[outputFileLines.Count - 1].Substring(6));
                                inAParagraph = false;
                                continue;
                            }
                        }
                        else      //Continue paragraph
                        {
                            outputFileLines.Add("      " + FormatDita(lineToProcess));
                            continue;
                        }
                    }
                    if (inACodeBlock)
                    {
                        if (lineToProcess.Contains("```"))    //End of code block
                        {
                            outputFileLines.Add("    </codeblock>");
                            inACodeBlock = false;
                            continue;
                        }
                        else
                        {
                            outputFileLines.Add(lineToProcess);
                            continue;
                        }
                    }
                    
                    
                    if (lineToProcess.StartsWith("  ") && lineToProcess.Contains(" #"))   //Starting a table (inATable is false if we got here)
                    {
                        outputFileLines.Add("    <simpletable>");
                        outputFileLines.Add(formatTableRow(FormatDita(lineToProcess.Substring(2))));
                        inATable = true;
                    }
                    else if (lineToProcess.StartsWith("= "))    //Secondary header
                    {
                        outputFileLines.Add(String.Format("  <p><b>{0}</b></p>", FormatDita(lineToProcess.Substring(2))));
                        continue;
                    }
                    else if (lineToProcess.StartsWith("- "))    //New definition list 
                    {
                        outputFileLines.Add("    <dl>");
                        outputFileLines.Add("      <dlentry>");
                        outputFileLines.Add("        <dt>");
                        outputFileLines.Add("          " + FormatDita(lineToProcess.Substring(2)));    //TODO: Write parser for ` and such
                        outputFileLines.Add("        </dt>");
                        inAList = true;
                        continue;
                    }
                    else if (lineToProcess.Contains("```"))    //New code block
                    {
                        outputFileLines.Add("    <codeblock>");
                        inACodeBlock = true;
                        continue;
                    }
                    else if (String.IsNullOrWhiteSpace(lineToProcess)) //Probably an empty line after a heading; do nothing
                    {
                        continue;
                    }
                    else    //Not a match for any known formatting- start a new paragraph
                    {
                        outputFileLines.Add("    <p>");
                        outputFileLines.Add("      " + FormatDita(lineToProcess));
                        inAParagraph = true;
                        continue;
                    }


                }
                //close out document
                if (inAParagraph)
                {
                    outputFileLines.Add("    </p>");
                    inAParagraph = false;
                }
                if (inAList)
                {
                    outputFileLines.Add("    </simpletable>");
                    inAList = false;
                }
                if (inAList)
                {
                    outputFileLines.Add("    </dl>");
                }
                
                outputFileLines.Add("  </body>");
                outputFileLines.Add("</topic>");
                appendLine(String.Format("Done reading {0}!", doc.topicId));
                //All done! Write to file
                File.WriteAllLines(doc.outputFilename, outputFileLines, Encoding.UTF8);
            }
            
        }
        string formatTableRow(string input)
        {
            string[] cells = input.Split('#');
            StringBuilder tableRow = new StringBuilder("      <strow>");
            //Bold the first cell
            tableRow.Append(String.Format("<stentry><b>{0}</b></stentry>", FormatDita(cells[0])));
            for (int cell = 1; cell < cells.Length; cell++)
            {
                tableRow.Append(String.Format("<stentry>{0}</stentry>", FormatDita(cells[cell])));
            }
            tableRow.Append("</strow>");
            return tableRow.ToString();
        }
        string FormatDita(string input)
        {
            String output = input;
            //Replace angle brackets and ampersands with escape sequences
            output = output.Replace("<", "&lt;");
            output = output.Replace(">", "&gt;");
            output = output.Replace("&", "&amp;");

            //Replace `s with alternating <codeph> and </codeph> tags (back to front, since swapping them changes the file length)
            if (output.Contains("`"))
            {
                List<int> ticks = output.AllIndexesOf("`");
                if (ticks.Count % 2 == 0)   //There should be an even number of ticks! Three ticks means something else!
                {
                    for (int tick = ticks.Count - 2; tick >= 0; tick -= 2)
                    {
                        var tempString = new StringBuilder(output);
                        tempString.Remove(ticks[tick + 1], 1);
                        tempString.Insert(ticks[tick + 1], "</codeph>");
                        tempString.Remove(ticks[tick], 1);
                        tempString.Insert(ticks[tick], "<codeph>");
                        output = tempString.ToString();
                    }
                }
            }
            //Replace **s with alternating <b> and </b tags
            if (output.Contains("**"))
            {
                List<int> bolds = output.AllIndexesOf("**");
                if (bolds.Count % 2 == 0)   //There should be an even number of bolds! 
                {
                    for (int bold = bolds.Count - 2; bold >= 0; bold -= 2)
                    {
                        var tempString = new StringBuilder(output);
                        tempString.Remove(bolds[bold + 1], 2);
                        tempString.Insert(bolds[bold + 1], "</b>");
                        tempString.Remove(bolds[bold], 2);
                        tempString.Insert(bolds[bold], "<b>");
                        output = tempString.ToString();
                    }
                }
            }
            return output;
        }
        
        
    }
   
    class documentToConvert
    {
        public string filename { get; set; }
        public string topicId { get; set; }
        public string outputFilename { get; set; }
        public string  title { get; set; }
    }
    
}
namespace MyExtensions
{
    public static class FindAll
    {
        //borrowed this from Stack Overflow
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}
