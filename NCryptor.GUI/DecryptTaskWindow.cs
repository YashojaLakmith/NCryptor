using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCryptor.GUI
{
    internal class DecryptTaskWindow : StatusWindow
    {
        public DecryptTaskWindow(IParentWindowAccess parentWindow, IEnumerable<string> paths, string outputDir, byte[] key) : base(parentWindow, paths, outputDir, key)
        {
        }

        protected override Task BeginTask()
        {
            throw new NotImplementedException();
        }

        protected override string GetOutputFilePath(string inputFile)
        {
            throw new NotImplementedException();
        }
    }
}
