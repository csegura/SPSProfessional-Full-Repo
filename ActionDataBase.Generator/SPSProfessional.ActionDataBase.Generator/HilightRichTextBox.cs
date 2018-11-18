using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SPSProfessional.ActionDataBase.Generator
{
    public class HilightRichTextBox : RichTextBox
    {
        private readonly SyntaxSettings _settings;
        private static bool _enablePaint;
        private string _textLine;
        private int _contentLength;
        private int _lineLength;
        private int _lineStart;
        private int _lineEnd;
        private string _regexKeywords;
        private string _regexSymbols;
        private int _currentSelection;
        private bool _processing;

        public HilightRichTextBox()
        {
            _settings = new SyntaxSettings();
            _enablePaint = true;
            _textLine = string.Empty;
            _regexKeywords = string.Empty;
            _regexSymbols = string.Empty;
        }

        #region Properties

        /// <summary>
        /// The settings.
        /// </summary>
        public SyntaxSettings Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Gets or sets the current text in the rich text box.
        /// </summary>
        /// <value></value>
        /// <returns>The text displayed in the control.</returns>
        public new string Text
        {
            set
            {
                base.Text = value;
                ProcessAll();
            }
            get { return base.Text; }
        }

        #endregion

        //[DllImport("user32")]
        //private static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        //private const int WM_SETREDRAW = 0xB;

        //int paintFrozen;

        //private bool FreezePainting
        //{
        //    get { return paintFrozen == 0; }
        //    set
        //    {
        //        if (value && IsHandleCreated && Visible)
        //            if (0 == paintFrozen++)
        //                SendMessage(Handle, WM_SETREDRAW, 0, 0);

        //        if (!value)
        //        {
        //            if (paintFrozen == 0) return;
        //            if (0 == --paintFrozen)
        //            {
        //                SendMessage(Handle, WM_SETREDRAW, 1, 0);
        //                Invalidate(true);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x00f)
            {
                if (_enablePaint)
                {
                    base.WndProc(ref m);
                }
                else
                {
                    m.Result = IntPtr.Zero;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// OnTextChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            if (!_processing)
            {
                // Calculate shit here.
                _contentLength = TextLength;

                int nCurrentSelectionStart = SelectionStart;
                int nCurrentSelectionLength = SelectionLength;

                _enablePaint = false;

                // Find the start of the current line.
                _lineStart = nCurrentSelectionStart;
                while ((_lineStart > 0) && (Text[_lineStart - 1] != '\n'))
                {
                    _lineStart--;
                }
                // Find the end of the current line.
                _lineEnd = nCurrentSelectionStart;
                while ((_lineEnd < Text.Length) && (Text[_lineEnd] != '\n'))
                {
                    _lineEnd++;
                }
                // Calculate the length of the line.
                _lineLength = _lineEnd - _lineStart;
                // Get the current line.
                _textLine = Text.Substring(_lineStart, _lineLength);

                // Process this line.
                ProcessLine();

                _enablePaint = true;
            }
        }

        /// <summary>
        /// Process a line.
        /// </summary>
        private void ProcessLine()
        {

            // Save the position and make the whole line black
            int nPosition = SelectionStart;
            SelectionStart = _lineStart;
            SelectionLength = _lineLength;
            SelectionColor = Color.Black;

            // Process the keywords
            ProcessRegex(_regexKeywords, Settings.KeywordColor);

            // Process the symbols            
            ProcessRegex(_regexSymbols, Settings.SymbolsColor);

            // Process numbers
            if (Settings.EnableIntegers)
            {
                ProcessRegex("\\b(?:[0-9]*\\.)?[0-9]+\\b", Settings.IntegerColor);
            }
            // Process strings
            if (Settings.EnableStrings)
            {
                ProcessRegex("\"[^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*\"", Settings.StringColor);
            }
            // Process comments
            if (Settings.EnableComments && !string.IsNullOrEmpty(Settings.Comment))
            {
                ProcessRegex(Settings.Comment + ".*$", Settings.CommentColor);
            }

            SelectionStart = nPosition;
            SelectionLength = 0;
            SelectionColor = Color.Black;

            _currentSelection = nPosition;

            _processing = false;
        }

        /// <summary>
        /// Process a regular expression.
        /// </summary>
        /// <param name="strRegex">The regular expression.</param>
        /// <param name="color">The color.</param>
        private void ProcessRegex(string strRegex, Color color)
        {
            Regex regKeywords =
                new Regex(strRegex, RegexOptions.IgnoreCase |
                                    RegexOptions.Compiled |
                                    (_processing ? RegexOptions.Multiline : 0));
            Match regMatch;

            for (regMatch = regKeywords.Match(_textLine); regMatch.Success; regMatch = regMatch.NextMatch())
            {
                // Process the words
                int nStart = _lineStart + regMatch.Index;
                int nLenght = regMatch.Length;
                SelectionStart = nStart;
                SelectionLength = nLenght;
                SelectionColor = color;
            }
        }

        /// <summary>
        /// Compiles the keywords as a regular expression.
        /// </summary>
        public void CompileKeywords()
        {
            for (int i = 0; i < Settings.Keywords.Count; i++)
            {
                string strKeyword = Settings.Keywords[i];

                if (i == Settings.Keywords.Count - 1)
                {
                    _regexKeywords += "\\b" + strKeyword + "\\b";
                }
                else
                {
                    _regexKeywords += "\\b" + strKeyword + "\\b|";
                }
            }
        }

        /// <summary>
        /// Compiles the keywords as a regular expression.
        /// </summary>
        public void CompileRegexSymbols()
        {
            for (int i = 0; i < Settings.Symbols.Count; i++)
            {
                string strSymbols = Settings.Symbols[i];
                _regexSymbols += strSymbols;
            }
        }

        /// <summary>
        /// Compiles the keywords as a regular expression.
        /// </summary>
        public void CompileRegexKeywords()
        {
            for (int i = 0; i < Settings.Keywords.Count; i++)
            {
                string strKeywords = Settings.Keywords[i];
                _regexKeywords += strKeywords;
            }
        }

        public void ProcessAllLines()
        {
            _enablePaint = false;

            int nStartPos = 0;
            int i = 0;
            int nOriginalPos = SelectionStart;
            while (i < Lines.Length)
            {
                _textLine = Lines[i];
                _lineStart = nStartPos;
                _lineEnd = _lineStart + _textLine.Length;

                ProcessLine();
                i++;

                nStartPos += _textLine.Length + 1;
            }

            _enablePaint = true;
        }

        public void ProcessAll()
        {
            _enablePaint = false;

            _lineStart = 0;
            _lineEnd = Lines.Length;
            _lineLength = TextLength;
            _textLine = Text;
            _processing = true;

            ProcessLine();

            _enablePaint = true;
        }

        public void GoToLineAndColumn(int line, int column)
        {
            int offset = 0;

            for (int i = 0; i < line - 1 && i < Lines.Length; i++)
            {
                offset += Lines[i].Length + 1;
            }

            Focus();
            Select(offset + column, 0);
        }
    }

    /// <summary>
    /// Class to store syntax objects in.
    /// </summary>
    public class SyntaxList
    {
        public List<string> m_rgList = new List<string>();
        public Color m_color = new Color();
    }

    /// <summary>
    /// Settings for the keywords and colors.
    /// </summary>
    public class SyntaxSettings
    {
        private SyntaxList m_rgKeywords = new SyntaxList();
        private SyntaxList m_rgSymbols = new SyntaxList();
        private string m_strComment = "";
        private Color m_colorComment = Color.Green;
        private Color m_colorString = Color.Gray;
        private Color m_colorInteger = Color.Red;
        private bool m_bEnableComments = true;
        private bool m_bEnableIntegers = true;
        private bool m_bEnableStrings = true;

        #region Properties

        /// <summary>
        /// A list containing all keywords.
        /// </summary>
        public List<string> Keywords
        {
            get { return m_rgKeywords.m_rgList; }
        }

        /// <summary>
        /// The color of keywords.
        /// </summary>
        public Color KeywordColor
        {
            get { return m_rgKeywords.m_color; }
            set { m_rgKeywords.m_color = value; }
        }

        /// <summary>
        /// A list containing all keywords.
        /// </summary>
        public List<string> Symbols
        {
            get { return m_rgSymbols.m_rgList; }
        }

        /// <summary>
        /// The color of keywords.
        /// </summary>
        public Color SymbolsColor
        {
            get { return m_rgSymbols.m_color; }
            set { m_rgSymbols.m_color = value; }
        }

        /// <summary>
        /// A string containing the comment identifier.
        /// </summary>
        public string Comment
        {
            get { return m_strComment; }
            set { m_strComment = value; }
        }

        /// <summary>
        /// The color of comments.
        /// </summary>
        public Color CommentColor
        {
            get { return m_colorComment; }
            set { m_colorComment = value; }
        }

        /// <summary>
        /// Enables processing of comments if set to true.
        /// </summary>
        public bool EnableComments
        {
            get { return m_bEnableComments; }
            set { m_bEnableComments = value; }
        }

        /// <summary>
        /// Enables processing of integers if set to true.
        /// </summary>
        public bool EnableIntegers
        {
            get { return m_bEnableIntegers; }
            set { m_bEnableIntegers = value; }
        }

        /// <summary>
        /// Enables processing of strings if set to true.
        /// </summary>
        public bool EnableStrings
        {
            get { return m_bEnableStrings; }
            set { m_bEnableStrings = value; }
        }

        /// <summary>
        /// The color of strings.
        /// </summary>
        public Color StringColor
        {
            get { return m_colorString; }
            set { m_colorString = value; }
        }

        /// <summary>
        /// The color of integers.
        /// </summary>
        public Color IntegerColor
        {
            get { return m_colorInteger; }
            set { m_colorInteger = value; }
        }

        #endregion
    }
}