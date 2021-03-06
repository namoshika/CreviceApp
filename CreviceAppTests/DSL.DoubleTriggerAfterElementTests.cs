﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.User;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL.Tests
{
    [TestClass()]
    public class DoubleTriggerAfterElementTests
    {
        [TestMethod()]
        public void funcTest()
        {
            var ctx = new Core.UserActionExecutionContext(new Point());
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            var called = false;
            var afterElement = ifElement.@after(_ => { called = true; });
            Assert.IsFalse(called);
            root.whenElements[0].ifDoubleTriggerButtonElements[0].afterElements[0].func(ctx);
            Assert.IsTrue(called);
        }
    }
}