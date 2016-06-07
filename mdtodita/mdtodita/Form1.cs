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
                                                }
                                                break;
                                        }
                                    }
                                    Documents.Add(curDocument);
                                    appendLine(String.Format("Document read: {0} {1} {2}", curDocument.filename, curDocument.topicId, curDocument.outputFilename));
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
                bool inAList = false;

                // Read file into list
                while ((line = file.ReadLine()) != null) {
                    inputFileLines.Add(line);
                }

                //Parse into output
                for(int lineNumber = 0; lineNumber < inputFileLines.Count; lineNumber++)
                {
                    string lineToProcess = inputFileLines[lineNumber];

                    //Is this part of a list (that starts with two spaces)?
                    if (inAList)
                    {
                        //Still in list
                        if (lineToProcess.StartsWith("  "))
                        {
                            outputFileLines.Add(String.Format("  <li>{0}</li>", lineToProcess.Substring(2)));
                            continue;
                        }
                        else  //No longer in list
                        {
                            outputFileLines.Add("</ul>");
                            inAList = false;
                        }

                    }
                    
                    if (lineToProcess.StartsWith("=="))     //Two-line header
                    {
                        outputFileLines.Add(String.Format("<p><b>{0}</b></p>", inputFileLines[lineNumber - 1]));
                    }
                    else if (lineToProcess.StartsWith("  "))    //Starting an unordered list
                    {
                        outputFileLines.Add("<ul>");
                        outputFileLines.Add(String.Format("  <li>{0}</li>", lineToProcess.Substring(2)));
                        inAList = true;
                    }
                    
                }
                appendLine(String.Format("Done reading {0}!", doc.topicId));
            }
            
        }
    }
    class documentToConvert
    {
        public string filename { get; set; }
        public string topicId { get; set; }
        public string outputFilename { get; set; }
    }
}
