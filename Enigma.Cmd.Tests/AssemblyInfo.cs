// The console app reports failure through Environment.ExitCode and writes its
// indicator to Console.Error, both of which are process-wide. Tests that read them
// have to be the only ones running, so this assembly runs its collections in order
// rather than at once.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
