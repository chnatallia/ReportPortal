using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPortal.Tests
{
    public class Launch ()
    {
        public string owner;
        public string description;
        public int id;
        public string uuid;
    }

    public class ApiResponse()
    {
        public string message;
    }

    public class Attribute
    {
        public string key;
        public string value;
    }

    public class Payload
    {
        public string mode;
        public string description;
        public List<Attribute> attributes;
        
    }

}
