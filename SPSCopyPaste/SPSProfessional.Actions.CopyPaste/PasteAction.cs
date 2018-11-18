using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.Actions.CopyPaste
{
    /// <summary>
    /// This control handle the paste action    
    /// </summary>
    internal class PasteAction : WebControl
    {
        private string _eventArgs;

        /// <summary>
        /// Register the javascript action
        /// Check for postbacks
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
#if (DEBUG1)
            FileStream fileStream;
            try
            {
                fileStream = new FileStream("C:\\spslog.txt", FileMode.Append);
            }
            catch (Exception)
            {
                fileStream = new FileStream("C:\\spslog.txt", FileMode.OpenOrCreate);
            }
            TextWriterTraceListener traceListener = new TextWriterTraceListener(fileStream);
            Debug.Listeners.Add(traceListener);
            Debug.AutoFlush = true;
#endif
            Debug.WriteLine("PasteAction-OnLoad wef");

            try
            {
                // Register script to handle Copy & Paste
                ClientScriptManager clientScript = Page.ClientScript;
                const string scriptName = "SPSProfessional_Actions_CopyPaste";

                if (!clientScript.IsClientScriptBlockRegistered(scriptName))
                {
                    // Register script for Document Libraries
                    clientScript.RegisterClientScriptBlock(GetType(),
                                                           scriptName,
                                                           GetScript(),
                                                           true);
                }

                ProcessPostBack();
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Debug.Close();
#if (DEBUG1)
                fileStream.Close();
#endif
            }
        }
       
        internal void ProcessPostBack()
        {
            // Get Postback data
            if (Page.IsPostBack)
            {
                // Decode data
                string eventArg = Page.Request["__EVENTARGUMENT"];
                string eventId = Page.Request["__EVENTTARGET"];
                if (eventArg != null && eventId == ID)
                {
                    _eventArgs = SPEncode.HtmlDecode(eventArg);
                }

                // Get data
                if (!string.IsNullOrEmpty(_eventArgs)
                    && (_eventArgs.StartsWith("ProCopy|")
                        || _eventArgs.StartsWith("ProCut|")))
                {
                    PasteHere();
                }

                // Get data
                if (!string.IsNullOrEmpty(_eventArgs)
                    && (_eventArgs.StartsWith("ProCopyPic|")
                        || _eventArgs.StartsWith("ProCutPic|")))
                {
                    PastePicsHere();
                }
            }
        }


        /// <summary>
        /// Does the paste action
        /// </summary>
        private void PasteHere()
        {
            // Pro
            // SiteUrl
            // FileUrl
            Debug.WriteLine("** _eventArgs: " + _eventArgs);
            string[] pasteData = _eventArgs.Split('|');
            SPSite site;
            SPWeb web;

            SPWeb currWeb = SPContext.Current.Web;
            SPList list = SPContext.Current.List;

            // Rootfolder contains the current folder
            string destinationFolderUrl = Page.Request.QueryString["RootFolder"];

            // Check for valid url
            // Default views (aka root folders) don´t contain a RootFolder QueryString
            if (string.IsNullOrEmpty(destinationFolderUrl))
            {
                destinationFolderUrl = currWeb.Url + "/" + SPEncode.UrlEncodeAsUrl(
                    NormalizeUrl(list.RootFolder.ToString()));
                // list.Title
            }

            try
            {
                // Open source site and to get the file or folder
                using(site = new SPSite(pasteData[1]))
                {
                    web = site.OpenWeb();

                    // Folder object from url
                    // Reopen too get elevated privileges                    
                    SPFolder destinationFolder = currWeb.GetFolder(destinationFolderUrl);

                    Debug.WriteLine("Destination Folder:" + destinationFolderUrl);
                    Debug.WriteLine("Source Site:" + pasteData[1]);
                    Debug.WriteLine("Source File-Folder:" + pasteData[2]);

                    // Paste the file
                    object fileOrFolder = web.GetFileOrFolderObject(pasteData[2]);

                    if (fileOrFolder is SPFile)
                    {
                        Debug.WriteLine("** Coping File");
                        SPFile file = fileOrFolder as SPFile;
                        CopyFile(file, destinationFolder);
                    }
                    else if (fileOrFolder is SPFolder)
                    {
                        Debug.WriteLine("** Coping Folder");
                        SPFolder folder = fileOrFolder as SPFolder;
                        CopyFolder(folder, destinationFolder);
                    }

                    //Delete the file if the action is "Cut"
                    if (pasteData[0].ToLower() == "ProCut".ToLower())
                    {
                        if (fileOrFolder is SPFile)
                        {
                            Debug.WriteLine("** Deleting File");
                            SPFile file = fileOrFolder as SPFile;
                            //file.Delete();
                            file.Recycle();
                        }
                        else if (fileOrFolder is SPFolder)
                        {
                            Debug.WriteLine("** Deleting Folder");
                            SPFolder folder = fileOrFolder as SPFolder;
                            folder.ParentWeb.AllowUnsafeUpdates = true;
                            //folder.Delete();
                            folder.Recycle();
                            folder.ParentWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }

                RefreshPage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Does the paste action
        /// </summary>
        private void PastePicsHere()
        {
            // Pro
            // SiteUrl
            // PictureLibrary ID
            // FileUrl
            Debug.WriteLine("** _eventArgs: " + _eventArgs);
            string[] pasteData = _eventArgs.Split('|');

            SPWeb currWeb = SPContext.Current.Web;
            SPList list = SPContext.Current.List;


            // Rootfolder contains the current folder
            string destinationFolderUrl = Page.Request.QueryString["RootFolder"];

            // Check for valid url
            // Default views (aka root folders) don´t contain a RootFolder QueryString
            if (string.IsNullOrEmpty(destinationFolderUrl))
            {
                //destinationFolderUrl = currWeb.ServerRelativeUrl + "/" + SPEncode.UrlEncodeAsUrl(list.Title);
                destinationFolderUrl = currWeb.Url + "/" +
                    SPEncode.UrlEncodeAsUrl(NormalizeUrl(list.RootFolder.ToString()));
                // list.Title
            }

            try
            {
                // Open source site and to get the file or folder
                using(SPSite site = new SPSite(pasteData[1]))
                {
                    using(SPWeb web = site.OpenWeb())
                    {
                        // Folder object from url
                        // Reopen too get elevated privileges
                        // SPWeb destinationWeb = site.OpenWeb(currWeb.ID);
                        Debug.WriteLine("Destination Folder:" + destinationFolderUrl);
                        SPFolder destinationFolder = currWeb.GetFolder(destinationFolderUrl);

                        Debug.WriteLine("Destination Folder:" + destinationFolderUrl);
                        Debug.WriteLine("Source Site:" + pasteData[1]);
                        Debug.WriteLine("Source List ID:" + pasteData[2]);
                        Debug.WriteLine("Source List ID:" + pasteData[3]);
                        Debug.WriteLine("Source Items IDs:" + pasteData[4]);

                        string[] pictureIds = pasteData[4].Split(';');
                        SPList sourceList = web.Lists[new Guid(pasteData[2])];

                        // Paste the file

                        foreach (string id in pictureIds)
                        {
                            SPListItem listItem = sourceList.GetItemById(Int32.Parse(id));

                            string itemFolder = web.ServerRelativeUrl + "/" +
                                                listItem.Url.Substring(0,
                                                                       listItem.Url.LastIndexOf('/'));

                            Debug.WriteLine(string.Format("Source Folder {0} - {1}", itemFolder, pasteData[3]));

                            if (itemFolder != SPEncode.HtmlDecode(pasteData[3]))
                            {
                                Guid guidFile = listItem.File.UniqueId;
                                SPFile file = web.GetFile(guidFile);

                                Debug.WriteLine("** Coping File");
                                CopyFile(file, destinationFolder);

                                //Delete the file if the action is "Cut"
                                if (pasteData[0].ToLower() == "ProCutPic".ToLower())
                                {
                                    //file.Delete();
                                    file.Recycle();
                                }
                            }
                        }
                    }
                }

                RefreshPage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// After paste we need a refres in order to show changes
        /// </summary>
        private void RefreshPage()
        {
            Debug.WriteLine("Refresh");
            //base.Page.Response.Redirect(Page.Request.Url.ToString());
            base.Page.Response.Redirect(Page.Request.Url.ToString(),false);
        }

        /// <summary>
        /// Generate the Copy & Paste Javascript Actions
        /// </summary>
        /// <returns></returns>
        private string GetScript()
        {
            string script;
            script =
                    @"                            
                    function SPSProfessionalActionCopy(web,item)
                    {
                        window.clipboardData.setData('Text','ProCopy|'+web+'|'+item);
                    }
                    function SPSProfessionalActionCut(web,item)
                    {
                        window.clipboardData.setData('Text','ProCut|'+web+'|'+item);
                    }
                    function SPSProfessionalActionPaste()
                    {
                        var data = window.clipboardData.getData('Text');
                        __doPostBack('" + ID + "',data);" +
                    @"  }";
            return script;
        }

        


        /// <summary>
        /// Copy a file
        /// </summary>
        /// <param name="file">Source file</param>
        /// <param name="destinationFolder">Destination folder</param>
        /// <returns></returns>
        private void CopyFile(SPFile file, SPFolder destinationFolder)
        {
            Debug.WriteLine("* CopyFile");
            SPFile newFile;
            string fileName = GetNewFileName(file.Name, destinationFolder, 0);

            Debug.WriteLine("* CopyFile " + fileName);

            try
            {
                if (file.Versions != null && file.Versions.Count > 0)
                {
                    SPSecurity.RunWithElevatedPrivileges(() => CopyVersions(file,
                                                                            destinationFolder,
                                                                            fileName));

                    Debug.WriteLine("* VCopyFile " + file.Name + " " + file.MajorVersion + "."
                                    + file.MinorVersion);

                    newFile = CopyFileEnsureCheckIn(destinationFolder, file, fileName);
                }
                else
                {
                    Debug.WriteLine("* CopyFile " + file.Name);

                    newFile = destinationFolder.Files.Add(fileName,
                                                          file.OpenBinary(SPOpenBinaryOptions.SkipVirusScan),
                                                          true,
                                                          file.CheckInComment,
                                                          false);

                    EnsureCheckIn(newFile, file.CheckInComment);

                    CopyMetadata(file, newFile, false);
                    newFile.Item["Created"] = file.TimeCreated;
                    newFile.Item["Modified"] = file.TimeLastModified;
                    newFile.Item["Author"] = file.Author;
                    newFile.Item.UpdateOverwriteVersion();
                }

                if (file.Level.ToString() == "Published"
                    && destinationFolder.Item != null
                    && destinationFolder.Item.ParentList.EnableMinorVersions
                    && newFile != null)
                {
                    newFile.Publish(file.CheckInComment);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CF");
                Debug.WriteLine(ex);
            }

            destinationFolder.Update();
        }


        public static Regex FileVer = new Regex("\\w*\\s{1}\\(((?<vers>\\d+))\\)",
                                                RegexOptions.IgnorePatternWhitespace
                                                | RegexOptions.Compiled
                );

        private string GetNewFileName(string fileName, SPFolder destinationFolder, int tries)
        {
            string tryFileName = fileName;
            tries += 1;
            Debug.WriteLine("==== Test File === " + fileName);

            try
            {
                if (destinationFolder.Files[tryFileName] != null)
                {
                    //Match m = FileVer.Match(tryFileName);
                    //FileVer.Replace(tryFileName, NewFileNameEvaluator);
                    //if (m != null)
                    //{

                    //}

                    string newFileName = Path.GetFileNameWithoutExtension(tryFileName);

                    if (tries == 1 && !tryFileName.Contains(string.Format("({0})", tries)))
                    {
                        newFileName = string.Format("{0} ({1})",
                                                    newFileName,
                                                    tries);
                    }

                    if (tries > 1 && tryFileName.Contains(string.Format("({0})", tries - 1)))
                    {
                        newFileName = newFileName.Replace(
                                string.Format("({0})", tries - 1),
                                string.Format("({0})", tries));
                    }

                    string newFile = string.Format("{0}{1}",
                                                   newFileName,
                                                   Path.GetExtension(tryFileName));

                    Debug.WriteLine("=== Try:" + newFileName);

                    return GetNewFileName(newFile, destinationFolder, tries);
                }
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("* File not exist (" + tryFileName + ")");
            }

            return tryFileName;
        }

        /// <summary>
        /// Copies the versions.
        /// </summary>
        /// <param name="fileFrom">The file from.</param>
        /// <param name="folderTo">The folder to.</param>
        /// <param name="fileName">Name of the file.</param>
        private void CopyVersions(SPFile fileFrom, SPFolder folderTo, string fileName)
        {
            using(SPSite elevatedSite = new SPSite(folderTo.ParentWeb.Site.ID))
            {
                using(SPWeb elevatedWeb = elevatedSite.OpenWeb(folderTo.ParentWeb.ID))
                {
                    elevatedWeb.AllowUnsafeUpdates = true;

                    SPUser user = elevatedWeb.CurrentUser;
                    Debug.WriteLine("USER & PERMISSIONS: " + user.Name);
                    Debug.WriteLine(elevatedWeb.EffectiveBasePermissions.ToString());

                    Debug.WriteLine(string.Format("* Copy Versions: {0}", fileFrom.Versions.Count));

                    SortedList sortedList = new SortedList();
                    ICollection keysInList = sortedList.Keys;

                    // destination URL - path that will be used for copied file 
                    // to target library including filename
                    string destURL = folderTo.Url + "/" + fileName;

                    foreach (SPFileVersion version in fileFrom.Versions)
                    {
                        Debug.WriteLine(string.Format("Versions: {0} {1}",
                                                      version.VersionLabel,
                                                      version.Created));

                        if (version.IsCurrentVersion)
                        {
                            Debug.WriteLine(
                                    string.Format("Is current version: {0} {1}",
                                                  version.VersionLabel,
                                                  version.Created));
                        }

                        //parses version number from previous versions URL
                        string tempKey = GetVersion(version);

                        //converts string number to int in order to be sorted correctly
                        //adds to Sorted list as a new key
                        Debug.WriteLine("TempKey:" + tempKey);

                        sortedList.Add(int.Parse(tempKey), string.Empty);

                        Debug.WriteLine(string.Format("Add key {0} = {1}",
                                                      tempKey,
                                                      int.Parse(tempKey)));
                    }
                    //since items in sorted list are now actually sorted correctly
                    //we start with this list in order to process the versions
                    //to copy them in the correct order
                    foreach (object key in keysInList)
                    {
                        //as we iterate the keys in the sorted list (the version numbers)
                        //we then run a comparison on the actual versions to find which one matches
                        //the key so we can process each one in order
                        foreach (SPFileVersion newVer in fileFrom.Versions)
                        {
                            //parses version number from previous versions URL again 
                            //in order to compare it to key stored in SortedList.
                            string temp = GetVersion(newVer);

                            //checks to see if version matches key 
                            if (temp == key.ToString())
                            {
                                Debug.WriteLine(
                                        string.Format(
                                                "Temp key {0} => current {1} status {2} version {3}",
                                                temp,
                                                key,
                                                newVer.File.CheckOutStatus,
                                                newVer.VersionLabel));

                                // opens file for processing and calls method to 
                                // determine major/minor status

                                //byte[] verFile = newVer.File.OpenBinary(SPOpenBinaryOptions.SkipVirusScan);

                                //byte[] verFile = newVer.OpenBinary();
                                //CopyRightVersion(int.Parse(temp),
                                //                 folderTo,
                                //                 destURL,
                                //                 verFile,
                                //                 newVer.CheckInComment);

                                CopyRightVersion(int.Parse(temp),
                                                 folderTo,
                                                 destURL,
                                                 newVer);

                                Debug.WriteLine(string.Format("* COPIED {0} - v{1}",
                                                              newVer.File.Name,
                                                              newVer.VersionLabel));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="newVer">The new ver.</param>
        /// <returns>Version as string</returns>
        private string GetVersion(SPFileVersion newVer)
        {
            //string temp;
            string[] parts = newVer.Url.Split('/');

            if (parts.Length >= 1)
            {
                return parts[1];
            }

            return string.Empty;
        }


        /// <summary>
        /// method to determine major/minor status
        /// of srcFileVersioned and publish accordingly
        /// </summary>
        /// <param name="num">parsed srcFileVersioned number from file's URL</param>
        /// <param name="folderTo">The folder to.</param>
        /// <param name="destUrl">The dest URL.</param>
        /// <param name="srcFileVersioned">The version.</param>
        private void CopyRightVersion(int num,
                                      SPFolder folderTo,
                                      string destUrl,
                                      SPFileVersion srcFileVersioned)
        {
            try
            {
                const int baseNum = 512;
                decimal d = num / baseNum;
                int i = (int) Math.Floor(d) * 512;

                Debug.WriteLine("* CRV " + i + " " + folderTo.Url + " (" + destUrl + ")");

                // SPFile tempFile = destinationFolder.Files[file.Name];

                SPFile copFileVers = CopyFileVersioned(i, folderTo, srcFileVersioned, destUrl);

                //major publish (eg 1.0, 2.0, 3.0)
                if (num == i)
                {
                    if (SPContext.Current.List.EnableMinorVersions)
                    {
                        copFileVers.Publish(srcFileVersioned.CheckInComment);
                    }
                }
                //minor (eg 0.1, 1.1, 2.3)
                else
                {
                }

                folderTo.Update();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CRV");
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Copies the file ensure check in.
        /// </summary>
        /// <param name="folderTo">The folder to.</param>
        /// <param name="srcFile">The file.</param>
        /// <param name="destUrl">The dest URL.</param>
        /// <returns></returns>
        private SPFile CopyFileEnsureCheckIn(SPFolder folderTo, SPFile srcFile, string destUrl)
        {
            Debug.WriteLine("* CF Ensured ");

            EnsureCheckOut(folderTo, 1000, srcFile, destUrl);

            SPFile newFile = folderTo.Files.Add(destUrl,
                                                srcFile.OpenBinary(SPOpenBinaryOptions.SkipVirusScan),
                                                true,
                                                srcFile.CheckInComment,
                                                false);

            EnsureCheckIn(newFile, srcFile.CheckInComment);
            CopyMetadata(srcFile, newFile, false);
            newFile.Item["Created"] = srcFile.TimeCreated;
            newFile.Item["Modified"] = srcFile.TimeLastModified;
            newFile.Item["Author"] = srcFile.Author;
            newFile.Item.UpdateOverwriteVersion();

            Debug.WriteLine("* COPIED " + newFile.Name + " ");

            return newFile;
        }

        /// <summary>
        /// Copies the file versioned.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="folderTo">The folder to.</param>
        /// <param name="srcFile">The version.</param>
        /// <param name="destUrl">The dest URL.</param>
        /// <returns></returns>
        private SPFile CopyFileVersioned(int i,
                                         SPFolder folderTo,
                                         SPFileVersion srcFile,
                                         string destUrl)
        {
            Debug.WriteLine("* Versioned ");
            Debug.WriteLine("i " + i);
            Debug.WriteLine("folderTo " + folderTo.Url);
            Debug.WriteLine("srcFile " + srcFile.VersionLabel);
            Debug.WriteLine("destUrl " + destUrl);

            Debug.WriteLine("EnsureCheckOut");

            EnsureCheckOut(folderTo, i, srcFile.File, destUrl);

            Debug.WriteLine("Add File to new folder");

            SPFile newFile = folderTo.Files.Add(destUrl,
                                                srcFile.OpenBinary(),
                                                true,
                                                srcFile.CheckInComment,
                                                false);

            EnsureCheckIn(newFile, srcFile.CheckInComment);
            CopyMetadata(srcFile.File, newFile, false);
            newFile.Item["Created"] = srcFile.Created;
            newFile.Item["Modified"] = srcFile.Created;
            newFile.Item["Author"] = srcFile.CreatedBy;
            newFile.Item.UpdateOverwriteVersion();


            Debug.WriteLine("* COPIED " + newFile.Name + " ");

            return newFile;
        }

        /// <summary>
        /// Ensures the check in.
        /// </summary>
        /// <param name="newFile">The new file.</param>
        /// <param name="comment">The comment.</param>
        private void EnsureCheckIn(SPFile newFile, string comment)
        {
            Debug.WriteLine("* EnsureCheckIn");
            if (newFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
            {
                Debug.WriteLine("* In " + newFile.CheckOutStatus);
                newFile.CheckIn(comment);
            }
        }

        /// <summary>
        /// Ensures the check out.
        /// </summary>
        /// <param name="folderTo">The folder to.</param>
        /// <param name="i">The i.</param>
        /// <param name="file">The file.</param>
        /// <param name="fileName">Name of the file.</param>
        private void EnsureCheckOut(SPFolder folderTo, int i, SPFile file, string fileName)
        {
            Debug.WriteLine("* EnsureCheckOut " + file.CheckOutStatus);
            try
            {
                //  > 512 more than one file            
                if (SPContext.Current.List.ForceCheckout && i > 512)
                {
                    folderTo.Files[fileName].CheckOut();
                }
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("No version to checkout.");
            }
        }


        /// <summary>
        /// Copy a folder inside other folder
        /// </summary>
        /// <param name="folder">Source folder</param>
        /// <param name="destinationFolder">Destination folder</param>
        /// <returns></returns>
        /// TODO: Comprobar si una carpeta se copia sobre si misma.
        private void CopyFolder(SPFolder folder, SPFolder destinationFolder)
        {
            //if (!destinationFolder.Url.Contains(folder.Url))
            try
            {
                SPList list = destinationFolder.ParentWeb.Lists[destinationFolder.ParentListId];
                string newFolderUrl = destinationFolder.Url + "/" + folder.Name;

                Debug.WriteLine("Destination Folder Name:" + destinationFolder.Name);
                Debug.WriteLine("Destination Folder Url :" + destinationFolder.Url);
                Debug.WriteLine("        New Folder Url :" + newFolderUrl);

                // Make the new folder
                SPFolder newFolder = destinationFolder.SubFolders.Add(folder.Name);
                destinationFolder.Update();

                SPUser user = SPContext.Current.Web.CurrentUser;
                Debug.WriteLine("USER & PERMISSIONS: " + user.Name);
                Debug.WriteLine(list.ParentWeb.EffectiveBasePermissions.ToString());

                // list.Update(); BUG !!
                list.ParentWeb.AllowUnsafeUpdates = true;
                list.ParentWeb.Update();
                list.ParentWeb.AllowUnsafeUpdates = false;

                Debug.WriteLine("Folder:" + folder.Name + " Dest:" + destinationFolder.Name);

                // Copy Folders recursive
                foreach (SPFolder subFolder in folder.SubFolders)
                {
                    CopyFolder(subFolder, newFolder);
                }

                // Recursive file and folder copies
                foreach (SPFile file in folder.Files)
                {
                    CopyFile(file, newFolder);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*CopyFolder");
                Debug.WriteLine(ex);
            }
            return;
        }

        /// <summary>
        /// Copies the metadata.
        /// </summary>
        /// <param name="srcFile">The SRC file.</param>
        /// <param name="newFile">The new file.</param>
        /// <param name="update">if set to <c>true</c> [update].</param>
        private void CopyMetadata(SPFile srcFile, SPFile newFile, bool update)
        {
            foreach (SPField srcField in srcFile.Item.Fields)
            {
                try
                {
                    string internalName = srcField.InternalName;
                    SPField destField = newFile.Item.Fields.GetField(internalName);
                    if (destField != null
                        && !srcField.Hidden
                        && !srcField.ReadOnlyField
                        && srcField.CanBeDeleted)
                    {
                        newFile.Item[destField.Id] = srcFile.Item[srcField.Id];

                        Debug.WriteLine(string.Format("{0}[{1}]", srcField.InternalName, srcFile.Item[destField.Id]));
                        Debug.WriteLine(string.Format("{0}[{1}]", destField.InternalName, newFile.Item[destField.Id]));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("*CopyMetadata");
                    Debug.WriteLine(ex);
                }
            }
            if (update)
            {
                try
                {
                    newFile.Item.Update();
                    newFile.Update();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("*CopyMetadata2");
                    Debug.WriteLine(ex);
                }
            }
        }

        // ==========================================================================

        private readonly Regex invalidCharsRegex =
                new Regex(@"[\*\?\|\\\t/:""'<>#{}%~&]", RegexOptions.Compiled);
      
        /// <summary>
        /// Returns a folder or file name that
        /// conforms to SharePoint's naming restrictions
        /// </summary>
        /// <param name="original">The original file or folder name.
        /// For files, this should be the file name without the extension.</param>
        /// <returns></returns>
        private string NormalizeUrl(string original)
                                                
        {            
            // remove invalid characters and some initial replacements
            string friendlyName = invalidCharsRegex.Replace(original, String.Empty).Trim();
            
            return friendlyName;
        }
    }
}