﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Configs;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.PopupWindow;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class GuiTest
    {
        [TestMethod]
        public void LoginWindowTest()
        {
            new GUI.LoginWindow(new Config(new FileInfo(""))).ShowDialog();
        }

        [TestMethod]
        public void MainWindowTest()
        {
            new GUI.MainWindow(new Config(new FileInfo(""))).ShowDialog();
        }

        [TestMethod]
        public void ProgressWindowTest()
        {
            var progress = new LeafNodeProgress("Common");
            var progressWindow = new GUI.ProgressWindow(progress);
           
            var t=new Task(() =>
            {
                progressWindow.ShowDialog();
            });
            t.Start();
            while (progress.Percent<100D)
            {
                Thread.Sleep(3000);
                progress.Percent += 1.5D;
            }
            t.Wait();
        }


        [TestMethod]
        public void SingleSelectTest()
        {
            var cancel = new SingleSelectWindow(null, "Test", "TestField", new List<String> { "asdasd", "dasdddd" }, new FieldReference<String>("dasdddd")).ShowDialog();
        }

        [TestMethod]
        public void NotifyTest()
        {
            var notify = new NotifyWindow(null, "The Declaration of Independence", "We hold these truths to be self-evident, that all men are created equal, that they are endowed by their Creator with certain unalienable Rights, that among these are Life, Liberty and the pursuit of Happiness.--That to secure these rights, Governments are instituted among Men, deriving their just powers from the consent of the governed, --That whenever any Form of Government becomes destructive of these ends, it is the Right of the People to alter or to abolish it, and to institute new Government, laying its foundation on such principles and organizing its powers in such form, as to them shall seem most likely to effect their Safety and Happiness. Prudence, indeed, will dictate that Governments long established should not be changed for light and transient causes; and accordingly all experience hath shewn, that mankind are more disposed to suffer, while evils are sufferable, than to right themselves by abolishing the forms to which they are accustomed. But when a long train of abuses and usurpations, pursuing invariably the same Object evinces a design to reduce them under absolute Despotism, it is their right, it is their duty, to throw off such Government, and to provide new Guards for their future security.--Such has been the patient sufferance of these Colonies; and such is now the necessity which constrains them to alter their former Systems of Government. The history of the present King of Great Britain is a history of repeated injuries and usurpations, all having in direct object the establishment of an absolute Tyranny over these States. To prove this, let Facts be submitted to a candid world.");
            notify.ShowDialog();
        }
    }
}
