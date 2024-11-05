using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Security.WebAuthn;

namespace WebauthnTest
{
    class Program : NUIApplication
    {
        static public TextEditor text;
        protected override void OnCreate()
        {
            base.OnCreate();
            RunTests();
        }

        public void RunTests()
        {
            Window.Instance.KeyEvent += OnKeyEvent;

            text = new TextEditor
            {
                HorizontalAlignment = HorizontalAlignment.Begin,
                TextColor = Color.White,
                PointSize = 5.0f,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                WidthResizePolicy = ResizePolicyType.FillToParent,
                LineWrapMode = LineWrapMode.Word
            };
            Window.Instance.GetDefaultLayer().Add(text);

            var testCases = GetTestCases();

            foreach (TestCaseBase testCase in testCases)
            {
                (bool result, string message) = testCase.Run();
                text.Text += $"\n {testCase.TestName} <<<<< {(result ? "OK" : "FAIL")} {message}";
            }
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        public static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }

        private IEnumerable<TestCaseBase> GetTestCases()
        {
            Assembly asm = typeof(TestCaseBase).GetTypeInfo().Assembly;
            Type testCaseType = typeof(TestCaseBase);

            var tests = from test in asm.GetTypes()
                        where testCaseType.IsAssignableFrom(test) && !test.GetTypeInfo().IsInterface && !test.GetTypeInfo().IsAbstract
                        select Activator.CreateInstance(test) as TestCaseBase;

            return from test in tests
                   orderby test.TestName
                   select test;
        }
    }
}
