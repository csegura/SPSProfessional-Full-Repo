using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SPSProfessional.SharePoint.Admin.ListTools
{
    public class SPSRegisterableTag
    {
        public string TagPrefix;
        public string TagName;
        public string Src;
        public string Assembly;
        public string NameSpace;
    }

    internal class SPSTemplateManagerIO
    {
        // Fields
        private bool _contentChanged;
        private const string FileName = "DefaultTemplates.ascx";
        private readonly string _folderName;
        private readonly string _fullFileName;
        private readonly List<string> _headers;
        private readonly Dictionary<string, string> _templates;

        // Methods
        public SPSTemplateManagerIO()
        {
            _folderName =
            string.Format(@"{0}\Microsoft Shared\web server extensions\12\Template\controltemplates",
                          Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
            _fullFileName = string.Format(@"{0}\{1}", _folderName, FileName);
            _headers = new List<string>();
            _templates = new Dictionary<string, string>();

            LoadDefaultTemplate();
        }

        private void LoadDefaultTemplate()
        {
            string fileContent;

            using (var stream = File.Open(_fullFileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    fileContent = reader.ReadToEnd();
                }
            }
            LoadHeaders(fileContent);
            LoadTemplateRendering(fileContent);
        }

        private void LoadHeaders(string fileContent)
        {
            var regex = new Regex("<%@((?!%>).)*%>");

            foreach (Match match in regex.Matches(fileContent))
            {
                _headers.Add(match.Value);
            }
        }

        private void LoadTemplateRendering(string fileContent)
        {
            var regex =
                new Regex(
                    "<SharePoint:RenderingTemplate((?!>).)*>((?!</SharePoint:RenderingTemplate>).)*</SharePoint:RenderingTemplate>",
                    RegexOptions.Singleline);
            var regex2 = new Regex("ID=\"(?<value>((?!\").)*)\"");
            foreach (Match match in regex.Matches(fileContent))
            {
                MatchCollection matchs = regex2.Matches(match.Value);
                if (matchs.Count > 0)
                {
                    _templates.Add(matchs[0].Groups["value"].Value, match.Value);
                }
            }
        }

        private void BackupFile(string newFileName)
        {
            if (File.Exists(string.Format(@"{0}\{1}", _folderName, newFileName)))
            {
                File.Delete(string.Format(@"{0}\{1}", _folderName, newFileName));
            }
            File.Copy(_fullFileName, string.Format(@"{0}\{1}", _folderName, newFileName));
        }

        internal void Save()
        {
            Save(string.Empty);
        }

        internal void Save(string backupFileName)
        {
            if (_contentChanged)
            {
                if (!string.IsNullOrEmpty(backupFileName))
                {
                    BackupFile(backupFileName);
                }
                File.Delete(_fullFileName);
                using (FileStream stream = File.Create(_fullFileName))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        foreach (string str in _headers)
                        {
                            writer.WriteLine(str);
                        }
                        foreach (string str2 in _templates.Values)
                        {
                            writer.WriteLine(str2);
                        }
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
        }

        internal void RestoreFile(string backupFileName)
        {
            if (!File.Exists(string.Format(@"{0}\{1}", _folderName, backupFileName)))
            {
                throw new Exception("Backup file doest not exists");
            }
            if (File.Exists(_fullFileName))
            {
                File.Delete(_fullFileName);
            }
            File.Move(string.Format(@"{0}\{1}", _folderName, backupFileName), _fullFileName);
        }

        internal void AddHeader(string header)
        {
            _headers.Add(header);
            _contentChanged = true;
        }

        internal void AddTemplate(string id, string content)
        {
            _templates.Add(id,content);
            _contentChanged = true;
        }

        internal void ChangeTemplate(string id, string content)
        {
            _templates[id] = content;
            _contentChanged = true;
        }

        internal string GetTemplate(string id)
        {
            return _templates[id];
        }

        internal bool CheckIfTemplateExist(string id)
        {
            return _templates.ContainsKey(id);
        }

        internal void RemoveTemplate(string id)
        {
            if (_templates.ContainsKey(id))
            {
                _templates.Remove(id);
                _contentChanged = true;
            }            
        }

        internal bool CheckIfTagExist(SPSRegisterableTag tag)
        {
            foreach (string str in _headers)
            {
                if ((str.Contains(tag.TagPrefix) 
                    && str.Contains(tag.Assembly)) 
                    && str.Contains(tag.NameSpace))
                {
                    return true;
                }
            }
            return false;
        }

        internal void DeleteTag(SPSRegisterableTag tag)
        {
            foreach (string str in _headers)
            {
                 if ((str.Contains(tag.TagPrefix) 
                    && str.Contains(tag.Assembly)) 
                    && str.Contains(tag.NameSpace))                
                {
                    _headers.Remove(str);
                    _contentChanged = true;
                    break;
                }
            }
        }
    }

    public class SPSTemplateManager
    {
        private readonly SPSTemplateManagerIO _spsTemplateManagerIO;

        
        // Methods
        public SPSTemplateManager()
        {
            _spsTemplateManagerIO = new SPSTemplateManagerIO();
        }

        public void AddRegisterTagNamespace(SPSRegisterableTag tag)
        {
            if (!_spsTemplateManagerIO.CheckIfTagExist(tag))
            {
                _spsTemplateManagerIO.AddHeader(
                    string.Format("<%@ Register TagPrefix=\"{0}\" Assembly=\"{1}\" namespace=\"{2}\" %>",
                                  tag.TagPrefix,
                                  tag.Assembly,
                                  tag.NameSpace));
            }
        }

        public void AddRegisterTagSource(SPSRegisterableTag tag)
        {
            _spsTemplateManagerIO.AddHeader(string.Format("<%@ Register TagPrefix=\"{0}\" TagName=\"{1}\" src=\"{2}\" %>", 
                                        tag.TagPrefix,
                                        tag.TagName, 
                                        tag.Src));
        }

        public void AddRenderingTemplate(string templateId, string templateDetail)
        {
            _spsTemplateManagerIO.AddTemplate(templateId, templateDetail);
        }

        public void AlterRenderingTemplate(string templateId, string templateDetail)
        {
            _spsTemplateManagerIO.ChangeTemplate(templateId, templateDetail);
        }
        
        public string GetRenderingTemplate(string templateId)
        {
            return _spsTemplateManagerIO.GetTemplate(templateId);
        }

        public bool CheckIfTagNamespaceExist(SPSRegisterableTag tag)
        {
            return _spsTemplateManagerIO.CheckIfTagExist(tag);
        }

        public bool CheckIfRegisterTagSourceExist(string tagPrefix, string tagName, string src)
        {
            return CheckIfTagNamespaceExist(new SPSRegisterableTag {TagPrefix = tagPrefix, TagName = tagName, Src = src});
        }

        public void RemoveRegisterTagNamespace(SPSRegisterableTag tag)
        {
            _spsTemplateManagerIO.DeleteTag(tag);
        }

        public void RemoveRegisterTagSouce(string tagPrefix, string tagName, string src)
        {
            _spsTemplateManagerIO.DeleteTag(new SPSRegisterableTag {TagPrefix = tagPrefix, TagName = tagName, Src = src});
        }

        public void RemoveRenderingTemplate(string templateId)
        {
           _spsTemplateManagerIO.RemoveTemplate(templateId);
        }

        public bool RenderingTemplateExists(string templateId)
        {
            return _spsTemplateManagerIO.CheckIfTemplateExist(templateId);
        }

        public void Save(string backupFileName)
        {
            _spsTemplateManagerIO.Save(backupFileName);
        }

        public void Save()
        {
            _spsTemplateManagerIO.Save();
        }
        
    }
}