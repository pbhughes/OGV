using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OGV.Admin.Models;
using System.Collections.ObjectModel;
using System.IO;


namespace UnitTestProject1
{
    [TestClass]
    public class AdminObjectsTests
    {
        [TestMethod]
        public void TestLoadBoards()
        {
            BoardList bList = new BoardList();
            ObservableCollection<Board> _boards;
            _boards = bList.Load().Result;

            Assert.IsTrue((_boards.Count) == 3);

            foreach (var board in bList.Boards)
                Assert.IsTrue((board.Agendas.Count == 1), "Agenda Count");
        }

        [TestMethod]
        public void TestParseAgenda()
        {
            string fileName = @"C:\Users\barkley.hughes\Documents\Visual Studio 2013\Projects\PrismExampleOne\UnitTestProject1\bin\Debug\Agendas\Housing Board\agenda.oga";
            BoardList bl = new BoardList();
            Agenda a = bl.ParseAgenda(new FileInfo(fileName));
        }

        [TestMethod]
        public void TestItemToString()
        {
            string fileName = @"C:\Users\barkley.hughes\Documents\Visual Studio 2013\Projects\PrismExampleOne\UnitTestProject1\bin\Debug\Agendas\Housing Board\agenda.oga";
            FileInfo f = new FileInfo(fileName);
            Agenda a = new Agenda();
            a = a.ParseAgenda(f);

            string content = a.Items[0].ToString();

            Assert.IsNotNull(content);

        }

        [TestMethod]
        public void TestAgendaToString()
        {
            string fileName = @"C:\Users\barkley.hughes\Documents\Visual Studio 2013\Projects\PrismExampleOne\UnitTestProject1\bin\Debug\Agendas\Housing Board\agenda.oga";
            FileInfo f = new FileInfo(fileName);
            Agenda a = new Agenda();
            a = a.ParseAgenda(f);

            string content = a.ToString();

            Assert.IsNotNull(content);
        }
    }
}
