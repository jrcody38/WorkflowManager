using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace WorkflowManager.Test
{
    [TestClass]
    public class WorkFlowManagerTest
    {
        string _ouputText;

        [TestMethod]
        public void WorkFlowParserTest()
        {
            var text = Helper.GetContentsFromEmbeddedFile("WorkflowManager.Test.Samples.input.txt");
            var workFlow = WorkflowParser.ParseInput(text);

            Assert.AreEqual("exampleDoc", workFlow.Name);
            Assert.AreEqual(2, workFlow.Sequences.Count);

            Assert.AreEqual("Sequence_1", workFlow.Sequences[0].Name);
            Assert.AreEqual(5, workFlow.Sequences[0].Tasks.Count);
            Assert.AreEqual("replace", workFlow.Sequences[0].Tasks[2].Name);
            Assert.AreEqual("fantasy good", workFlow.Sequences[0].Tasks[2].Parameter.ToString());

            Assert.AreEqual("Sequence_2", workFlow.Sequences[1].Name);
            Assert.AreEqual(4, workFlow.Sequences[1].Tasks.Count);
            Assert.AreEqual("writefile", workFlow.Sequences[1].Tasks[3].Name);
            Assert.AreEqual("example1.out", workFlow.Sequences[1].Tasks[3].Parameter.ToString());

        }

        [TestMethod]
        public void FileManagerGrepTest()
        {
            var text = Helper.GetContentsFromEmbeddedFile("WorkflowManager.Test.Samples.fileManagerInput.txt");
            var output = FileManager.Grep(text, "grepSymbol");
            var grepLines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            Assert.AreEqual(4, grepLines.Length - 1); // last line is empty new line
        }

        [TestMethod]
        public void FileManagerSortTest()
        {
            var text = Helper.GetContentsFromEmbeddedFile("WorkflowManager.Test.Samples.fileManagerInput.txt");
            var output = FileManager.Sort(text);
            var lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var z = "z";
            var y = "y";
         
            Assert.AreEqual(z.Trim(), lines[0].Trim());
            Assert.AreEqual(y.Trim(), lines[1].Trim());
            Assert.AreEqual("12ABC10 grepSymbol", lines[4].Trim());
            Assert.AreEqual("B", lines[14].Trim());
            Assert.AreEqual("z6 fantasy", lines[22].Trim());
        }

        [TestMethod]
        public void FileManagerReplaceTest()
        {
            var text = Helper.GetContentsFromEmbeddedFile("WorkflowManager.Test.Samples.fileManagerInput.txt");
            var output = FileManager.Replace(text, "replaceSymbol", "newSymbol");
            var grepLines = FileManager.Grep(output, "newSymbol").Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            Assert.AreEqual(4, grepLines.Length - 1); // last line is empty new line
        }

        [TestMethod]
        public void WorkFlowHandlerSingleThreadTest()
        {
            _ouputText = "";

            using (ShimsContext.Create())
            {
                WorkflowManager.Fakes.ShimFileManager.ReadFileString = (key) =>
                {
                    return Helper.GetContentsFromEmbeddedFile("WorkflowManager.Test.Samples.fileManagerInput.txt");
                };

                WorkflowManager.Fakes.ShimFileManager.WriteFileStringString = (key1, key2) =>
                {
                    _ouputText = key1;
                };
                
                var text = Helper.GetContentsFromEmbeddedFile("WorkflowManager.Test.Samples.input2.txt");
                new WorkflowHandler().Execute(WorkflowParser.ParseInput(text));

                var lines = _ouputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var z = "z";
                var y = "y";

                Assert.AreEqual(z.Trim(), lines[0].Trim());
                Assert.AreEqual(y.Trim(), lines[1].Trim());
                Assert.AreEqual("12ABC10 grepSymbol", lines[4].Trim());
                Assert.AreEqual("B", lines[14].Trim());
                Assert.AreEqual("z6 good", lines[22].Trim());

            }       
        }
    }
}
