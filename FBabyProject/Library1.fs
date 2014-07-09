namespace FBabyProject
open NUnit.Framework

[<TestFixture>]
type Class1() = 
   [<Test>]
   let test=
    Assert.AreEqual(2,2)

