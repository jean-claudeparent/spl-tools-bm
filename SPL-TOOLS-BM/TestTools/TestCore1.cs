using SPL_TOOLS_BM;


namespace TestTools
{
    [TestClass]
    public sealed class TestCore1
    {
        [TestMethod]
        public void TestGetToken()
        {
            SPL_TOOLS_BM.Core  myCore = new SPL_TOOLS_BM.Core()   ;
            string Result = myCore.GetToken  ("unknownuser1756","falsepostingkey");
            
                           Assert.Fail("Not yet implemented"); 
        }
    }
}
