using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.AsCastAnalyzer,
    Analyzer1.Analyzer1CodeFixProvider>;

namespace Analyzer1.Test
{
    interface IService { }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class MyAttribute : System.Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        // This is a positional argument
        public MyAttribute()
        {
            this.positionalString = positionalString;

        }

        public string PositionalString
        {
            get { return positionalString; }
        }

        // This is a named argument
        public int NamedInt { get; set; }
    }

    [My]
    class Service
    {

    }

    [TestClass]
    public class Analyzer1UnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"
namespace Analyzer1.Test
{
    interface IService { }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class MyAttribute : System.Attribute
    {
        public MyAttribute()
        {
        }
    }

    [My]
    class Service : IService
    {

    }

    [My]
    class Service2 : IService { }
}";

            await CSharpAnalyzerVerifier<Analyzer1Analyzer>.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task AsWithNoNullCheck()
        {
            string test = @"
using System;

class Wrong 
{
    class Nested {
        public string Value {get;set;}
    }
    public int M(object x) {

        var str = x as string;
        str = x as string;
        return str.Length;
    }

    public int M2(object x) {
        
        var o = new Nested();
        o.Value = x as string;
        return o.Value.Length;
    }

    public int M3(object x) {

        var str = x as string;
        if (str == null)
            return str.Length;
        return 0;
    }
}
";
            await CSharpAnalyzerVerifier<AsCastAnalyzer>.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("Analyzer1").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
