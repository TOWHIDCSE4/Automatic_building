using System;
using System.Runtime.Serialization;
using O3.Commons.DataSpecs;
using O3.Foundation;

namespace DaiwaRentalGD.Optimization.Samples.Foundation
{
    /// <summary>
    /// Test Case 2 from
    /// MOBES: A multiobjective evolution strategy for
    /// constrained optimization problems.
    /// </summary>
    /// 
    /// <remarks>
    /// References:
    /// <list type="bullet">
    /// <item>
    /// Binh, To Thanh, and Ulrich Korn.
    /// "MOBES: A multiobjective evolution strategy for
    /// constrained optimization problems."
    /// In The third international conference on genetic algorithms
    /// (Mendel 97), vol. 25, p. 27. 1997.
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class BinhAndKorn2Problem : Problem
    {
        #region Constructors

        public BinhAndKorn2Problem() : base()
        {
            Name = DefaultName;

            InitializeIOSpecs();
        }

        protected BinhAndKorn2Problem(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        private void InitializeIOSpecs()
        {
            // * About input specs and output specs *
            //
            // This problem has 2 input specs (`InputSpecs` property)
            // and 4 output specs (`OutputSpecs` property),
            // all of which have real number (`RealDataSpec`) as
            // their data specs.
            //
            // This means that a solution to this problem will
            // have 2 inputs and 4 outputs, all of which are real numbers.

            // All input specs and output specs should be added before
            // a solution to this problem is created.
            //
            // If input specs or output specs are changed after
            // solutions are created, existing solutions should not be used.
            // New solutions should be created and used instead.

            // * Add input specs to the problem * 
            //
            // The input specs are ordered - their indices are based on
            // the order they are added.

            AddInputSpec(X1);
            AddInputSpec(X2);

            // * Add output specs to the problem *
            //
            // The output specs are ordered - their indices are based on
            // the order they are added. Output specs and input specs
            // have separate index spaces - Both input indices and
            // output indices start from 0.
            //
            // Among these 4 output specs,
            // 2 are objectives to minimize (`F1` and `F2`),
            // and the other 2 are constraints (`G1` and `G2`).
            //
            // An output spec that is an objective and a constraint
            // at the same time is also supported.

            AddOutputSpec(F1);
            AddOutputSpec(F2);
            AddOutputSpec(G1);
            AddOutputSpec(G2);
        }

        protected override void Evaluate(Solution solution)
        {
            // * About evaluation *
            //
            // The evaluation logic of a problem is defined in this method.
            //
            // This test problem is defined as below:
            //
            // Objective functions:
            //
            // - Minimize: f1(x1, x2) = 4 * x1 ^ 2 + 4 * x2 ^ 2
            // - Minimize: f2(x1, x2) = (x1 - 5) ^ 2 + (x2 - 5) ^ 2
            //
            // Subject to (constraints):
            //
            // - g1(x1, x2) = (x1 - 5) ^ 2 + x2 ^ 2 - 25 <= 0
            // - g2(x1, x2) = -(x1 - 8) ^ 2 - (x2 + 3) ^ 2 + 7.7 <= 0
            //
            // Both objectives and constraints are defined as output specs
            // with specific property settings. Please refer to properties
            // `F1`, `F2`, `G1` and `G2` for how their properties are set.

            // * About solution *
            //
            // A `Solution` instance stores both the inputs and the
            // corresponding outputs of a solution to a problem.
            // When `solution` is passed as argument, it already has
            // its inputs set (e.g. manually, or by a Design of Experiments,
            // etc.). Also `solution.State` is set to `Expired`.

            // * Reading inputs from solution *
            //
            // To evaluate the outputs (objective functions, constraints,
            // both or neither), first we need to get input values.
            //
            // Data types of inputs and outputs are determined at runtime.
            // Thus we need to provide the type parameter to
            // `Solution.GetInput<T>()` and `Solution.GetOutput<T>()`
            // to specify the data type needed.
            // 
            // The data type is specified by `InputSpec.DataSpec` and
            // `OutputSpec.DataSpec` properties, which are instances of
            // `IDataSpec`, and often `IDataSpec<T>`. The type parameter `T`
            // for `Solution.GetInput<T>()` and `Solution.GetOutput<T>()`
            // must be among the `IDataSpec<T>` implemented by
            // `InputSpec.DataSpec`.
            //
            // For instance, `X1.DataSpec` is a `RealDataSpec`, which
            // implements both `IDataSpec<double>` and `IDataSpec<int>`.
            // So we can use `solution.GetInput<double>()` and
            // `solution.GetInput<int>()`. Conversion will take place
            // when `T` is different from that of the underlying data.
            //
            // `Solution.GetInput<T>()` has another overload where
            // an input index can be used in place of an input spec.
            // For instance, `x2` can also be read with
            // `solution.GetInput<double>(1)`.
            // Same goes for `Solution.GetOutput<T>()`.

            double x1 = solution.GetInput<double>(X1);
            double x2 = solution.GetInput<double>(X2);

            // Calculate outputs f1, f2, g1, g2 as defined above.

            double f1 = 4.0 * x1 * x1 + 4.0 * x2 * x2;
            double f2 = (x1 - 5.0) * (x1 - 5.0) + (x2 - 5.0) * (x2 - 5.0);

            double g1 = (x1 - 5.0) * (x1 - 5.0) + x2 * x2 - 25.0;
            double g2 =
                -(x1 - 8.0) * (x1 - 8.0) - (x2 + 3.0) * (x2 + 3.0) + 7.7;

            // * Writing outputs to solution *
            //
            // Once the outputs are calculated, they need to be
            // written back to the solution. `Solution.SetOutput()` is
            // structured similar to `Solution.GetOutput()` except that
            // it does not require a type parameter.

            solution.SetOutput(F1, f1);
            solution.SetOutput(F2, f2);
            solution.SetOutput(G1, g1);
            solution.SetOutput(G2, g2);

            // * About solution state *
            //
            // `solution` was passed in with its state set to `Expired`.
            //
            // `solution.State` will be set to `Updated` after `Evaluate()`
            // returns if nothing went wrong in `Evaluate()`. There are
            // two scenarios where `solution.State` will be set to `Failed`:
            //
            // - An exception was thrown and uncaught in `Evaluate()`
            // - `solution.Fail()` was called in `Evaluate()`
            //
            // Although a solution's outputs can be set any time,
            // only an updated solution's outputs are meaningful.
        }

        #endregion

        #region Properties

        public InputSpec X1 { get; } = new InputSpec
        {
            Name = "x1",
            DataSpec = new RealDataSpec(-15.0, 30.0)
        };

        public InputSpec X2 { get; } = new InputSpec
        {
            Name = "x2",
            DataSpec = new RealDataSpec(-15.0, 30.0)
        };

        public OutputSpec F1 { get; } = new OutputSpec
        {
            Name = "f1",
            DataSpec = new RealDataSpec(),
            ObjectiveType = ObjectiveType.Minimize
        };

        public OutputSpec F2 { get; } = new OutputSpec
        {
            Name = "f2",
            DataSpec = new RealDataSpec(),
            ObjectiveType = ObjectiveType.Minimize
        };

        public OutputSpec G1 { get; } = new OutputSpec
        {
            Name = "g1",
            DataSpec = new RealDataSpec(),
            Constraint = Constraint.CreateLessEqual(0.0)
        };

        public OutputSpec G2 { get; } = new OutputSpec
        {
            Name = "g2",
            DataSpec = new RealDataSpec(),
            Constraint = Constraint.CreateLessEqual(0.0)
        };

        public static BinhAndKorn2Problem Default { get; } =
            new BinhAndKorn2Problem();

        #endregion

        #region Constants

        public const string DefaultName = "Binh and Korn 2";

        #endregion
    }
}
