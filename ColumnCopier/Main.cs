using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ColumnCopier
{
    public partial class Main : Form
    {
        #region Private Fields

        private const string NumberOfColumnsFormat = "# of Columns: {0}";
        private const string CurrentColumnFormat = "Current Column #: {0}";
        private const string NumberOfRowsFormat = "# of Rows: {0}";

        private string saveFile = "";

        private const string BaseExecutableName = "ColumnCopier";
        private const string SaveVersion = "1.0";
        private const string SaveFileExtension = ".ccs";

        private int requestID = 0;
        
        private string defaultColumnPriority = "Number";

        private string currentRequest = "";
        Queue<string> historyLog = new Queue<string>();
        private Dictionary<string, Request> history = new Dictionary<string, Request>();

        #endregion Private Fields

        #region Public Constructors

        public Main()
        {
            InitializeComponent();

            string defaultName = Title;

            string fileToLoad = "";
            if (BaseExecutableName != ExecutableName)
                fileToLoad = $"{BaseExecutableName}{SaveFileExtension}";
            else
                fileToLoad = $"ColumnCopier-{DateTime.Now.Ticks}{SaveFileExtension}";
            
            LoadSettings($"{ExecutableDirectory}\\{fileToLoad}");
        }

        #endregion Public Constructors

        private void SetupTitle()
        {
            Title = $"Column Copier - {ExecutableVersion} - {saveFile}";
        }

        #region Private Properties

        private string ExecutableDirectory
        {
            get
            {
                var dirName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                
                return dirName.Remove(0, 6);
            }
        }

        private string ExecutableName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase); }
        }

        private string ExecutableVersion
        {
            get { return this.ProductVersion; }
        }

        private string ClipBoard
        {
            get
            {
                return Clipboard.ContainsText()
                    ? Clipboard.GetText()
                    : "";
            }
            set
            {
                Clipboard.Clear();
                Clipboard.SetText(value);
            }
        }

        private string ColumnText
        {
            get { return column_txt.Text; }
            set { column_txt.Text = value; }
        }

        private string CurrentColumnText
        {
            get { return currentColumn_txt.Text; }
            set { currentColumn_txt.Text = value; }
        }

        private int CurrentLine
        {
            get { return ParseTextToInt(line_txt.Text); }
            set { line_txt.Text = value.ToString(); }
        }

        private int DefaultColumn
        {
            get { return ParseTextToInt(defaultColumn_txt.Text); }
            set { defaultColumn_txt.Text = value.ToString(); }
        }

        private string DefaultColumnName
        {
            get { return defaultColumnText_txt.Text; }
            set { defaultColumnText_txt.Text = value; }
        }

        private int MaxHistory
        {
            get { return ParseTextToInt(history_txt.Text); }
            set { history_txt.Text = value.ToString(); }
        }

        private int Threshold
        {
            get { return ParseTextToInt(threshold_txt.Text, 5); }
            set { threshold_txt.Text = value.ToString(); }
        }

        private string NumberOfColumnsText
        {
            get { return columnNum_txt.Text; }
            set { columnNum_txt.Text = value; }
        }

        private string NumberOfRowsText
        {
            get { return rowNum_txt.Text; }
            set { rowNum_txt.Text = value; }
        }

        private string ReplaceText
        {
            get { return replaceText_txt.Text; }
            set { replaceText_txt.Text = value; }
        }

        private string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        #endregion Private Properties

        #region Public Methods

        public void ResetFields()
        {
            ColumnText = "";
            column_cmb.Items.Clear();

            NumberOfColumnsText = string.Format(NumberOfColumnsFormat, 0);
            NumberOfRowsText = string.Format(NumberOfRowsFormat, 0);
            CurrentColumnText = string.Format(CurrentColumnFormat, 0);
        }

        public string ReplaceNewLineInText()
        {
            var lines = history[currentRequest].GetColumnTextLines(column_cmb.SelectedIndex);

            if (lines.Count > 0)
            {
                StringBuilder str = new StringBuilder();

                for (int i = 0; i < lines.Count; i++)
                    str.AppendFormat("{0}{1}", lines[i], ReplaceText);

                return str.ToString();
            }
            
            return "";
        }

        public bool LoadSettings(string file)
        {
            try
            {
                if (!File.Exists(file))
                    return false;

                var doc = XDocument.Load(file);



                ResetFields();
                saveFile = file;
                SetupTitle();

                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        public bool SaveSettings(string file)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);

                var str = new StringBuilder();

                str.AppendLine("<ColumnCopier>");

                str.AppendLine(string.Format("<SaveVersion>{0}</SaveVersion>", SaveVersion));
                str.AppendLine("<Settings>");
                str.AppendLine(string.Format("<ShowOnTop>{0}</ShowOnTop>", isTop_cbx.Checked));
                str.AppendLine(string.Format("<DataHasHeaders>{0}</DataHasHeaders>", header_cxb.Checked));
                str.AppendLine(string.Format("<NextLine>{0}</NextLine>", line_txt.Text));
                str.AppendLine(string.Format("<ReplaceText>{0}</ReplaceText>", ReplaceText));
                str.AppendLine(string.Format("<DefaultColumn>{0}</DefaultColumn>", DefaultColumn));
                str.AppendLine(string.Format("<DefaultColumnName>{0}</DefaultColumnName>", DefaultColumnName));
                str.AppendLine(string.Format("<Threshold>{0}</Threshold>", Threshold));
                str.AppendLine(string.Format("<Priority>{0}</Priority>", defaultColumnPriority));
                str.AppendLine(string.Format("<MaxHistory>{0}</MaxHistory>", MaxHistory));
                str.AppendLine("</Settings>");

                str.AppendLine("<History>");
                str.AppendLine(string.Format("<CurrentRequest>{0}</CurrentRequest>", currentRequest));
                str.AppendLine(string.Format("<CurrentColumn>{0}</CurrentColumn>", history[currentRequest].CurrentColumn));
                str.AppendLine("<Requests>");
                foreach(var request in history)
                    str.AppendLine(request.Value.ToXmlText());
                str.AppendLine("</Requests>");
                str.AppendLine("</History>");

                str.AppendLine("</ColumnCopier>");

                var doc = XDocument.Parse(str.ToString());
                doc.Save(file);

                return true;
            }
            catch (Exception ex)
            {
                if (true) ;
            }

            return false;
        }

        public void LoadRequest(string name)
        {
            currentRequest = name;
            ColumnText = "";

            column_cmb.Items.Clear();
            foreach (var newColumn in history[currentRequest].ColumnKeys.Values)
                column_cmb.Items.Add(newColumn);

            column_cmb.SelectedIndex = DetermineSelectedIndex();
        }

        public int DetermineSelectedIndex()
        {
            switch (defaultColumnPriority)
            {
                case "Number":
                    if (DefaultColumn >= 0)
                        return DefaultColumn;
                    break;
                case "Name":
                    int currentClosestDistance = int.MaxValue;
                    int currentClosestID = int.MaxValue;

                    for (int i = 0; i < history[currentRequest].ColumnKeys.Count; i++)
                    {
                        int computedDistance = Compute(DefaultColumnName, history[currentRequest].ColumnKeys[i]);

                        if (computedDistance < currentClosestDistance)
                        {
                            currentClosestDistance = computedDistance;
                            currentClosestID = i;
                        }
                    }

                    if (currentClosestDistance <= Threshold)
                        return currentClosestID;

                    break;
            }

            if (DefaultColumn >= 0)
                return DefaultColumn;

            return 0;
        }

        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public void LoadColumn(int columnNumber)
        {
            ColumnText = history[currentRequest].GetColumnText(columnNumber);
            NumberOfColumnsText = string.Format(NumberOfColumnsFormat, history[currentRequest].NumberOfColumns);
            NumberOfRowsText = string.Format(NumberOfRowsFormat, history[currentRequest].CurrentColumnNumberOfRows);
            CurrentColumnText = string.Format(CurrentColumnFormat, history[currentRequest].CurrentColumnInteger);
        }

        public void CreateRequest(string text)
        {
            var newRequest = new Request(text, requestID++, header_cxb.Checked);

            history.Add(newRequest.Name, newRequest);
            historyLog.Enqueue(newRequest.Name);

            var numberOfHistoryToDelete = historyLog.Count - MaxHistory;
            if (numberOfHistoryToDelete > 0)
            {
                for (int i = 0; i < numberOfHistoryToDelete; i++)
                    history.Remove(historyLog.Dequeue());
            }


            history_cmb.Items.Clear();
            foreach (var historyRequest in history.Keys)
                history_cmb.Items.Add(historyRequest);
            history_cmb.SelectedIndex = history_cmb.Items.Count - 1;

            LoadRequest(newRequest.Name);
        }

        #endregion Public Methods

        #region Private Methods

        private int ParseTextToInt(string text, int defaultValue = 0)
        {
            int output;
            if (int.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        private void clearHistory_btn_Click(object sender, EventArgs e)
        {
            history.Clear();
            history_cmb.Items.Clear();

            ResetFields();
        }

        private void column_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadColumn(column_cmb.SelectedIndex);
        }

        private void copy_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = ColumnText;
        }

        private void copyLine_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = history[currentRequest].GetNextLine();
        }

        private void copyReplace_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = ReplaceNewLineInText();
        }

        private void history_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest(history_cmb.Text);
        }

        private void isTop_cbx_CheckedChanged(object sender, EventArgs e)
        {
            if (isTop_cbx.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;

            SaveSettings(saveFile);
        }

        private void loadSettings_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileSelector = new OpenFileDialog();
            fileSelector.Filter = $"{SaveFileExtension} | Column Copier Save File";
            fileSelector.InitialDirectory = ExecutableDirectory;

            fileSelector.ShowDialog();
            var file = fileSelector.FileName;

            LoadSettings(file);
        }

        private void number_rbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (number_rbtn.Checked)
                defaultColumnPriority = "Number";
        }

        private void paste_btn_Click(object sender, EventArgs e)
        {
            CreateRequest(ClipBoard);
            SaveSettings(saveFile);
        }

        private void pasteCopy_btn_Click(object sender, EventArgs e)
        {
            CreateRequest(ClipBoard);
            ClipBoard = ColumnText;
            SaveSettings(saveFile);
        }

        private void replaceComma_btn_Click(object sender, EventArgs e)
        {
            ReplaceText = ", ";
        }

        private void saveAsNew_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileSelector = new SaveFileDialog();
            fileSelector.Filter = $"{SaveFileExtension} | Column Copier Save File";
            fileSelector.InitialDirectory = ExecutableDirectory;

            fileSelector.ShowDialog();
            var file = fileSelector.FileName;

            SaveSettings(file);
        }

        private void saveSettings_btn_Click(object sender, EventArgs e)
        {
            SaveSettings(saveFile);
        }

        private void text_rbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (text_rbtn.Checked)
                defaultColumnPriority = "Name";
        }

        #endregion Private Methods

        private void header_cxb_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}