using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcadProjectVersionManager
{
    class VersionManager
    {
        //Beta version release 2nd
        private readonly String RELEASE_VERSION = "0.9.1";

        public VersionManager() { }
        public String getReleaseVersion()
        {
            return this.RELEASE_VERSION;
        }
    }
}
