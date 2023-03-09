using System;
using System.Collections.Generic;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DataSpecs;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Pareto;
using O3.Commons.RandomNumberGenerators;
using O3.Commons.TerminationConditions;
using O3.Foundation;
using O3.Nsga;
using O3.Nsga.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.Nsga
{
    /// <summary>
    /// Provides sample usage of <see cref="Nsga2Solver"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class Nsga2SolverSampleTests
    {
        #region Constructors

        public Nsga2SolverSampleTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Utilities (creating solvers)

        private static Nsga2Solver CreateSampleSolver()
        {
            // * About `Solver` *
            //
            // `Solver` is a foundational class in O3, which produces
            // optimized solutions to a problem. All solver types derive
            // from `Solver`, including `Nsga2Solver`, the subject of
            // this test class.
            //
            // A solver produces solutions to a problem (`Solver.Problem`)
            // with its `Solve()` method. Typically, `Solve()` calls
            // `Update()` iteratively until the latter signals
            // a finishing condition.
            //
            // `Update()` contains the core logic of a specific solver and is
            // to be implemented by `Solver` subclasses.
            // 
            // Solutions produced are stored in `Solutions` property, which
            // can be accessed during solving process (intermedate solutions)
            // or after solving process is done (final solutions).

            // * About `Nsga2Solver` *
            //
            // `Nsga2Solver` provides a modularized implementation of
            // NSGA-II (Nondominated Sorting Genetic Algorithm II).
            // 
            // `Nsga2Solver` consists of the following:
            //
            // - Problem: The `Problem` to which `Solution`s will be created
            //
            // - Components: Each component is responsible for a certain
            //   aspect of the optimization logic, including:
            //
            //   - DoE (Design of Experiments)
            //   - Crossover operator
            //   - Mutation operator
            //   - Selection operator (for NSGA-II, this is fixed to
            //     binary tournament selection)
            //   - Termination condition
            //   - Solution archive
            //
            // - Parameters
            //
            //   - Population size
            //   - Max generation
            //
            // Please note that there are also parameters on the components
            // mentioned above, e.g. mutation rates are defined on
            // the mutation operator instead of `Nsga2Solver` itself.

            // The code below shows how a sample `Nsga2Solver` is created
            // and how its components and parameters are set up.

            var solver = new Nsga2Solver
            {
                // A solver must keep a reference to
                // the problem it tries to solve.
                Problem = BinhAndKorn2Problem.Default,

                // To limit the size of test outputs,
                // we use a small population and few generations here.
                PopulationSize = 4,
                MaxGeneration = 4
            };

            // Now we set up each component of `Nsga2Solver`.

            // DoE

            SetDesignOfExps(solver);

            // Crossover operator

            SetCrossoverOp(solver);

            // Mutation operator

            SetMutationOp(solver);

            // Termination condition

            SetTerminationCondition(solver);

            // Solution archive

            SetSolutionArchive(solver);

            return solver;
        }

        private static void SetDesignOfExps(Nsga2Solver solver)
        {
            // * About DoE and `DesignOfExps` *
            // 
            // Generally speaking, DoE (Design of Experiments) is
            // the process of creating variations of initial conditions
            // of experiments so that different outputs and their relations to
            // inputs can be analyzed.
            //
            // In the context of genetic algorihtms, DoE is typically
            // used to create the initial population (first generation).
            //
            // `DesignOfExps` subclasses in O3 initialize the inputs of
            // given `Solution` objects. The outputs of solutions need
            // to be updated by separated calls to `Solution.Update()`
            // afterwards.

            // Here we use a common DoE, one that initializes inputs by
            // setting them to uniformly distributed random real numbers.

            var doe = new RealUniformDistributionDoe
            {
                // DoE needs to know the problem of the solutions
                // to be initialized.

                Problem = solver.Problem,

                // The RNG (Random Number Generator) used for
                // creating the uniform distribution.
                // There are other types of RNGs too, such as
                // `MersenneTwisterRng` from
                // `O3.Commons.RandomNumberGenerators` namespace
                // (`O3.Commons` assembly).

                RandomNumberGenerator = new XorshiftRng
                {
                    Seed = 0
                }
            };

            // Initially, `.InputRanges` is empty
            // and thus `RealUniformDistributionDoe.Update()` will not set
            // any solution inputs.

            Assert.Empty(doe.InputRanges);

            // Calling `.SetInputRangesFromProblem()` will populate
            // `.InputRanges` based on problem input specs
            // with default settings.
            //
            // There is a range for the uniform distribution of
            // every input spec from the problem.

            doe.SetInputRangesFromProblem();

            Assert.Equal(doe.Problem.InputSpecs.Count, doe.InputRanges.Count);

            Assert.All(

                doe.InputRanges,

                inputRange =>
                {
                    // By default, all input ranges are enabled, which means
                    // `doe` will initialize all inputs if the data type is
                    // supported.

                    Assert.True(inputRange.IsEnabled);

                    // Only inputs that can be converted to `double`
                    // are supported.

                    var dataSpec = inputRange.InputSpec.DataSpec;

                    Assert.Equal(
                        dataSpec is IDataSpec<double>,
                        inputRange.IsApplicable
                    );

                    // Furthermore, if the data spec is `RealDataSpec`
                    // (which implements `IDataSpec<double>`),
                    // then the default range of the input range
                    // will be retrieved from the data spec.
                    //
                    // Please note that if
                    // `RealDataSpec.Min` is `double.NegativeInfinity` or
                    // `RealDataSpec.Max` is `double.PositiveInfinity`,
                    // the corresponding `inputRange` will becomes invalid
                    // (because the range is now infinite) and the
                    // input will be set to NaN.

                    if (dataSpec is RealDataSpec realDataSpec)
                    {
                        Assert.Equal(realDataSpec.Min, inputRange.Min);
                        Assert.Equal(realDataSpec.Max, inputRange.Max);
                    }

                    // `InputRange.OverrideMin` and `InputRange.OverrideMax`
                    // can be used to override the default range. They
                    // will be effective when `InputRange.UseOverride`
                    // is set to true. By default overrides are disabled.

                    Assert.False(inputRange.UseOverride);
                }
            );

            // Attach the created DoE to `NsgaSolver`.

            solver.DesignOfExps = doe;
        }

        private static void SetCrossoverOp(Nsga2Solver solver)
        {
            // Crossovoer operator is a type of component on
            // `Nsga2Solver` and other genetic algorithms,
            // all inheriting from `CrossoverOp` base class.

            // Here we use one that works well for real numbers,
            // the Alpha-Blend crossover operator.

            var cxOp = new BlendCxOp
            {
                // The crossover operator needs to know the problem
                // in order to properly set up its parameters
                // for each input spec.

                Problem = solver.Problem,

                // Please see `SetDesignOfExps()` for more info on RNG.

                RandomNumberGenerator = new XorshiftRng
                {
                    Seed = 1
                }
            };

            // Initially, `.BlendAlphas` is empty
            // and thus the crossover logic will affect no solution input.

            Assert.Empty(cxOp.BlendAlphas);

            // Calling `.SetBlendAlphasFromProblem()` will populate
            // `.BlendAlphas` based on problem input specs
            // with default settings.
            //
            // For each input spec on the problem,
            // there is a blend alpha object describing how crossover logic
            // should blend that input on a pair of solutions.

            cxOp.SetBlendAlphasFromProblem();

            Assert.Equal(
                cxOp.Problem.InputSpecs.Count,
                cxOp.BlendAlphas.Count
            );

            // The indices for `.BlendAlphas` correspond to
            // indices for `Problem.InputSpecs`.
            //
            // For a given blend alpha, it specifies:
            //
            // - `Alpha`: The degree of blending. if alpha = a, then
            //   the result of blending x and y will be in the range
            //   [x - a * (y - x), y + a * (y - x)], assuming y >= x.
            //
            // - `Probability`: The probability of blend crossover happening
            //   on the corresponding input.

            cxOp.BlendAlphas[0].Alpha = 0.5;
            cxOp.BlendAlphas[0].Probability = 0.8;

            cxOp.BlendAlphas[1].Alpha = 0.5;
            cxOp.BlendAlphas[1].Probability = 0.8;

            // Attach the created crossover operator to `Nsga2Solver`.

            solver.CrossoverOp = cxOp;
        }

        private static void SetMutationOp(Nsga2Solver solver)
        {
            // Mutation op is a type of component used in `Nsga2Solver`
            // and other genetic algorithms.

            // Here we create a `RealUniformOffsetMutOp` which works
            // on real number inputs by adding random numbers
            // to the current input values. The random numbers
            // are uniformly distributed.

            var mutOp = new RealUniformOffsetMutOp
            {
                // Mutation operator needs to know the problem so that
                // parameters for input specs can be set up.

                Problem = solver.Problem,

                // Mutation operator usually relies on a random source.
                // Please refer to `SetDesignOfExps()` for more info on RNG.

                RandomNumberGenerator = new XorshiftRng
                {
                    Seed = 2
                }
            };

            // Initially, `.RealUniformOffsets` is empty
            // and thus the mutation logic will affect no solution input.

            Assert.Empty(mutOp.RealUniformOffsets);

            // Calling `.SetOffsetsFromProblem()` will populate
            // `.RealUniformOffsets` based on problem input specs
            // with default settings.
            //
            // The offset for each input is set up individually.

            mutOp.SetOffsetsFromProblem();

            Assert.Equal(
                mutOp.RealUniformOffsets.Count,
                mutOp.Problem.InputSpecs.Count
            );

            // If the offset uses relative mode, then
            // `.RealUniformOffsets[i].MaxOffset` will be calculated from
            // `.RelativeMaxOffset` and the range of the data spec of
            // the input spec. This only works when the data spec
            // is `RealDataSpec` and its `.Min` and `.Max` are finite
            // (similar to `RealUniformDistributionDoe` - please see
            // `SetDesignOfExps()`). Otherwise after mutation the input
            // will become NaN, because max offset cannot be calculated from
            // an infinite range.
            //
            // For example, if the `RealDataSpec` is in the range of
            // [2.0, 5.0], and `.RelativeMaxOffset` is 0.2, then
            // the max offset for mutating the input will be
            // (5.0 - 2.0) * 0.2 = 0.6. A uniformly distributed random
            // real number in [-0.6, 0.6] will be added to the input.
            //
            // The offset can also use absolute mode when `.UseRelative` is
            // set to false. In this case, `.AbsoluteMaxOffset` will
            // be used as `.MaxOffset` and the range of the data spec
            // does not matter.
            //
            // The probability of mutating each input can be set up
            // individually with the `.Probability` on each offset.

            mutOp.RealUniformOffsets[0].UseRelative = true;
            mutOp.RealUniformOffsets[0].RelativeMaxOffset = 0.2;
            mutOp.RealUniformOffsets[0].Probability = 0.1;

            mutOp.RealUniformOffsets[1].UseRelative = true;
            mutOp.RealUniformOffsets[1].RelativeMaxOffset = 0.2;
            mutOp.RealUniformOffsets[1].Probability = 0.1;

            // Attach the mutation operator to `Nsga2Solver`.

            solver.MutationOp = mutOp;
        }

        private static void SetTerminationCondition(Nsga2Solver solver)
        {
            // `Nsga2Solver` queries termination condition at certain points
            // (including before evaluating each solution) to see
            // if it should finish/terminate the optimization.

            // If `.TerminationCondition` is null, then `Nsga2Solver`
            // will run until it reaches the end of `.MaxGeneration`.

            // Here we use a simple time limit termination condition
            // to show how it is set up. Please refer to test classes under
            // `DaiwaRentalGD.Optimization.Samples.TerminationConditions`
            // namespace for sample usage of other types of
            // termination conditions.

            var termCond = new TimeLimitTermCond();

            solver.TerminationCondition = termCond;

            termCond.MaxDuration = TimeSpan.FromMinutes(5.0);
        }

        private static void SetSolutionArchive(Nsga2Solver solver)
        {
            // Solution archive is a data structure that stores
            // the solutions to be returned as the final results.
            // Usually it keeps a subset of all the best solutions
            // found so far based on different criteria.

            // For `Nsga2Solver, if `Nsga2Solver.SolutionArchive`
            // is set, then `Nsga2Solver.Solutions` returns
            // `Nsga2Solver.SolutionArchive.Solutions`.
            // Otherwise, `Nsga2Solver.Solutions` returns
            // `Nsga2Solver.Population`, i.e. solutions in the population
            // from the most recent generation.

            // Currently 2 types of solutions archives are supported
            // in O3, i.e. `ParetoSet` and `EpsilonParetoSet`.
            // Please refer to test classes in
            // `DaiwaRentalGD.Optimization.Samples.SolutionArchives` namespace
            // for their sample usage.

            solver.SolutionArchive = new ParetoSet();
        }

        private void SetEventHandlers(Nsga2Solver solver)
        {
            // `Nsga2Solver` supports various events so that
            // information such as the solver status, latest solutions and
            // optimization progress can be accessed via event handlers.

            // The code below shows how to work with 4 of the `Nsga2Sovler`
            // events.

            // * `Updated` event *
            //
            // `Updated` event is from base class `Solver`, which gets called
            // after `Solver.Update()` returns.
            // Specific solvers may implement `Update() and trigger `Updated`
            // differently because their inner logic varies.
            //
            // For `Nsga2Solver`, `Updated` is triggered at the end of
            // every generation.

            solver.Updated += Solver_Updated;

            // * `SolutionUpdated` event *
            //
            // `Nsga2Solver` also triggers `SolutionUpdated` event after
            // a solution is updated. `SolutionUpdated` is thus triggered
            // more frequently than `Updated`.
            //
            // Please note that the number of times `SolutionUpdated` gets
            // triggered in each generation is usually smaller than
            // the population size, because not all solutions
            // in the population get evaluated. They can simply be carried
            // over from the previous generation because mutation rate
            // and crossovoer rate are usually below 1.0.

            solver.SolutionUpdated += Solver_SolutionUpdated;

            // * `StatusChanged` event *
            //
            // This event is from the base class `Solver`.
            //
            // A solver in O3 is a finite state machine that transitions
            // between certain pairs of states. This state is stored in
            // `Solver.Status` (the word "status" is used here because
            // "state" may collectively refer to the `Solver` object state
            // and cause ambiguity).
            //
            // `StatusChanged` is triggered whenever the status of the solver
            // is changed, including:
            // 
            // - Stopped => Running
            // - Running => Pausing
            // - Pausing => Paused
            // - Paused => Stopped
            // - Running => Finishing
            // - Finishing => Finished
            // - Finished => Stopped
            // - Running => Failing
            // - Failing => Failed
            // - Failed => Stopped

            solver.StatusChanged += Solver_StatusChanged;

            // * `PhaseChanged` event *
            //
            // This event is specific to `Nsga2Solver`. Each generation
            // of the evolution will undergo multiple phases, which can be
            // queried using `Nsga2Solver.Phase`. Some phases
            // from enum `Nsga2SolverPhase` might be skipped in
            // a given generation, e.g. first and later generations
            // can be different, termination condition can lead to a
            // partially finished generation, etc.

            solver.PhaseChanged += Solver_PhaseChanged;
        }

        private void Solver_StatusChanged(
            object sender, SolverStatusChangedEventArgs e
        )
        {
            OutputHelper.WriteLine(
                "{0}: [Status changed] {1} -> {2}",
                (sender as Nsga2Solver).Name,
                e.PreviousStatus,
                e.CurrentStatus
            );
        }

        private void Solver_Updated(object sender, EventArgs e)
        {
            OutputHelper.WriteLine(
                "{0}: [Updated] Generation = {1}",
                (sender as Nsga2Solver).Name,
                (sender as Nsga2Solver).CurrentGeneration

            );
        }

        private void Solver_PhaseChanged(
            object sender, Nsga2SolverPhaseChangedEventArgs e
        )
        {
            OutputHelper.WriteLine(
                "{0}: [Phase changed] {1} -> {2}",
                (sender as Nsga2Solver).Name,
                e.PreviousPhase,
                e.CurrentPhase
            );
        }

        private void Solver_SolutionUpdated(
            object sender, Nsga2SolverSolutionUpdatedEventArgs e
        )
        {
            OutputHelper.WriteLine(
                "{0}: [Solution updated] {1} ({2} -> {3})",
                (sender as Nsga2Solver).Name,
                e.Solution.Name,
                e.PreviousSolutionState,
                e.Solution.State
            );
        }

        #endregion

        #region Tests (using solvers)

        [Theory]
        [MemberData(nameof(TestSolveWithoutSteppingData))]
        public void TestSolveWithoutStepping(Nsga2Solver solver)
        {
            OutputHelper.WriteLine(
                Nsga2SolverInfoStringCreator.Default
                .GetInfoString(solver)
            );

            OutputHelper.WriteLine(string.Empty);

            // * About solving, updating and stepping *
            //
            // The base class `Solver` runs by iteratively calling
            // `Update()` inside `Solve()` until a finishing condition
            // is signaled (by setting `Solver.Status` to `Finishing` with
            // Finish()`).
            //
            // If stepping is enabled (by setting `IsStepping` to true), then
            // `Solve()` will return after each call to `Update()`.
            //
            // Here we set `IsStepping` to false so that the `solver`
            // can run all the way to the end.

            solver.IsStepping = false;

            solver.Solve();

            OutputHelper.WriteLine(
                Nsga2SolverInfoStringCreator.Default
                .GetInfoString(solver)
            );
        }

        public static IEnumerable<object[]> TestSolveWithoutSteppingData()
        {
            yield return new object[]
            {
                CreateSampleSolver()
            };
        }

        [Theory]
        [MemberData(nameof(TestSolveBySteppingData))]
        public void TestSolveByStepping(Nsga2Solver solver)
        {
            OutputHelper.WriteLine(
                Nsga2SolverInfoStringCreator.Default
                .GetInfoString(solver)
            );

            OutputHelper.WriteLine(string.Empty);

            // By setting `IsStepping` to true, `Solve()` will return
            // after each call to `Update()`. Since `IsStepping`, `Solve()`
            // and `Update()` are all members of the base class `Solver`,
            // how `Update()` is implemented is up to the specific
            // solver logic. For `Nsga2Solver`, `Update()` computes
            // one generation.
            //
            // `Solve()` returns true if optimization is not finished and
            // false otherwise.

            solver.IsStepping = true;

            while (solver.Solve())
            {
                OutputHelper.WriteLine(
                    Nsga2SolverInfoStringCreator.Default
                    .GetInfoString(solver)
                );

                OutputHelper.WriteLine(string.Empty);
            }
        }

        public static IEnumerable<object[]> TestSolveBySteppingData()
        {
            yield return new object[]
            {
                CreateSampleSolver()
            };
        }

        [Theory]
        [MemberData(nameof(TestEventsData))]
        public void TestEvents(Nsga2Solver solver)
        {
            // Set up the event handlers.

            SetEventHandlers(solver);

            // The sequence of outputs written by the event handlers
            // provides some information on when the events are triggered
            // and how to access event information.

            solver.Solve();
        }

        public static IEnumerable<object[]> TestEventsData()
        {
            yield return new object[]
            {
                CreateSampleSolver()
            };
        }

        #endregion

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
