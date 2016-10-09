using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class Builder
    {
        public PSXProject       Project     { get; protected set; }
        public BuildEnvironment Environment { get; protected set; }

        public Builder()
        {
            Project     = null;
            Environment = null;
        }

        public void Initialize(PSXProject project)
        {
            Project     = project;
            Environment = new BuildEnvironment();
            Environment.Initialize();
        }

        public bool Build()
        {
            bool result = false;

                        result = Upload();
            if (result) result = Compile();
            if (result) result = Link();
            if (result) result = CreateExecutable();
            if (result) result = Download();

            return result;
        }

        protected bool Upload()
        {
            return Environment.CopyFilesToRemote(Project.Files);
        }

        protected bool Compile()
        {
            bool result = false;

            return result;
        }

        protected bool Link()
        {
            bool result = false;

            return result;
        }

        protected bool CreateExecutable()
        {
            bool result = false;

            return result;
        }

        protected bool Download()
        {
            bool result = false;

            return result;
        }
    }
}
