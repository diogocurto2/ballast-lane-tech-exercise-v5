// These tests create and drop their own databases, so they must not run concurrently with each
// other: parallel CREATE DATABASE statements contend on the template lock and time out.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
