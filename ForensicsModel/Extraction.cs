using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model
{
    public class Extraction
    {
        private String _ExtractionName;

        public String ExtractionName
        {
            get { return _ExtractionName; }
            set { _ExtractionName = value; }
        }

        private String _ExtractionPath;

        public String ExtractionPath
        {
            get { return _ExtractionPath; }
            set { _ExtractionPath = value; }
        }
        private String _EXTRACTION_VERSION;

        public String EXTRACTION_VERSION
        {
            get { return _EXTRACTION_VERSION; }
            set { _EXTRACTION_VERSION = value; }
        }
    }

    public class Extraction_Hash
    {
        private String _ExtractionFileName;

        public String ExtractionFileName
        {
            get { return _ExtractionFileName; }
            set { _ExtractionFileName = value; }
        }

        private String _ExtractionPath;

        public String ExtractionPath
        {
            get { return _ExtractionPath; }
            set { _ExtractionPath = value; }
        }
        private String _EXTRACTION_HASH;

        public String EXTRACTION_HASH
        {
            get { return _EXTRACTION_HASH; }
            set { _EXTRACTION_HASH = value; }
        }
    }
}
