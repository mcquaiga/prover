using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prover.Tests.Data.Sql.Storage
{
    [TestClass]
    public class TestRunSqlRepositoryTests
    {
        //private MockRepository mockRepository;
        //private Mock<TestRunSqlContext> mockDbContext;

        //[TestInitialize]
        //public void TestInitialize()
        //{
        //    var tests = new List<TestRunDto>()
        //    {
        //        new TestRunDao(){ }
        //    }.AsQueryable();

        //    var mockSet = CreateDbSetMock(tests);
        //    mockDbContext = new Mock<TestRunSqlContext>();
        //    mockDbContext.Setup(m => m.TestRuns).Returns(mockSet.Object);
        //}

        //[TestCleanup]
        //public void TestCleanup()
        //{
        //    this.mockRepository.VerifyAll();
        //}

        //[TestMethod]
        //public void Get_Test()
        //{
        //    TestRunSqlRepository testRunSqlRepository = CreateTestRunSqlRepository();
        //    var test = testRunSqlRepository.Query().FirstOrDefault();

        //}

        //private TestRunSqlRepository CreateTestRunSqlRepository()
        //{
        //    return new TestRunSqlRepository(
        //        this.mockDbContext.Object);
        //}


        //private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        //{
        //    var elementsAsQueryable = elements.AsQueryable();
        //    var dbSetMock = new Mock<DbSet<T>>();

        //    dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
        //    dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
        //    dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
        //    dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

        //    return dbSetMock;
        //}
    }
}