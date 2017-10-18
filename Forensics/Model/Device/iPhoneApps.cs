using Forensics.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Plists;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Device
{
    class iPhoneApps : Dictionary<string, iPhoneIPA>
    {
        //public Dictionary<string, iPhoneIPA> apps = new Dictionary<string, iPhoneIPA>();
        public string appsDirectory;

        public void LoadIPAs(BackgroundWorker worker, DoWorkEventArgs e)
        {
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();
            csb.DataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ipa.db");

            SQLiteConnection cnn = new SQLiteConnection(csb.ConnectionString);
            cnn.Open();

            using (SQLiteCommand cmd0 = cnn.CreateCommand())
            {
                cmd0.CommandText =
@"CREATE TABLE IF NOT EXISTS ipa (
    fileName TEXT,
    softwareVersionBundleId TEXT,
    itemName TEXT,
    totalSize INTEGER,
    iTunesArtwork BLOB
);";
                cmd0.ExecuteNonQuery();
            }

            SQLiteCommand cmd = cnn.CreateCommand();
            cmd.CommandText = "select fileName,softwareVersionBundleId,itemName,totalSize from ipa where fileName=@fn";

            SQLiteParameter lookupValue = new SQLiteParameter("@fn");
            cmd.Parameters.Add(lookupValue);



            SQLiteCommand inscmd = cnn.CreateCommand();
            inscmd.CommandText = "insert into ipa (fileName,softwareVersionBundleId,itemName,totalSize) values (@fileName,@softwareVersionBundleId,@itemName,@totalSize)";
            SQLiteParameter[] insprm = new SQLiteParameter[4];
            insprm[0] = new SQLiteParameter("@fileName");
            insprm[1] = new SQLiteParameter("@softwareVersionBundleId");
            insprm[2] = new SQLiteParameter("@itemName");
            insprm[3] = new SQLiteParameter("@totalSize");
            inscmd.Parameters.AddRange(insprm);


            try
            {
                appsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                appsDirectory = Path.Combine(appsDirectory, "iTunes", "Mobile Applications");

                DirectoryInfo d = new DirectoryInfo(appsDirectory);

                FileInfo[] fi = d.GetFiles("*.ipa");

                int lastprogress = -1;

                for (int i = 0; i < fi.Length; ++i)
                {
                    if (worker.CancellationPending) e.Cancel = true;

                    //System.Threading.Thread.Sleep(20);

                    int progress = (i * 100 / fi.Length);
                    if (lastprogress != progress)
                    {
                        lastprogress = progress;
                        worker.ReportProgress(progress, fi[i].Name);
                    }



                    lookupValue.Value = fi[i].Name;

                    using (var o = cmd.ExecuteReader())
                    {
                        if (o.HasRows)
                        {
                            iPhoneIPA ipa = new iPhoneIPA();

                            ipa.fileName = (string)o[0];
                            ipa.softwareVersionBundleId = (string)o[1];
                            ipa.itemName = (string)o[2];
                            ipa.totalSize = (uint)(long)o[3];


                            // if the BundleId has been already found in some .ipa,
                            // we assume this one is more recent
                            if (this.ContainsKey(ipa.softwareVersionBundleId))
                                this[ipa.softwareVersionBundleId] = ipa;
                            else
                                this.Add(ipa.softwareVersionBundleId, ipa);

                            continue;
                        }
                    }


                    try
                    {
                        using (ZipStorer zip = ZipStorer.Open(fi[i].FullName, FileAccess.Read))
                        {
                            iPhoneIPA ipa = new iPhoneIPA();

                            ipa.fileName = fi[i].Name;

                            foreach (ZipStorer.ZipFileEntry f in zip.ReadCentralDir())
                            {
                                if (worker.CancellationPending) e.Cancel = true;

                                // computes the files total size
                                ipa.totalSize += f.FileSize;

                                // analyzes the app metadata
                                if (f.FilenameInZip == "iTunesMetadata.plist")
                                {
                                    MemoryStream mem = new MemoryStream();
                                    zip.ExtractFile(f, mem);

                                    ipa.softwareVersionBundleId = f.Comment;

                                    if (mem.Length <= 8) continue;

                                    byte[] header = new byte[8];
                                    mem.Position = 0;
                                    mem.Read(header, 0, 8);
                                    mem.Position = 0;
                                    if (BitConverter.ToString(header, 0) == "62-70-6C-69-73-74-30-30")      // "bplist00"
                                    {
                                        try
                                        {
                                            // iTunesMetadata.plist is a binary plist ?
                                            BinaryPlistReader pr = new BinaryPlistReader();
                                            IDictionary dd = pr.ReadObject(mem);

                                            // yes                                    
                                            ipa.softwareVersionBundleId = dd["softwareVersionBundleId"] as string;
                                            ipa.itemName = dd["softwareVersionBundleId"] as string;
                                        }
                                        catch (ArgumentException)
                                        {
                                            // error reading bplist
                                        }
                                    }
                                    else
                                    {

                                        using (StreamReader sr = new StreamReader(mem))
                                        {
                                            xdict dd = xdict.open(sr);

                                            if (dd != null)
                                            {
                                                dd.findKey("softwareVersionBundleId", out ipa.softwareVersionBundleId);
                                                dd.findKey("itemName", out ipa.itemName);
                                            }
                                        }
                                    }

                                }
                            }

                            // if we have found the app id
                            if (ipa.softwareVersionBundleId != null)
                            {

                                insprm[0].Value = ipa.fileName;
                                insprm[1].Value = ipa.softwareVersionBundleId;
                                insprm[2].Value = ipa.itemName;
                                insprm[3].Value = ipa.totalSize;

                                inscmd.ExecuteNonQuery();

                                //Debug.WriteLine("{0} {1}", fi[i].Name, softwareVersionBundleId);

                                // if the BundleId has been already found in some .ipa,
                                // we assume this one is more recent
                                if (this.ContainsKey(ipa.softwareVersionBundleId))
                                    this[ipa.softwareVersionBundleId] = ipa;
                                else
                                    this.Add(ipa.softwareVersionBundleId, ipa);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                        // ignore all error when reading the .ipa
                    }

                }
            }
            catch (DirectoryNotFoundException /*ex*/)
            {
                //apps = null;
                this.Clear();
                appsDirectory = null;
                //MessageBox.Show(ex.ToString());
            }

            //this.appsCatalog = apps;
            //this.appsDirectory = appsDirectory;
        }
    }
}
