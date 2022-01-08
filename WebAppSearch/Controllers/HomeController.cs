using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppSearch.Models;
using System.IO;
using System.Text.RegularExpressions;

namespace WebAppSearch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Initialization.
            HomeViewModel model = new HomeViewModel { FileAttach = null };

            try
            {
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return this.View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HomeViewModel model)
        {
            // Initialization.
            string importFilePath = string.Empty;
            string exportFilePath = string.Empty;

            try
            {
                // Verification
                if (ModelState.IsValid)
                {
                    // Initialization.
                    string folderPath = "~/Content/temp_upload_files/";

                    if (model.FileAttach != null) 
                    {
                        // Settings.
                        importFilePath = this.Server.MapPath(folderPath + model.FileAttach.FileName);

                        // Deleting same name files.
                        System.IO.File.Delete(importFilePath);

                        // Converting to bytes.
                        byte[] uploadedFile = new byte[model.FileAttach.InputStream.Length];
                        model.FileAttach.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                        // Uploading file.
                        this.WriteBytesToFile(this.Server.MapPath(folderPath), uploadedFile, model.FileAttach.FileName);
                    }


                    if (model.Keyword != string.Empty) 
                    {
                        // Initialization.
                        model.Data = new List<contents>();
                        DirectoryInfo di = new DirectoryInfo(this.Server.MapPath(folderPath));

                        //get all the .csv files
                        foreach (FileInfo fi in di.GetFiles().Where(e => e.Extension == ".csv"))
                        {
                            int i = 0;
                            string[] lines = System.IO.File.ReadAllLines(fi.FullName);
                            foreach (string line in lines)
                            {
                                // csv with seperate ;
                                string[] columns = line.Split(';');
                                if (i > 0) 
                                { 
                                    model.Data.Add(new contents { id = columns[0], content = columns[1], counts = 0 });
                                }
                                i++;
                            }

                        }

                        MatchCollection matches;
                        bool foundMatch = false;
                        model.results = new List<contents>();
                        foreach (contents item in model.Data)
                        {
                            // insensitive. Case insensitive match (ignores case of [a-zA-Z])
                            Regex exp = new Regex("(?i)" + model.Keyword.Trim());
                            matches = exp.Matches(item.content);
                            if (matches.Count > 0)
                            {
                                foundMatch = true;
                                item.counts = matches.Count;
                                model.results.Add(item);
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info
            return this.View(model);
        }

        private void WriteBytesToFile(string rootFolderPath, byte[] fileBytes, string filename)
        {
            try
            {
                // Verification.
                if (!Directory.Exists(rootFolderPath))
                {
                    // Initialization.
                    string fullFolderPath = rootFolderPath;

                    // Settings.
                    string folderPath = new Uri(fullFolderPath).LocalPath;

                    // Create.
                    Directory.CreateDirectory(folderPath);
                }

                // Initialization.                
                string fullFilePath = rootFolderPath + filename;

                // Create.
                FileStream fs = System.IO.File.Create(fullFilePath);

                // Close.
                fs.Flush();
                fs.Dispose();
                fs.Close();

                // Write Stream.
                BinaryWriter sw = new BinaryWriter(new FileStream(fullFilePath, FileMode.Create, FileAccess.Write));

                // Write to file.
                sw.Write(fileBytes);

                // Closing.
                sw.Flush();
                sw.Dispose();
                sw.Close();
            }
            catch (Exception ex)
            {
                // Info.
                throw ex;
            }
        }

    }
}