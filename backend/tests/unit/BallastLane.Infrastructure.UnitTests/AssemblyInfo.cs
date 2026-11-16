// Tests that mutate process-wide state (environment variables) run sequentially so one test cannot
// observe another test's variable.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
