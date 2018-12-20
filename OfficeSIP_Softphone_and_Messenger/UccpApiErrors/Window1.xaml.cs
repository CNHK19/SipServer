// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace UccpApiErrors
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UccpApiErrors.UccApiErr.h"))
            using (StreamReader sr = new StreamReader(stream))
            {
                textSource.AppendText(sr.ReadToEnd());
            }

            Convert();
        }

        private void Convert()
        {
            try
            {
                // Parse

                Regex reVerify = new Regex(@"^#define\s+UCC_", RegexOptions.Multiline);
                MatchCollection matchesVerify = reVerify.Matches(textSource.Text);

                Regex re = new Regex(@"^// MessageText:\r\n//\r\n// (?<message>[^\r\n]+)\r\n//\r\n#define\s+(?<id>[A-Z0-9_]+)\s+\(\(HRESULT\)(?<code>0x[0-9A-F]{8})L\)", RegexOptions.Multiline);
                MatchCollection matches = re.Matches(textSource.Text);

                if (matchesVerify.Count != matches.Count)
                    throw new Exception(String.Format("Verification error: Verification regex found {0} maches, work regex found {1} maches.", matchesVerify.Count, matches.Count));

                // Generate

                textResult.AppendText("using System;\r\n");
                textResult.AppendText("\r\n");
                textResult.AppendText("namespace Uccapi\r\n");
                textResult.AppendText("{\r\n");
                textResult.AppendText("\tpublic class Errors\r\n");
                textResult.AppendText("\t{\r\n");

                foreach (Match match in matches)
                {
                    textResult.AppendText(String.Format("\t\tpublic const UInt32 {0} = {1};\r\n", match.Groups["id"], match.Groups["code"], match.Groups["message"]));
                }

                textResult.AppendText("\r\n");
                textResult.AppendText("\t\tpublic static string ToString(int code)\r\n");
                textResult.AppendText("\t\t{\r\n");
                textResult.AppendText("\t\t\treturn ToString((UInt32)code);\r\n");
                textResult.AppendText("\t\t}\r\n");

                textResult.AppendText("\r\n");
                textResult.AppendText("\t\tpublic static string ToString(UInt32 code)\r\n");
                textResult.AppendText("\t\t{\r\n");
                textResult.AppendText("\t\t\tswitch(code)\r\n");
                textResult.AppendText("\t\t\t{\r\n");

                foreach (Match match in matches)
                {
                    textResult.AppendText(String.Format("\t\t\t\tcase {0}: return @\"{2}\";\r\n", match.Groups["id"], match.Groups["code"], match.Groups["message"]));
                }

                textResult.AppendText("\t\t\t}\r\n");
                textResult.AppendText("\t\t\treturn @\"\";\r\n");
                textResult.AppendText("\t\t}\r\n");
                textResult.AppendText("\t}\r\n");

                textResult.AppendText("}\r\n");
            }
            catch (Exception e)
            {
                textResult.Text = e.Message;
            }
        }
    }
}
