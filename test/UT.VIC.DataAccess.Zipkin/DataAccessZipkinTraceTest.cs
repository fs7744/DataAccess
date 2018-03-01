using AspectCore.DynamicProxy;
using Moq;
using System;
using System.Linq;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Zipkin;
using Xunit;
using zipkin4net;
using zipkin4net.Annotation;
using zipkin4net.Tracers;

namespace UT.VIC.DataAccess.Zipkin
{
    public class DataAccessZipkinTraceTest
    {
        private InMemoryTracer tracer = new InMemoryTracer();
        private DataAccessZipkinTrace sut = new DataAccessZipkinTrace();

        public DataAccessZipkinTraceTest()
        {
            TraceManager.RegisterTracer(tracer);
            TraceManager.SamplingRate = 1.0f;
            var mockLogger = new Mock<ILogger>();
            TraceManager.Start(mockLogger.Object);
        }

        private void CheckDataAccessRecords()
        {
            Assert.NotEmpty(tracer.Records);
            tracer.Records.TryDequeue(out zipkin4net.Record record);
            Assert.IsType<LocalOperationStart>(record.Annotation);
            Assert.Equal("DataAccess", (record.Annotation as LocalOperationStart).OperationName);

            tracer.Records.TryDequeue(out record);
            Assert.IsType<LocalOperationStop>(record.Annotation);

            tracer.Records.TryDequeue(out record);
            Assert.IsType<TagAnnotation>(record.Annotation);
            var tag = record.Annotation as TagAnnotation;
            Assert.Equal("method", tag.Key);
            Assert.Equal("Void Record(DateTime,DateTime,AspectContext,Exception)", tag.Value);

            tracer.Records.TryDequeue(out record);
            Assert.IsType<ServiceName>(record.Annotation);
            Assert.Equal("db", (record.Annotation as ServiceName).Service);
        }

        private void CheckError(string error)
        {
            tracer.Records.TryDequeue(out zipkin4net.Record record);
            Assert.IsType<TagAnnotation>(record.Annotation);
            var tag = record.Annotation as TagAnnotation;
            Assert.Equal("error", tag.Key);
            Assert.Equal(error, tag.Value);
        }

        [Fact]
        public void WhenHasCurrentTraceHasErrAndIsCommandThenRecordCommandData()
        {
            tracer.Records.Clear();
            var mockContext = new Mock<AspectContext>();
            var mockImplementation = new Mock<IDataCommand>();
            mockImplementation.SetupGet(i => i.Text).Returns("select top 1");
            mockImplementation.SetupGet(i => i.ConnectionString).Returns("sql connection");
            mockImplementation.SetupGet(i => i.Timeout).Returns(40);

            Trace.Current = Trace.Create();
            mockContext.SetupGet(i => i.ServiceMethod).Returns(typeof(DataAccessZipkinTrace).GetMethods().First());
            mockContext.SetupGet(i => i.Implementation).Returns(mockImplementation.Object);
            mockContext.SetupGet(i => i.Parameters).Returns(new object[] { new { A = "Test" } });
            sut.Record(DateTime.Now, DateTime.Now, mockContext.Object, new Exception("test1"));
            CheckDataAccessRecords();
            CheckError("System.Exception: test1");

            tracer.Records.TryDequeue(out zipkin4net.Record record);
            Assert.IsType<TagAnnotation>(record.Annotation);
            var tag = record.Annotation as TagAnnotation;
            Assert.Equal("sql", tag.Key);
            Assert.Equal("select top 1", tag.Value);

            tracer.Records.TryDequeue(out record);
            Assert.IsType<TagAnnotation>(record.Annotation);
            tag = record.Annotation as TagAnnotation;
            Assert.Equal("connection", tag.Key);
            Assert.Equal("sql connection", tag.Value);

            tracer.Records.TryDequeue(out record);
            Assert.IsType<TagAnnotation>(record.Annotation);
            tag = record.Annotation as TagAnnotation;
            Assert.Equal("timeout", tag.Key);
            Assert.Equal("40", tag.Value);

            tracer.Records.TryDequeue(out record);
            Assert.IsType<TagAnnotation>(record.Annotation);
            tag = record.Annotation as TagAnnotation;
            Assert.Equal("parameters", tag.Key);
            Assert.Equal("[{\"A\":\"Test\"}]", tag.Value);
        }

        [Fact]
        public void WhenHasCurrentTraceHasErrNotCommandThenRecordHasError()
        {
            tracer.Records.Clear();
            var mockContext = new Mock<AspectContext>();
            Trace.Current = Trace.Create();
            mockContext.SetupGet(i => i.ServiceMethod).Returns(typeof(DataAccessZipkinTrace).GetMethods().First());
            sut.Record(DateTime.Now, DateTime.Now, mockContext.Object, new Exception("test"));
            CheckDataAccessRecords();
            CheckError("System.Exception: test");
        }

        [Fact]
        public void WhenHasCurrentTraceNoErrThenRecordNoError()
        {
            tracer.Records.Clear();
            var mockContext = new Mock<AspectContext>();
            Trace.Current = Trace.Create();
            mockContext.SetupGet(i => i.ServiceMethod).Returns(typeof(DataAccessZipkinTrace).GetMethods().First());
            sut.Record(DateTime.Now, DateTime.Now, mockContext.Object, null);
            CheckDataAccessRecords();
        }

        [Fact]
        public void WhenNoCurrentTraceThenCanNotRecord()
        {
            Trace.Current = null;
            tracer.Records.Clear();
            sut.Record(DateTime.Now, DateTime.Now, null, null);
            Assert.Empty(tracer.Records);
        }
    }
}