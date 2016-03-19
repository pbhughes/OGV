using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure.ExtendedObjects;
using Infrastructure.Models;

namespace OGV2Tests
{
    [TestClass]
    public class ExtendedTreeTests
    {

        [TestMethod]
        public void TestExtendedTreeViewToString()
        {
            ExtendedTreeView tree = new ExtendedTreeView();


            ExtendedTreeNode x = new ExtendedTreeNode();
            Item i = new Item();
            i.Title = "Test Title";
            i.Description = "Test description";
            i.TimeStamp = TimeSpan.Zero;

            Item ii1 = new Item() { Title = "sub node 1", Description = "sub node 1 description", TimeStamp = TimeSpan.Zero };
            Item ii2 = new Item() { Title = "sub node 2", Description = "sub node 2 description", TimeStamp = TimeSpan.Zero };

            ExtendedTreeNode n11 = new ExtendedTreeNode();
            n11.AgendaItem = ii1;
            ExtendedTreeNode n12 = new ExtendedTreeNode();
            n12.AgendaItem = ii2;

            x.Nodes.Add(n11);
            x.Nodes.Add(n12);

            x.AgendaItem = i;

            tree.Nodes.Add(x);

            string output = tree.ToString();

            Assert.IsNotNull(output);
        }

        [TestMethod]
        public void TestExtendedTreeNodeToString()
        {
            ExtendedTreeNode x = new ExtendedTreeNode();
            Item i = new Item();
            i.Title = "Test Title";
            i.Description = "Test description";

            Item ii1 = new Item() { Title = "sub node 1", Description = "sub node 1 description" };
            Item ii2 = new Item() { Title = "sub node 2", Description = "sub node 2 description" };

            ExtendedTreeNode n11 = new ExtendedTreeNode();
            n11.AgendaItem = ii1;
            ExtendedTreeNode n12 = new ExtendedTreeNode();
            n12.AgendaItem = ii2;

            x.Nodes.Add(n11);
            x.Nodes.Add(n12);

            x.AgendaItem = i;

            string output = x.ToString();

            Assert.IsNotNull(output);
        }
    }
}
