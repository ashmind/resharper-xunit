using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace XunitContrib.Runner.ReSharper.RemoteRunner
{
    public class XunitTestRun
    {
        private readonly RemoteTaskServer server;
        private readonly ITestFrameworkExecutor executor;
        private readonly TaskProvider taskProvider;

        public XunitTestRun(RemoteTaskServer server, ITestFrameworkExecutor executor, TaskProvider taskProvider)
        {
            this.server = server;
            this.executor = executor;
            this.taskProvider = taskProvider;
        }

        public void RunTests(IEnumerable<ITestCase> testCases)
        {
            var logger = new ReSharperRunnerLogger(server, taskProvider);
            // TODO: Set any execution options?
            // Hmm. testCases is serialised, so can't be an arbitrary IEnumerable<>
            var options = TestFrameworkOptions.ForExecution();
            executor.RunTests(testCases.ToList(), logger, options);
            logger.Finished.WaitOne();
        }
    }
}