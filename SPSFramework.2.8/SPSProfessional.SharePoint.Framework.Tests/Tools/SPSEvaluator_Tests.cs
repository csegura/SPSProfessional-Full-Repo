using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.Tests.Tools
{
    [TestFixture]
    public class SPSEvaluator_Tests
    {
        private readonly SPSEvaluator _evaluator;
        private string _result;

        public static string[] Functions
        {
            get
            {
                return new[]
                           {
                                   "DateNow()",
                                   "DateTimeNow()",
                                   "MonthNumber()",
                                   "DayNumber()",
                                   "YearNumber()",
                                   "Empty()",
                                   //"QueryStringNull('Var1',value)",
                                   //"QueryString('Var2')",
                                   "DateFormat('s')",
                                   "DateCalcFormat('d',M+1)",
                                   "DateCalc(M+1)"
                           };
            }
        }

        public SPSEvaluator_Tests()
        {
            _evaluator = new SPSEvaluator();
        }

        [Test]
        public void EvaluateFunctions()
        {
            foreach (string function in Functions)
            {
                _result = _evaluator.Evaluate(function);
                Assert.IsNotNull(_result);
            }
        }

        //[Test]
        //public void EvaluateException()
        //{
        //    Assert.ThrowsDelegate del = () => _evaluator.Evaluate("DateFormat('8?')");
        //    Assert.Throws<SPSEvaluatorException>(del);
        //}
    }
}