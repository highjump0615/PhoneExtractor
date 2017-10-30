// MBDB/MBDX decoder
// René DEVICHI 2010

using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;


namespace mbdbdump
{
    class SortFile : Comparer<mbdb.MBFileRecord>
    {
        bool byInode;

        public SortFile(bool byInode)
        {
            this.byInode = byInode;
        }

        public override int Compare(mbdb.MBFileRecord x, mbdb.MBFileRecord y)
        {
            int i;

            if (byInode)
            {
                i = x.inode.CompareTo(y.inode);
                if (i != 0) return i;
            }

            i = x.Domain.CompareTo(y.Domain);
            if (i != 0) return i;

            i = x.Path.CompareTo(y.Path);
            if (i != 0) return i;

            return 0;
        }


        static private string unixMode(int mode)
        {
            if (mode == 0xA000) return "symlink";
            else if (mode == 0x4000) return "dir";
            else if (mode == 0x8000) return "file";

            string s = "";

            for (int i = 0; i < 3; ++i)
            {
                s = (((mode & 1) == 1) ? "x" : "-") + s;
                s = (((mode & 2) == 2) ? "w" : "-") + s;
                s = (((mode & 4) == 4) ? "r" : "-") + s;
                mode /= 8;
            }

            if (mode != 0)
            {
                s = "???";
            }

            return s;
        }

        class Program
        {
            static void Main(string[] args)
            {
                bool dump = true;
                bool sortInode = true;
                bool checks = false;
                bool stats = false;

                List<mbdb.MBFileRecord> files;

                files = mbdb.ReadMBDB((args.Length >= 1) ? args[0] : "");

                if (files == null)
                    return;

                int i = 0;

                if (sortInode)
                {
                    files.Sort(new SortFile(true));
                }

                foreach (var rec in files)
                {
                    if (dump)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("record {0} of {1}", ++i, files.Count);

                        Console.WriteLine("  key    {0}", rec.key);
                        Console.WriteLine("  domain {0}", rec.Domain);
                        Console.WriteLine("  path   {0}", rec.Path);
                        Console.WriteLine("  inode  {0,10}", rec.inode);
                        if (rec.LinkTarget != null) Console.WriteLine("  target {0}", rec.LinkTarget);
                        if (rec.DataHash != null) Console.WriteLine("  hash   {0}", rec.DataHash);
                        if (rec.alwaysNull != null) Console.WriteLine("  unk3   {0}", rec.alwaysNull);

                        Console.WriteLine("  mode   {1,4} {2} ({0})", Convert.ToString(rec.Mode & 0x1FF, 8), unixMode(rec.Mode & 0xF000), unixMode(rec.Mode & 0x1FF));

                        Console.WriteLine("  time   {0}", rec.aTime);

                        // length is unsignificant if link or dir
                        if ((rec.Mode & 0xF000) == 0x8000) Console.WriteLine("  length {0}", rec.FileLength);

                        Console.WriteLine("  data   {0}", rec.data);
                        for (int j = 0; j < rec.PropertyCount; ++j)
                        {
                            Console.WriteLine("  pn[{0}]  {1}", j, rec.Properties[j].Name);
                            Console.WriteLine("  pv[{0}]  {1}", j, rec.Properties[j].Value);
                        }
                    }


                    // some assertions...
                    if (checks)
                    {
                        Debug.Assert(rec.alwaysZero == 0);
                        if (rec.LinkTarget != null) Debug.Assert((rec.Mode & 0xF000) == 0xA000);
                        if (rec.DataHash != null) Debug.Assert(rec.DataHash.Length == 40);
                        Debug.Assert(rec.alwaysNull == null);
                        if (rec.Domain.StartsWith("AppDomain-")) Debug.Assert(rec.GroupId == 501 && rec.UserId == 501);
                        if (rec.FileLength != 0) Debug.Assert((rec.Mode & 0xF000) == 0x8000);

                        // file
                        if ((rec.Mode & 0xF000) == 0x8000)
                        {
                            Debug.Assert(rec.flag != 0);
                        }

                        // symlink
                        if ((rec.Mode & 0xF000) == 0xA000)
                        {
                            Debug.Assert(rec.flag == 0 && rec.FileLength == 0);
                        }

                        // directory
                        if ((rec.Mode & 0xF000) == 0x4000)
                        {
                            Debug.Assert(/*rec.flag == 0 &&*/ rec.FileLength == 0);
                        }
                    }
                }


                // some stats with Linq
                if (stats)
                {
                    foreach (var mode in new ushort[] { 0x4000, 0x8000, 0xA000 })
                    {
                        Console.WriteLine();
                        Console.WriteLine("For element 0x{0:x4} ({1}) ->", mode, unixMode(mode));
                        Console.WriteLine("  * different modes:");
                        foreach (var x in
                            (from rec in files
                             where ((rec.Mode & 0xF000) == mode)
                             select (rec.Mode & 0x1FF)).Distinct())
                        {
                            Console.WriteLine("     {0,3} {1}", Convert.ToString(x, 8), unixMode(x));
                        }

                        var zz =
                            (from rec in files
                             where ((rec.Mode & 0xF000) == mode)
                             select rec.flag).Distinct().Aggregate("", (s, z) => s += " " + z.ToString());

                        Console.WriteLine();
                        Console.WriteLine("  * different flags:{2}", mode, unixMode(mode), zz);
                    }
                }

            }
        }
    }

}
